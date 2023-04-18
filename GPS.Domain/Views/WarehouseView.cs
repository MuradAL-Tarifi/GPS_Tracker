using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Domain.Views
{
    public class WarehouseView
    {
        public long Id { get; set; }
        public long FleetId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string LandCoordinates { get; set; }
        public double LandAreaInSquareMeter { get; set; }
        public string LicenseNumber { get; set; }
        public string LicenseIssueDate { get; set; }
        public string LicenseExpiryDate { get; set; }
        public string ManagerMobile { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public string ReferenceKey { get; set; }
        public string WaslActivityType { get; set; } = "SFDA";
        public bool IsLinkedWithWasl { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }

        public FleetView Fleet { get; set; }
        public List<InventoryView> Inventories { get; set; }
    }
}
