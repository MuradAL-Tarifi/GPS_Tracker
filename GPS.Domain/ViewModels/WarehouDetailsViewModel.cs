using GPS.Domain.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Domain.ViewModels
{
   public class WarehouDetailsViewModel
    {
        public WarehouseView WarehouseView { get; set; }
        public List<InventorySensorsViewModel> LsInventorySensors { get; set; } = new List<InventorySensorsViewModel>();
    }
}
