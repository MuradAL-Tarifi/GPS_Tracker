using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Domain.Views
{
    public class SensorView
    {
        public SensorView()
        {
           
        }
        public long Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        public string Serial { get; set; }
        [Required]
        public int BrandId { get; set; }
        public DateTime? CalibrationDate { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public BrandView Brand { get; set; }
        public List<SensorView> SensorsList { get; set; }
        public string InventoryName { get; set; }
        public string WarehouseName { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? DateOfTheLastReading { get; set; }

    }
}
