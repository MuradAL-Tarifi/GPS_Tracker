using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Domain.ViewModels
{
    public class AgentDashboardView
    {
        public int ReportsCount { get; set; }
        public int CustomAlertsCount { get; set; }
        public int WarehousesCount { get; set; }
        public int WaslLinkedWarehousesCount { get; set; }
        public int WaslLinkedInventoriesCount { get; set; }
        public int WaslNotLinkedWarehousesCount { get; set; }
        public int WaslNotLinkedInventoriesCount { get; set; }
        public int InventoriesCount { get; set; }
        public int SensorsCount { get; set; }
        public int NotWorkingSensorsCount { get; set; }
        public int WorkingSensorsCount { get; set; }
        public int GatewaysCount { get; set; }
        public List<GroupedSensors> LsGroupedSensors { get; set; } = new List<GroupedSensors>();
        public List<GroupedGateways> LsGroupedGateways { get; set; } = new List<GroupedGateways>();
    }
}
