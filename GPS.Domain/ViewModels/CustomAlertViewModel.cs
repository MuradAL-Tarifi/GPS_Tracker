using GPS.Domain.Models;
using GPS.Domain.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Domain.ViewModels
{
   public class CustomAlertViewModel
    {
        public CustomAlert CustomAlert { set; get; }
        public List<InventoryView> LsInventoryView { set; get; } = new List<InventoryView>();
    }
}
