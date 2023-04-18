using Dapper;
using GPS.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace GPS.API.Server.Services
{
    public interface IDapperRepository
    {
        Task<int> InsertInventoryHistoryAsync(InventoryHistoryView inventoryHistoryView);
        Task<OnlineInventoryHistory> GetInventoryHistoryBySensorSerialAsync(string sensorSerial);
        Task<int> InsertOnlineInventoryHistoryAsync(OnlineInventoryHistory onlineInventoryHistory);
        Task<int> UpdateOnlineInventoryHistoryAsync(OnlineInventoryHistory onlineInventoryHistory);
        Task<InventorySensor> GetSensorBySerial(string Serial);
        //Task<List<SensorAlert>> GetSensorAlertsBySerial(string Serial);
        Task<Inventory> GetInventoryById(long Id);
        Task<List<string>> GetSensorsSerialByInventoryId(long InventoryId);
        Task<string> GetSensorSN(string Serial);
    }
    public class DapperRepository : IDapperRepository
    {
        private readonly IConfiguration _configuration;

        public DapperRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<int> InsertInventoryHistoryAsync(InventoryHistoryView inventoryHistoryView)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("HistoryConnection")))
            {
                var sql = @"insert into InventoryHistory (InventoryId, GatewayIMEI, Serial, Temperature, Humidity, IsLowVoltage, GpsDate, Alram, GSMStatus) values 
                          (@InventoryId, @GatewayIMEI, @Serial, @Temperature, @Humidity, @IsLowVoltage, @GpsDate, @Alram, @GSMStatus)";

                var parameters = new DynamicParameters();
                parameters.Add("InventoryId", inventoryHistoryView.InventoryId);
                parameters.Add("GatewayIMEI", inventoryHistoryView.GatewayIMEI);
                parameters.Add("Serial", inventoryHistoryView.Serial);
                parameters.Add("Temperature", inventoryHistoryView.Temperature);
                parameters.Add("Humidity", inventoryHistoryView.Humidity);
                parameters.Add("IsLowVoltage", inventoryHistoryView.IsLowVoltage);
                parameters.Add("GpsDate", inventoryHistoryView.GpsDate);
                parameters.Add("Alram", inventoryHistoryView.Alram);
                parameters.Add("GSMStatus", inventoryHistoryView.GSMStatus);

                return await connection.ExecuteAsync(sql, parameters);
            }
        }
        public async Task<OnlineInventoryHistory> GetInventoryHistoryBySensorSerialAsync(string sensorSerial)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var sql = "select * from OnlineInventoryHistory where LOWER(TRIM(Serial)) = @Serial";

                var parameters = new DynamicParameters();
                parameters.Add("Serial", sensorSerial.ToLower());

                return await connection.QueryFirstOrDefaultAsync<OnlineInventoryHistory>(sql, parameters);
            }
        }
        public async Task<int> InsertOnlineInventoryHistoryAsync(OnlineInventoryHistory onlineInventoryHistory)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
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
        public async Task<int> UpdateOnlineInventoryHistoryAsync(OnlineInventoryHistory onlineInventoryHistory)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
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
        public async Task<InventorySensor> GetSensorBySerial(string Serial)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                //var sql = @"SELECT Id, InventoryId, Serial, Name
                //                FROM InventorySensor
                //                WHERE IsDeleted = 0 And Serial = @Serial";

                var sql = @"select InventorySensor.Id as Id,InventorySensor.InventoryId as InventoryId, Sensor.Serial as Serial,
               Sensor.Name as Name from InventorySensor inner join Sensor on Sensor.Id = InventorySensor.SensorId 
               WHERE InventorySensor.IsDeleted = 0 And Sensor.IsDeleted = 0 And LOWER(TRIM(Sensor.Serial)) = @Serial";

                var parameters = new DynamicParameters();
                parameters.Add("Serial", Serial.ToLower());

                var sensor = await connection.QueryFirstOrDefaultAsync<InventorySensor>(sql, parameters);

                //if (sensor != null)
                //{
                //    sensor.SensorAlerts = await GetSensorAlertsBySerial(sensor.Serial);
                //}

                return sensor;
            }
        }
        //public async Task<List<SensorAlert>> GetSensorAlertsBySerial(string Serial)
        //{
        //    using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
        //    {
        //        var sql = @"SELECT Id,InventorySensorId, SensorAlertTypeLookupId, IsActive, IsSMS, IsEmail, FromValue, ToValue
        //                        FROM SensorAlert
        //                        WHERE IsActive = 1 And InventorySensorId = @Serial";

        //        var parameters = new DynamicParameters();
        //        parameters.Add("Serial", Serial);

        //        var _result = await connection.QueryAsync<SensorAlert>(sql, parameters);

        //        return _result.ToList();
        //    }
        //}
        public async Task<Inventory> GetInventoryById(long Id)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var sql = @"SELECT *
                                FROM Inventory
                                WHERE IsDeleted = 0 And Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("Id", Id);

                var inventory = await connection.QueryFirstOrDefaultAsync<Inventory>(sql, parameters);
                return inventory;
            }
        }

        public async Task<List<string>> GetSensorsSerialByInventoryId(long InventoryId)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var sql = @"select Sensor.Serial from InventorySensor inner join Sensor on Sensor.Id = InventorySensor.SensorId 
               WHERE InventorySensor.IsDeleted = 0 And InventorySensor.InventoryId = @InventoryId";

                var parameters = new DynamicParameters();
                parameters.Add("InventoryId", InventoryId);

                var _result = await connection.QueryAsync<string>(sql, parameters);

                return _result.ToList();
            }
        }

        public async Task<string> GetSensorSN(string Serial)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var sql = @"select Sensor.Serial from Sensor where IsDeleted = 0 And LOWER(TRIM(Sensor.Serial)) = @Serial";
                var parameters = new DynamicParameters();
                parameters.Add("Serial", Serial.ToLower());

                return await connection.QueryFirstOrDefaultAsync<string>(sql, parameters);
            }
        }
    }
}
