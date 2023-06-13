using GPS.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Domain.Views
{
    public class AlertSensorView
    {
        public int Id { get; set; }
        [Required]
        public long? InventoryId { get; set; }
        [Required]
        public long? WarehouseId { get; set; }
        [Required]
        public double? MinValueTemperature { get; set; }
        [Required]
        public double? MaxValueTemperature { get; set; }
        [Required]
        public double? MinValueHumidity { get; set; }
        [Required]
        public double? MaxValueHumidity { get; set; }
        [Required]
        public string ToEmails { get; set; }
        public int? AlertTypeLookupId { get; set; }
        public int? Interval { get; set; }
        [Required]
        public string Serial { get; set; }
        public DateTime? CreatedDate { get; set; }
        [Required]
        public string UserName { get; set; }
        public WarehouseView Warehouse { get; set; }
        public InventoryView Inventory { get; set; }
    }
}
