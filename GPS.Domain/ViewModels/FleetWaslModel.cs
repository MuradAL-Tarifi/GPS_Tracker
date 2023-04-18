using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Domain.ViewModels
{
    public class FleetWaslModel
    {
        public long FleetId { get; set; }

        public string FleetName { get; set; }

        public string FleetNameEn { get; set; }

        public string FleetManagerEmail { get; set; }

        public string FleetManagerMobile { get; set; }

        public string FleetSupervisorEmail { get; set; }

        public string FleetSupervisorMobile { get; set; }

        public long? FleetDetailsId { get; set; }

        [MaxLength(10)]
        public string IdentityNumber { get; set; }

        [RegularExpression(@"^\d{4}-((0\d)|(1[012]))-(([012]\d)|3[01])$", ErrorMessage = "Date is not valid")]
        [MaxLength(10)]
        public string DateOfBirthHijri { get; set; }

        public string CommercialRecordNumber { get; set; }

        [RegularExpression(@"^\d{4}-((0\d)|(1[012]))-(([012]\d)|3[01])$", ErrorMessage = "Date is not valid")]
        [MaxLength(10)]
        public string CommercialRecordIssueDateHijri { get; set; }

        [MaxLength(13)]
        public string PhoneNumber { get; set; }

        [MaxLength(4)]
        public string ExtensionNumber { get; set; }

        [EmailAddress(ErrorMessage = "Email is not valid")]
        [MaxLength(50)]
        public string EmailAddress { get; set; }

        [MaxLength(50)]
        public string ManagerName { get; set; }

        [MaxLength(13)]
        public string ManagerPhoneNumber { get; set; }

        [MaxLength(13)]
        public string ManagerMobileNumber { get; set; }

        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }

        public string ReferanceNumber { get; set; }

        public bool IsLinkedWithWasl { get; set; } = false;
        public string ActivityType { get; set; }
        public string SFDACompanyActivities { get; set; }

        public string FleetType { get; set; }
    }
}
