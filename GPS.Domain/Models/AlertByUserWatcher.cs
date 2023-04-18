using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Domain.Models
{
   public class AlertByUserWatcher
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public long AlertId { get; set; }
        public DateTime ViewDate { get; set; }
    }
}
