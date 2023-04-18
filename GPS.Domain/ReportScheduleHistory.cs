using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Domain
{
    public class ReportScheduleHistory
    {
        public long Id { get; set; }
        public long ReportScheduleId { get; set; }
        public int ScheduleTypeId { get; set; }
        public DateTime DueDateTime { get; set; }
    }
}
