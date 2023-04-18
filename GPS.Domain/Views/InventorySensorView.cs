using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Domain.Views
{
    public class InventorySensorView
    {
        public long Id { get; set; }
        public long InventoryId { get; set; }
        public long SensorId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }

        public OnlineInventoryHistoryView OnlineHistory { get; set; }
        public InventoryView Inventory { get; set; }
        public SensorView SensorView { get; set; }
    }
}
