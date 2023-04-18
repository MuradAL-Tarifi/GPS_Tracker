using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Domain.ViewModels
{
   public class OnlineInventorySensors
    {
        public long InventoryId { get; set; }
        public List<InventorySensorTemperatureModel> LsInventorySensorTemperatureModel { get; set; } = new List<InventorySensorTemperatureModel>();
}
}
