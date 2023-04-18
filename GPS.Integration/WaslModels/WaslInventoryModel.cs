using System;
using System.Collections.Generic;
using System.Text;

namespace GPS.Integration.WaslModels
{
    public class WaslInventoryModel
    {
        /// <summary>
        /// Mandatory 
        /// Value should be always “SFDA”
        /// </summary>
        public string Activity { get; set; } = "SFDA";

        /// <summary>
        /// Mandatory 
        /// Inventory name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Mandatory 
        /// Inventory number should be unique for each inventory with the same operating company
        /// </summary>
        public string InventoryNumber { get; set; }

        /// <summary>
        /// Mandatory 
        /// Refer to appendix for table of all storing category codes
        /// </summary>
        public string StoringCategory { get; set; }
    }
}
