using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Domain.Views
{
    public class AlertByUserWatcherView
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public long AlertId { get; set; }
    }
}
