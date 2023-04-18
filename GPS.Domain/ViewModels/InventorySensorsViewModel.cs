using GPS.Domain.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Domain.ViewModels
{
   public class InventorySensorsViewModel
    {
        public InventoryView InventoryView { get; set; }
        public List<SensorView> LsSensor { get; set; } =  new List<SensorView>();
    }
}
