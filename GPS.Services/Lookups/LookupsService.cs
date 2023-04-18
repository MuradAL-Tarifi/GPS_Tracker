using AutoMapper;
using ClosedXML.Excel;
using GPS.DataAccess.Repository.UnitOfWork;
using GPS.Domain.DTO;
using GPS.Domain.ViewModels;
using GPS.Domain.Views;
using GPS.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Services.Lookups
{
    public class LookupsService : ILookupsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<LookupsService> _logger;
        private readonly IHostEnvironment _hostEnvironment;
        public LookupsService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<LookupsService> logger,
            IHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _hostEnvironment = hostEnvironment;
        }

        public async Task<ReturnResult<List<AlertTypeLookupView>>> GetAlertTypesAsync()
        {
            var result = new ReturnResult<List<AlertTypeLookupView>>();
            try
            {
                var alertType = await _unitOfWork.LookupsRepository.GetAlertTypesAsync();
                result.Success(_mapper.Map<List<AlertTypeLookupView>>(alertType));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        public async Task<ReturnResult<AdminDashboardView>> GetAdminDasboardAsync()
        {
            var result = new ReturnResult<AdminDashboardView>();
            try
            {
                var dashboard = await _unitOfWork.LookupsRepository.GetAdminDasboardAsync();
                result.Success(dashboard);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        public async Task<ReturnResult<AgentDashboardView>> GetAgentDasboardAsync(string userId)
        {
            var result = new ReturnResult<AgentDashboardView>();
            try
            {
                var dashboard = await _unitOfWork.LookupsRepository.GetAgentDasboardAsync(userId);
                result.Success(dashboard);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        public async Task<WaslCodeView> GetWaslResultCode(string code)
        {
            var entity = await _unitOfWork.LookupsRepository.GetWaslResultCode(code);
            if (entity != null)
            {
                return _mapper.Map<WaslCodeView>(entity);
            }
            else
            {
                return new WaslCodeView()
                {
                    MessageAr = "خطأ غير معروف",
                    MessageEn = "Unknown Error"
                };
            }
        }

        public async Task<ReturnResult<List<AlertTypeLookupView>>> GetCustomAlertTypesAsync()
        {
            var result = new ReturnResult<List<AlertTypeLookupView>>();
            try
            {
                var properties = await _unitOfWork.LookupsRepository.GetCustomAlertTypesAsync();
                result.Success(_mapper.Map<List<AlertTypeLookupView>>(properties));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        public async Task<ReturnResult<List<GatewayView>>> GetGatewaysAsync()
        {
            var result = new ReturnResult<List<GatewayView>>();
            try
            {
                var properties = await _unitOfWork.LookupsRepository.GetGatewaysAsync();
                result.Success(_mapper.Map<List<GatewayView>>(properties));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        public async Task<ReturnResult<List<WarehouseView>>> GetWarehousesAsync(long? FleetId)
        {
            var result = new ReturnResult<List<WarehouseView>>();
            try
            {
                var warehouses = await _unitOfWork.LookupsRepository.GetWarehousesAsync(FleetId);
                result.Success(_mapper.Map<List<WarehouseView>>(warehouses));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }
        public async Task<ReturnResult<List<WarehouseView>>> GetWarehousesAndInventoriesAsync(long? FleetId)
        {
            var result = new ReturnResult<List<WarehouseView>>();
            try
            {
                var warehousesView = _mapper.Map<List<WarehouseView>>(await _unitOfWork.LookupsRepository.GetWarehousesAsync(FleetId));
                foreach (var warehouseView in warehousesView)
                {
                    warehouseView.Inventories = _mapper.Map<List<InventoryView>>(await _unitOfWork.LookupsRepository.GetInventoriesAsync(warehouseView.Id));
                }
                result.Success(warehousesView);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        public async Task<ReturnResult<List<InventoryView>>> GetInventoriesAsync(long WarehouseId)
        {
            var result = new ReturnResult<List<InventoryView>>();
            try
            {
                var inventories = await _unitOfWork.LookupsRepository.GetInventoriesAsync(WarehouseId);

                result.Success(_mapper.Map<List<InventoryView>>(inventories));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        public async Task<ReturnResult<List<InventorySensorView>>> GetInventorySensorsAsync(long InventoryId)
        {
            var result = new ReturnResult<List<InventorySensorView>>();
            try
            {
                var inventorySensors = await _unitOfWork.LookupsRepository.GetInventorySensorsAsync(InventoryId);
                result.Success(_mapper.Map<List<InventorySensorView>>(inventorySensors));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        public async Task<ReturnResult<List<ReportTypeLookupView>>> GetReportTypesAsync()
        {
            var result = new ReturnResult<List<ReportTypeLookupView>>();
            try
            {
                var properties = await _unitOfWork.LookupsRepository.GetReportTypesAsync();
                result.Success(_mapper.Map<List<ReportTypeLookupView>>(properties));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        public async Task<ReturnResult<List<DayOfWeekLookupView>>> GetDaysOfWeekAsync()
        {
            var result = new ReturnResult<List<DayOfWeekLookupView>>();
            try
            {
                var properties = await _unitOfWork.LookupsRepository.GetDaysOfWeekAsync();
                result.Success(_mapper.Map<List<DayOfWeekLookupView>>(properties));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        //public async Task<ReturnResult<List<InventoryView>>> GetInventoriesByUserIdAsync(string UserId)
        //{
        //    var result = new ReturnResult<List<InventoryView>>();
        //    try
        //    {
        //        var inventories = await _unitOfWork.LookupsRepository.GetInventoriesByUserIdAsync(UserId);

        //        result.Success(_mapper.Map<List<InventoryView>>(inventories));
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, ex.Message, result);
        //        result.ServerError(ex.Message);
        //    }
        //    return result;
        //}

        public async Task<ReturnResult<List<WarehouseView>>> GetWarehousesByUserIdAsync(string UserId)
        {
            var result = new ReturnResult<List<WarehouseView>>();
            var warehouses = new List<WarehouseView>();
            try
            {
                var inventories = await _unitOfWork.UserRepository.GetUserInventoriesAndWarehouesAsync(UserId);
                foreach(var inventory in inventories)
                {
                    if (!warehouses.Any(x => x.Id == inventory.WarehouseId))
                    {
                        warehouses.Add(_mapper.Map<WarehouseView>(inventory.Warehouse));
                    }
                }
                result.Success(_mapper.Map<List<WarehouseView>>(warehouses.ToList()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }
        public async Task<ReturnResult<List<InventoryView>>> GetInventoriesByUserIdAsync(string UserId)
        {
            var result = new ReturnResult<List<InventoryView>>();
            try
            {
                var inventories = await _unitOfWork.UserRepository.GetUserInventoriesAsync(UserId);

                result.Success(_mapper.Map<List<InventoryView>>(inventories));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        public async Task<ReturnResult<List<UserView>>> GetUsersAsync(long FleetId)
        {
            var result = new ReturnResult<List<UserView>>();

            try
            {
                var users = await _unitOfWork.LookupsRepository.GetUsersByFleetIdAsync(FleetId);

                result.Success(_mapper.Map<List<UserView>>(users));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        public async Task<ReturnResult<byte[]>> GetDefaultExcelFileToAddNewSensorsAsync()
        {
            var result = new ReturnResult<byte[]>();
            try
            {
                string path = Path.Combine(_hostEnvironment.ContentRootPath, @"wwwroot\files\default_excel_file\") + "default.xlsx";
                byte[] bytes = await System.IO.File.ReadAllBytesAsync(path);
                result.Success(bytes);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;

        }

        public ReturnResult<List<SensorView>> ImportSensorsFromExcelAsync(IFormFile file)
        {
            var result = new ReturnResult<List<SensorView>>();
            try
            {
                string path = Path.Combine(_hostEnvironment.ContentRootPath, @"wwwroot\files");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string fileName = Path.GetFileName(file.FileName);
                string filePath = Path.Combine(path, fileName);
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                using (var workBook = new XLWorkbook(filePath))
                {
                    var workSheet = workBook.Worksheet(1);
                    var firstRowUsed = workSheet.FirstRowUsed();
                    var firstPossibleAddress = workSheet.Row(firstRowUsed.RowNumber()).FirstCell().Address;
                    var lastPossibleAddress = workSheet.LastCellUsed().Address;

                    // Get a range with the remainder of the worksheet data (the range used)
                    var range = workSheet.Range(firstPossibleAddress, lastPossibleAddress).AsRange(); //.RangeUsed();
                                                                                                      // Treat the range as a table (to be able to use the column names)
                    var table = range.AsTable();
                    //Specify what are all the Columns you need to get from Excel
                    var dataList = new List<string[]>
                    {
                        table.DataRange.Rows()
                            .Select(tableRow =>
                                tableRow.Field("SERIAL_NO")
                                    .GetString())
                            .ToArray(),
                        table.DataRange.Rows()
                            .Select(tableRow => tableRow.Field("NAME").GetString())
                            .ToArray(),
                        table.DataRange.Rows()
                            .Select(tableRow => tableRow.Field("CALIBRATION DATE (YYYY/MM/DD)").GetString())
                            .ToArray(),
                        table.DataRange.Rows()
                        .Select(tableRow => tableRow.Field("BRAND_ID").GetString())
                        .ToArray()
                    };
                    var sensors = new List<SensorView>();
                    var rows = dataList.Select(array => array.Length).Concat(new[] { 0 }).Max();
                    for (var j = 0; j < rows; j++)
                    {
                        string SERIAL_NO = dataList[0][j];
                        string NAME = dataList[1][j];
                        string CALIBRATION_DATE = dataList[2][j];
                        string BRAND_ID = dataList[3][j];
                        bool isValid = IsValidateProperties(BRAND_ID, NAME, CALIBRATION_DATE, SERIAL_NO);
                        if (isValid)
                        {
                            sensors.Add(
                                        new SensorView()
                                        {
                                            BrandId = GPSHelper.ToInt32(BRAND_ID),
                                            Name = NAME,
                                            CalibrationDate = GPSHelper.StringToDateTime(CALIBRATION_DATE),
                                            Serial = SERIAL_NO,
                                        }
                                      );
                        }
                    }
                    result.Success(sensors);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        private bool IsValidateProperties(string BRAND_ID, string NAME, string CALIBRATION_DATE, string SERIAL_NO)
        {
            bool isValidProperties = true;
            if (string.IsNullOrEmpty(BRAND_ID) 
                || string.IsNullOrEmpty(NAME) 
                || string.IsNullOrEmpty(CALIBRATION_DATE) 
                || string.IsNullOrEmpty(SERIAL_NO) 
                || !BRAND_ID.All(Char.IsDigit))
            {
                isValidProperties = false;
            }
            return isValidProperties;
        }

        public async Task<ReturnResult<List<WarehouseView>>> GetWarehousesAndInventoriesAsyncAndSensors(long? FleetId)
        {
            var result = new ReturnResult<List<WarehouseView>>();
            try
            {
                var warehousesView = _mapper.Map<List<WarehouseView>>(await _unitOfWork.LookupsRepository.GetWarehousesAsync(FleetId));
                foreach (var warehouseView in warehousesView)
                {
                    warehouseView.Inventories = _mapper.Map<List<InventoryView>>(await _unitOfWork.LookupsRepository.GetInventoriesAsync(warehouseView.Id));
                    foreach (var inventory in warehouseView.Inventories)
                    {
                        var resultInventorySensors = await GetInventorySensorsAsync(inventory.Id);
                        if(resultInventorySensors.IsSuccess && resultInventorySensors.Data.Count > 0)
                        {
                            inventory.InventorySensors = resultInventorySensors.Data;
                        }
                    }
                }
                result.Success(warehousesView);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }
    }
}
