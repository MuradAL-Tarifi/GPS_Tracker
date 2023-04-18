using System.Collections.Generic;

namespace GPS.Integration.WaslModels
{
    public class WaslWarehouse
    {
        /// <summary>
        /// Warehouse name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// City of Warehouse
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// For the warehouse location on the map
        /// </summary>
        public decimal Latitude { get; set; }

        /// <summary>
        /// For the warehouse location on the map
        /// </summary>
        public decimal Longitude { get; set; }

        /// <summary>
        /// Warehouse license number it should be unique for each warehouse
        /// </summary>
        public string LicenseNumber { get; set; }

        /// <summary>
        /// Warehouse license issue date in ISO8601 format(yyyy-MM-dd)
        /// </summary>
        public string LicenseIssueDate { get; set; }

        /// <summary>
        /// Warehouse license expiry date in ISO8601 format(yyyy-MM-dd)
        /// </summary>
        public string LicenseExpiryDate { get; set; }

        /// <summary>
        /// The reference number that given by Wasl after the registration of the warehouse
        /// </summary>
        public string ReferenceNumber { get; set; }

        public List<WaslInventory> Inventories { get; set; }
    }
}
