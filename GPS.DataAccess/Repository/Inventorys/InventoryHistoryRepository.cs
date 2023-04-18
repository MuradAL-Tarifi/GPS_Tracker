using Dapper;
using GPS.Domain.DTO;
using GPS.Domain.Models;
using GPS.Domain.Views;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GPS.DataAccess.Repository.Inventorys
{
    public class InventoryHistoryRepository : IInventoryHistoryRepository
    {
        private readonly AppSettings _appSettings;

        public InventoryHistoryRepository(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public async Task<int> InsertAsync(InventoryHistoryView inventoryHistoryView)
        {
            using (var connection = new SqlConnection(_appSettings.ConnectionStrings.HistoryConnection))
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

        public async Task<List<InventoryHistory>> GetByInventoryIdAsync(long InventoryId, DateTime fromDate, DateTime toDate)
        {
            using (var connection = new SqlConnection(_appSettings.ConnectionStrings.HistoryConnection))
            {
                var parameters = new DynamicParameters();
                parameters.Add("InventoryId", InventoryId);
                parameters.Add("FromDate", fromDate);
                parameters.Add("ToDate", toDate);

                var result = await connection.QueryAsync<InventoryHistory>("InventoryHistorySelectByFilter", parameters, commandType: System.Data.CommandType.StoredProcedure);
                return result.ToList();
            }
        }

        public async Task<List<InventoryHistory>> GetByInventoryIdAndSensorSerialAsync(long InventoryId, string sensorSerial, DateTime fromDate, DateTime toDate)
        {
            using (var connection = new SqlConnection(_appSettings.ConnectionStrings.HistoryConnection))
            {
                var parameters = new DynamicParameters();
                parameters.Add("InventoryId", InventoryId);
                parameters.Add("FromDate", fromDate);
                parameters.Add("ToDate", toDate);

                var result = await connection.QueryAsync<InventoryHistory>("InventoryHistorySelectByFilter", parameters, commandType: System.Data.CommandType.StoredProcedure);

                return result.Where(x => x.Serial == sensorSerial).ToList();
            }
        }

        public async Task<List<InventoryHistory>> GetBySensorsSerialsAsync(List<string> sensorSerial, DateTime fromDate, DateTime toDate)
        {
            using (var connection = new SqlConnection(_appSettings.ConnectionStrings.HistoryConnection))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@sensors_serials", string.Join(",", sensorSerial));
                parameters.Add("FromDate", fromDate);
                parameters.Add("ToDate", toDate);

                var result = await connection.QueryAsync<InventoryHistory>("InventoryHistoryBySensorsSerialsByFilter", parameters,commandTimeout:300, commandType: System.Data.CommandType.StoredProcedure);

                return result.ToList();
            }
            
        }
    }
}
