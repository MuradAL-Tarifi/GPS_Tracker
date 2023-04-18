using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Models
{
    public class AppSettings
    {
        public Security Security { get; set; }

        public string RedisConfiguration { get; set; }
    }

    public class Security
    {
        public bool Enable { get; set; }
        public string ApiKey { get; set; }
    }
}
