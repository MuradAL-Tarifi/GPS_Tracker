using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Domain.Views
{
    public class PrivilegeTypeView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string NameEn { get; set; }
        public bool IsDeleted { get; set; }
        public int? Order { get; set; }
        public string RoleId { get; set; }
        public bool Editable { get; set; }
    }
}
