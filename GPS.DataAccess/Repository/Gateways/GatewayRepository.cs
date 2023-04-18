
using GPS.DataAccess.Context;
using GPS.Domain.DTO;
using GPS.Domain.Models;
using GPS.Domain.Views;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GPS.DataAccess.Repository.Gateways
{
    public class GatewayRepository : IGatewayRepository
    {
        private readonly TrackerDBContext _dbContext;

        public GatewayRepository(TrackerDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PagedResult<Gateway>> SearchAsync(string SearchString, int PageNumber, int pageSize)
        {
            var pagedList = new PagedResult<Gateway>();
            var skip = (PageNumber - 1) * pageSize;

            pagedList.TotalRecords = await _dbContext.Gateway.Where(x => !x.IsDeleted &&
            (string.IsNullOrEmpty(SearchString) || (x.Name.Contains(SearchString) 
            || x.IMEI.Contains(SearchString) || x.SIMNumber.Contains(SearchString))))
                .CountAsync();

            pagedList.List = await _dbContext.Gateway.Where(x => !x.IsDeleted &&
                 (string.IsNullOrEmpty(SearchString) || (x.Name.Contains(SearchString) 
                 || x.IMEI.Contains(SearchString) || x.SIMNumber.Contains(SearchString))))
                    .OrderByDescending(x => x.CreatedDate)
                    .Skip(skip).Take(pageSize)
                    .AsNoTracking().ToListAsync();

            return pagedList;
        }

        public async Task<Gateway> FindByIdAsync(long? Id)
        {
            var gateway = await _dbContext.Gateway.Where(x => !x.IsDeleted && x.Id == Id).AsNoTracking().FirstOrDefaultAsync();
            return gateway;
        }

        public async Task<bool> AddAsync(Gateway gateway)
        {
            await _dbContext.Gateway.AddAsync(gateway);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(GatewayView model)
        {
            var gateway = await _dbContext.Gateway.Where(x => x.Id == model.Id).AsNoTracking().FirstOrDefaultAsync();
            if (gateway == null)
            {
                return false;
            }

            gateway.Name = model.Name;
            gateway.IMEI = model.IMEI;
            gateway.BrandId = model.BrandId;
            gateway.SIMNumber = model.SIMNumber;
            gateway.ExpirationDate = model.ExpirationDate;
            gateway.IsActive = model.IsActive;
            gateway.UpdatedBy = model.UpdatedBy;
            gateway.UpdatedDate = DateTime.Now;

            gateway.ActivationDate = model.ActivationDate;
            gateway.SIMCardExpirationDate = model.SIMCardExpirationDate;
            gateway.NumberOfMonths = model.NumberOfMonths;
            gateway.Notes = model.Notes;

            _dbContext.Gateway.Update(gateway);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<Gateway> DeleteAsync(long Id, string UpdatedBy)
        {
            var gateway = await _dbContext.Gateway.FindAsync(Id);
            if (gateway == null)
            {
                return null;
            }

            gateway.IsDeleted = true;
            gateway.UpdatedBy = UpdatedBy;
            gateway.UpdatedDate = DateTime.Now;

            var deleted = _dbContext.Gateway.Update(gateway);
            await _dbContext.SaveChangesAsync();

            return deleted.Entity;
        }

        public async Task<bool> IsNameExistsAsync(string Name)
        {

            var count = await _dbContext.Gateway.Where(x => !x.IsDeleted && x.Name == Name).CountAsync();
            return count > 0;
        }

        public async Task<bool> IsIMEIExistsAsync(string IMEI)
        {
            var count = await _dbContext.Gateway.Where(x => !x.IsDeleted && x.IMEI == IMEI).CountAsync();
            return count > 0;
        }
    }
}
