using System.Collections.Generic;

namespace GPS.Integration.WaslModels
{
    public class WaslDriver
    {
        public string ReferenceKey { get; set; }
        public string Name { get; set; }
        public string RegistrationTime { get; set; }

        public List<WaslLDriverOC> OperatingCompanies { get; set; }

    }
}
