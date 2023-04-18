using System;
using System.Collections.Generic;
using System.Text;

namespace GPS.Integration.WaslModels
{
    public class WaslWarehouseModel
    {
        /// <summary>
        /// Mandatory 
        /// Value should be always “SFDA”
        /// </summary>
        public string Activity { get; set; } = "SFDA";

        /// <summary>
        /// Mandatory 
        /// Warehouse name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Mandatory 
        /// City of Warehouse
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Mandatory
        /// Detailed Address of Warehouse on the Map
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Mandatory 
        /// For the warehouse location on the map
        /// </summary>
        public decimal? Latitude { get; set; }

        /// <summary>
        /// Mandatory 
        /// For the warehouse location on the map
        /// </summary>
        public decimal? Longitude { get; set; }

        /// <summary>
        /// Mandatory
        /// [{x: 33.0, y:22.0}, …] x is longitude and y is latitude
        /// </summary>
        public List<LandCoordinates> LandCoordinates { get; set; }

        /// <summary>
        /// Mandatory
        /// Warehouse license number it should be unique for each warehouse
        /// </summary>
        public string LicenseNumber { get; set; }

        /// <summary>
        /// Mandatory
        /// Warehouse license issue date in ISO8601 format(yyyy-MM-dd)
        /// </summary>
        public string LicenseIssueDate { get; set; }

        /// <summary>
        /// Mandatory
        /// Warehouse license expiry date in ISO8601 format(yyyy-MM-dd)
        /// </summary>
        public string LicenseExpiryDate { get; set; }

        /// <summary>
        /// Mandatory 
        /// Starts with +966
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Optional 
        /// Starts with +966
        /// </summary>
        public string ManagerMobile { get; set; }

        /// <summary>
        /// Optional 
        /// Email for the contact person
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Optional 
        /// Land area in square meter
        /// </summary>
        public string LandAreaInSquareMeter { get; set; }
    }
}
