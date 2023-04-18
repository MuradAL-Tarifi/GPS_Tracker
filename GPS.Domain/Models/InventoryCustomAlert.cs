using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Domain.Models
{
   public class InventoryCustomAlert
    {
        public long Id { get; set; }
        public long CustomAlertId { get; set; }
        public long InventorytId { get; set; }
    }
}
