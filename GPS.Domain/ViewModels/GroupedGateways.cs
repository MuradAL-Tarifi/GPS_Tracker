using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Domain.ViewModels
{
    public class GroupedGateways
    {
        public string GatewayBrandName { get; set; }
        public int CountPerBrand { get; set; }
        public double PercentageAmongBrands { get; set; }
    }
}
