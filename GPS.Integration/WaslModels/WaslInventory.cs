using System;
using System.Collections.Generic;
using System.Text;

namespace GPS.Integration.WaslModels
{
    public class WaslInventory
    {
        /// <summary>
        /// Inventory storing category
        /// </summary>
        public string StoringCategory { get; set; }

        /// <summary>
        /// Inventory name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A unique inventory number
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// The reference number that given by Wasl after the registration of the inventory
        /// </summary>
        public string ReferenceNumber { get; set; }
    }
}
