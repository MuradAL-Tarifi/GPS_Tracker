using System;
using System.Collections.Generic;
using System.Text;

namespace GPS.Domain.ViewModels
{
    public class TemperatureAndHumidityInventoryHistoryReportResult
    {
        public string Serial { get; set; }
        public string Name { get; set; }
        public long InventoryId { get; set; }
        public string InventoryName { get; set; }
        public long WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        public long FleetId { get; set; }
        public DateTime? CalibrationDate { get; set; }
        public bool IsCalibrated { get; set; }
        public TemperatureAndHumiditySensorHistoryReportResult SensorHistoryReport { get; set; }
    }
}
