using System;
using System.Collections.Generic;
using System.Text;

namespace GPS.Domain.Models
{
    public class DeviceType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
    }
}
