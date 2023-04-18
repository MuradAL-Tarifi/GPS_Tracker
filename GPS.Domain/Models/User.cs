
using System;
using System.Collections.Generic;
using System.Text;

namespace GPS.Domain.Models
{
    public class User
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string NameEn { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public bool IsDeleted { get; set; }

        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }

        public int? AgentId { get; set; }
        public long? FleetId { get; set; }

        public string AppId { get; set; }
        public bool EnableMobileAlerts { get; set; }
        public bool IsSuperAdmin { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsSubAdminAgent { get; set; }
        public Agent Agent { get; set; }
        public Fleet Fleet { get; set; }
    }
}
