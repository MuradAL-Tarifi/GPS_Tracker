using GPS.Domain.DTO;
using GPS.Domain.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Services.Sensors
{
   public interface ISensorService
    {
        /// <summary>
        ///  Get Sensor, Get Sensor by BrandId/ Search Sensor by Name
        /// </summary>
        /// <param name="fleetId"></param>
        /// <param name="SensorStatus"></param>
        /// <param name="BrandId"></param>
        /// <param name="SearchString"></param>
        /// <param name="PageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<ReturnResult<PagedResult<SensorView>>> SearchAsync(long? fleetId, int? SensorStatus, int? BrandId = null, string SearchString = "", int PageNumber = 1, int pageSize = 100);

        /// <summary>
        /// Find Sensor by Id
        /// </summary>
        Task<ReturnResult<SensorView>> FindbyIdAsync(long? Id);

        /// <summary>
        /// Add or Update Sensor
        /// </summary>
        /// <param name="sensor"></param>
        Task<ReturnResult<bool>> SaveAsync(SensorView sensor);

        /// <summary>
        /// Add or Update Sensors
        /// </summary>
        /// <param name="sensorsList"></param>
        Task<ReturnResult<bool>> SaveRangeAsync(List<SensorView> sensorsList, string UserId, long? inventoryId);

        /// <summary>
        /// Delete a Sensor
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="UpdatedBy"></param>
        Task<ReturnResult<bool>> DeleteAsync(long Id, string UpdatedBy);

        /// <summary>
        /// Get Sensor Count
        /// </summary>
        Task<ReturnResult<int>> CountAsync(int? BrandId = null);

        /// <summary>
        /// Get Sensor by FleetId
        /// </summary>
        /// <param name="BrandId"></param>
        Task<ReturnResult<List<SensorView>>> GetAllAsync(int? BrandId = null);

        Task<bool> IsSensorExistsAsync(string SensorSN);

    }
}
