using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Server.Models.RD07DataParser
{
    public class RD07Data
    {
        public long IMEI { get; set; }
        public DateTime GpsDate { get; set; }
        public string Alram { get; set; }
        public string GSMStatus { get; set; }
        public int GSMCSQ { get; set; }
        public string HardwareType { get; set; }
        public List<RD07Tag> RD07TagList { get; set; } = new List<RD07Tag>();
    }
}
