using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Models
{
    public class InventorySensorView
    {
        public long Id { get; set; }
        public long InventoryId { get; set; }
        public string Serial { get; set; }
        public string Name { get; set; }
        public string InventoryReferenceKey { get; set; }

        public List<SensorAlertView> SensorAlerts { get; set; }
    }
}
