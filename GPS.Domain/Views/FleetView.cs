using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Domain.Views
{
    public class FleetView
    {
        public long Id { get; set; }
        public int AgentId { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        [Required]
        [MaxLength(100)]
        public string NameEn { get; set; }
        [EmailAddress(ErrorMessage = "Email is not valid")]
        [MaxLength(50)]
        public string ManagerEmail { get; set; }
        [MaxLength(13)]
        public string ManagerMobile { get; set; }
        [EmailAddress(ErrorMessage = "Email is not valid")]
        [MaxLength(50)]
        public string SupervisorEmail { get; set; }
        [MaxLength(13)]
        public string SupervisorMobile { get; set; }
        public string TaxRegistrationNumber { get; set; }
        public string CommercialRegistrationNumber { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public byte[] LogoPhotoByte { set; get; }
        public IFormFile LogoPhoto { set; get; }
        public string LogoFileBase64 { get; set; }
        [MaxLength(50)]
        public string LogoPhotoExtention { get; set; }
        
        public AgentView Agent { get; set; }
        public FleetDetailsView FleetDetails { get; set; }
    }
}
