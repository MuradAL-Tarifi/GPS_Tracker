using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Domain.Models
{
    public class Sensor
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Serial { get; set; }
        public int BrandId { get; set; }
        public DateTime? CalibrationDate { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        [ForeignKey("BrandId")]
        public Brand Brand { get; set; }
    }
}
