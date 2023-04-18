using GPS.Domain.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Domain.Views
{
    public class ReportScheduleHistoryView
    {
        public long Id { get; set; }
        public long ReportScheduleId { get; set; }
        public ScheduleTypeEnum ScheduleTypeId { get; set; }
        public DateTime DueDateTime { get; set; }
    }
}
