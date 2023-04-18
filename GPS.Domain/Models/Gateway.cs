
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GPS.Domain.Models
{
    public class Gateway
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string IMEI { get; set; }
        public string SIMNumber { get; set; }
        public DateTime ExpirationDate { get; set; }
        public DateTime? ActivationDate { get; set; }
        public DateTime? SIMCardExpirationDate { get; set; }
        public int? NumberOfMonths { get; set; }
        public string Notes { get; set; }
        public int BrandId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }

        [ForeignKey("BrandId")]
        public Brand Brand { get; set; }
    }
}
