
using System;
using System.Collections.Generic;
using System.Text;

namespace GPS.Domain.Models
{
    public class Role
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string DisplayNameEn { get; set; }
        public int Order { get; set; }
    }
}
