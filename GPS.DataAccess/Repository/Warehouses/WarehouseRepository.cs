using GPS.DataAccess.Context;
using GPS.Domain.DTO;
using GPS.Domain.Models;
using GPS.Domain.Views;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.DataAccess.Repository.Warehouses
{
    public class WarehouseRepository : IWarehouseRepository
    {
        private readonly TrackerDBContext _dbContext;

        public WarehouseRepository(TrackerDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PagedResult<Warehouse>> SearchAsync(long? FleetId, int? waslLinkStatus, int? IsActive, string SearchString, int PageNumber, int pageSize)
        {
            bool isWaslLinked = waslLinkStatus == 1;
            bool isActive = IsActive == 1;

            var pagedList = new PagedResult<Warehouse>();
            var skip = (PageNumber - 1) * pageSize;

            pagedList.TotalRecords = await _dbContext.Warehouse.Where(x => !x.IsDeleted && (x.IsActive == isActive || !IsActive.HasValue) &&
            (!FleetId.HasValue || x.FleetId == FleetId) && (x.IsLinkedWithWasl == isWaslLinked || !waslLinkStatus.HasValue) &&
            (string.IsNullOrEmpty(SearchString) || (x.Name.Contains(SearchString))))
                .CountAsync();

            pagedList.List = await _dbContext.Warehouse.Where(x => !x.IsDeleted && (x.IsActive == isActive || !IsActive.HasValue) &&
                 (!FleetId.HasValue || x.FleetId == FleetId) && (x.IsLinkedWithWasl == isWaslLinked || !waslLinkStatus.HasValue) &&
                (string.IsNullOrEmpty(SearchString) || (x.Name.Contains(SearchString))))
                    .OrderByDescending(x => x.CreatedDate)
                    .Skip(skip).Take(pageSize)
                    .Include(x => x.Fleet)
                    //.Include(x => x.RegisterType)
                    .AsNoTracking().ToListAsync();

            return pagedList;
        }

        public async Task<Warehouse> FindByIdAsync(long? Id)
        {
            var warehouse = await _dbContext.Warehouse.Where(x => !x.IsDeleted && x.Id == Id)
                 .Include(x => x.Fleet)
               //.Include(x => x.RegisterType)
               .AsNoTracking().FirstOrDefaultAsync();

            return warehouse;
        }

        public async Task<Warehouse> AddAsync(Warehouse warehouse)
        {
            warehouse.CreatedDate = DateTime.Now;
            var added = await _dbContext.Warehouse.AddAsync(warehouse);
            await _dbContext.SaveChangesAsync();

            return added.Entity;
        }

        public async Task<Warehouse> UpdateAsync(WarehouseView model)
        {
            var warehouse = await _dbContext.Warehouse
                .Where(x => x.Id == model.Id)
                .FirstOrDefaultAsync();
            if (warehouse == null)
            {
                return null;
            }

            warehouse.FleetId = model.FleetId;



            warehouse.Latitude = model.Latitude;
            warehouse.Longitude = model.Longitude;
            warehouse.LandCoordinates = model.LandCoordinates;


            if (!warehouse.IsLinkedWithWasl)
            {
                warehouse.Name = model.Name;
                warehouse.Address = model.Address;
                warehouse.City = model.City;
                warehouse.LicenseNumber = model.LicenseNumber;
                warehouse.LicenseIssueDate = model.LicenseIssueDate;
                warehouse.LicenseExpiryDate = model.LicenseExpiryDate;
                warehouse.Phone = model.Phone;
                warehouse.ManagerMobile = model.ManagerMobile;
                warehouse.Email = model.Email;
                warehouse.LandAreaInSquareMeter = model.LandAreaInSquareMeter;
            }

            warehouse.IsActive = model.IsActive;
            warehouse.WaslActivityType = model.WaslActivityType;

            var updated = _dbContext.Update(warehouse);
            await _dbContext.SaveChangesAsync();

            return updated.Entity;
        }

        public async Task<Warehouse> DeleteAsync(long Id, string UpdatedBy)
        {
            var warehouse = await _dbContext.Warehouse.FindAsync(Id);
            if (warehouse == null)
            {
                return null;
            }

            warehouse.IsDeleted = true;
            warehouse.UpdatedBy = UpdatedBy;
            warehouse.UpdatedDate = DateTime.Now;

            var updated = _dbContext.Update(warehouse);
            await _dbContext.SaveChangesAsync();

            return updated.Entity;
        }

        public async Task<List<Warehouse>> GetByUserIdAsync(string userId)
        {
            var user = await _dbContext.User.Where(x => x.Id == userId).FirstOrDefaultAsync();
            if (user != null)
            {
                var userWarehouses = await _dbContext.UserWarehouse
               .Where(x => x.UserId == userId)
                .Select(x => x.WarehouseId)
               .ToListAsync();

                var warehouses = await _dbContext.Warehouse.Where(x => !x.IsDeleted &&
                (!user.FleetId.HasValue || x.FleetId == user.FleetId) &&
                (!userWarehouses.Any() || userWarehouses.Contains(x.Id)))
                   .AsNoTracking().ToListAsync();

                return warehouses;
            }
            else
            {
                return null;
            }
        }

        public async Task<bool> UpdateLinkedWithWaslInfoAsync(Warehouse warehouse)
        {
            var entity = await _dbContext.Warehouse.FirstOrDefaultAsync(x => x.Id == warehouse.Id);
            if (entity == null)
            {
                return false;
            }

            entity.IsLinkedWithWasl = warehouse.IsLinkedWithWasl;
            entity.UpdatedBy = warehouse.UpdatedBy;
            entity.UpdatedDate = warehouse.UpdatedDate;
            entity.ReferenceKey = warehouse.ReferenceKey;

            _dbContext.Warehouse.Update(entity);
            await _dbContext.SaveChangesAsync();

            return true;
        }


        public async Task<bool> UpdateWaslInfoAsync(Warehouse warehouse)
        {
            var entity = await _dbContext.Warehouse.FirstOrDefaultAsync(x => x.Id == warehouse.Id);
            if (entity == null)
            {
                return false;
            }

            entity.Name = warehouse.Name;
            entity.City = warehouse.City;
            entity.Address = warehouse.Address;
            entity.LicenseNumber = warehouse.LicenseNumber;
            entity.LicenseIssueDate = warehouse.LicenseIssueDate;
            entity.LicenseExpiryDate = warehouse.LicenseExpiryDate;
            entity.Phone = warehouse.Phone;
            entity.ManagerMobile = warehouse.ManagerMobile;
            entity.Email = warehouse.Email;
            entity.LandAreaInSquareMeter = warehouse.LandAreaInSquareMeter;

            entity.UpdatedBy = warehouse.UpdatedBy;
            entity.UpdatedDate = warehouse.UpdatedDate;

            _dbContext.Warehouse.Update(entity);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<List<Warehouse>> GetFleetLinkedWithWaslWarehousesAsync(long fleetId)
        {
            return await _dbContext.Warehouse.Where(x => !x.IsDeleted && x.IsLinkedWithWasl && x.FleetId == fleetId).ToListAsync();
        }
        public async Task<List<Warehouse>> FindWarehousesAsync(List<long> warehouseIds)
        {
            return await _dbContext.Warehouse.Where(x => warehouseIds.Contains(x.Id)).ToListAsync();
        }

        public async Task<List<Warehouse>> GetByFleetIdAsync(long fleetId)
        {
            return await _dbContext.Warehouse.Where(x => x.FleetId == fleetId).ToListAsync();
        }
        public async Task<List<Warehouse>> GetAllAsync()
        {
            return await _dbContext.Warehouse.Where(x => x.IsActive == true && x.IsDeleted ==false).ToListAsync();
        }
    }
}
