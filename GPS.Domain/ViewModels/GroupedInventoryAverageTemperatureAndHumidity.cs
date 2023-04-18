using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Domain.ViewModels
{
   public class GroupedInventoryAverageTemperatureAndHumidity
    {
        public List<InventorySensorTemperatureAndHumidityChartHeaderInfo>
            LsInventorySensorTemperatureAndHumidityChartHeaderInfo
        { get; set; } = new List<InventorySensorTemperatureAndHumidityChartHeaderInfo>();

        public string SensorName { get; set; }
    }
}
