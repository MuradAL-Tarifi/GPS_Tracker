namespace GPS.Integration.WaslModels
{
    public class WaslResult
    {
        public bool IsValid { get; set; }
        
        public bool IsDuplicate { get; set; }

        public string RejectionReason { get; set; }
        
        public string RejectionReasonMessageAr { get; set; }
        
        public string RejectionReasonMessageEn { get; set; }

        public string ReferenceKey { get; set; }

        public WaslVehicleInfo VehicleInfo { get; set; }

        public WaslDriverInfo DriverInfo { get; set; }
    }
}
