using System;
using System.Collections.Generic;
using System.Text;

namespace GPS.Domain.Models
{
    public class DayOfWeekLookup
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string NameEn { get; set; }
        public int RowOrder { get; set; }
    }
}
