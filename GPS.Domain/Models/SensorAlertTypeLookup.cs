using System;
using System.Collections.Generic;
using System.Text;

namespace GPS.Domain.Models
{
    public class SensorAlertTypeLookup
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string NameEn { get; set; }
        public int? RowOrder { get; set; }
        public bool IsRange { get; set; }
        public string DataType { get; set; }
        public bool IsDeleted { get; set; }
    }
}
