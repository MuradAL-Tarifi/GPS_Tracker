using System;
using System.Collections.Generic;
using System.Text;

namespace GPS.Domain.ViewModels
{
    public class InventorySensorTemperatureAndHumidityChartHeaderInfo
    {
        public DateTime HourText { get; set; }

        public decimal? AverageTemperature { get; set; }

        public decimal? AverageHumidity { get; set; }
    }
}
