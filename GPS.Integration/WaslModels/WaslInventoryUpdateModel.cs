using System;
using System.Collections.Generic;
using System.Text;

namespace GPS.Integration.WaslModels
{
    public class WaslInventoryUpdateModel
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
        /// Refer to appendix for table of all storing category codes
        /// </summary>
        public string StoringCategory { get; set; }
    }
}
