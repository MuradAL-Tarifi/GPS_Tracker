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
        Task<Smtpchecker> GetSMTPCheckerBySerial(string Serial);
        Task<AlertBySensor> GetAlertBySensorBySerial(string Serial);
        Task<AlertTracker> GetAlertTrackerBySerial(string Serial);
        Task<int> UpdateSMTPCheckerAsync(Smtpchecker smtpchecker);
        Task<int> InsertAlertTrackerAsync(AlertTracker alertTracker);
        Task<AlertTrakerDataLight> GetAlertTrakerDataLightAsync(string Serial, long? InventoryId, long? WarehouseId);
        Task<List<Smtpsetting>> GetSmtpsettingsAsync();
        Task<int> UpdateSmtpsettingAsync(int Id);

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
        public async Task<Smtpchecker> GetSMTPCheckerBySerial(string sensorSerial)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var sql = "select * from SMTPChecker where LOWER(TRIM(Serial)) = @Serial";

                var parameters = new DynamicParameters();
                parameters.Add("Serial", sensorSerial.ToLower());

                return await connection.QueryFirstOrDefaultAsync<Smtpchecker>(sql, parameters);
            }
        }
        public async Task<AlertBySensor> GetAlertBySensorBySerial(string sensorSerial)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var sql = "select * from AlertBySensor where LOWER(TRIM(Serial)) = @Serial";

                var parameters = new DynamicParameters();
                parameters.Add("Serial", sensorSerial.ToLower());

                return await connection.QueryFirstOrDefaultAsync<AlertBySensor>(sql, parameters);
            }
        }
        public async Task<AlertTracker> GetAlertTrackerBySerial(string sensorSerial)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var sql = "select * from AlertTracker where LOWER(TRIM(Serial)) = @Serial";

                var parameters = new DynamicParameters();
                parameters.Add("Serial", sensorSerial.ToLower());

                return await connection.QueryFirstOrDefaultAsync<AlertTracker>(sql, parameters);
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
        public async Task<int> UpdateSMTPCheckerAsync(Smtpchecker smtpchecker)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var sql = $@"Update SMTPChecker
                                    SET 
                                    Serial =@Serial ,
                                    IsSendHumidity=@IsSendHumidity,
                                    IsSendHumiditySecond=@IsSendHumiditySecond,
                                    IsSendTemperature=@IsSendTemperature,
                                    IsSendTemperatureSecond=@IsSendTemperatureSecond,
                                    UpdatedDateHumidity=@UpdatedDateHumidity,
                                    UpdatedDateTemperature=@UpdatedDateTemperature
                                    where Serial =@Serial ";

                var parameters = new DynamicParameters();
                parameters.Add("Serial", smtpchecker.Serial);
                parameters.Add("IsSendHumidity", smtpchecker.IsSendHumidity);
                parameters.Add("IsSendHumiditySecond", smtpchecker.IsSendHumiditySecond);
                parameters.Add("IsSendTemperature", smtpchecker.IsSendTemperature);
                parameters.Add("IsSendTemperatureSecond", smtpchecker.IsSendTemperatureSecond);
                parameters.Add("UpdatedDateHumidity", smtpchecker.UpdatedDateHumidity);
                parameters.Add("UpdatedDateTemperature", smtpchecker.UpdatedDateTemperature);

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
        public async Task<int> InsertAlertTrackerAsync(AlertTracker alertTracker)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var sql = @"Insert into AlertTracker values 
                                                    (@UserName,@AlertDateTime,@AlertType,@MonitoredUnit,@MessageForValue,
                                                    @Serial,@Zone,@WarehouseName,@SendTo,@IsSend,@AlertId,@Interval);";

                var parameters = new DynamicParameters();
                parameters.Add("UserName", alertTracker.UserName);
                parameters.Add("AlertDateTime", alertTracker.AlertDateTime);
                parameters.Add("AlertType", alertTracker.AlertType);
                parameters.Add("MonitoredUnit", alertTracker.MonitoredUnit);
                parameters.Add("MessageForValue", alertTracker.MessageForValue);
                parameters.Add("Serial", alertTracker.Serial);
                parameters.Add("Zone", alertTracker.Zone);
                parameters.Add("WarehouseName", alertTracker.WarehouseName);
                parameters.Add("SendTo", alertTracker.SendTo);
                parameters.Add("IsSend", alertTracker.IsSend);
                parameters.Add("AlertId", alertTracker.AlertId);
                parameters.Add("Interval", alertTracker.Interval);

                return await connection.ExecuteAsync(sql, parameters);
            }
        }
        public async Task<AlertTrakerDataLight> GetAlertTrakerDataLightAsync(string Serial, long? InventoryId, long? WarehouseId)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                AlertTrakerDataLight alertTrakerDataLight=new AlertTrakerDataLight();
                var sql = @"select Name + '( '+Serial +' )' from Sensor
                                                            where Serial = @Serial";
                var parameters = new DynamicParameters();
                parameters.Add("Serial", Serial.ToLower());

                alertTrakerDataLight.MonitoredUnit= await connection.QueryFirstOrDefaultAsync<string>(sql, parameters);


                var sql2 = @"select f.NameEn 
                                            from Inventory i , Warehouse w, Fleet f
                                            where i.WarehouseId=w.Id and w.FleetId=f.Id and 
                                            i.Id = @InventoryId";
                var parameters2 = new DynamicParameters();
                parameters2.Add("InventoryId", InventoryId);

                alertTrakerDataLight.Zone = await connection.QueryFirstOrDefaultAsync<string>(sql2, parameters2);


                var sql3 = @"select Name
                                        from  Warehouse
                                        where Id = @WarehouseId";
                var parameters3 = new DynamicParameters();
                parameters3.Add("WarehouseId", WarehouseId);

                alertTrakerDataLight.WarehouseName = await connection.QueryFirstOrDefaultAsync<string>(sql3, parameters3);

                return alertTrakerDataLight;
            }
        }

        public async Task<List<Smtpsetting>> GetSmtpsettingsAsync()
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var sql = @"Select * from SMTPSetting where CurrentEmailNumber <100";


                var smtpsetting = await connection.QueryAsync<Smtpsetting>(sql);
                return smtpsetting.ToList();
            }
        }

        public async Task<int> UpdateSmtpsettingAsync(int Id)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var sql = @"Update SMTPSetting
                                        set CurrentEmailNumber=((select  CurrentEmailNumber from SMTPSetting where Id=@Id) + 1)
                                        where Id = @Id";

                var parameters = new DynamicParameters();
                parameters.Add("Id", Id);


                return await connection.ExecuteAsync(sql, parameters);
            }
        }
    }
}
