
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace GPS.Domain.Models
{
    public class Warehouse
    {
        public long Id { get; set; }
        public long FleetId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        [Column(TypeName = "decimal(9,7)")]
        public decimal? Latitude { get; set; }
        [Column(TypeName = "decimal(9,7)")]
        public decimal? Longitude { get; set; }
        public string LandCoordinates { get; set; }
        public double LandAreaInSquareMeter { get; set; }
        public string LicenseNumber { get; set; }
        public string LicenseIssueDate { get; set; }
        public string LicenseExpiryDate { get; set; }
        public string ManagerMobile { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        //public bool IsRegistered { get; set; }
        //public int? RegisterTypeId { get; set; }
        public string ReferenceKey { get; set; }
        public string WaslActivityType { get; set; } = "SFDA";
        public bool IsLinkedWithWasl { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }

        public Fleet Fleet { get; set; }
        //public RegisterType RegisterType { get; set; }
        //public List<Inventory> Inventories { get; set; }
    }
}
