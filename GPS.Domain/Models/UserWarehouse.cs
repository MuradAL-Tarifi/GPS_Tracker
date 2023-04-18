using System;
using System.Collections.Generic;
using System.Text;

namespace GPS.Domain.Models
{
    public class UserWarehouse
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public long WarehouseId { get; set; }
        public Warehouse Warehouse { get; set; }
    }
}