using System;
using System.Collections.Generic;
using System.Text;

namespace GPS.Domain.Models
{
    public class UserPrivilege
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public int PrivilegeTypeId { get; set; }
        public bool IsActive { get; set; }
        public PrivilegeType PrivilegeType { get; set; }
    }
}
