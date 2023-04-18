using System;
using System.Collections.Generic;
using System.Text;

namespace GPS.Domain.Models
{
    public class UserRole
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public string RoleId { get; set; }
    }
}