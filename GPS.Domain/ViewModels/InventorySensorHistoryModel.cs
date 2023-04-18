using System;
using System.Collections.Generic;
using System.Text;

namespace GPS.Domain.ViewModels
{
    public class InventorySensorHistoryModel
    {
        public decimal? Temperature { get; set; }
        public decimal? Humidity { get; set; }
        public bool? IsLowVoltage { get; set; }
        public DateTime GpsDate { get; set; }
        public string Alram { get; set; }
        public string GSMStatus { get; set; }
    }
}
