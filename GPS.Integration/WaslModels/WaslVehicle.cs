using System.Collections.Generic;

namespace GPS.Integration.WaslModels
{
    public class WaslVehicle
    {
        public string ReferenceKey { get; set; }

        public WaslPlate Plate { get; set; }

        public WaslVehicleLocationInformation VehicleLocationInformation { get; set; }

        public List<WaslOperatingCompanies> OperatingCompanies { get; set; }
    }
}
