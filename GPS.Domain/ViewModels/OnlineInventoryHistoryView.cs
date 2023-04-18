using System;
using System.Collections.Generic;
using System.Text;

namespace GPS.Domain.ViewModels
{
   public class OnlineInventoryHistoryDto
    {
       public long InveroryId { get; set; }
        public string Name { get; set; }
        public string ReferenceKey { get; set; }
        public long Serial { get; set; }
        public decimal? Temperature { get; set; }
        public decimal? Humidity { get; set; }


    }
}
