using System;
using System.Collections.Generic;
using System.Text;

namespace GPS.Domain.ViewModels
{
    public class InventorySensorModel
    {
        public string Serial { get; set; }
        public string Name { get; set; }

        public List<InventorySensorHistoryModel> HistoryRecords { get; set; }
    }
}
