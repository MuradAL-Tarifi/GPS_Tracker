using System;
using System.Collections.Generic;

namespace GPS.Domain.Models
{
    public class RegisterType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string NameEn { get; set; }
        public bool IsDeleted { get; set; }
    }
}
