namespace GPS.Integration.WaslModels
{
    public class WaslOperatingCompanies
    {
        public string ReferenceKey { get; set; }
        public string Name { get; set; }
        public bool IsVehicleValid { get; set; }
        public string VehicleRejectionReason { get; set; }
    }
}
