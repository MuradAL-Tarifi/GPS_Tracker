using Dapper;
using GPS.DataAccess.Context;
using GPS.Domain.DTO;
using GPS.Domain.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.DataAccess.Repository.Inventorys
{
    public class OnlineInventoryHistoryRepository : IOnlineInventoryHistoryRepository
    {
        private readonly TrackerDBContext _dbContext;
        private readonly AppSettings _appSettings;
        public OnlineInventoryHistoryRepository(TrackerDBContext dbContext, AppSettings appSettings)
        {
            _dbContext = dbContext;
            _appSettings = appSettings;
        }
        public async Task<int> InsertAsync(OnlineInventoryHistory onlineInventoryHistory)
        {
            using (var connection = new SqlConnection(_appSettings.ConnectionStrings.DefaultConnection))
            {
                var sql = @"insert into OnlineInventoryHistory (GatewayIMEI, Serial, Temperature, Humidity, IsLowVoltage, GpsDate, Alram, GSMStatus) values 
                          (@GatewayIMEI, @Serial, @Temperature, @Humidity, @IsLowVoltage, @GpsDate, @Alram, @GSMStatus)";

                var parameters = new DynamicParameters();
                parameters.Add("Serial", onlineInventoryHistory.Serial);
                parameters.Add("GatewayIMEI", onlineInventoryHistory.GatewayIMEI);
                parameters.Add("Temperature", onlineInventoryHistory.Temperature);
                parameters.Add("Humidity", onlineInventoryHistory.Humidity);
                parameters.Add("IsLowVoltage", onlineInventoryHistory.IsLowVoltage);
                parameters.Add("GpsDate", onlineInventoryHistory.GpsDate);
                parameters.Add("Alram", onlineInventoryHistory.Alram);
                parameters.Add("GSMStatus", onlineInventoryHistory.GSMStatus);

                return await connection.ExecuteAsync(sql, parameters);
            }
        }

        public async Task<int> UpdateAsync(OnlineInventoryHistory onlineInventoryHistory)
        {
            using (var connection = new SqlConnection(_appSettings.ConnectionStrings.DefaultConnection))
            {
                var sql = $@"update OnlineInventoryHistory set 
                            Temperature=@Temperature, 
                            Humidity=@Humidity, 
                            IsLowVoltage=@IsLowVoltage, 
                            GpsDate=@GpsDate, 
                            Alram=@Alram, 
                            GSMStatus=@GSMStatus                            
                            where Serial=@Serial";

                var parameters = new DynamicParameters();
                parameters.Add("Serial", onlineInventoryHistory.Serial);
                parameters.Add("GatewayIMEI", onlineInventoryHistory.GatewayIMEI);
                parameters.Add("Temperature", onlineInventoryHistory.Temperature);
                parameters.Add("Humidity", onlineInventoryHistory.Humidity);
                parameters.Add("IsLowVoltage", onlineInventoryHistory.IsLowVoltage);
                parameters.Add("GpsDate", onlineInventoryHistory.GpsDate);
                parameters.Add("Alram", onlineInventoryHistory.Alram);
                parameters.Add("GSMStatus", onlineInventoryHistory.GSMStatus);

                return await connection.ExecuteAsync(sql, parameters);
            }
        }

        public async Task<OnlineInventoryHistory> GetBySensorSerialAsync(string sensorSerial)
        {
            using (var connection = new SqlConnection(_appSettings.ConnectionStrings.DefaultConnection))
            {
                var sql = "select* from OnlineInventoryHistory where Serial = @Serial";

                var parameters = new DynamicParameters();
                parameters.Add("Serial", sensorSerial);

                return await connection.QueryFirstOrDefaultAsync<OnlineInventoryHistory>(sql, parameters);
            }
        }

        public async Task<(List<InventorySensor>, List<OnlineInventoryHistory>)> SearchAsync(long fleetId, long? warehouseId, long? inventoryId)
        {
            var sensorList = await _dbContext.InventorySensor
                .Include(x => x.Inventory).ThenInclude(x => x.Warehouse).ThenInclude(x => x.Fleet)
                .Include(x => x.Inventory).ThenInclude(x => x.Gateway)
                .Include(x => x.Sensor)
                .Where(x => !x.IsDeleted && x.Inventory.Warehouse.FleetId == fleetId &&
                (!warehouseId.HasValue || x.Inventory.WarehouseId == warehouseId) &&
                (!inventoryId.HasValue || x.InventoryId == inventoryId))
                .AsNoTracking()
                .ToListAsync();

            var onlineHistory = await _dbContext.OnlineInventoryHistory.Where(x => sensorList.Select(c => c.Sensor.Serial).Contains(x.Serial)).ToListAsync();
            return (sensorList, onlineHistory);
        }
    }
}
