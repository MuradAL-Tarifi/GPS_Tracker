using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Domain.ViewModels
{
   public class FilterInventoryHistory
    {
        public long inventoryId { get; set; }
        public string sensorSerial { get; set; }
        public List<string> lsSensorSerials { get; set; }
        public string fromDate { get; set; }
        public string toDate { get; set; }
        public int pageNumber { get; set; }
        public int pageSize { get; set; }
        public string groupUpdatesByType { get; set; }
        public int? groupUpdatesValue { get; set; }
        public bool isEnglish { get; set; }
    }
}
