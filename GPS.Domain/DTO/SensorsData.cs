using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Domain.DTO
{
    public class SensorsData
    {
        public string BrandName { get; set; }
        public string SensorName { get; set; }
        public string InventoryName { get; set; }
        public string WarehouseName { get; set; }
        public DateTime? CalibrationDate { get; set; }
        public string Serial { get; set; }
    }
}
