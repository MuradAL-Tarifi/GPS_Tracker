using GPS.DataAccess.Context;
using GPS.Domain.DTO;
using GPS.Domain.Models;
using GPS.Domain.ViewModels;
using GPS.Domain.Views;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.DataAccess.Repository.Fleets
{
    public class FleetRepository : IFleetRepository
    {
        private readonly TrackerDBContext _dbContext;

        public FleetRepository(TrackerDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PagedResult<Fleet>> SearchAsync(string searchString, int? waslLinkStatus, int pageNumber, int pageSize)
        {
            bool isWaslLinked = waslLinkStatus == 1;
            var pagedList = new PagedResult<Fleet>();
            var skip = (pageNumber - 1) * pageSize;

            var filteredFleetsIds = await _dbContext.FleetDetails.Where(x => (x.IsLinkedWithWasl == isWaslLinked || !waslLinkStatus.HasValue))
                .Select(x => x.FleetId).ToListAsync();

            pagedList.TotalRecords = await _dbContext.Fleet.Where(x => !x.IsDeleted && (filteredFleetsIds.Contains(x.Id)) &&
            (string.IsNullOrEmpty(searchString) || (x.Name.Contains(searchString) || x.NameEn.Contains(searchString))))
                .CountAsync();

            pagedList.List = await _dbContext.Fleet.Where(x => !x.IsDeleted && (filteredFleetsIds.Contains(x.Id)) &&
                  (string.IsNullOrEmpty(searchString) || (x.Name.Contains(searchString) || x.NameEn.Contains(searchString))))
                    .OrderByDescending(x => x.CreatedDate)
                    .Skip(skip).Take(pageSize)
                    .Include(x => x.Agent)
                    .AsNoTracking().ToListAsync();

            return pagedList;
        }

        public async Task<Fleet> FindByIdAsync(long? Id)
        {
            var fleet = await _dbContext.Fleet.Where(x => !x.IsDeleted && x.Id == Id)
                .Include(x => x.Agent)
                .AsNoTracking().FirstOrDefaultAsync();

            return fleet;
        }

        public async Task<FleetDetails> FindDetailsByFleetIdAsync(long? fleetId)
        {
            var fleet = await _dbContext.FleetDetails.Where(x => !x.IsDeleted && x.FleetId == fleetId)
                .FirstOrDefaultAsync();

            return fleet;
        }

        public async Task<List<FleetDetails>> GetDetailsListByFleetIdAsync(long? fleetId)
        {
            var fleet = await _dbContext.FleetDetails.Where(x => !x.IsDeleted && x.FleetId == fleetId)
                .AsNoTracking().ToListAsync();

            return fleet;
        }

        public async Task<Fleet> AddAsync(Fleet fleet)
        {
            var obj = await _dbContext.Fleet.AddAsync(fleet);
            await _dbContext.SaveChangesAsync();
            return obj.Entity;
        }

        public async Task<bool> UpdateAsync(FleetView model)
        {
            var fleetMaster = await _dbContext.Fleet.Where(x => x.Id == model.Id).AsNoTracking().FirstOrDefaultAsync();
            if (fleetMaster == null)
            {
                return false;
            }

            fleetMaster.Name = model.Name;
            fleetMaster.NameEn = model.NameEn;
            fleetMaster.ManagerEmail = model.ManagerEmail;
            fleetMaster.ManagerMobile = model.ManagerMobile;
            fleetMaster.SupervisorEmail = model.SupervisorEmail;
            fleetMaster.SupervisorMobile = model.SupervisorMobile;
            fleetMaster.TaxRegistrationNumber = model.TaxRegistrationNumber;
            fleetMaster.CommercialRegistrationNumber = model.CommercialRegistrationNumber;

            fleetMaster.UpdatedBy = model.UpdatedBy;
            fleetMaster.UpdatedDate = DateTime.Now;

            _dbContext.Fleet.Update(fleetMaster);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<Fleet> DeleteAsync(long Id, string UpdatedBy)
        {
            var fleet = await _dbContext.Fleet.FindAsync(Id);
            if (fleet == null)
            {
                return null;
            }

            fleet.IsDeleted = true;
            fleet.UpdatedBy = UpdatedBy;
            fleet.UpdatedDate = DateTime.Now;

            var updated = _dbContext.Fleet.Update(fleet);
            await _dbContext.SaveChangesAsync();

            var fleetDetails = await _dbContext.FleetDetails.FindAsync(Id);
            if (fleetDetails != null)
            {
                fleetDetails.IsDeleted = true;
                fleetDetails.UpdatedBy = UpdatedBy;
                fleetDetails.UpdatedDate = DateTime.Now;

                _dbContext.FleetDetails.Update(fleetDetails);
                await _dbContext.SaveChangesAsync();
            }

            return updated.Entity;
        }

        public async Task<FleetDetails> UpdateLinkedWithWaslInfoAsync(FleetDetails fleetDetails)
        {
            var entity = await _dbContext.FleetDetails.FindAsync(fleetDetails.Id);
            if (entity == null)
            {
                return null;
            }

            entity.IsLinkedWithWasl = fleetDetails.IsLinkedWithWasl;
            entity.UpdatedBy = fleetDetails.UpdatedBy;
            entity.UpdatedDate = fleetDetails.UpdatedDate;
            entity.ReferanceNumber = fleetDetails.ReferanceNumber;

            var updated = _dbContext.FleetDetails.Update(entity);
            await _dbContext.SaveChangesAsync();

            return updated.Entity;
        }

        public async Task<int> CountAsync()
        {
            int Count = await _dbContext.Fleet.Where(x => !x.IsDeleted).CountAsync();
            return Count;
        }

        public async Task<int> CountLinkedWithWaslAsync()
        {
            int Count = await _dbContext.FleetDetails.Where(x => !x.IsDeleted && x.IsLinkedWithWasl).CountAsync();
            return Count;
        }

        public async Task<List<Fleet>> GetAllAsync()
        {
            var fleets = await _dbContext.Fleet.Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.CreatedDate).AsNoTracking().ToListAsync();

            return fleets;
        }

        public async Task<List<Fleet>> GetFleetsWASLAsync(int? AgentId)
        {
            var fleetIds = await _dbContext.Fleet.Where(x => !x.IsDeleted && x.AgentId == AgentId).Select(x => x.Id).ToListAsync();

            var fleetsDetailsIds = await _dbContext.FleetDetails.Where(x => !x.IsDeleted && fleetIds.Contains(x.FleetId) && x.IsLinkedWithWasl)
                .OrderByDescending(x => x.CreatedDate).AsNoTracking().Select(x => x.Id).ToListAsync();

            var fleetsWASL = await _dbContext.Fleet.Where(x => fleetsDetailsIds.Contains(x.Id)).ToListAsync();

            return fleetsWASL;
        }

        public async Task<bool> IsNameExistsAsync(int AgentId, string Name)
        {
            var count = await _dbContext.Fleet.Where(x => !x.IsDeleted && x.AgentId == AgentId && x.Name == Name).CountAsync();
            return count > 0;
        }

        public async Task<bool> IsNameEnExistsAsync(int AgentId, string NameEn)
        {
            var count = await _dbContext.Fleet.Where(x => !x.IsDeleted && x.AgentId == AgentId && x.NameEn == NameEn).CountAsync();
            return count > 0;
        }

        public async Task<bool> UpdateWaslDetailsAsync(FleetWaslModel model)
        {
            var fleetDetails = await _dbContext.FleetDetails
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.FleetId == model.FleetId);

            if (string.IsNullOrEmpty(model.ActivityType) || model.ActivityType != "SFDA")
            {
                model.SFDACompanyActivities = "";
            }

            if (fleetDetails == null)
            {
                fleetDetails = new FleetDetails()
                {
                    FleetId = model.FleetId,
                    IdentityNumber = model.IdentityNumber,
                    DateOfBirthHijri = model.DateOfBirthHijri,
                    CommercialRecordNumber = model.CommercialRecordNumber,
                    CommercialRecordIssueDateHijri = model.CommercialRecordIssueDateHijri,
                    PhoneNumber = model.PhoneNumber,
                    ExtensionNumber = model.ExtensionNumber,
                    EmailAddress = model.EmailAddress,
                    ManagerName = model.ManagerName,
                    ManagerPhoneNumber = model.ManagerPhoneNumber,
                    ManagerMobileNumber = model.ManagerMobileNumber,
                    //RegisterInWasl = model.RegisterInWasl,
                    //RegisterInRaqeb = model.RegisterInRaqeb,
                    ActivityType = model.ActivityType,
                    SFDACompanyActivities = model.SFDACompanyActivities,
                    IsLinkedWithWasl = false,
                    CreatedBy = model.UpdatedBy,
                    CreatedDate = DateTime.Now,
                    FleetType = model.FleetType
                };
                await _dbContext.FleetDetails.AddAsync(fleetDetails);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                // update
                fleetDetails.IdentityNumber = model.IdentityNumber;
                fleetDetails.DateOfBirthHijri = model.DateOfBirthHijri;
                fleetDetails.CommercialRecordNumber = model.CommercialRecordNumber;
                fleetDetails.CommercialRecordIssueDateHijri = model.CommercialRecordIssueDateHijri;
                fleetDetails.PhoneNumber = model.PhoneNumber;
                fleetDetails.ExtensionNumber = model.ExtensionNumber;
                fleetDetails.EmailAddress = model.EmailAddress;
                fleetDetails.ManagerName = model.ManagerName;
                fleetDetails.ManagerPhoneNumber = model.ManagerPhoneNumber;
                fleetDetails.ManagerMobileNumber = model.ManagerMobileNumber;
                fleetDetails.UpdatedBy = model.UpdatedBy;
                fleetDetails.UpdatedDate = DateTime.Now;

                //fleetDetails.RegisterInWasl = model.RegisterInWasl;
                //fleetDetails.RegisterInRaqeb = model.RegisterInRaqeb;
                fleetDetails.ActivityType = model.ActivityType;
                fleetDetails.SFDACompanyActivities = model.SFDACompanyActivities;
                fleetDetails.FleetType = model.FleetType;

                _dbContext.FleetDetails.Update(fleetDetails);
                await _dbContext.SaveChangesAsync();
            }

            return true;
        }

        public async Task<bool> AgentUpdateWaslDetailsAsync(FleetWaslModel model)
        {
            var fleetDetails = await _dbContext.FleetDetails
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.FleetId == model.FleetId);

            if (fleetDetails == null)
            {
                fleetDetails = new FleetDetails()
                {
                    FleetId = model.FleetId,
                    IdentityNumber = model.IdentityNumber,
                    DateOfBirthHijri = model.DateOfBirthHijri,
                    CommercialRecordNumber = model.CommercialRecordNumber,
                    CommercialRecordIssueDateHijri = model.CommercialRecordIssueDateHijri,
                    PhoneNumber = model.PhoneNumber,
                    ExtensionNumber = model.ExtensionNumber,
                    EmailAddress = model.EmailAddress,
                    ManagerName = model.ManagerName,
                    ManagerPhoneNumber = model.ManagerPhoneNumber,
                    ManagerMobileNumber = model.ManagerMobileNumber,
                    IsLinkedWithWasl = false,
                    CreatedBy = model.UpdatedBy,
                    CreatedDate = DateTime.Now,
                };
                await _dbContext.FleetDetails.AddAsync(fleetDetails);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                // update
                fleetDetails.IdentityNumber = model.IdentityNumber;
                fleetDetails.DateOfBirthHijri = model.DateOfBirthHijri;
                fleetDetails.CommercialRecordNumber = model.CommercialRecordNumber;
                fleetDetails.CommercialRecordIssueDateHijri = model.CommercialRecordIssueDateHijri;
                fleetDetails.PhoneNumber = model.PhoneNumber;
                fleetDetails.ExtensionNumber = model.ExtensionNumber;
                fleetDetails.EmailAddress = model.EmailAddress;
                fleetDetails.ManagerName = model.ManagerName;
                fleetDetails.ManagerPhoneNumber = model.ManagerPhoneNumber;
                fleetDetails.ManagerMobileNumber = model.ManagerMobileNumber;
                fleetDetails.UpdatedBy = model.UpdatedBy;
                fleetDetails.UpdatedDate = DateTime.Now;

                _dbContext.FleetDetails.Update(fleetDetails);
                await _dbContext.SaveChangesAsync();
            }

            return true;
        }

        public async Task<bool> IsAnyLinkedWithWaslAsync(long fleetId)
        {
            var anyLinked = false;

            anyLinked =
               await _dbContext.Warehouse.AnyAsync(x => x.FleetId == fleetId && !x.IsDeleted && x.IsLinkedWithWasl);

            return anyLinked;
        }

        public async Task<FleetDetails> UpdateFleetDetailsWaslContactInfoAsync(FleetDetails fleetDetails)
        {
            var entity = await _dbContext.FleetDetails.FindAsync(fleetDetails.Id);
            if (entity == null)
            {
                return null;
            }

            entity.ManagerName = fleetDetails.ManagerName;
            entity.ManagerPhoneNumber = fleetDetails.ManagerPhoneNumber;
            entity.ManagerMobileNumber = fleetDetails.ManagerMobileNumber;

            var updated = _dbContext.FleetDetails.Update(entity);
            await _dbContext.SaveChangesAsync();

            return updated.Entity;
        }

        public async Task<Fleet> UpdateCompanySettingsAysnc(long FleetId, string updatedBy, CompanySettingViewModel companySettingView)
        {
            var entity = await _dbContext.Fleet.FindAsync(FleetId);
            if (entity == null)
            {
                return null;
            }

            entity.LogoPhotoByte = companySettingView.LogoPhotoByte;
            entity.LogoPhotoExtention = companySettingView.LogoPhotoExtention;
            entity.UpdatedBy = updatedBy;
            entity.UpdatedDate = DateTime.Now;

            var updated = _dbContext.Fleet.Update(entity);
            await _dbContext.SaveChangesAsync();

            return updated.Entity;
        }

        public async Task<bool> AddDefualtFleetDetailsAsync(FleetDetails fleetDetails)
        {
            await _dbContext.FleetDetails.AddAsync(fleetDetails);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteFleetDetailsAsync(long fleetId)
        {
            var entity = await _dbContext.FleetDetails.Where(x => x.FleetId == fleetId).FirstOrDefaultAsync();
            if (entity == null)
            {
                return false;
            }
            entity.IsDeleted = true;
            entity.UpdatedDate = DateTime.Now;
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();

            return true;
        }
    }
}

