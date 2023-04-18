using GPS.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GPS.DataAccess.Repository.Inventorys
{
    public interface IInventorySensorRepository
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inventoryId"></param>
        /// <returns></returns>
        Task<List<InventorySensor>> GetInventorySensorList(long inventoryId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inventoryId"></param>
        /// <param name="sensorSerial"></param>
        /// <returns></returns>
        Task<InventorySensor> GetInventorySensor(long inventoryId, string sensorSerial);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serial"></param>
        /// <returns></returns>
        Task<InventorySensor> GetBasicBySerial(string serial);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inventoryId"></param>
        /// <returns></returns>
        Task<List<InventorySensor>> GetByInventoryId(long inventoryId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inventoryId"></param>
        /// <returns></returns>
        Task<List<InventorySensor>> GetBasicByInventoryId(long inventoryId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sensorId"></param>
        /// <returns></returns>
        Task<bool> IsInventorySensorExistsAsync(long sensorId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sensorId"></param>
        /// <param name="updatedBy"></param>
        /// <returns></returns>
        Task<InventorySensor> DeleteSensorFromInventoryAsync(long sensorId, string updatedBy);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sensorIds"></param>
        /// <returns></returns>
        Task<List<InventorySensor>> GetListInventorySensor(List<long> sensorIds);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inventoryIds"></param>
        /// <returns></returns>
        Task<List<InventorySensor>> GetListInventorySensorByInventoryIdsAsync(List<long> inventoryIds);
    }
}
