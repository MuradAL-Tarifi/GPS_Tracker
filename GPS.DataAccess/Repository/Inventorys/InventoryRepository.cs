using GPS.DataAccess.Context;
using GPS.Domain.DTO;
using GPS.Domain.Models;
using GPS.Domain.Views;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace GPS.DataAccess.Repository.Inventorys
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly TrackerDBContext _dbContext;

        public InventoryRepository(TrackerDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PagedResult<Inventory>> SearchAsync(long? FleetId, long? WarehouseId, int? waslLinkStatus, int? IsActive, string SearchString, int PageNumber, int pageSize)
        {
            var pagedList = new PagedResult<Inventory>();
            var skip = (PageNumber - 1) * pageSize;
            bool isWaslLinked = waslLinkStatus == 1;
            bool isActive = IsActive == 1;

            pagedList.TotalRecords = await _dbContext.Inventory.Where(x => !x.IsDeleted &&
            (x.IsLinkedWithWasl == isWaslLinked || !waslLinkStatus.HasValue) &&
            (x.IsActive == isActive || !IsActive.HasValue) &&
            (!FleetId.HasValue || x.Warehouse.FleetId == FleetId) &&
            (!WarehouseId.HasValue || x.WarehouseId == WarehouseId) &&
            (string.IsNullOrEmpty(SearchString) || (x.Name.Contains(SearchString) || x.Gateway.Name.Contains(SearchString))))
                .CountAsync();

            pagedList.List = await _dbContext.Inventory.Where(x => !x.IsDeleted &&
            (x.IsLinkedWithWasl == isWaslLinked || !waslLinkStatus.HasValue) &&
            (x.IsActive == isActive || !IsActive.HasValue) &&
            (!FleetId.HasValue || x.Warehouse.FleetId == FleetId) &&
            (!WarehouseId.HasValue || x.WarehouseId == WarehouseId) &&
            (string.IsNullOrEmpty(SearchString) || (x.Name.Contains(SearchString)) || x.Gateway.Name.Contains(SearchString)))
                .OrderByDescending(x => x.CreatedDate)
                .Skip(skip).Take(pageSize)
                .Include(x => x.Warehouse).ThenInclude(x => x.Fleet)
                .Include(x => x.Gateway)
                //.Include(x => x.RegisterType)
                .AsNoTracking().ToListAsync();

            return pagedList;
        }

        public async Task<Inventory> FindByIdAsync(long? Id)
        {
            var inventory = await _dbContext.Inventory
                .Include(x => x.Warehouse).ThenInclude(x => x.Fleet)

                .Include(x => x.Gateway)
                //.Include(x => x.RegisterType)
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == Id);

            return inventory;
        }

        public async Task<Inventory> AddAsync(Inventory inventory, List<InventorySensor> list)
        {
            inventory.CreatedDate = DateTime.Now;

            var added = await _dbContext.Inventory.AddAsync(inventory);
            await _dbContext.SaveChangesAsync();

            list.ForEach(x => { x.InventoryId = added.Entity.Id; x.CreatedDate = DateTime.Now; });
            await AddInventorySensorsAsync(list);

            return added.Entity;
        }

        public async Task<bool> AddInventorySensorsAsync(List<InventorySensor> list)
        {
            await _dbContext.InventorySensor.AddRangeAsync(list);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<Inventory> UpdateAsync(InventoryView model)
        {
            var inventory = await _dbContext.Inventory.FirstOrDefaultAsync(x => x.Id == model.Id);

            if (inventory == null)
            {
                return null;
            }

            inventory.WarehouseId = model.WarehouseId;
            inventory.GatewayId = model.GatewayId;
            inventory.GatewayId = model.GatewayId;
            //inventory.RegisterTypeId = model.RegisterTypeId;
            inventory.InventoryNumber = model.InventoryNumber;
            inventory.IsActive = model.IsActive;

            if (!inventory.IsLinkedWithWasl)
            {
                inventory.Name = model.Name;
                inventory.SFDAStoringCategory = model.SFDAStoringCategory;
            }

            var updated = _dbContext.Inventory.Update(inventory);
            await _dbContext.SaveChangesAsync();

            // update old 
            var inventorySensors = await _dbContext.InventorySensor.Where(x => x.InventoryId == model.Id).ToListAsync();
            foreach (var item in inventorySensors)
            {
                item.IsDeleted = true;
                item.UpdatedBy = model.UpdatedBy;
                item.UpdatedDate = DateTime.Now;
            }
            _dbContext.InventorySensor.UpdateRange(inventorySensors);
            await _dbContext.SaveChangesAsync();

            // add new
            foreach (var item in model.InventorySensors)
            {
                var inventorySensor = new InventorySensor()
                {
                    InventoryId = model.Id,
                    SensorId = item.SensorId,
                    CreatedBy = model.UpdatedBy,
                    CreatedDate = DateTime.Now
                };

                await _dbContext.InventorySensor.AddAsync(inventorySensor);
            }

            await _dbContext.SaveChangesAsync();

            return updated.Entity;
        }

        public async Task<Inventory> DeleteAsync(long Id, string UpdatedBy)
        {

            var inventory = await _dbContext.Inventory.FindAsync(Id);
            if (inventory == null)
            {
                return null;
            }

            inventory.IsDeleted = true;
            inventory.UpdatedBy = UpdatedBy;
            inventory.UpdatedDate = DateTime.Now;

            var updated = _dbContext.Inventory.Update(inventory);
            await _dbContext.SaveChangesAsync();

            var inventorySensors = await _dbContext.InventorySensor.Where(x => x.InventoryId == Id).ToListAsync();
            foreach (var item in inventorySensors)
            {
                item.IsDeleted = true;
                item.UpdatedBy = UpdatedBy;
                item.UpdatedDate = DateTime.Now;
            }

            _dbContext.InventorySensor.UpdateRange(inventorySensors);
            await _dbContext.SaveChangesAsync();

            return updated.Entity;
        }

        public async Task<bool> UpdateLinkedWithWaslInfoAsync(Inventory inventory)
        {
            var entity = await _dbContext.Inventory.FirstOrDefaultAsync(x => x.Id == inventory.Id);
            if (entity == null)
            {
                return false;
            }

            entity.IsLinkedWithWasl = inventory.IsLinkedWithWasl;
            entity.UpdatedBy = inventory.UpdatedBy;
            entity.UpdatedDate = inventory.UpdatedDate;
            entity.ReferenceKey = inventory.ReferenceKey;

            _dbContext.Inventory.Update(entity);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<List<Inventory>> GetByWarehouseIdAsync(long warehouseId)
        {
            var inventories = await _dbContext.Inventory
                .Where(x => !x.IsDeleted && x.WarehouseId == warehouseId)
                .AsNoTracking()
                .ToListAsync();

            return inventories;
        }

        public async Task<bool> IsAnyLinkedWithWaslAsync(long warehouseId)
        {
            return await _dbContext.Inventory.AnyAsync(x => x.WarehouseId == warehouseId && !x.IsDeleted && x.IsLinkedWithWasl);
        }

        public async Task<bool> IsInventoryNumberExists(long fleetId, string inventoryNumber)
        {
            return await _dbContext.Inventory.AnyAsync(x => x.Warehouse.FleetId == fleetId && !x.IsDeleted && x.InventoryNumber == inventoryNumber);
        }

        public async Task<bool> UpdateWaslInfoAsync(Inventory inventory)
        {
            var entity = await _dbContext.Inventory.FirstOrDefaultAsync(x => x.Id == inventory.Id);
            if (entity == null)
            {
                return false;
            }

            entity.Name = inventory.Name;
            entity.SFDAStoringCategory = inventory.SFDAStoringCategory;
            entity.UpdatedBy = inventory.UpdatedBy;
            entity.UpdatedDate = inventory.UpdatedDate;

            _dbContext.Inventory.Update(entity);
            await _dbContext.SaveChangesAsync();

            return true;
        }

        public async Task<List<Inventory>> GetWarehouseLinkedWithWaslInventoriesAsync(long warehouseId)
        {
            var inventories = await _dbContext.Inventory.Where(x => !x.IsDeleted && x.IsLinkedWithWasl && x.WarehouseId == warehouseId).ToListAsync();
            return inventories;
        }
        public async Task<List<Inventory>> GetInventoriesByWarehouseId(long warehouseId)
        {
            return await _dbContext.Inventory.Where(x => x.WarehouseId == warehouseId).ToListAsync();
        }
        public async Task<bool> IsGatewayRegisteredWithInventoryAsync(long gatewayId)
        {
            return await _dbContext.Inventory.AnyAsync(x => x.GatewayId == gatewayId);
        }

        public async Task<List<Inventory>> FindInventoriesAsync(List<long> inventoryIds)
        {
            return await _dbContext.Inventory.Where(x => inventoryIds.Any(id => id == x.Id)).ToListAsync();
        }

        public async Task<List<Inventory>> GetInventoryByFleetIdAsync(long fleetId)
        {
            return await _dbContext.Inventory.Where(x => x.Warehouse.FleetId == fleetId).Include(x => x.Warehouse).ToListAsync();
        }
        public async Task<List<Inventory>> FindInventoriesIncludingWarehousesAsync(List<long> inventoryIds) 
        {
            return await _dbContext.Inventory.Where(x => inventoryIds.Any(id => id == x.Id)).Include(x => x.Warehouse).AsNoTracking().ToListAsync();
        }
    }

}
