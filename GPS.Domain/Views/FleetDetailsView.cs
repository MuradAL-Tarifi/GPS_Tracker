using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Domain.Views
{
    public class FleetDetailsView
    {
        public long Id { get; set; }
        public long FleetId { get; set; }
        [MaxLength(10)]
        public string IdentityNumber { get; set; }
        [RegularExpression(@"^[1]\d{3}[-](0[1-9]|[12]\d|3[01])[-](0[1-9]|1[0-2])$", ErrorMessage = "Date is not valid")]
        [MaxLength(10)]
        public string DateOfBirthHijri { get; set; }
        public string CommercialRecordNumber { get; set; }
        [RegularExpression(@"^[1]\d{3}[-](0[1-9]|[12]\d|3[01])[-](0[1-9]|1[0-2])$", ErrorMessage = "Date is not valid")]
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
        public bool IsLinkedWithWasl { get; set; }
        public string ActivityType { get; set; }
        public string SFDACompanyActivities { get; set; }
    }
}
