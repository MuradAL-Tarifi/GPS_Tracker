using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Domain.Views
{
    public class InventoryView
    {
        public InventoryView()
        {
            InventorySensors = new List<InventorySensorView>();
        }

        public long Id { get; set; }
        public long WarehouseId { get; set; }
        public long GatewayId { get; set; }
        public string Name { get; set; }
        public string InventoryNumber { get; set; }
        public bool IsActive { get; set; }

        public string WaslActivityType { get; set; } = "SFDA";
        public string SFDAStoringCategory { get; set; }
        public bool IsLinkedWithWasl { get; set; }
        public string ReferenceKey { get; set; }
        //public int? RegisterTypeId { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }

        public WarehouseView Warehouse { get; set; }
        public GatewayView Gateway { get; set; }

        public List<InventorySensorView> InventorySensors { get; set; } = new List<InventorySensorView>();
    }
}
