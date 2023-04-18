using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Domain.Views
{
    public class SensorAlertTypeLookupView
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
