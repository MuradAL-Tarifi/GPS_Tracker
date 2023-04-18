using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Domain.Views
{
   public class SystemSettingView
    {
        public long Id { get; set; }
        [MaxLength(200)]
        public string CompanyName { get; set; }
        public byte[] LogoPhotoByte { set; get; }
        public IFormFile LogoPhoto { set; get; }
        public string LogoFileBase64 { get; set; }
        
        [MaxLength(1000)]
        public string GoogleApiKey { get; set; }
        [MaxLength(1000)]
        public string WaslApiKey { get; set; }
        public bool EnableSMTP { get; set; }
        [MaxLength(500)]
        public string SMTP_HOST { get; set; }
        public int SMTP_PORT { get; set; }
        public bool SMTP_IsSslEnabled { get; set; }
        public string SMTP_Address { get; set; }
        [MaxLength(500)]
        public string SMTP_DisplayName { get; set; }
        [MaxLength(100)]
        public string SMTP_Password { get; set; }
        public bool EnableSMS { get; set; }
        [MaxLength(500)]
        public string SMS_GatewayURL { get; set; }
        [MaxLength(100)]
        public string SMS_Password { get; set; }
        [MaxLength(100)]
        public string SMS_Username { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
