using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Models
{
    public class Inventory
    {
        public long Id { get; set; }
        public long WarehouseId { get; set; }
        public long GatewayId { get; set; }
        public string Name { get; set; }
        public string InventoryNumber { get; set; }
        public string SFDAStoringCategory { get; set; }
        public bool IsActive { get; set; }
        public bool IsLinkedWithWasl { get; set; }
        public int? RegisterTypeId { get; set; }
        public string ReferenceKey { get; set; }

    }
}
