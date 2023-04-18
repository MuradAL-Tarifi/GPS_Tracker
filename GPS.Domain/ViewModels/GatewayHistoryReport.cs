using GPS.Domain.Views;
using System;
using System.Collections.Generic;
using System.Text;

namespace GPS.Domain.ViewModels
{
    public class GatewayHistoryReport
    {
        public decimal? MaxTemperature { get; set; }
        
        public decimal? MaxHumidity { get; set; }
        
        public int TotalRecords { get; set; }

        public List<InventoryHistoryView> Records { get; set; }
    }
}
