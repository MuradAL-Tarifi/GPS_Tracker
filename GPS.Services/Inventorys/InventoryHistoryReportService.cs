using AutoMapper;
using ClosedXML.Excel;
using DinkToPdf;
using DinkToPdf.Contracts;
using GPS.API.Proxy;
using GPS.DataAccess.Repository.UnitOfWork;
using GPS.Domain.DTO;
using GPS.Domain.ViewModels;
using GPS.Domain.Views;
using GPS.Helper;
using GPS.Resources;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GPS.Services.Inventorys
{
    public class InventoryHistoryReportService : IInventoryHistoryReportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<InventoryHistoryReportService> _logger;
        private readonly IStringLocalizer<SharedResources> _sharedLocalizer;
        private readonly IInventoryHistoryApiProxy _inventoryHistoryApiProxy;

        private readonly IConverter _pdfConverter;
        private readonly IHostingEnvironment _hostingEnvironment;
        public InventoryHistoryReportService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<InventoryHistoryReportService> logger,
            IStringLocalizer<SharedResources> sharedLocalizer,
            IInventoryHistoryApiProxy inventoryHistoryApiProxy,
            IConverter pdfConverter,
            IHostingEnvironment hostingEnvironment)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _sharedLocalizer = sharedLocalizer;
            _inventoryHistoryApiProxy = inventoryHistoryApiProxy;
            _pdfConverter = pdfConverter;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<ReturnResult<GatewayHistoryReport>> PagedHistory(long inventoryId, string sensorSerial, string fromDate, string toDate, int pageNumber, int pageSize)
        {
            var result = new ReturnResult<GatewayHistoryReport>();
            try
            {
                if (pageNumber > 1)
                {
                    pageSize--;
                }

                var sensorList = await _unitOfWork.InventorySensorRepository.GetInventorySensorList(inventoryId);

                var sensorListView = _mapper.Map<List<InventorySensorView>>(sensorList);

                var pagedhistoryResult = await _inventoryHistoryApiProxy.PagedHistory(inventoryId, sensorSerial, fromDate, toDate, pageNumber, pageSize);
                if (pagedhistoryResult.IsSuccess)
                {
                    var pagedhistory = pagedhistoryResult.Data;

                    pagedhistory.Records.ForEach(x => x.InventorySensor = sensorListView.FirstOrDefault(s => s.SensorView.Serial == x.Serial));

                    result.Success(pagedhistory);
                }
                else
                {
                    result = new ReturnResult<GatewayHistoryReport>()
                    {
                        IsSuccess = false,
                        HttpCode = pagedhistoryResult.HttpCode,
                        ErrorList = pagedhistoryResult.ErrorList
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        public async Task<ReturnResult<TemperatureAndHumiditySensorHistoryReportResult>> PagedSensorTemperatureAndHumidityHistory(long inventoryId, string sensorSerial,
            string fromDate, string toDate, int pageNumber, int pageSize, string groupUpdatesByType, int? groupUpdatesValue, bool isEnglish)
        {
            var result = await _inventoryHistoryApiProxy.PagedSensorTemperatureAndHumidityHistory(inventoryId, sensorSerial, fromDate, toDate, pageNumber,
                pageSize, groupUpdatesByType, groupUpdatesValue, isEnglish);
            return result;
        }


        #region Excel
        public async Task<ReturnResult<byte[]>> InventoryHistoryReportExcel(long inventoryId, string fromDate, string toDate, string groupUpdatesByType, int? groupUpdatesValue, bool isEnglish)
        {
            SetCulture(isEnglish);
            var finalResult = new ReturnResult<byte[]>();

            var historyResult = await GetGroupedTemperatureAndHumidityInventoryHistoryReport(inventoryId, null, fromDate, toDate, groupUpdatesByType, groupUpdatesValue, isEnglish);
            if (historyResult.IsSuccess)
            {
                try
                {
                    string title = _sharedLocalizer["InventoryHistoryReport"];

                    string fleetName = "";
                    string warehouseName = "";
                    string inventoryName = "";
                    string sensorName = "";

                    var inventory = await _unitOfWork.InventoryRepository.FindByIdAsync(inventoryId);
                    if (inventory != null)
                    {
                        fleetName = isEnglish ? inventory.Warehouse.Fleet.NameEn : inventory.Warehouse.Fleet.Name;
                        warehouseName = inventory.Warehouse.Name;
                        inventoryName = inventory.Name;
                    }

                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        foreach (var sensorItem in historyResult.Data)
                        {
                            sensorName = $@"{sensorItem.Name} / {sensorItem.Serial}";

                            IXLWorksheet ws = wb.Worksheets.Add($"{sensorItem.Name}-{sensorItem.Serial}");

                            var headerInfo = sensorItem.SensorHistoryReport.HeaderInfo;
                            var monthList = sensorItem.SensorHistoryReport.MonthList;

                            #region Fill Worksheet

                            ws.Rows().Style.Fill.BackgroundColor = XLColor.White;
                            ws.Rows().Height = 30;
                            ws.Style.Font.FontName = "Segoe UI";
                            int cIndex = 1;

                            // 1 empty row from A1 to E1
                            ws.Row(cIndex).Height = 10;
                            ws.Range(cIndex, 2, cIndex, 4).Merge();
                            cIndex++;

                            // 2 title row from B2 to E2 - title cell from E2 to E2
                            var TitleCell = ws.Cell(cIndex, 4);
                            TitleCell.Value = title;
                            TitleCell.Style.Font.Bold = true;
                            TitleCell.Style.Font.FontColor = XLColor.White;
                            TitleCell.Style.Fill.BackgroundColor = XLColor.FromArgb(119, 19, 38);
                            TitleCell.Style.Font.FontSize = 22;
                            ws.Range(cIndex, 2, cIndex, 3).Merge();
                            //ws.Range(cIndex, 3, cIndex, 4).Merge();
                            ws.Row(cIndex).Height = 80;
                            cIndex++;

                            // 3 separator row from B3 to E3
                            var sepCell = ws.Cell(cIndex, 2);
                            sepCell.Value = "";
                            ws.Row(cIndex).Height = 10;
                            ws.Range(cIndex, 2, cIndex, 4).Style.Fill.BackgroundColor = XLColor.FromArgb(119, 19, 38);
                            ws.Range(cIndex, 2, cIndex, 4).Merge();
                            cIndex++;

                            // 4,5,6 separator cell from B4 to B5
                            ws.Range(cIndex, 2, cIndex + 4, 2).Merge();
                            ws.Range(cIndex, 2, cIndex + 4, 2).Style.Fill.BackgroundColor = XLColor.FromArgb(119, 19, 38);

                            // 4 info row from C4 to E4
                            ws.Range(cIndex, 3, cIndex + 4, 4).Style.Fill.BackgroundColor = XLColor.FromArgb(242, 242, 242);
                            ws.Range(cIndex, 3, cIndex + 4, 4).Style.Font.FontSize = 12;

                            ws.Range(cIndex, 3, cIndex + 4, 3).Style.Font.Bold = true;
                            ws.Range(cIndex, 3, cIndex + 4, 3).Style.Font.FontColor = XLColor.FromArgb(119, 19, 38);

                            // 5 info row from C5 to E5
                            ws.Range(cIndex, 4, cIndex, 4).Merge();
                            ws.Range(cIndex, 3, cIndex, 4).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                            ws.Range(cIndex, 3, cIndex, 4).Style.Border.OutsideBorderColor = XLColor.White;

                            // C5 to D5
                            ws.Cell(cIndex, 4).SetValue(fleetName);
                            // E5
                            ws.Cell(cIndex, 3).Value = _sharedLocalizer["Fleet"].Value;

                            ws.Row(cIndex).Height = 25;
                            cIndex++;

                            // 5 info row from C5 to E5
                            ws.Range(cIndex, 4, cIndex, 4).Merge();
                            ws.Range(cIndex, 3, cIndex, 4).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                            ws.Range(cIndex, 3, cIndex, 4).Style.Border.OutsideBorderColor = XLColor.White;

                            // C5 to D5
                            ws.Cell(cIndex, 4).SetValue(warehouseName);
                            // E5
                            ws.Cell(cIndex, 3).Value = _sharedLocalizer["Warehouse"].Value;

                            ws.Row(cIndex).Height = 25;
                            cIndex++;

                            // 6 info row from C6 to E6
                            ws.Range(cIndex, 4, cIndex, 4).Merge();
                            ws.Range(cIndex, 3, cIndex, 4).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                            ws.Range(cIndex, 3, cIndex, 4).Style.Border.OutsideBorderColor = XLColor.White;

                            // C6 to D6
                            ws.Cell(cIndex, 4).SetValue(inventoryName);
                            // E6
                            ws.Cell(cIndex, 3).Value = _sharedLocalizer["Inventory"].Value;

                            ws.Row(cIndex).Height = 25;
                            cIndex++;

                            // 6 info row from C6 to E6
                            ws.Range(cIndex, 4, cIndex, 4).Merge();
                            ws.Range(cIndex, 3, cIndex, 4).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                            ws.Range(cIndex, 3, cIndex, 4).Style.Border.OutsideBorderColor = XLColor.White;

                            // C6 to D6
                            ws.Cell(cIndex, 4).SetValue(sensorName);
                            // E6
                            ws.Cell(cIndex, 3).Value = _sharedLocalizer["Sensor"].Value;

                            ws.Row(cIndex).Height = 25;
                            cIndex++;

                            ws.Range(cIndex, 4, cIndex, 4).Merge();
                            ws.Range(cIndex, 3, cIndex, 4).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                            ws.Range(cIndex, 3, cIndex, 4).Style.Border.OutsideBorderColor = XLColor.White;

                            // C4 to D4
                            ws.Cell(cIndex, 4).SetValue(fromDate + " → " + toDate);

                            // E4
                            ws.Cell(cIndex, 3).Value = _sharedLocalizer["Date"].Value;

                            ws.Row(cIndex).Height = 25;
                            cIndex++;


                            // 7 summury row from B7 to H7
                            var dataSumRow = ws.Row(cIndex);
                            dataSumRow.Height = 30;
                            dataSumRow.Style.Font.Bold = true;
                            ws.Range(cIndex, 2, cIndex, 4).Merge();
                            ws.Cell(cIndex, 2).Style.Fill.BackgroundColor = XLColor.FromArgb(220, 230, 241);
                            ws.Cell(cIndex, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                            ws.Cell(cIndex, 2).Style.Border.OutsideBorderColor = XLColor.White;
                            string dataSum = $"[{_sharedLocalizer["MaxTemperature"]} : {headerInfo.MaxTemperature}°] [{_sharedLocalizer["MaxHumidity"]} : {headerInfo.MaxHumidity}%] | [{_sharedLocalizer["MinTemperature"]} : {headerInfo.MinTemperature}°] [{_sharedLocalizer["MinHumidity"]} : {headerInfo.MinHumidity}%]";
                            ws.Cell(cIndex, 2).SetValue(dataSum);
                            ws.Cell(cIndex, 2).RichText.Substring(0, dataSum.IndexOf('|') - 1).SetFontColor(XLColor.FromHtml("#dc3545"));
                            ws.Cell(cIndex, 2).RichText.Substring(dataSum.IndexOf('|') + 1).SetFontColor(XLColor.FromHtml("#17a2b8"));
                            //ws.Cell(cIndex, 2).Style.Font.FontColor = XLColor.Red;
                            if (!isEnglish)
                            {
                                ws.Cell(cIndex, 2).Style.Alignment.SetReadingOrder(XLAlignmentReadingOrderValues.RightToLeft);
                            }
                            cIndex++;


                            // 8 data header row from B7 to H7
                            var HeadersRow = ws.Row(cIndex);
                            HeadersRow.Height = 30;
                            HeadersRow.Style.Font.Bold = true;
                            HeadersRow.Style.Font.FontColor = XLColor.White;
                            HeadersRow.Style.Font.FontSize = 12;
                            ws.Range(cIndex, 2, cIndex, 4).Style.Fill.BackgroundColor = XLColor.FromArgb(79, 129, 189);
                            ws.Range(cIndex, 2, cIndex, 4).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                            ws.Range(cIndex, 2, cIndex, 4).Style.Border.OutsideBorderColor = XLColor.White;
                            ws.Cell(cIndex, 2).Value = "";
                            ws.Cell(cIndex, 3).Value = _sharedLocalizer["Temperature"].Value;
                            ws.Cell(cIndex, 4).Value = _sharedLocalizer["Humidity"].Value;

                            cIndex++;

                            // 9 data header row from B7 to H7
                            HeadersRow = ws.Row(cIndex);
                            HeadersRow.Height = 30;
                            HeadersRow.Style.Font.Bold = true;
                            //HeadersRow.Style.Font.FontColor = XLColor.Black;
                            HeadersRow.Style.Font.FontSize = 12;
                            ws.Range(cIndex, 2, cIndex, 4).Style.Fill.BackgroundColor = XLColor.FromHtml("#9BC2E6");
                            ws.Range(cIndex, 2, cIndex, 4).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                            ws.Range(cIndex, 2, cIndex, 4).Style.Border.OutsideBorderColor = XLColor.White;
                            ws.Cell(cIndex, 2).Value = _sharedLocalizer["Total"].Value;

                            if (headerInfo.MaxTemperature.HasValue)
                            {
                                dataSum = $"{headerInfo.MaxTemperature}￬ | {headerInfo.AverageTemperature}〜 | {headerInfo.MinTemperature}￪";
                                ws.Cell(cIndex, 3).SetValue(dataSum);
                                ws.Cell(cIndex, 3).RichText.Substring(dataSum.IndexOf('￬'), 1).SetFontColor(XLColor.FromHtml("#17a2b8"));
                                ws.Cell(cIndex, 3).RichText.Substring(dataSum.IndexOf('￪'), 1).SetFontColor(XLColor.FromHtml("#dc3545"));
                            }

                            if (headerInfo.MaxHumidity.HasValue)
                            {
                                dataSum = $"{headerInfo.MaxHumidity}￬ | {headerInfo.AverageHumidity}〜 | {headerInfo.MinHumidity}￪";
                                ws.Cell(cIndex, 4).SetValue(dataSum);
                                ws.Cell(cIndex, 4).RichText.Substring(dataSum.IndexOf('￬'), 1).SetFontColor(XLColor.FromHtml("#17a2b8"));
                                ws.Cell(cIndex, 4).RichText.Substring(dataSum.IndexOf('￪'), 1).SetFontColor(XLColor.FromHtml("#dc3545"));
                            }

                            cIndex++;

                            foreach (var monthItem in monthList)
                            {
                                HeadersRow = ws.Row(cIndex);
                                HeadersRow.Height = 60;
                                HeadersRow.Style.Font.Bold = true;
                                //HeadersRow.Style.Font.FontColor = XLColor.Black;
                                HeadersRow.Style.Font.FontSize = 12;
                                ws.Range(cIndex, 2, cIndex, 4).Style.Fill.BackgroundColor = XLColor.FromHtml("#BDD7EE");
                                ws.Range(cIndex, 2, cIndex, 4).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                                ws.Range(cIndex, 2, cIndex, 4).Style.Border.OutsideBorderColor = XLColor.White;

                                var dateText = monthItem.HeaderInfo.DateText.Trim().Replace(" ", "").Replace("\r\n", "").Replace("<br/>", Environment.NewLine);
                                ws.Cell(cIndex, 2).SetValue(dateText);

                                if (monthItem.HeaderInfo.MaxTemperature.HasValue)
                                {
                                    dataSum = $"{monthItem.HeaderInfo.MaxTemperature}￬ | {monthItem.HeaderInfo.AverageTemperature}〜 | {monthItem.HeaderInfo.MinTemperature}￪";
                                    ws.Cell(cIndex, 3).SetValue(dataSum);
                                    ws.Cell(cIndex, 3).RichText.Substring(dataSum.IndexOf('￬'), 1).SetFontColor(XLColor.FromHtml("#17a2b8"));
                                    ws.Cell(cIndex, 3).RichText.Substring(dataSum.IndexOf('￪'), 1).SetFontColor(XLColor.FromHtml("#dc3545"));
                                }

                                if (monthItem.HeaderInfo.MaxHumidity.HasValue)
                                {
                                    dataSum = $"{monthItem.HeaderInfo.MaxHumidity}￬ | {monthItem.HeaderInfo.AverageHumidity}〜 | {monthItem.HeaderInfo.MinHumidity}￪";
                                    ws.Cell(cIndex, 4).SetValue(dataSum);
                                    ws.Cell(cIndex, 4).RichText.Substring(dataSum.IndexOf('￬'), 1).SetFontColor(XLColor.FromHtml("#17a2b8"));
                                    ws.Cell(cIndex, 4).RichText.Substring(dataSum.IndexOf('￪'), 1).SetFontColor(XLColor.FromHtml("#dc3545"));
                                }

                                cIndex++;

                                foreach (var dayItem in monthItem.DayList)
                                {
                                    HeadersRow = ws.Row(cIndex);
                                    HeadersRow.Height = 30;
                                    HeadersRow.Style.Font.Bold = true;
                                    //HeadersRow.Style.Font.FontColor = XLColor.Black;
                                    HeadersRow.Style.Font.FontSize = 12;
                                    ws.Range(cIndex, 2, cIndex, 4).Style.Fill.BackgroundColor = XLColor.FromHtml("#DDEBF7");
                                    ws.Range(cIndex, 2, cIndex, 4).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                                    ws.Range(cIndex, 2, cIndex, 4).Style.Border.OutsideBorderColor = XLColor.White;
                                    ws.Cell(cIndex, 2).Value = dayItem.HeaderInfo.DateText;

                                    if (dayItem.HeaderInfo.MaxTemperature.HasValue)
                                    {
                                        dataSum = $"{dayItem.HeaderInfo.MaxTemperature}￬ | {dayItem.HeaderInfo.AverageTemperature}〜 | {dayItem.HeaderInfo.MinTemperature}￪";
                                        ws.Cell(cIndex, 3).SetValue(dataSum);
                                        ws.Cell(cIndex, 3).RichText.Substring(dataSum.IndexOf('￬'), 1).SetFontColor(XLColor.FromHtml("#17a2b8"));
                                        ws.Cell(cIndex, 3).RichText.Substring(dataSum.IndexOf('￪'), 1).SetFontColor(XLColor.FromHtml("#dc3545"));
                                    }

                                    if (dayItem.HeaderInfo.MaxHumidity.HasValue)
                                    {
                                        dataSum = $"{dayItem.HeaderInfo.MaxHumidity}￬ | {dayItem.HeaderInfo.AverageHumidity}〜 | {dayItem.HeaderInfo.MinHumidity}￪";
                                        ws.Cell(cIndex, 4).SetValue(dataSum);
                                        ws.Cell(cIndex, 4).RichText.Substring(dataSum.IndexOf('￬'), 1).SetFontColor(XLColor.FromHtml("#17a2b8"));
                                        ws.Cell(cIndex, 4).RichText.Substring(dataSum.IndexOf('￪'), 1).SetFontColor(XLColor.FromHtml("#dc3545"));
                                    }

                                    cIndex++;

                                    bool byHour = dayItem.HistoryList.Count > 0 && dayItem.HistoryList.FirstOrDefault().GPSDate.Contains("fa-arrow-up");
                                    foreach (var item in dayItem.HistoryList)
                                    {
                                        ws.Row(cIndex).Height = 30;
                                        if (byHour)
                                        {
                                            ws.Row(cIndex).Height = 60;
                                        }
                                        if (cIndex % 2 == 0)
                                        {
                                            ws.Range(cIndex, 2, cIndex, 4).Style.Fill.BackgroundColor = XLColor.FromArgb(242, 242, 242);
                                        }
                                        else
                                        {
                                            ws.Range(cIndex, 2, cIndex, 4).Style.Fill.BackgroundColor = XLColor.White;
                                        }
                                        ws.Range(cIndex, 2, cIndex, 4).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                                        ws.Range(cIndex, 2, cIndex, 4).Style.Border.OutsideBorderColor = XLColor.White;


                                        if (byHour)
                                        {
                                            var gPSDate = item.GPSDate.Replace("<br><i class='fa fa-arrow-up text-success'></i><br>", Environment.NewLine + "￪" + Environment.NewLine);
                                            ws.Cell(cIndex, 2).SetValue(gPSDate);
                                            ws.Cell(cIndex, 2).RichText.Substring(gPSDate.IndexOf('￪'), 1).SetFontColor(XLColor.FromHtml("#28a745"));
                                            ws.Cell(cIndex, 2).RichText.Substring(gPSDate.IndexOf('￪'), 1).SetBold();

                                            if (!string.IsNullOrEmpty(item.Temperature))
                                            {
                                                var temperature1 = ParseGroupedMaxMinAverageTempAndHumidityFromHtml(item.Temperature);
                                                ws.Cell(cIndex, 3).SetValue(temperature1);
                                                ws.Cell(cIndex, 3).RichText.Substring(temperature1.IndexOf('￬'), 1).SetFontColor(XLColor.FromHtml("#17a2b8"));
                                                ws.Cell(cIndex, 3).RichText.Substring(temperature1.IndexOf('￬'), 1).SetBold();
                                                ws.Cell(cIndex, 3).RichText.Substring(temperature1.IndexOf('￪'), 1).SetFontColor(XLColor.FromHtml("#dc3545"));
                                                ws.Cell(cIndex, 3).RichText.Substring(temperature1.IndexOf('￪'), 1).SetBold();
                                            }

                                            if (!string.IsNullOrEmpty(item.Humidity))
                                            {
                                                var humidity1 = ParseGroupedMaxMinAverageTempAndHumidityFromHtml(item.Humidity);
                                                ws.Cell(cIndex, 4).SetValue(humidity1);
                                                ws.Cell(cIndex, 4).RichText.Substring(humidity1.IndexOf('￬'), 1).SetFontColor(XLColor.FromHtml("#17a2b8"));
                                                ws.Cell(cIndex, 4).RichText.Substring(humidity1.IndexOf('￬'), 1).SetBold();
                                                ws.Cell(cIndex, 4).RichText.Substring(humidity1.IndexOf('￪'), 1).SetFontColor(XLColor.FromHtml("#dc3545"));
                                                ws.Cell(cIndex, 4).RichText.Substring(humidity1.IndexOf('￪'), 1).SetBold();
                                            }
                                        }
                                        else
                                        {
                                            ws.Cell(cIndex, 2).SetValue(item.GPSDate);
                                            ws.Cell(cIndex, 3).SetValue(item.Temperature);
                                            ws.Cell(cIndex, 4).SetValue(item.Humidity);
                                        }

                                        cIndex++;
                                    }
                                }
                            }

                            cIndex--;
                            ws.Range(1, 1, cIndex, 1).Merge();
                            wb.FindCells(x => x.Value.ToString() != "__").Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                            wb.FindCells(x => x.Value.ToString() != "__").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                            // Column width
                            ws.Column("1").Width = 1;
                            ws.Column("2").Width = 30;
                            ws.Column("3").Width = 40;
                            ws.Column("4").Width = 60;

                            // Outside border
                            ws.Range(2, 2, cIndex, 4).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                            ws.Range(2, 2, cIndex, 4).Style.Border.OutsideBorderColor = XLColor.FromArgb(119, 19, 38);

                            // right white borders
                            //ws.Range(4, 4, 5, 3).Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                            //ws.Range(4, 4, 5, 3).Style.Border.LeftBorderColor = XLColor.White;

                            ws.Range(4, 3, 8, 3).Style.Border.RightBorder = XLBorderStyleValues.Thick;
                            ws.Range(4, 3, 8, 3).Style.Border.RightBorderColor = XLColor.White;

                            ws.Range(9, 2, cIndex, 3).Style.Border.RightBorder = XLBorderStyleValues.Thick;
                            ws.Range(9, 2, cIndex, 3).Style.Border.RightBorderColor = XLColor.White;

                            // logo image
                            var logoPath = Directory.GetCurrentDirectory() + @"\assets\images\logo_EWE.png";

                            var image = ws.AddPicture(logoPath)
                                .MoveTo((IXLCell)ws.Cell("B2").Address, 60, 15)
                                .Scale(0.6);

                            ws.Range(1, 5, cIndex, 5).Merge();
                            ws.Column("5").Width = 500;
                            ws.Column("5").Style.Fill.BackgroundColor = XLColor.White;

                            // freez first 11 rows
                            //ws.SheetView.FreezeRows(11);

                            if (!isEnglish)
                            {
                                ws.RightToLeft = true;
                            }
                            #endregion
                        }

                        wb.FindCells(x => x.Value.ToString() != "__").Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                        wb.FindCells(x => x.Value.ToString() != "__").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                        using (MemoryStream ms = new MemoryStream())
                        {
                            wb.SaveAs(ms);
                            ms.Flush();
                            finalResult.Success(ms.ToArray());
                        }
                    }
                }
                catch (Exception ex)
                {
                    finalResult.ServerError(ex.Message);
                }
            }
            else
            {
                finalResult = new ReturnResult<byte[]>()
                {
                    IsSuccess = false,
                    HttpCode = historyResult.HttpCode,
                    ErrorList = historyResult.ErrorList
                };
            }

            return finalResult;
        }

        public async Task<ReturnResult<byte[]>> InventorySensorHistoryReportExcel(long inventoryId, string sensorSerial, string fromDate, string toDate, string groupUpdatesByType, int? groupUpdatesValue, bool isEnglish)
        {
            SetCulture(isEnglish);

            var finalResult = new ReturnResult<byte[]>();
            var result = await _inventoryHistoryApiProxy.GroupedSensorTemperatureAndHumidityHistory(inventoryId, sensorSerial, fromDate, toDate, groupUpdatesByType, groupUpdatesValue, isEnglish);
            if (result.IsSuccess)
            {
                try
                {
                    string title = _sharedLocalizer["InventorySensorHistoryReport"];

                    string fleetName = "";
                    string warehouseName = "";
                    string inventoryName = "";
                    string sensorName = "";

                    var inventorySensor = await _unitOfWork.InventorySensorRepository.GetInventorySensor(inventoryId, sensorSerial);
                    if (inventorySensor != null)
                    {
                        fleetName = isEnglish ? inventorySensor.Inventory.Warehouse.Fleet.NameEn : inventorySensor.Inventory.Warehouse.Fleet.Name;
                        warehouseName = inventorySensor.Inventory.Warehouse.Name;
                        inventoryName = inventorySensor.Inventory.Name;
                        sensorName = $@"{inventorySensor.Sensor.Name} / {inventorySensor.Sensor.Serial}";
                    }

                    var headerInfo = result.Data.HeaderInfo;

                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        IXLWorksheet ws = wb.Worksheets.Add(title);

                        #region Fill Worksheet

                        ws.Rows().Style.Fill.BackgroundColor = XLColor.White;
                        ws.Rows().Height = 30;
                        ws.Style.Font.FontName = "Segoe UI";
                        int cIndex = 1;

                        // 1 empty row from A1 to E1
                        ws.Row(cIndex).Height = 10;
                        ws.Range(cIndex, 2, cIndex, 4).Merge();
                        cIndex++;

                        // 2 title row from B2 to E2 - title cell from E2 to E2
                        var TitleCell = ws.Cell(cIndex, 4);
                        TitleCell.Value = title;
                        TitleCell.Style.Font.Bold = true;
                        TitleCell.Style.Font.FontColor = XLColor.White;
                        TitleCell.Style.Fill.BackgroundColor = XLColor.FromArgb(119, 19, 38);
                        TitleCell.Style.Font.FontSize = 22;
                        ws.Range(cIndex, 2, cIndex, 3).Merge();
                        //ws.Range(cIndex, 3, cIndex, 4).Merge();
                        ws.Row(cIndex).Height = 80;
                        cIndex++;

                        // 3 separator row from B3 to E3
                        var sepCell = ws.Cell(cIndex, 2);
                        sepCell.Value = "";
                        ws.Row(cIndex).Height = 10;
                        ws.Range(cIndex, 2, cIndex, 4).Style.Fill.BackgroundColor = XLColor.FromArgb(119, 19, 38);
                        ws.Range(cIndex, 2, cIndex, 4).Merge();
                        cIndex++;

                        // 4,5,6 separator cell from B4 to B5
                        ws.Range(cIndex, 2, cIndex + 4, 2).Merge();
                        ws.Range(cIndex, 2, cIndex + 4, 2).Style.Fill.BackgroundColor = XLColor.FromArgb(119, 19, 38);

                        // 4 info row from C4 to E4
                        ws.Range(cIndex, 3, cIndex + 4, 4).Style.Fill.BackgroundColor = XLColor.FromArgb(242, 242, 242);
                        ws.Range(cIndex, 3, cIndex + 4, 4).Style.Font.FontSize = 12;

                        ws.Range(cIndex, 3, cIndex + 4, 3).Style.Font.Bold = true;
                        ws.Range(cIndex, 3, cIndex + 4, 3).Style.Font.FontColor = XLColor.FromArgb(119, 19, 38);

                        // 5 info row from C5 to E5
                        ws.Range(cIndex, 4, cIndex, 4).Merge();
                        ws.Range(cIndex, 3, cIndex, 4).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                        ws.Range(cIndex, 3, cIndex, 4).Style.Border.OutsideBorderColor = XLColor.White;

                        // C5 to D5
                        ws.Cell(cIndex, 4).SetValue(fleetName);
                        // E5
                        ws.Cell(cIndex, 3).Value = _sharedLocalizer["Fleet"].Value;

                        ws.Row(cIndex).Height = 25;
                        cIndex++;

                        // 5 info row from C5 to E5
                        ws.Range(cIndex, 4, cIndex, 4).Merge();
                        ws.Range(cIndex, 3, cIndex, 4).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                        ws.Range(cIndex, 3, cIndex, 4).Style.Border.OutsideBorderColor = XLColor.White;

                        // C5 to D5
                        ws.Cell(cIndex, 4).SetValue(warehouseName);
                        // E5
                        ws.Cell(cIndex, 3).Value = _sharedLocalizer["Warehouse"].Value;

                        ws.Row(cIndex).Height = 25;
                        cIndex++;

                        // 6 info row from C6 to E6
                        ws.Range(cIndex, 4, cIndex, 4).Merge();
                        ws.Range(cIndex, 3, cIndex, 4).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                        ws.Range(cIndex, 3, cIndex, 4).Style.Border.OutsideBorderColor = XLColor.White;

                        // C6 to D6
                        ws.Cell(cIndex, 4).SetValue(inventoryName);
                        // E6
                        ws.Cell(cIndex, 3).Value = _sharedLocalizer["Inventory"].Value;

                        ws.Row(cIndex).Height = 25;
                        cIndex++;

                        // 6 info row from C6 to E6
                        ws.Range(cIndex, 4, cIndex, 4).Merge();
                        ws.Range(cIndex, 3, cIndex, 4).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                        ws.Range(cIndex, 3, cIndex, 4).Style.Border.OutsideBorderColor = XLColor.White;

                        // C6 to D6
                        ws.Cell(cIndex, 4).SetValue(sensorName);
                        // E6
                        ws.Cell(cIndex, 3).Value = _sharedLocalizer["Sensor"].Value;

                        ws.Row(cIndex).Height = 25;
                        cIndex++;

                        ws.Range(cIndex, 4, cIndex, 4).Merge();
                        ws.Range(cIndex, 3, cIndex, 4).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                        ws.Range(cIndex, 3, cIndex, 4).Style.Border.OutsideBorderColor = XLColor.White;

                        // C4 to D4
                        ws.Cell(cIndex, 4).SetValue(fromDate + " → " + toDate);

                        // E4
                        ws.Cell(cIndex, 3).Value = _sharedLocalizer["Date"].Value;

                        ws.Row(cIndex).Height = 25;
                        cIndex++;


                        // 7 summury row from B7 to H7
                        var dataSumRow = ws.Row(cIndex);
                        dataSumRow.Height = 30;
                        dataSumRow.Style.Font.Bold = true;
                        ws.Range(cIndex, 2, cIndex, 4).Merge();
                        ws.Cell(cIndex, 2).Style.Fill.BackgroundColor = XLColor.FromArgb(220, 230, 241);
                        ws.Cell(cIndex, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                        ws.Cell(cIndex, 2).Style.Border.OutsideBorderColor = XLColor.White;
                        string dataSum = $"[{_sharedLocalizer["MaxTemperature"]} : {headerInfo.MaxTemperature}°] [{_sharedLocalizer["MaxHumidity"]} : {headerInfo.MaxHumidity}%] | [{_sharedLocalizer["MinTemperature"]} : {headerInfo.MinTemperature}°] [{_sharedLocalizer["MinHumidity"]} : {headerInfo.MinHumidity}%]";
                        ws.Cell(cIndex, 2).SetValue(dataSum);
                        ws.Cell(cIndex, 2).RichText.Substring(0, dataSum.IndexOf('|') - 1).SetFontColor(XLColor.FromHtml("#dc3545"));
                        ws.Cell(cIndex, 2).RichText.Substring(dataSum.IndexOf('|') + 1).SetFontColor(XLColor.FromHtml("#17a2b8"));
                        //ws.Cell(cIndex, 2).Style.Font.FontColor = XLColor.Red;
                        if (!isEnglish)
                        {
                            ws.Cell(cIndex, 2).Style.Alignment.SetReadingOrder(XLAlignmentReadingOrderValues.RightToLeft);
                        }
                        cIndex++;


                        // 8 data header row from B7 to H7
                        var HeadersRow = ws.Row(cIndex);
                        HeadersRow.Height = 30;
                        HeadersRow.Style.Font.Bold = true;
                        HeadersRow.Style.Font.FontColor = XLColor.White;
                        HeadersRow.Style.Font.FontSize = 12;
                        ws.Range(cIndex, 2, cIndex, 4).Style.Fill.BackgroundColor = XLColor.FromArgb(79, 129, 189);
                        ws.Range(cIndex, 2, cIndex, 4).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                        ws.Range(cIndex, 2, cIndex, 4).Style.Border.OutsideBorderColor = XLColor.White;
                        ws.Cell(cIndex, 2).Value = "";
                        ws.Cell(cIndex, 3).Value = _sharedLocalizer["Temperature"].Value;
                        ws.Cell(cIndex, 4).Value = _sharedLocalizer["Humidity"].Value;

                        cIndex++;

                        // 9 data header row from B7 to H7
                        HeadersRow = ws.Row(cIndex);
                        HeadersRow.Height = 30;
                        HeadersRow.Style.Font.Bold = true;
                        //HeadersRow.Style.Font.FontColor = XLColor.Black;
                        HeadersRow.Style.Font.FontSize = 12;
                        ws.Range(cIndex, 2, cIndex, 4).Style.Fill.BackgroundColor = XLColor.FromHtml("#9BC2E6");
                        ws.Range(cIndex, 2, cIndex, 4).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                        ws.Range(cIndex, 2, cIndex, 4).Style.Border.OutsideBorderColor = XLColor.White;
                        ws.Cell(cIndex, 2).Value = _sharedLocalizer["Total"].Value;

                        if (headerInfo.MaxTemperature.HasValue)
                        {
                            dataSum = $"{headerInfo.MaxTemperature}￬ | {headerInfo.AverageTemperature}〜 | {headerInfo.MinTemperature}￪";
                            ws.Cell(cIndex, 3).SetValue(dataSum);
                            ws.Cell(cIndex, 3).RichText.Substring(dataSum.IndexOf('￬'), 1).SetFontColor(XLColor.FromHtml("#17a2b8"));
                            ws.Cell(cIndex, 3).RichText.Substring(dataSum.IndexOf('￪'), 1).SetFontColor(XLColor.FromHtml("#dc3545"));
                        }

                        if (headerInfo.MaxHumidity.HasValue)
                        {
                            dataSum = $"{headerInfo.MaxHumidity}￬ | {headerInfo.AverageHumidity}〜 | {headerInfo.MinHumidity}￪";
                            ws.Cell(cIndex, 4).SetValue(dataSum);
                            ws.Cell(cIndex, 4).RichText.Substring(dataSum.IndexOf('￬'), 1).SetFontColor(XLColor.FromHtml("#17a2b8"));
                            ws.Cell(cIndex, 4).RichText.Substring(dataSum.IndexOf('￪'), 1).SetFontColor(XLColor.FromHtml("#dc3545"));
                        }

                        cIndex++;

                        foreach (var monthItem in result.Data.MonthList)
                        {
                            HeadersRow = ws.Row(cIndex);
                            HeadersRow.Height = 60;
                            HeadersRow.Style.Font.Bold = true;
                            //HeadersRow.Style.Font.FontColor = XLColor.Black;
                            HeadersRow.Style.Font.FontSize = 12;
                            ws.Range(cIndex, 2, cIndex, 4).Style.Fill.BackgroundColor = XLColor.FromHtml("#BDD7EE");
                            ws.Range(cIndex, 2, cIndex, 4).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                            ws.Range(cIndex, 2, cIndex, 4).Style.Border.OutsideBorderColor = XLColor.White;

                            var dateText = monthItem.HeaderInfo.DateText.Trim().Replace(" ", "").Replace("\r\n", "").Replace("<br/>", Environment.NewLine);
                            ws.Cell(cIndex, 2).SetValue(dateText);

                            if (monthItem.HeaderInfo.MaxTemperature.HasValue)
                            {
                                dataSum = $"{monthItem.HeaderInfo.MaxTemperature}￬ | {monthItem.HeaderInfo.AverageTemperature}〜 | {monthItem.HeaderInfo.MinTemperature}￪";
                                ws.Cell(cIndex, 3).SetValue(dataSum);
                                ws.Cell(cIndex, 3).RichText.Substring(dataSum.IndexOf('￬'), 1).SetFontColor(XLColor.FromHtml("#17a2b8"));
                                ws.Cell(cIndex, 3).RichText.Substring(dataSum.IndexOf('￪'), 1).SetFontColor(XLColor.FromHtml("#dc3545"));
                            }

                            if (monthItem.HeaderInfo.MaxHumidity.HasValue)
                            {
                                dataSum = $"{monthItem.HeaderInfo.MaxHumidity}￬ | {monthItem.HeaderInfo.AverageHumidity}〜 | {monthItem.HeaderInfo.MinHumidity}￪";
                                ws.Cell(cIndex, 4).SetValue(dataSum);
                                ws.Cell(cIndex, 4).RichText.Substring(dataSum.IndexOf('￬'), 1).SetFontColor(XLColor.FromHtml("#17a2b8"));
                                ws.Cell(cIndex, 4).RichText.Substring(dataSum.IndexOf('￪'), 1).SetFontColor(XLColor.FromHtml("#dc3545"));
                            }

                            cIndex++;

                            foreach (var dayItem in monthItem.DayList)
                            {
                                HeadersRow = ws.Row(cIndex);
                                HeadersRow.Height = 30;
                                HeadersRow.Style.Font.Bold = true;
                                //HeadersRow.Style.Font.FontColor = XLColor.Black;
                                HeadersRow.Style.Font.FontSize = 12;
                                ws.Range(cIndex, 2, cIndex, 4).Style.Fill.BackgroundColor = XLColor.FromHtml("#DDEBF7");
                                ws.Range(cIndex, 2, cIndex, 4).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                                ws.Range(cIndex, 2, cIndex, 4).Style.Border.OutsideBorderColor = XLColor.White;
                                ws.Cell(cIndex, 2).Value = dayItem.HeaderInfo.DateText;

                                if (dayItem.HeaderInfo.MaxTemperature.HasValue)
                                {
                                    dataSum = $"{dayItem.HeaderInfo.MaxTemperature}￬ | {dayItem.HeaderInfo.AverageTemperature}〜 | {dayItem.HeaderInfo.MinTemperature}￪";
                                    ws.Cell(cIndex, 3).SetValue(dataSum);
                                    ws.Cell(cIndex, 3).RichText.Substring(dataSum.IndexOf('￬'), 1).SetFontColor(XLColor.FromHtml("#17a2b8"));
                                    ws.Cell(cIndex, 3).RichText.Substring(dataSum.IndexOf('￪'), 1).SetFontColor(XLColor.FromHtml("#dc3545"));
                                }

                                if (dayItem.HeaderInfo.MaxHumidity.HasValue)
                                {
                                    dataSum = $"{dayItem.HeaderInfo.MaxHumidity}￬ | {dayItem.HeaderInfo.AverageHumidity}〜 | {dayItem.HeaderInfo.MinHumidity}￪";
                                    ws.Cell(cIndex, 4).SetValue(dataSum);
                                    ws.Cell(cIndex, 4).RichText.Substring(dataSum.IndexOf('￬'), 1).SetFontColor(XLColor.FromHtml("#17a2b8"));
                                    ws.Cell(cIndex, 4).RichText.Substring(dataSum.IndexOf('￪'), 1).SetFontColor(XLColor.FromHtml("#dc3545"));
                                }

                                cIndex++;

                                bool byHour = dayItem.HistoryList.Count > 0 && dayItem.HistoryList.FirstOrDefault().GPSDate.Contains("fa-arrow-up");
                                foreach (var item in dayItem.HistoryList)
                                {
                                    ws.Row(cIndex).Height = 30;
                                    if (byHour)
                                    {
                                        ws.Row(cIndex).Height = 60;
                                    }
                                    if (cIndex % 2 == 0)
                                    {
                                        ws.Range(cIndex, 2, cIndex, 4).Style.Fill.BackgroundColor = XLColor.FromArgb(242, 242, 242);
                                    }
                                    else
                                    {
                                        ws.Range(cIndex, 2, cIndex, 4).Style.Fill.BackgroundColor = XLColor.White;
                                    }
                                    ws.Range(cIndex, 2, cIndex, 4).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                                    ws.Range(cIndex, 2, cIndex, 4).Style.Border.OutsideBorderColor = XLColor.White;


                                    if (byHour)
                                    {
                                        var gPSDate = item.GPSDate.Replace("<br><i class='fa fa-arrow-up text-success'></i><br>", Environment.NewLine + "￪" + Environment.NewLine);
                                        ws.Cell(cIndex, 2).SetValue(gPSDate);
                                        ws.Cell(cIndex, 2).RichText.Substring(gPSDate.IndexOf('￪'), 1).SetFontColor(XLColor.FromHtml("#28a745"));
                                        ws.Cell(cIndex, 2).RichText.Substring(gPSDate.IndexOf('￪'), 1).SetBold();

                                        if (!string.IsNullOrEmpty(item.Temperature))
                                        {
                                            var temperature1 = ParseGroupedMaxMinAverageTempAndHumidityFromHtml(item.Temperature);
                                            ws.Cell(cIndex, 3).SetValue(temperature1);
                                            ws.Cell(cIndex, 3).RichText.Substring(temperature1.IndexOf('￬'), 1).SetFontColor(XLColor.FromHtml("#17a2b8"));
                                            ws.Cell(cIndex, 3).RichText.Substring(temperature1.IndexOf('￬'), 1).SetBold();
                                            ws.Cell(cIndex, 3).RichText.Substring(temperature1.IndexOf('￪'), 1).SetFontColor(XLColor.FromHtml("#dc3545"));
                                            ws.Cell(cIndex, 3).RichText.Substring(temperature1.IndexOf('￪'), 1).SetBold();
                                        }

                                        if (!string.IsNullOrEmpty(item.Humidity))
                                        {
                                            var humidity1 = ParseGroupedMaxMinAverageTempAndHumidityFromHtml(item.Humidity);
                                            ws.Cell(cIndex, 4).SetValue(humidity1);
                                            ws.Cell(cIndex, 4).RichText.Substring(humidity1.IndexOf('￬'), 1).SetFontColor(XLColor.FromHtml("#17a2b8"));
                                            ws.Cell(cIndex, 4).RichText.Substring(humidity1.IndexOf('￬'), 1).SetBold();
                                            ws.Cell(cIndex, 4).RichText.Substring(humidity1.IndexOf('￪'), 1).SetFontColor(XLColor.FromHtml("#dc3545"));
                                            ws.Cell(cIndex, 4).RichText.Substring(humidity1.IndexOf('￪'), 1).SetBold();
                                        }
                                    }
                                    else
                                    {
                                        ws.Cell(cIndex, 2).SetValue(item.GPSDate);
                                        ws.Cell(cIndex, 3).SetValue(item.Temperature);
                                        ws.Cell(cIndex, 4).SetValue(item.Humidity);
                                    }

                                    cIndex++;
                                }
                            }
                        }

                        cIndex--;
                        ws.Range(1, 1, cIndex, 1).Merge();
                        wb.FindCells(x => x.Value.ToString() != "__").Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                        wb.FindCells(x => x.Value.ToString() != "__").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                        // Column width
                        ws.Column("1").Width = 1;
                        ws.Column("2").Width = 30;
                        ws.Column("3").Width = 40;
                        ws.Column("4").Width = 60;

                        // Outside border
                        ws.Range(2, 2, cIndex, 4).Style.Border.OutsideBorder = XLBorderStyleValues.Thick;
                        ws.Range(2, 2, cIndex, 4).Style.Border.OutsideBorderColor = XLColor.FromArgb(119, 19, 38);

                        // right white borders
                        //ws.Range(4, 4, 5, 3).Style.Border.LeftBorder = XLBorderStyleValues.Thick;
                        //ws.Range(4, 4, 5, 3).Style.Border.LeftBorderColor = XLColor.White;

                        ws.Range(4, 3, 8, 3).Style.Border.RightBorder = XLBorderStyleValues.Thick;
                        ws.Range(4, 3, 8, 3).Style.Border.RightBorderColor = XLColor.White;

                        ws.Range(9, 2, cIndex, 3).Style.Border.RightBorder = XLBorderStyleValues.Thick;
                        ws.Range(9, 2, cIndex, 3).Style.Border.RightBorderColor = XLColor.White;

                        // logo image
                        var logoPath = Directory.GetCurrentDirectory() + @"\assets\images\logo_EWE.png";

                        var image = ws.AddPicture(logoPath)
                            .MoveTo((IXLCell)ws.Cell("B2").Address, 60, 15)
                            .Scale(0.6);

                        ws.Range(1, 5, cIndex, 5).Merge();
                        ws.Column("5").Width = 500;
                        ws.Column("5").Style.Fill.BackgroundColor = XLColor.White;

                        // freez first 11 rows
                        //ws.SheetView.FreezeRows(11);

                        if (!isEnglish)
                        {
                            ws.RightToLeft = true;
                        }
                        #endregion

                        using (MemoryStream ms = new MemoryStream())
                        {
                            wb.SaveAs(ms);
                            ms.Flush();
                            finalResult.Success(ms.ToArray());
                        }
                    }
                }
                catch (Exception ex)
                {
                    finalResult.ServerError(ex.Message);
                }
            }
            else
            {
                finalResult = new ReturnResult<byte[]>()
                {
                    IsSuccess = false,
                    HttpCode = result.HttpCode,
                    ErrorList = result.ErrorList
                };
            }
            return finalResult;
        }
        #endregion

        public async Task<ReturnResult<List<InventorySensorModel>>> InventoryHistoryReport(long inventoryId, string fromDate, string toDate)
        {
            return await GetInventoryHistoryReport(inventoryId, fromDate, toDate);
        }

        public async Task<ReturnResult<List<TemperatureAndHumidityInventoryHistoryReportResult>>> TemperatureAndHumidityInventoryHistoryReport(long inventoryId, string sensorsSN, string fromDate, string toDate, string groupUpdatesByType, int? groupUpdatesValue, bool isEnglish)
        {
            return await GetGroupedTemperatureAndHumidityInventoryHistoryReport(inventoryId, sensorsSN, fromDate, toDate, groupUpdatesByType, groupUpdatesValue, isEnglish);
        }
        public async Task<ReturnResult<byte[]>> InventorySensorHistoryReportPDF(long inventoryId, string sensorSerial, string fromDate, string toDate, string groupUpdatesByType, int? groupUpdatesValue, bool isEnglish)
        {
            SetCulture(isEnglish);

            var finalResult = new ReturnResult<byte[]>();
            var result = await _inventoryHistoryApiProxy.GroupedSensorTemperatureAndHumidityHistory(inventoryId, sensorSerial, fromDate, toDate, groupUpdatesByType, groupUpdatesValue, isEnglish);
            if (result.IsSuccess)
            {
                try
                {
                    string title = _sharedLocalizer["InventorySensorHistoryReport"];

                    string fleetName = "";
                    string warehouseName = "";
                    string inventoryName = "";
                    string sensorName = "";
                    string calibrationDate = "";
                    string calibrationStatus = "";
                    bool isCalibrated = false;
                    var inventorySensor = await _unitOfWork.InventorySensorRepository.GetInventorySensor(inventoryId, sensorSerial);
                   
                    if (inventorySensor != null)
                    {
                        fleetName = isEnglish ? inventorySensor.Inventory.Warehouse.Fleet.NameEn : inventorySensor.Inventory.Warehouse.Fleet.Name;
                        warehouseName = inventorySensor.Inventory.Warehouse.Name;
                        inventoryName = inventorySensor.Inventory.Name;
                        sensorName = $@"{inventorySensor.Sensor.Name} / {inventorySensor.Sensor.Serial}";
                        calibrationDate = GPSHelper.DateToFormatedString(inventorySensor.Sensor.CalibrationDate.Value);
                        isCalibrated = inventorySensor.Sensor.CalibrationDate.Value > DateTime.Now ? true : false;
                        calibrationStatus = isCalibrated ? _sharedLocalizer["Calibrated"] : _sharedLocalizer["NotCalibrated"];
                    }

                    var systemSetting = await _unitOfWork.SystemSettingRepository.LoadSystemSettingAsync();
                    var fleet = inventorySensor.Inventory.Warehouse.Fleet;
                    //var base64FleetLogo = string.Empty;
                    var imgExtention = string.Empty;
                    if (fleet.LogoPhotoByte != null)
                    {
                        if (fleet.LogoPhotoByte.Length > 0)
                        {
                            //base64FleetLogo = Convert.ToBase64String(fleet.LogoPhotoByte);
                            imgExtention = fleet.LogoPhotoExtention;
                            SaveImage(_hostingEnvironment.ContentRootPath + @"\assets\imgs", fleet.LogoPhotoByte, fleet.Id + fleet.LogoPhotoExtention);
                        }
                    }
                    
                    if (string.IsNullOrEmpty(imgExtention))
                    {
                        if (systemSetting.LogoPhotoByte != null)
                        {
                            //base64FleetLogo = Convert.ToBase64String(fleet.LogoPhotoByte);
                            imgExtention = ".png";
                            SaveImage(_hostingEnvironment.ContentRootPath + @"\assets\imgs", systemSetting.LogoPhotoByte, fleet.Id + imgExtention);
                        }
                    }

                    var headerInfo = result.Data.HeaderInfo;

                    #region Create Html Template

                    var logoPath = _hostingEnvironment.ContentRootPath + @"\assets\imgs\logo.png";

                    string dir = isEnglish ? "ltr" : "rtl";

                    var colSpan = 1;
                    if (!string.IsNullOrEmpty(groupUpdatesByType))
                    {
                        colSpan = 1;
                    }

                    var sb = new StringBuilder();
                    var headerHtml = new StringBuilder();
                    var dataInfoHtml = new StringBuilder();
                    var dataHeaderHtml = new StringBuilder();
                    var dataBodyHtml = new StringBuilder();

                    //var src = !string.IsNullOrEmpty(base64FleetLogo) ? $"data:image/{fleet.LogoPhotoExtention}; base64,{base64FleetLogo}" : logoPath;
                    var src = !string.IsNullOrEmpty(imgExtention) ? _hostingEnvironment.ContentRootPath + @"\assets\imgs\" + fleet.Id + imgExtention : logoPath;

                    headerHtml.Append($@"<html><head></head><body>
                                                    <table class='table' dir='{dir}'>
                                             <tr>
                                                 <td colspan='2' class='logo'>
                                                     <img src='{src}'/>
                                                 </td>
                                                 <td colspan='2' style='background-color: #0a6f6d !important;color: #fff !important;font-size: 26px !important;font-weight: bold !important;'>{title}</td>
                                             </tr>
                                             <tr>
                                                 <td colspan='4' class='row-sep'></td>
                                             </tr>
                                             <tr class='col-sep'>
                                                 <td rowspan='8'></td>
                                             </tr>
                                             <tr class='info'>
                                                 <td>{_sharedLocalizer["Fleet"]}</td>
                                                 <td colspan='2'>{fleetName}</td>
                                             </tr>   
                                             <tr class='info'>
                                                 <td>{_sharedLocalizer["Warehouse"]}</td>
                                                 <td colspan='2'>{warehouseName}</td>
                                             </tr>     
                                             <tr class='info'>
                                                 <td>{_sharedLocalizer["Inventory"]}</td>
                                                 <td colspan='2'>{inventoryName}</td>
                                             </tr>                               
                                             <tr class='info'>
                                                 <td>{_sharedLocalizer["Sensor"]}</td>
                                                 <td colspan='2'>{sensorName}</td>
                                             </tr>
                                             <tr class='info'>
                                                 <td>{_sharedLocalizer["Date"]}</td>
                                                 <td colspan='2' dir='{dir}'>{_sharedLocalizer["from"]}: <span dir='ltr'>{fromDate}</span> - {_sharedLocalizer["to"]}: <span dir='ltr'>{toDate}</span></td>
                                             </tr>
                                             <tr class='info'>
                                                <td>{_sharedLocalizer["CalibrationDate"]}</td>
                                                <td colspan='2'>{calibrationDate}</td>
                                             </tr>
                                             <tr class='info'>
                                                <td>{_sharedLocalizer["CalibrationStatus"]}</td>
                                                <td colspan='2'><span class = 'calibartion-status' {(isCalibrated ? "style='background-color: #218838;'" : "style='background-color: #dc3545;'")}>{calibrationStatus}</span></td>
                                             </tr>
                                             <tr class='footer'>
                                                 <td dir='{dir}' colspan='9'>
                                                     <div class='d-inline'>
                                                         <span class='red' dir='{dir}'>[{_sharedLocalizer["MaxTemperature"]} : <span dir='ltr'>{headerInfo.MaxTemperature}°C</span>]</span>
                                                     </div>
                                                    <div class='d-inline'>
                                                         <span class='red' dir='{dir}'>[{_sharedLocalizer["MaxHumidity"]} : <span dir='ltr'>{headerInfo.MaxHumidity}%</span>]</span>
                                                     </div>
                                                        |
                                                      <div class='d-inline'>
                                                         <span class='text-info' dirdir='{dir}'>[{_sharedLocalizer["MinTemperature"]} : <span dir='ltr'>{headerInfo.MinTemperature}°C</span>]</span>
                                                     </div>
                                                     <div class='d-inline'>
                                                         <span class='text-info' dirdir='{dir}'>[{_sharedLocalizer["MinHumidity"]} : <span dir='ltr'>{headerInfo.MinHumidity}%</span>]</span>
                                                     </div>
                                                 </td>
                                             </tr>
                                       </table>");

                    dataInfoHtml.Append($@"<table class='table-group-header' dir='{dir}'>
                                <tr style='background-color: #9BC2E6 !important;color: black !important;'>
                                    <th colspan='{colSpan}'></th>
                                    <th>
                                       {_sharedLocalizer["Temperature"]}
                                    </th>
                                    <th>
                                        {_sharedLocalizer["Humidity"]}
                                    </th>
                                </tr>
                                <tr style='background-color: #0a6f6d !important;color: #fff !important;'>
                                    <th colspan='{colSpan}' class='align-middle'>{_sharedLocalizer["Total"]}</th>
                                    <th class='group-header'>
                                            {(headerInfo.MaxTemperature.HasValue ? ($@"<table class='table'>
                                                <tr>
                                                    <td><i class='fa fa-arrow-up text-danger text-bold'></i> {headerInfo.MaxTemperature}</td>
                                                    <td><i class='fa fa-random text-bold'></i> {headerInfo.AverageTemperature}</td>
                                                    <td><i class='fa fa-arrow-down text-info text-bold'></i> {headerInfo.MinTemperature}</td>
                                                </tr>
                                            </table>") : "-")}
                                    </th>
                                    <th class='group-header'>
                                            {(headerInfo.MaxHumidity.HasValue ? ($@"<table class='table'>
                                                <tr>
                                                    <td><i class='fa fa-arrow-up text-danger text-bold'></i> {headerInfo.MaxHumidity}</td>
                                                    <td><i class='fa fa-random text-bold'></i> {headerInfo.AverageHumidity}</td>
                                                    <td><i class='fa fa-arrow-down text-info text-bold'></i> {headerInfo.MinHumidity}</td>
                                                </tr>
                                            </table>") : "-")}
                                    </th>
                                </tr>
                                ");

                    foreach (var monthItem in result.Data.MonthList)
                    {
                        dataBodyHtml.Append($@"
                                    <tr style='background-color: #0a6f6d !important;color: #fff !important;'>
                                         <th colspan='{colSpan}' class='align-middle'>
                                             {monthItem.HeaderInfo.DateText}
                                         </th>
                                         <th class='group-header'>
                                                 {(monthItem.HeaderInfo.MaxTemperature.HasValue ? ($@"<table class='table'>
                                                     <tr>
                                                         <td><i class='fa fa-arrow-up text-danger text-bold'></i> {monthItem.HeaderInfo.MaxTemperature}</td>
                                                         <td><i class='fa fa-random text-bold'></i> {monthItem.HeaderInfo.AverageTemperature}</td>
                                                         <td><i class='fa fa-arrow-down text-info text-bold'></i> {monthItem.HeaderInfo.MinTemperature}</td>
                                                     </tr>
                                                 </table>") : "-")}
                                         </th>
                                         <th class='group-header'>
                                                 {(monthItem.HeaderInfo.MaxHumidity.HasValue ? ($@"<table class='table'>
                                                     <tr>
                                                         <td><i class='fa fa-arrow-up text-danger text-bold'></i> {monthItem.HeaderInfo.MaxHumidity}</td>
                                                         <td><i class='fa fa-random text-bold'></i> {monthItem.HeaderInfo.AverageHumidity}</td>
                                                         <td><i class='fa fa-arrow-down text-info text-bold'></i> {monthItem.HeaderInfo.MinHumidity}</td>
                                                     </tr>
                                                 </table>") : "-")}
                                         </th>
                                    </tr>");

                        foreach (var dayItem in monthItem.DayList)
                        {
                            dataBodyHtml.Append($@"
                            <tr style='background-color: #0a6f6d !important;color: #fff !important;'>
                                <th colspan='{colSpan}' class='align-middle day-sep'>
                                    {dayItem.HeaderInfo.DateText}
                                </th>
                                <th class='group-header day-sep'>
                                        {(dayItem.HeaderInfo.AverageTemperature.HasValue ? ($@"<table class='table'>
                                            <tr>
                                                <td><i class='fa fa-arrow-up text-danger text-bold'></i> {dayItem.HeaderInfo.MaxTemperature}</td>
                                                <td><i class='fa fa-random text-bold'></i> {dayItem.HeaderInfo.AverageTemperature}</td>
                                                <td><i class='fa fa-arrow-down text-info text-bold'></i> {dayItem.HeaderInfo.MinTemperature}</td>
                                            </tr>
                                        </table>") : "-")}
                                </th>
                                <th class='group-header day-sep'>
                                        {(dayItem.HeaderInfo.AverageHumidity.HasValue ? ($@"<table class='table'>
                                            <tr>
                                                <td><i class='fa fa-arrow-up text-danger text-bold'></i> {dayItem.HeaderInfo.MaxHumidity}</td>
                                                <td><i class='fa fa-random text-bold'></i> {dayItem.HeaderInfo.AverageHumidity}</td>
                                                <td><i class='fa fa-arrow-down text-info text-bold'></i> {dayItem.HeaderInfo.MinHumidity}</td>
                                            </tr>
                                        </table>") : "-")}
                                </th>
                            </tr>");

                            int index = 0;
                            foreach (var item in dayItem.HistoryList)
                            {
                                dataBodyHtml.Append($@"
                                <tr class='data' {(index % 2 == 0 ? "style='background-color: #d8d8d8 !important;color: #342f2f !important;'" : "")}>
                                    <td class='' dir='ltr'>
                                        {item.GPSDate}
                                    </td>
                                    <td class='group-header'>
                                        {item.Temperature}
                                    </td>
                                    <td class='group-header'>
                                        {item.Humidity}
                                    </td>
                                </tr>");
                                index++;
                            }
                        }
                    }

                    sb.Append(headerHtml.ToString());
                    sb.Append(dataInfoHtml.ToString());
                    sb.Append(dataHeaderHtml.ToString());
                    sb.Append(dataBodyHtml.ToString());
                    sb.Append(@"</table></body></html>");
                    #endregion

                    #region PDF Settings
                    var globalSettings = new GlobalSettings
                    {
                        ColorMode = ColorMode.Color,
                        Orientation = Orientation.Portrait,
                        PaperSize = PaperKind.A4,
                        Margins = new MarginSettings { Top = 2, Left = 2, Right = 2 },
                        DocumentTitle = title
                    };
                    var footerText = !string.IsNullOrEmpty(systemSetting.CompanyName) ? string.Format(_sharedLocalizer["InventoryHistoryReportTitle"].Value, systemSetting.CompanyName) :
                       string.Format(_sharedLocalizer["InventoryHistoryReportTitle"].Value, string.Empty);
                    var objectSettings = new ObjectSettings
                    {
                        PagesCount = true,
                        HtmlContent = sb.ToString(),
                        WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(), "assets/pdfStyle", "styles.css") },
                        FooterSettings = { FontName = "Segoe UI", FontSize = 9, Line = true, Left = GPSHelper.ToDateTimeString(DateTime.Now), Center = footerText, Right = "Page [page] of [toPage]" }
                    };
                    #endregion

                    var pdf = new HtmlToPdfDocument()
                    {
                        GlobalSettings = globalSettings,
                        Objects = { objectSettings }
                    };

                    var file = _pdfConverter.Convert(pdf);
                    finalResult.Success(file);
                }
                catch (Exception ex)
                {
                    finalResult.ServerError(ex.Message);
                }
            }
            else
            {
                finalResult = new ReturnResult<byte[]>()
                {
                    IsSuccess = false,
                    HttpCode = result.HttpCode,
                    ErrorList = result.ErrorList
                };
            }
            return finalResult;
        }
        public async Task<ReturnResult<byte[]>> InventoryHistoryReportPDF(long inventoryId,string sensorsSN, string fromDate, string toDate, string groupUpdatesByType, int? groupUpdatesValue, bool isEnglish)
        {
            SetCulture(isEnglish);
            var finalResult = new ReturnResult<byte[]>();

            var historyResult = await GetGroupedTemperatureAndHumidityInventoryHistoryReport(inventoryId, sensorsSN, fromDate, toDate, groupUpdatesByType, groupUpdatesValue, isEnglish);
            if (historyResult.IsSuccess)
            {
                try
                {
                    string title = _sharedLocalizer["InventoryHistoryReport"];

                    string fleetName = "";
                    string warehouseName = "";
                    string inventoryName = "";

                    var inventory = await _unitOfWork.InventoryRepository.FindByIdAsync(inventoryId);
                    if (inventory != null)
                    {
                        fleetName = isEnglish ? inventory.Warehouse.Fleet.NameEn : inventory.Warehouse.Fleet.Name;
                        warehouseName = inventory.Warehouse.Name;
                        inventoryName = inventory.Name;
                    }

                    var fleet = inventory.Warehouse.Fleet;
                    var systemSetting = await _unitOfWork.SystemSettingRepository.LoadSystemSettingAsync();
                    var imgExtention = string.Empty;
                    if (fleet.LogoPhotoByte != null)
                    {
                        if (fleet.LogoPhotoByte.Length > 0)
                        {
                            //base64FleetLogo = Convert.ToBase64String(fleet.LogoPhotoByte);
                            imgExtention = fleet.LogoPhotoExtention;
                            SaveImage(_hostingEnvironment.ContentRootPath + @"\assets\imgs", fleet.LogoPhotoByte, fleet.Id + fleet.LogoPhotoExtention);
                        }
                    }

                    if (string.IsNullOrEmpty(imgExtention))
                    {
                        if (systemSetting.LogoPhotoByte != null)
                        {
                            //base64FleetLogo = Convert.ToBase64String(fleet.LogoPhotoByte);
                            imgExtention = ".png";
                            SaveImage(_hostingEnvironment.ContentRootPath + @"\assets\imgs", systemSetting.LogoPhotoByte, fleet.Id + imgExtention);
                        }
                    }


                    #region Create Html Template

                    var logoPath = _hostingEnvironment.ContentRootPath + @"\assets\imgs\logo.png";

                    //var src = !string.IsNullOrEmpty(base64FleetLogo) ? $"data:image/{fleet.LogoPhotoExtention}; base64,{base64FleetLogo}" : logoPath;

                    var src = !string.IsNullOrEmpty(imgExtention) ? _hostingEnvironment.ContentRootPath + @"\assets\imgs\"+ fleet.Id + imgExtention : logoPath;

                    string dir = isEnglish ? "ltr" : "rtl";
                    var colSpan = 1;
                    if (!string.IsNullOrEmpty(groupUpdatesByType))
                    {
                        colSpan = 1;
                    }

                    var sb = new StringBuilder();
                    var dataBodyHtml = new StringBuilder();

                    dataBodyHtml.Append($@"<html><head></head><body>
                                 <table class='table' dir='{dir}'>
                                 <tr>
                                    <td colspan='2' class='logo'>
                                        <img src='{src}' />
                                    </td>
                                    <td colspan='2' style='background-color: #0a6f6d !important;color: #fff !important;font-size: 26px !important;font-weight: bold !important;'>{title}</td>
                                 </tr>
                                 <tr>
                                    <td colspan='4' class='row-sep'></td>
                                 </tr>
                                 <tr class='info'>
                                    <td>{_sharedLocalizer["Fleet"]}</td>
                                    <td colspan='2'>{fleetName}</td>
                                 </tr>   
                                 <tr class='info'>
                                    <td>{_sharedLocalizer["Warehouse"]}</td>
                                    <td colspan='2'>{warehouseName}</td>
                                </tr>     
                                <tr class='info'>
                                    <td>{_sharedLocalizer["Inventory"]}</td>
                                    <td colspan='2'>{inventoryName}</td>
                                </tr>                               
                                <tr class='info'>
                                    <td>{_sharedLocalizer["Date"]}</td>
                                    <td colspan='2' dir='{dir}'>{_sharedLocalizer["from"]}: <span dir='ltr'>{fromDate}</span> - {_sharedLocalizer["to"]}: <span dir='ltr'>{toDate}</span></td>
                                </tr>
                                </table>");

                    int index = 0;
                    foreach (var sensorItem in historyResult.Data)
                    {
                        var headerInfo = sensorItem.SensorHistoryReport.HeaderInfo;
                        var monthList = sensorItem.SensorHistoryReport.MonthList;
                        var calibrationStatus = sensorItem.IsCalibrated ? _sharedLocalizer["Calibrated"] : _sharedLocalizer["NotCalibrated"];
                        dataBodyHtml.Append($@"<table class='table' style='margin: 0;border-top: 4px solid #acb1ae' dir='{dir}'>
                               <tr class='info-sensor-inventory'>
                                   <td>{_sharedLocalizer["Sensor"]}</td>
                                    <td colspan='2'>{sensorItem.Name}</td>
                                </tr> 
                                <tr class='info-sensor-inventory'>
                                   <td>{_sharedLocalizer["CalibrationDate"]}</td>
                                    <td colspan='2'>{GPSHelper.DateToFormatedString(sensorItem.CalibrationDate.Value)}</td>
                                </tr>
                                <tr class='info-sensor-inventory'>
                                   <td>{_sharedLocalizer["CalibrationStatus"]}</td>
                                    <td colspan='2'><span class = 'calibartion-status' {(sensorItem.IsCalibrated ? "style='background-color: #218838;'" : "style='background-color: #dc3545;'")}>{calibrationStatus}</span></td>
                                </tr>
                                <tr class='sep-header footer'>
                                    <td dir='{dir}' colspan='2'>
                                        <div class='d-inline'>
                                            <span class='red' dir='{dir}'>[{_sharedLocalizer["MaxTemperature"]} : <span dir='ltr'>{headerInfo.MaxTemperature}°C</span>]</span>
                                        </div>
                                        <div class='d-inline'>
                                            <span class='red' dir='{dir}'>[{_sharedLocalizer["MaxHumidity"]} : <span dir='ltr'>{headerInfo.MaxHumidity}%</span>]</span>
                                        </div>
                                           |
                                         <div class='d-inline'>
                                            <span class='text-info' dir='{dir}'>[{_sharedLocalizer["MinTemperature"]} : <span dir='ltr'>{headerInfo.MinTemperature}°C</span>]</span>
                                        </div>
                                        <div class='d-inline'>
                                            <span class='text-info' dir='{dir}'>[{_sharedLocalizer["MinHumidity"]} : <span dir='ltr'>{headerInfo.MinHumidity}%</span>]</span>
                                        </div>
                                    </td>
                                </tr></table>");

                        dataBodyHtml.Append($@"<table class='table-group-header' dir='{dir}' style='margin-bottom: 10px;'>
                                <tr style='background-color: #0a6f6d !important;color: #fff !important;'>
                                    <th colspan='{colSpan}'></th>
                                    <th>
                                       {_sharedLocalizer["Temperature"]}
                                    </th>
                                    <th>
                                        {_sharedLocalizer["Humidity"]}
                                    </th>
                                </tr>
                                <tr style='background-color: #0a6f6d !important;color: #fff !important;'>
                                    <th colspan='{colSpan}' class='align-middle'>{_sharedLocalizer["Total"]}</th>
                                    <th class='group-header'>
                                            {(headerInfo.MaxTemperature.HasValue ? ($@"<table class='table'>
                                                <tr>
                                                    <td><i class='fa fa-arrow-up text-danger text-bold'></i> {headerInfo.MaxTemperature}</td>
                                                    <td><i class='fa fa-random text-bold'></i> {headerInfo.AverageTemperature}</td>
                                                    <td><i class='fa fa-arrow-down text-info text-bold'></i> {headerInfo.MinTemperature}</td>
                                                </tr>
                                            </table>") : "-")}
                                    </th>
                                    <th class='group-header'>
                                            {(headerInfo.MaxHumidity.HasValue ? ($@"<table class='table'>
                                                <tr>
                                                    <td><i class='fa fa-arrow-up text-danger text-bold'></i> {headerInfo.MaxHumidity}</td>
                                                    <td><i class='fa fa-random text-bold'></i> {headerInfo.AverageHumidity}</td>
                                                    <td><i class='fa fa-arrow-down text-info text-bold'></i> {headerInfo.MinHumidity}</td>
                                                </tr>
                                            </table>") : "-")}
                                    </th>
                                </tr>
                                ");

                        foreach (var monthItem in monthList)
                        {
                            dataBodyHtml.Append($@"
                                    <tr style='background-color: #0a6f6d !important;color: #fff !important;'>
                                         <th colspan='{colSpan}' class='align-middle'>
                                             {monthItem.HeaderInfo.DateText}
                                         </th>
                                         <th class='group-header'>
                                                 {(monthItem.HeaderInfo.MaxTemperature.HasValue ? ($@"<table class='table'>
                                                     <tr>
                                                         <td><i class='fa fa-arrow-up text-danger text-bold'></i> {monthItem.HeaderInfo.MaxTemperature}</td>
                                                         <td><i class='fa fa-random text-bold'></i> {monthItem.HeaderInfo.AverageTemperature}</td>
                                                         <td><i class='fa fa-arrow-down text-info text-bold'></i> {monthItem.HeaderInfo.MinTemperature}</td>
                                                     </tr>
                                                 </table>") : "-")}
                                         </th>
                                         <th class='group-header'>
                                                 {(monthItem.HeaderInfo.MaxHumidity.HasValue ? ($@"<table class='table'>
                                                     <tr>
                                                         <td><i class='fa fa-arrow-up text-danger text-bold'></i> {monthItem.HeaderInfo.MaxHumidity}</td>
                                                         <td><i class='fa fa-random text-bold'></i> {monthItem.HeaderInfo.AverageHumidity}</td>
                                                         <td><i class='fa fa-arrow-down text-info text-bold'></i> {monthItem.HeaderInfo.MinHumidity}</td>
                                                     </tr>
                                                 </table>") : "-")}
                                         </th>
                                    </tr>");

                            foreach (var dayItem in monthItem.DayList)
                            {
                                dataBodyHtml.Append($@"
                            <tr style='background-color: #0a6f6d !important;color: #fff !important;'>
                                <th colspan='{colSpan}' class='align-middle day-sep'>
                                    {dayItem.HeaderInfo.DateText}
                                </th>
                                <th class='group-header day-sep'>
                                        {(dayItem.HeaderInfo.AverageTemperature.HasValue ? ($@"<table class='table'>
                                            <tr>
                                                <td><i class='fa fa-arrow-up text-danger text-bold'></i> {dayItem.HeaderInfo.MaxTemperature}</td>
                                                <td><i class='fa fa-random text-bold'></i> {dayItem.HeaderInfo.AverageTemperature}</td>
                                                <td><i class='fa fa-arrow-down text-info text-bold'></i> {dayItem.HeaderInfo.MinTemperature}</td>
                                            </tr>
                                        </table>") : "-")}
                                </th>
                                <th class='group-header day-sep'>
                                        {(dayItem.HeaderInfo.AverageHumidity.HasValue ? ($@"<table class='table'>
                                            <tr>
                                                <td><i class='fa fa-arrow-up text-danger text-bold'></i> {dayItem.HeaderInfo.MaxHumidity}</td>
                                                <td><i class='fa fa-random text-bold'></i> {dayItem.HeaderInfo.AverageHumidity}</td>
                                                <td><i class='fa fa-arrow-down text-info text-bold'></i> {dayItem.HeaderInfo.MinHumidity}</td>
                                            </tr>
                                        </table>") : "-")}
                                </th>
                            </tr>");

                                index = 0;
                                foreach (var item in dayItem.HistoryList)
                                {
                                    dataBodyHtml.Append($@"
                                <tr class='data' {(index % 2 == 0 ? "style='background-color: #d8d8d8 !important;color: #342f2f !important;'" : "")}>
                                    <td class='' dir='ltr'>
                                        {item.GPSDate}
                                    </td>
                                    <td class='group-header'>
                                        {item.Temperature}
                                    </td>
                                    <td class='group-header'>
                                        {item.Humidity}
                                    </td>
                                </tr>");
                                    index++;
                                }
                            }
                        }
                        //dataBodyHtml.Append("<div style='page-break-after: always;'></div>");
                    }
                    sb.Append(dataBodyHtml.ToString());
                    sb.Append(@"</table></body></html>");
                    #endregion

                    #region PDF Settings
                    var globalSettings = new GlobalSettings
                    {
                        ColorMode = ColorMode.Color,
                        Orientation = Orientation.Portrait,
                        PaperSize = PaperKind.A4,
                        Margins = new MarginSettings { Top = 2, Left = 2, Right = 2 },
                        DocumentTitle = title
                    };
                    var footerText = !string.IsNullOrEmpty(systemSetting.CompanyName) ? string.Format(_sharedLocalizer["InventoryHistoryReportTitle"].Value, systemSetting.CompanyName) :
                        string.Format(_sharedLocalizer["InventoryHistoryReportTitle"].Value, string.Empty);
                    var objectSettings = new ObjectSettings
                    {
                        PagesCount = true,
                        HtmlContent = sb.ToString(),
                        WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(), "assets/pdfStyle", "styles.css") },
                        FooterSettings = { FontName = "Segoe UI", FontSize = 9, Line = true, Left = GPSHelper.ToDateTimeString(DateTime.Now), Center = footerText, Right = "Page [page] of [toPage]" }
                    };
                    #endregion

                    var pdf = new HtmlToPdfDocument()
                    {
                        GlobalSettings = globalSettings,
                        Objects = { objectSettings }
                    };

                    var file = _pdfConverter.Convert(pdf);
                    finalResult.Success(file);
                }
                catch (Exception ex)
                {
                    finalResult.ServerError(ex.Message);
                }
            }
            else
            {
                finalResult = new ReturnResult<byte[]>()
                {
                    IsSuccess = false,
                    HttpCode = historyResult.HttpCode,
                    ErrorList = historyResult.ErrorList
                };
            }

            return finalResult;
        }


        public async Task<ReturnResult<List<GroupedInventoryAverageTemperatureAndHumidity>>> InventorySensorsAverageTemperatureAndHumidityByHourAsync(long inventoryId, string fromDate, string toDate)
        {
           return await _inventoryHistoryApiProxy.InventorySensorsAverageTemperatureAndHumidityByHour(inventoryId, fromDate, toDate);
        }

        private string ParseGroupedMaxMinAverageTempAndHumidityFromHtml(string htmlString)
        {
            if (string.IsNullOrEmpty(htmlString))
            {
                return "";
            }

            var text = htmlString.Replace("\r\n", "")
                                       .Replace("<table class='table m-0'>", "").Replace("</table>", "").Replace("<tr>", "").Replace("</tr>", "").Replace("<td>", "").Replace("</td>", "")
                                       .Replace("<i class='fa fa-arrow-up text-danger'></i> ", "￪")
                                       .Replace("<i class='fa fa-arrow-down text-info'></i> ", " | ￬")
                                       .Replace("<i class='fa fa-random'></i> ", " | 〜")
                                       .Replace(" ", "");

            var textArray = text.Split("|");

            text = $"{textArray[1].Substring(1, textArray[1].Length - 1)}￬ | {textArray[2].Substring(1, textArray[2].Length - 1)}〜 | {textArray[0].Substring(1, textArray[0].Length - 1)}￪";

            return text;
        }


        private async Task<ReturnResult<List<InventorySensorModel>>> GetInventoryHistoryReport(long inventoryId, string fromDate, string toDate)
        {
            var result = new ReturnResult<List<InventorySensorModel>>();
            try
            {
                var sensorList = await _unitOfWork.InventorySensorRepository.GetBasicByInventoryId(inventoryId);
                var sensorListView = _mapper.Map<List<InventorySensorView>>(sensorList);

                var historyResult = await _inventoryHistoryApiProxy.GetByInventoryId(inventoryId, fromDate, toDate);
                var historyView = _mapper.Map<List<InventoryHistoryView>>(historyResult.Data);

                historyView.ForEach(x => x.InventorySensor = sensorListView.FirstOrDefault(s => s.SensorView.Serial == x.Serial));

                var inventorySensorHistoryGroupList = historyView.Where(x => x.InventorySensor != null).GroupBy(x => x.InventorySensor).ToList();

                List<InventorySensorModel> groupedHistory = new List<InventorySensorModel>();
                foreach (var groupItem in inventorySensorHistoryGroupList)
                {
                    var historyRecords = new List<InventorySensorHistoryModel>();
                    foreach (var historyItem in groupItem.ToList())
                    {
                        historyRecords.Add(new InventorySensorHistoryModel()
                        {
                            Alram = historyItem.Alram,
                            GpsDate = historyItem.GpsDate,
                            GSMStatus = historyItem.GSMStatus,
                            IsLowVoltage = historyItem.IsLowVoltage,
                            Humidity = historyItem.Humidity,
                            Temperature = historyItem.Temperature
                        });
                    }

                    groupedHistory.Add(new InventorySensorModel()
                    {
                        Name = groupItem.Key.SensorView.Name,
                        Serial = groupItem.Key.SensorView.Serial,
                        HistoryRecords = historyRecords
                    });
                }

                result.Success(groupedHistory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        // Grouped Temperature And Humidity Inventory History Report
        private async Task<ReturnResult<List<TemperatureAndHumidityInventoryHistoryReportResult>>> GetGroupedTemperatureAndHumidityInventoryHistoryReport(long inventoryId,string sensorsSN, string fromDate,
            string toDate, string groupUpdatesByType, int? groupUpdatesValue, bool isEnglish)
        {
            var result = new ReturnResult<List<TemperatureAndHumidityInventoryHistoryReportResult>>();
            try
            {
                var sensorList = await _unitOfWork.InventorySensorRepository.GetBasicByInventoryId(inventoryId);
                
                if (!string.IsNullOrEmpty(sensorsSN))
                {
                    var listSelectedSensors = GPSHelper.StringToList(sensorsSN);
                    sensorList = sensorList.Where(s => listSelectedSensors.Any(x => x == s.Sensor.Serial)).ToList();
                }
                var groupedHistory = new List<TemperatureAndHumidityInventoryHistoryReportResult>();
                foreach (var item in sensorList)
                {
                    var sensorHistoryReportResult = await _inventoryHistoryApiProxy.GroupedSensorTemperatureAndHumidityHistory(inventoryId, item.Sensor.Serial, fromDate, toDate, groupUpdatesByType, groupUpdatesValue, isEnglish);
                    if (sensorHistoryReportResult.IsSuccess)
                    {
                        groupedHistory.Add(new TemperatureAndHumidityInventoryHistoryReportResult()
                        {
                            Name = item.Sensor.Name,
                            Serial = item.Sensor.Serial,
                            CalibrationDate = item.Sensor.CalibrationDate,
                            IsCalibrated = item.Sensor.CalibrationDate >= DateTime.Now ? true : false,
                            SensorHistoryReport = sensorHistoryReportResult.Data
                        });
                    }
                }

                result.Success(groupedHistory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }
        public async Task<ReturnResult<List<TemperatureAndHumidityInventoryHistoryReportResult>>> TemperatureAndHumidityInventoryHistoryReport(FilterInventoryHistory filterInventoryHistory)
        {
            return await GetGroupedTemperatureAndHumidityInventoryHistoryReport(filterInventoryHistory);
        }
        private async Task<ReturnResult<List<TemperatureAndHumidityInventoryHistoryReportResult>>> GetGroupedTemperatureAndHumidityInventoryHistoryReport(FilterInventoryHistory filterInventoryHistory)
        {
            var result = new ReturnResult<List<TemperatureAndHumidityInventoryHistoryReportResult>>();
            try
            {
                var sensorHistoryReportResult = await _inventoryHistoryApiProxy.GroupedSensorTemperatureAndHumidityHistory(filterInventoryHistory);
                if (sensorHistoryReportResult.IsSuccess)
                {
                    if (sensorHistoryReportResult.Data.Count > 0)
                    {
                        var inventoryIds = sensorHistoryReportResult.Data.Select(x => (long)x.InventoryId).Distinct().ToList();
                        var inventories = await _unitOfWork.InventoryRepository.FindInventoriesIncludingWarehousesAsync(inventoryIds);
                        var sensorserials = sensorHistoryReportResult.Data.Select(x => x.SensorSerial).Distinct().ToList();
                        var sensors = await _unitOfWork.SensorRepository.AllSensorsBySerialNumberAsync(sensorserials);
                        //var groupedSensorHistoryByInventory = sensorHistoryReportResult.Data.GroupBy(x => x.InventoryId);
                        var groupedHistory = new List<TemperatureAndHumidityInventoryHistoryReportResult>();
                        foreach (var data in sensorHistoryReportResult.Data)
                        {
                            var sensor = sensors.Where(x => x.Serial == data.SensorSerial).FirstOrDefault();
                            var inventory = inventories.Where(x => x.Id == (long)data.InventoryId).FirstOrDefault();
                            groupedHistory.Add(new TemperatureAndHumidityInventoryHistoryReportResult()
                            {
                                Name = sensor.Name,
                                CalibrationDate = sensor.CalibrationDate,
                                IsCalibrated = sensor.CalibrationDate >= DateTime.Now ? true : false,
                                Serial = data.SensorSerial,
                                WarehouseId = inventory.WarehouseId,
                                InventoryId = (long)data.InventoryId,
                                WarehouseName = inventory.Warehouse.Name,
                                InventoryName = inventory.Name,
                                FleetId = inventory.Warehouse.FleetId,
                                SensorHistoryReport = data
                            }); ;
                        }
                        result.Success(groupedHistory);
                    }
                    else
                    {
                        result.NotFound(_sharedLocalizer["NoRecordsFound"]);
                    }
                }
                else
                {
                    result.HttpCode = sensorHistoryReportResult.HttpCode;
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }
        private async Task<ReturnResult<List<InventoryHistoryView>>> SensorHistory(long inventoryId, string sensorSerial, string fromDate, string toDate)
        {
            var result = new ReturnResult<List<InventoryHistoryView>>();

            try
            {
                var sensorList = await _unitOfWork.InventorySensorRepository.GetInventorySensorList(inventoryId);
                var sensorListView = _mapper.Map<List<InventorySensorView>>(sensorList);

                var historyResult = await _inventoryHistoryApiProxy.SensorHistory(inventoryId, sensorSerial, fromDate, toDate);
                var historyView = historyResult.Data;

                historyView.ForEach(x => x.InventorySensor = sensorListView.FirstOrDefault(s => s.SensorView.Serial == x.Serial));

                result.Success(historyView);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, result);
                result.ServerError(ex.Message);
            }
            return result;
        }

        private void SetCulture(bool isEnglish)
        {
            if (isEnglish)
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
            }
            else
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("ar-SA");
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("ar-SA");
            }
        }
        private void SaveImage(string folderPath, byte[] img, string fileName)
        {
            if (System.IO.Directory.Exists(folderPath))
            {
                using (MemoryStream ms = new MemoryStream(img))
                {
                    using (Bitmap bm2 = new Bitmap(ms))
                    {
                        bm2.Save(folderPath + "\\" + fileName);
                        bm2.Dispose();
                        //System.IO.File.WriteAllBytes(Path.Combine(folderPath, fileName), img);
                    }
                    ms.Dispose();
                }
            }
        }

        public async Task<ReturnResult<byte[]>> InventoryHistoryReportPDF(FilterInventoryHistory filterInventoryHistory)
        {
            SetCulture(filterInventoryHistory.isEnglish);
            var finalResult = new ReturnResult<byte[]>();

            var historyResult = await GetGroupedTemperatureAndHumidityInventoryHistoryReport(filterInventoryHistory);
            if (historyResult.IsSuccess)
            {
                try
                {
                    string title = _sharedLocalizer["WarehouseInventoryHistoryReport"];
                    var warehouses = historyResult.Data.Select(x => new { WarehouseId = x.WarehouseId, WarehouseName = x.WarehouseName }).DistinctBy(x => x.WarehouseId).ToList();
                    var inventories = historyResult.Data.Select(x => new { InventoryId = x.InventoryId, InventoryName = x.InventoryName, WarehouseId = x.WarehouseId }).DistinctBy(x => x.InventoryId).ToList();
                    var fleet = historyResult.Data.Count > 0 ? await _unitOfWork.FleetRepository.FindByIdAsync(historyResult.Data.First().FleetId) : null;
                    string fleetName = filterInventoryHistory.isEnglish ? fleet.NameEn : fleet.Name;
                    var systemSetting = await _unitOfWork.SystemSettingRepository.LoadSystemSettingAsync();
                    var imgExtention = string.Empty;


                    if (fleet.LogoPhotoByte != null)
                    {
                        if (fleet.LogoPhotoByte.Length > 0)
                        {
                            //base64FleetLogo = Convert.ToBase64String(fleet.LogoPhotoByte);
                            imgExtention = fleet.LogoPhotoExtention;
                            SaveImage(_hostingEnvironment.ContentRootPath + @"\assets\imgs", fleet.LogoPhotoByte, fleet.Id + fleet.LogoPhotoExtention);
                        }
                    }

                    if (string.IsNullOrEmpty(imgExtention))
                    {
                        if (systemSetting.LogoPhotoByte != null)
                        {
                            //base64FleetLogo = Convert.ToBase64String(fleet.LogoPhotoByte);
                            imgExtention = ".png";
                            SaveImage(_hostingEnvironment.ContentRootPath + @"\assets\imgs", systemSetting.LogoPhotoByte, fleet.Id + imgExtention);
                        }
                    }


                    #region Create Html Template

                    var logoPath = _hostingEnvironment.ContentRootPath + @"\assets\imgs\logo.png";

                    //var src = !string.IsNullOrEmpty(base64FleetLogo) ? $"data:image/{fleet.LogoPhotoExtention}; base64,{base64FleetLogo}" : logoPath;

                    var src = !string.IsNullOrEmpty(imgExtention) ? _hostingEnvironment.ContentRootPath + @"\assets\imgs\" + fleet.Id + imgExtention : logoPath;

                    string dir = filterInventoryHistory.isEnglish ? "ltr" : "rtl";
                    var colSpan = 1;
                    if (!string.IsNullOrEmpty(filterInventoryHistory.groupUpdatesByType))
                    {
                        colSpan = 1;
                    }

                    var sb = new StringBuilder();
                    var dataBodyHtml = new StringBuilder();
                    dataBodyHtml.Append($@"<html><head></head><body>");

                    foreach (var warehouse in warehouses)
                    {
                        var head = $@"<table class='table' dir='{dir}'>
                                 <tr>
                                    <td colspan='2' class='logo'>
                                        <img src='{src}' />
                                    </td>
                                    <td colspan='2' style='background-color: #0a6f6d !important;color: #fff !important;font-size: 26px !important;font-weight: bold !important;'>{title}</td>
                                 </tr>
                                 <tr>
                                    <td colspan='4' class='row-sep'></td>
                                 </tr>
                                 <tr class='info'>
                                    <td>{_sharedLocalizer["Fleet"]}</td>
                                    <td colspan='2'>{fleetName}</td>
                                 </tr>   
                                 <tr class='info'>
                                    <td>{_sharedLocalizer["Warehouse"]}</td>
                                    <td colspan='2'>{warehouse.WarehouseName}</td>
                                </tr>                                  
                                <tr class='info'>
                                    <td>{_sharedLocalizer["Date"]}</td>
                                    <td colspan='2' dir='{dir}'>{_sharedLocalizer["from"]}: <span dir='ltr'>{filterInventoryHistory.fromDate}</span> - {_sharedLocalizer["to"]}: <span dir='ltr'>{filterInventoryHistory.toDate}</span></td>
                                </tr>
                                </table>";
                        dataBodyHtml.Append(head);

                        var filteredInventories = inventories.Where(x => x.WarehouseId == warehouse.WarehouseId).ToList();
                        
                        foreach (var inventory in filteredInventories)
                        {
                            int index = 0;
                            var filteredSensors = historyResult.Data.Where(x => x.WarehouseId == warehouse.WarehouseId && x.InventoryId == inventory.InventoryId).ToList();
                            //var sensors = historyResult.Data.Where(x => x.WarehouseId == warehouse.WarehouseId && x.InventoryId == inventory.InventoryId).Select(x => x.Serial).Distinct().ToList();
                            foreach (var sensorItem in filteredSensors)
                            {
                                var headerInfo = sensorItem.SensorHistoryReport.HeaderInfo;
                                var monthList = sensorItem.SensorHistoryReport.MonthList;
                                var calibrationStatus = sensorItem.IsCalibrated ? _sharedLocalizer["Calibrated"] : _sharedLocalizer["NotCalibrated"];
                                dataBodyHtml.Append($@"<table class='table' style='margin: 0;border-top: 4px solid #acb1ae' dir='{dir}'>
                                <tr class='info-sensor-inventory'>
                                   <td>{_sharedLocalizer["Inventory"]}</td>
                                    <td colspan='2'>{inventory.InventoryName}</td>
                                </tr>
                                <tr class='info-sensor-inventory'>
                                   <td>{_sharedLocalizer["Sensor"]}</td>
                                    <td colspan='2'>{sensorItem.Name}</td>
                                </tr> 
                                <tr class='info-sensor-inventory'>
                                   <td>{_sharedLocalizer["CalibrationDate"]}</td>
                                    <td colspan='2'>{GPSHelper.DateToFormatedString(sensorItem.CalibrationDate.Value)}</td>
                                </tr>
                                <tr class='info-sensor-inventory'>
                                   <td>{_sharedLocalizer["CalibrationStatus"]}</td>
                                    <td colspan='2'><span class = 'calibartion-status' {(sensorItem.IsCalibrated ? "style='background-color: #218838;'" : "style='background-color: #dc3545;'")}>{calibrationStatus}</span></td>
                                </tr>
                                <tr class='sep-header footer'>
                                    <td dir='{dir}' colspan='2'>
                                        <div class='d-inline'>
                                            <span class='red' dir='{dir}'>[{_sharedLocalizer["MaxTemperature"]} : <span dir='ltr'>{headerInfo.MaxTemperature}°C</span>]</span>
                                        </div>
                                        <div class='d-inline'>
                                            <span class='red' dir='{dir}'>[{_sharedLocalizer["MaxHumidity"]} : <span dir='ltr'>{headerInfo.MaxHumidity}%</span>]</span>
                                        </div>
                                           |
                                         <div class='d-inline'>
                                            <span class='text-info' dir='{dir}'>[{_sharedLocalizer["MinTemperature"]} : <span dir='ltr'>{headerInfo.MinTemperature}°C</span>]</span>
                                        </div>
                                        <div class='d-inline'>
                                            <span class='text-info' dir='{dir}'>[{_sharedLocalizer["MinHumidity"]} : <span dir='ltr'>{headerInfo.MinHumidity}%</span>]</span>
                                        </div>
                                    </td>
                                </tr></table>");

                                dataBodyHtml.Append($@"<table class='table-group-header' dir='{dir}' style='margin-bottom: 10px;'>
                                <tr style='background-color: #0a6f6d !important;color: #fff !important;'>
                                    <th colspan='{colSpan}'></th>
                                    <th>
                                       {_sharedLocalizer["Temperature"]}
                                    </th>
                                    <th>
                                        {_sharedLocalizer["Humidity"]}
                                    </th>
                                </tr>
                                <tr style='background-color: #0a6f6d !important;color: #fff !important;'>
                                    <th colspan='{colSpan}' class='align-middle'>{_sharedLocalizer["Total"]}</th>
                                    <th class='group-header'>
                                            {(headerInfo.MaxTemperature.HasValue ? ($@"<table class='table'>
                                                <tr>
                                                    <td><i class='fa fa-arrow-up text-danger text-bold'></i> {headerInfo.MaxTemperature}</td>
                                                    <td><i class='fa fa-random text-bold'></i> {headerInfo.AverageTemperature}</td>
                                                    <td><i class='fa fa-arrow-down text-info text-bold'></i> {headerInfo.MinTemperature}</td>
                                                </tr>
                                            </table>") : "-")}
                                    </th>
                                    <th class='group-header'>
                                            {(headerInfo.MaxHumidity.HasValue ? ($@"<table class='table'>
                                                <tr>
                                                    <td><i class='fa fa-arrow-up text-danger text-bold'></i> {headerInfo.MaxHumidity}</td>
                                                    <td><i class='fa fa-random text-bold'></i> {headerInfo.AverageHumidity}</td>
                                                    <td><i class='fa fa-arrow-down text-info text-bold'></i> {headerInfo.MinHumidity}</td>
                                                </tr>
                                            </table>") : "-")}
                                    </th>
                                </tr>
                                ");

                                foreach (var monthItem in monthList)
                                {
                                    dataBodyHtml.Append($@"
                                    <tr style='background-color: #0a6f6d !important;color: #fff !important;'>
                                         <th colspan='{colSpan}' class='align-middle'>
                                             {monthItem.HeaderInfo.DateText}
                                         </th>
                                         <th class='group-header'>
                                                 {(monthItem.HeaderInfo.MaxTemperature.HasValue ? ($@"<table class='table'>
                                                     <tr>
                                                         <td><i class='fa fa-arrow-up text-danger text-bold'></i> {monthItem.HeaderInfo.MaxTemperature}</td>
                                                         <td><i class='fa fa-random text-bold'></i> {monthItem.HeaderInfo.AverageTemperature}</td>
                                                         <td><i class='fa fa-arrow-down text-info text-bold'></i> {monthItem.HeaderInfo.MinTemperature}</td>
                                                     </tr>
                                                 </table>") : "-")}
                                         </th>
                                         <th class='group-header'>
                                                 {(monthItem.HeaderInfo.MaxHumidity.HasValue ? ($@"<table class='table'>
                                                     <tr>
                                                         <td><i class='fa fa-arrow-up text-danger text-bold'></i> {monthItem.HeaderInfo.MaxHumidity}</td>
                                                         <td><i class='fa fa-random text-bold'></i> {monthItem.HeaderInfo.AverageHumidity}</td>
                                                         <td><i class='fa fa-arrow-down text-info text-bold'></i> {monthItem.HeaderInfo.MinHumidity}</td>
                                                     </tr>
                                                 </table>") : "-")}
                                         </th>
                                    </tr>");

                                    foreach (var dayItem in monthItem.DayList)
                                    {
                                        dataBodyHtml.Append($@"
                                        <tr style='background-color: #0a6f6d !important;color: #fff !important;'>
                                            <th colspan='{colSpan}' class='align-middle day-sep'>
                                                {dayItem.HeaderInfo.DateText}
                                            </th>
                                            <th class='group-header day-sep'>
                                                    {(dayItem.HeaderInfo.AverageTemperature.HasValue ? ($@"<table class='table'>
                                                        <tr>
                                                            <td><i class='fa fa-arrow-up text-danger text-bold'></i> {dayItem.HeaderInfo.MaxTemperature}</td>
                                                            <td><i class='fa fa-random text-bold'></i> {dayItem.HeaderInfo.AverageTemperature}</td>
                                                            <td><i class='fa fa-arrow-down text-info text-bold'></i> {dayItem.HeaderInfo.MinTemperature}</td>
                                                        </tr>
                                                    </table>") : "-")}
                                            </th>
                                            <th class='group-header day-sep'>
                                                    {(dayItem.HeaderInfo.AverageHumidity.HasValue ? ($@"<table class='table'>
                                                        <tr>
                                                            <td><i class='fa fa-arrow-up text-danger text-bold'></i> {dayItem.HeaderInfo.MaxHumidity}</td>
                                                            <td><i class='fa fa-random text-bold'></i> {dayItem.HeaderInfo.AverageHumidity}</td>
                                                            <td><i class='fa fa-arrow-down text-info text-bold'></i> {dayItem.HeaderInfo.MinHumidity}</td>
                                                        </tr>
                                                    </table>") : "-")}
                                            </th>
                                        </tr>");

                                        index = 0;
                                        foreach (var item in dayItem.HistoryList)
                                        {
                                            dataBodyHtml.Append($@"
                                            <tr class='data' {(index % 2 == 0 ? "style='background-color: #d8d8d8 !important;color: #342f2f !important;'" : "")}>
                                                <td class='' dir='ltr'>
                                                    {item.GPSDate}
                                                </td>
                                                <td class='group-header'>
                                                    {item.Temperature}
                                                </td>
                                                <td class='group-header'>
                                                    {item.Humidity}
                                                </td>
                                            </tr>");
                                            index++;
                                        }
                                    }
                                }
                                dataBodyHtml.Append(@"</table>");
                                dataBodyHtml.Append("<div style='page-break-after: always;'></div>");
                            }
                            //dataBodyHtml.Append(@"</table>");
                        }
                     
                    }
                    sb.Append(dataBodyHtml.ToString());
                    sb.Append(@"</body></html>");

                    #endregion
                    #region PDF Settings
                    var globalSettings = new GlobalSettings
                    {
                        ColorMode = ColorMode.Color,
                        Orientation = Orientation.Portrait,
                        PaperSize = PaperKind.A4,
                        Margins = new MarginSettings { Top = 2, Left = 2, Right = 2 },
                        DocumentTitle = title
                    };
                    var footerText = !string.IsNullOrEmpty(systemSetting.CompanyName) ? string.Format(_sharedLocalizer["WarehouseInventoryHistoryReportTitle"].Value, systemSetting.CompanyName) :
                        string.Format(_sharedLocalizer["WarehouseInventoryHistoryReportTitle"].Value, string.Empty);
                    var objectSettings = new ObjectSettings
                    {
                        PagesCount = true,
                        HtmlContent = sb.ToString(),
                        WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(), "assets/pdfStyle", "styles.css") },
                        FooterSettings = { FontName = "Segoe UI", FontSize = 9, Line = true, Left = GPSHelper.ToDateTimeString(DateTime.Now), Center = footerText, Right = "Page [page] of [toPage]" }
                    };
                    #endregion

                    var pdf = new HtmlToPdfDocument()
                    {
                        GlobalSettings = globalSettings,
                        Objects = { objectSettings }
                    };

                    var file = _pdfConverter.Convert(pdf);
                    finalResult.Success(file);
                }
                catch (Exception ex)
                {
                    finalResult.ServerError(ex.Message);
                }
            }
            else
            {
                finalResult = new ReturnResult<byte[]>()
                {
                    IsSuccess = false,
                    HttpCode = historyResult.HttpCode,
                    ErrorList = historyResult.ErrorList
                };
            }

            return finalResult;
        }
    }
}
