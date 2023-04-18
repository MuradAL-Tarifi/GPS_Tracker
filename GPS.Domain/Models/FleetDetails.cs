using System;
using System.Collections.Generic;

namespace GPS.Domain.Models
{
    public class FleetDetails
    {
        public long Id { get; set; }
        public long FleetId { get; set; }
        public string IdentityNumber { get; set; }
        public string DateOfBirthHijri { get; set; }
        public string CommercialRecordNumber { get; set; }
        public string CommercialRecordIssueDateHijri { get; set; }
        public string PhoneNumber { get; set; }
        public string ExtensionNumber { get; set; }
        public string EmailAddress { get; set; }
        public string ManagerName { get; set; }
        public string ManagerPhoneNumber { get; set; }
        public string ManagerMobileNumber { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string ReferanceNumber { get; set; }

        //public bool IsRegistered { get; set; }
        //public int? RegisterTypeId { get; set; }
        //public bool RegisterInWasl { get; set; }

        public bool IsLinkedWithWasl { get; set; }

        //public RegisterType RegisterType { get; set; }
        public string ActivityType { get; set; }
        public string SFDACompanyActivities { get; set; }
        public string FleetType { get; set; }
        //public Fleet Fleet { get; set; }
    }
}
