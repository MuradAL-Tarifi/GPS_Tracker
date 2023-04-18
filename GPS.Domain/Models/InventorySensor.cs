
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace GPS.Domain.Models
{
    public class InventorySensor
    {
        public long Id { get; set; }
        public long InventoryId { get; set; }
        public long SensorId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public Inventory Inventory { get; set; }
        public Sensor Sensor { get; set; }
    }
}
