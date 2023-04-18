using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Domain.Models
{
   public class SensorAlertHisotry
    {
        public long Id { get; set; }
        public long CustomerAlertId { get; set; }
        public long SensorId { get; set; }
        public DateTime LastAlertDate { get; set; }
        
    }
}
