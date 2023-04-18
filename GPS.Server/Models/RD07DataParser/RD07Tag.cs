using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Server.Models.RD07DataParser
{
    public class RD07Tag
    {
        public long SN { get; set; }
        public bool IsLowVoltage { get; set; }
        public decimal Humidity { get; set; }
        public decimal Temperature { get; set; }
        public DateTime RTC { get; set; }
    }
}
