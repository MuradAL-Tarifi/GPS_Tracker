using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Domain.Views
{
    public class UserView
    {
        public string Id { get; set; }
        [Required]
        [MaxLength(200)]
        public string UserName { get; set; }
        [Required]
        [MaxLength(20)]
        public string Password { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [Required]
        [MaxLength(50)]
        public string NameEn { get; set; }
        [EmailAddress(ErrorMessage = "Email is not valid")]
        [MaxLength(200)]
        public string Email { get; set; }
        public bool IsActive { get; set; } = false;
        [Required]
        public string ExpirationDateText { get; set; }
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
        public AgentView Agent { get; set; }
        public FleetView Fleet { get; set; }
        public RoleView Role { get; set; } = new RoleView();

        public long[] WarehouseIds { get; set; }
        public string[] WarehouseNames { get; set; }

        public string InventoriesIds { get; set; }

    }
}
