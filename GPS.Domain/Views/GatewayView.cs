using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Domain.Views
{
    public class GatewayView
    {
        public long Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        //[RegularExpression("([1-9][0-9]*)")]
        public string IMEI { get; set; }

        //[Required]
        //[RegularExpression("([1-9][0-9]*)")]
        [StringLength(16)]
        public string SIMNumber { get; set; }

        [Required]
        public string ExpirationDateText { get; set; }
        public DateTime ExpirationDate { get; set; }

        public DateTime? ActivationDate { get; set; }
        public string ActivationDateText { get; set; }
        public DateTime? SIMCardExpirationDate { get; set; }
        public string SIMCardExpirationDateText { get; set; }
        [RegularExpression("([1-9][0-9]*)")]
        public int? NumberOfMonths { get; set; }
        public string Notes { get; set; }
        [Required]
        public int BrandId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public BrandView Brand { get; set; }
    }
}
