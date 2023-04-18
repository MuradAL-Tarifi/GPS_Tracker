using GPS.Domain.Views;
using System;
using System.Collections.Generic;
using System.Text;

namespace GPS.Domain.ViewModels
{
    public class GatewayHistoryOnlineReport
    {
        public decimal? MaxTemperature { get; set; }
        
        public decimal? MaxHumidity { get; set; }
        
        public List<OnlineInventoryHistoryView> Records { get; set; }
    }
}
