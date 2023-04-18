using AutoMapper;
using GPS.DataAccess.Context;

using GPS.Domain.DTO;
using GPS.Domain.Models;

using GPS.Domain.Views;
using GPS.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.DataAccess.Repository.CustomAlerts
{
   public class CustomAlertRepository: ICustomAlertRepository
    {
        private readonly TrackerDBContext _dbContext;


        public CustomAlertRepository(TrackerDBContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<CustomAlert> UpdateAsync(CustomAlert CustomAlert, long[] InvertoryIds)
        {
            await DeleteInventoryCustomAlertAsync(CustomAlert.Id);
            var _customAlert = await _dbContext.CustomAlert.Where(x => x.Id == CustomAlert.Id).Include(x => x.AlertTypeLookup).FirstOrDefaultAsync();
            if (_customAlert == null)
            {
                return null;
            }
            _customAlert.Title = CustomAlert.Title;
            _customAlert.AlertTypeLookupId = CustomAlert.AlertTypeLookupId;
            _customAlert.MinValueHumidity = CustomAlert.MinValueHumidity;
            _customAlert.MinValueTemperature = CustomAlert.MinValueTemperature;
            _customAlert.MaxValueHumidity = CustomAlert.MaxValueHumidity;
            _customAlert.MaxValueTemperature = CustomAlert.MaxValueTemperature;
            _customAlert.Interval = CustomAlert.Interval;
            if (!string.IsNullOrEmpty(CustomAlert.UserIds))
            {
                _customAlert.UserIds = CustomAlert.UserIds;
                _customAlert.ToEmails = null;
            }
            else
            {
                _customAlert.ToEmails = CustomAlert.ToEmails;
                _customAlert.UserIds = null;
            }
            _customAlert.IsActive = CustomAlert.IsActive;
            _customAlert.UpdatedBy = CustomAlert.UpdatedBy;
            _customAlert.UpdatedDate = DateTime.Now;

            var updated = _dbContext.CustomAlert.Update(_customAlert);
            await _dbContext.SaveChangesAsync();

            await AddInventoryCustomAlertAsync(_customAlert.Id, InvertoryIds);
            return updated.Entity;
        }
        public async Task<CustomAlert> AddAsync(CustomAlert CustomAlert, long[] InvertoryIds)
        {
            CustomAlert.CreatedDate = DateTime.Now;
            if (!string.IsNullOrEmpty(CustomAlert.UserIds))
            {
                CustomAlert.UserIds = CustomAlert.UserIds;
                CustomAlert.ToEmails = null;
            }
            else
            {
                CustomAlert.ToEmails = CustomAlert.ToEmails;
                CustomAlert.UserIds = null;
            }

            var added = await _dbContext.CustomAlert.AddAsync(CustomAlert);
            await _dbContext.SaveChangesAsync();
            await AddInventoryCustomAlertAsync(added.Entity.Id, InvertoryIds);
            return added.Entity;
        }

        private async Task<bool> AddInventoryCustomAlertAsync(long CustomAlertId, long[] InvertoryIds)
        {
            // add InventoryCustomAlert
            List<InventoryCustomAlert> LsInventoryCustomAlert = new List<InventoryCustomAlert>();
            InvertoryIds.ToList().ForEach(x => LsInventoryCustomAlert.Add(new InventoryCustomAlert { InventorytId = x, CustomAlertId = CustomAlertId }));
            await _dbContext.InventoryCustomAlert.AddRangeAsync(LsInventoryCustomAlert);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        private async Task<bool> DeleteInventoryCustomAlertAsync(long CustomAlertId)
        {
            var LsInventoryCustomAlert = await _dbContext.InventoryCustomAlert.Where(x => x.CustomAlertId == CustomAlertId).ToListAsync();
            _dbContext.InventoryCustomAlert.RemoveRange(LsInventoryCustomAlert);
            return true;
        }
        public async Task<CustomAlert> DeleteAsync(long Id, string UpdatedBy)
        {
            await DeleteInventoryCustomAlertAsync(Id);
            var customAlert = await _dbContext.CustomAlert.FindAsync(Id);
            if (customAlert == null)
            {
                return null;
            }

            customAlert.IsDeleted = true;
            customAlert.UpdatedBy = UpdatedBy;
            customAlert.UpdatedDate = DateTime.Now;

            var updated = _dbContext.CustomAlert.Update(customAlert);
            await _dbContext.SaveChangesAsync();

            return updated.Entity;
        }

        public async Task<List<Inventory>> GetCustomAlertInventoriesAsync(long CustomAlertId)
        {
            var inventories = new List<Inventory>();
            var LsInventoryCustomAlert = await _dbContext.InventoryCustomAlert.Where(x => x.CustomAlertId == CustomAlertId).ToListAsync();
            for(var i = 0; i < LsInventoryCustomAlert.Count; i++)
            {
                var inventory = await _dbContext.Inventory
                    .Include(x => x.Warehouse).FirstOrDefaultAsync(x => x.Id == LsInventoryCustomAlert[i].InventorytId && x.IsActive);
               if(inventory != null)
                {
                    inventories.Add(inventory);
                }
            }
            return inventories;
        }

        public async Task<List<CustomAlert>> SearchAsync(long FleetId, long? WarehouseId = null, long? InventoryId = null, int? IsActive = null, string SearchString = "")
        {
            var CustomAlertIds = new List<long>();
            bool active = IsActive == 1;

            if (InventoryId > 0)
            {
                CustomAlertIds = await _dbContext.InventoryCustomAlert.Where(x => x.InventorytId == InventoryId)
                    .AsNoTracking().Select(x => x.CustomAlertId).ToListAsync();
            }
            if ((WarehouseId > 0 && (!InventoryId.HasValue || InventoryId == 0)))
            {
               var inventoryIds = await _dbContext.Inventory.Where(x => x.WarehouseId == WarehouseId)
                    .AsNoTracking().Select(x => x.Id).ToListAsync();
                CustomAlertIds = await _dbContext.InventoryCustomAlert.Where(x => inventoryIds.Contains(x.InventorytId))
                    .AsNoTracking().Select(x => x.CustomAlertId).ToListAsync();
            }
           return await _dbContext.CustomAlert.Where(x => !x.IsDeleted && (x.Title.Contains(SearchString) ||
            string.IsNullOrEmpty(SearchString)) && (!IsActive.HasValue || x.IsActive == active) && x.FleetId == FleetId &&
            (CustomAlertIds.Contains(x.Id) || !InventoryId.HasValue))
                .OrderByDescending(x => x.CreatedDate).AsNoTracking().ToListAsync();
        }

        public async Task<CustomAlert> FindAsync(long CustomAleryId)
        {
            return await _dbContext.CustomAlert.FindAsync(CustomAleryId);
        }

        public async Task<List<CustomAlert>> GetAllActiveAsync()
        {
            return await _dbContext.CustomAlert
                .Where(x => x.IsActive && !x.IsDeleted)
                .AsNoTracking()
                .Include(x => x.AlertTypeLookup)
                .ToListAsync();
        }
    }
}
