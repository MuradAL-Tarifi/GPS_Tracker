using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Domain.Models
{
    public class UserInventory
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public long InventoryId { get; set; }
        public Inventory Inventory { get; set; }
    }
}
