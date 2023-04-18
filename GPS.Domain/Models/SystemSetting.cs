
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Domain.Models
{
   public class SystemSetting
    {
        public long Id { get; set; }
        public string CompanyName { get; set; }
        public byte[] LogoPhotoByte { set; get; }
        public string GoogleApiKey { get; set; }
        public string WaslApiKey { get; set; }
        public bool EnableSMTP { get; set; }
        public string SMTP_HOST { get; set; }
        public int SMTP_PORT { get; set; }
        public bool SMTP_IsSslEnabled { get; set; }
        public string SMTP_Address { get; set; }
        public string SMTP_DisplayName { get; set; }
        public string SMTP_Password { get; set; }
        public bool EnableSMS { get; set; }
        public string SMS_GatewayURL { get; set; }
        public string SMS_Password { get; set; }
        public string SMS_Username { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
    }
}
