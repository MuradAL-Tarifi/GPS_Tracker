using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Domain.ViewModels
{
    public class GenericSearchModel
    {
        public string UserId { get; set; }
        public long? FleetId { get; set; } = null;
        public long? ReportTypeLookupId { get; set; } = null;
        public string SearchString { get; set; } = "";
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 100;
    }
}
