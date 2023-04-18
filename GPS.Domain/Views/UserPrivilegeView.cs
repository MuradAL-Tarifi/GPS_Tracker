using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Domain.Views
{
    public class UserPrivilegeView
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public int PrivilegeTypeId { get; set; }
        public bool IsActive { get; set; }
        public PrivilegeTypeView PrivilegeType { get; set; }
    }
}
