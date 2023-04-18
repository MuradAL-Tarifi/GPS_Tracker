using GPS.Domain.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Domain.ViewModels
{
   public class AlertViewModel
    {
        public InventoryView Inventory { get; set; }
        public WarehouseView Warehouse { get; set; }
        public SensorView Sensor { get; set; }
        public AlertView Alert { get; set; }
        public int NumberOfNewNotifications { get; set; } = 0;

    }
}
