using GPS.Domain.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Domain.ViewModels
{
    public class UserPrivilegesViewModel
    {
        public List<UserPrivilegeView> Privileges { get; set; }
        public UserView User { get; set; }
    }
}
