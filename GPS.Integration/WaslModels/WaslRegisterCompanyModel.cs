namespace GPS.Integration.WaslModels
{
    public class WaslRegisterCompanyModel
    {
        public string IdentityNumber { get; set; }
        public string CommercialRecordNumber { get; set; }
        public string CommercialRecordIssueDateHijri { get; set; }
        public string PhoneNumber { get; set; }
        public string ExtensionNumber { get; set; }
        public string EmailAddress { get; set; }
        public string ManagerName { get; set; }
        public string ManagerPhoneNumber { get; set; }
        public string ManagerMobileNumber { get; set; }
        public string Activity { get; set; } = "";
        public string SFDACompanyActivity { get; set; } = "";
    }
}
