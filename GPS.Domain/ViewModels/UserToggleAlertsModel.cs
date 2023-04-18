using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Domain.ViewModels
{
    public class UserToggleAlertsModel
    {
        public string UserId { get; set; }
        public string AppId { get; set; }
        public bool Enabled { get; set; }
    }
}
