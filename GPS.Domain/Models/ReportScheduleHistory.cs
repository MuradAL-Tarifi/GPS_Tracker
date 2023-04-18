using System;
using System.Collections.Generic;
using System.Text;

namespace GPS.Domain.Models
{
    public class ReportScheduleHistory
    {
        public long Id { get; set; }
        public long ReportScheduleId { get; set; }
        public int ScheduleTypeId { get; set; }
        public DateTime DueDateTime { get; set; }
    }
}
