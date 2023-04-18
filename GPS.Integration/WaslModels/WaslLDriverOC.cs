namespace GPS.Integration.WaslModels
{
    public class WaslLDriverOC
    {
        public string ReferenceKey { get; set; }
        public string OperatingCompanyName { get; set; }
        public bool IsDriverValid { get; set; }
        public string DriverRejectionReason { get; set; }
    }
}
