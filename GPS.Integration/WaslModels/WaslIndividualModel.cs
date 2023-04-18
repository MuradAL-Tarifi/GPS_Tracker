namespace GPS.Integration.WaslModels
{
    public class WaslIndividualModel
    {
        public string IdentityNumber { get; set; }
        public string DateOfBirthHijri { get; set; }
        public string PhoneNumber { get; set; }
        public string ExtensionNumber { get; set; }
        public string EmailAddress { get; set; }
        public string Activity { get; set; } = "";
        public string SFDACompanyActivity { get; set; } = "";
    }
}
