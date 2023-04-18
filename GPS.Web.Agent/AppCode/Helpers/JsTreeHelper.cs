using GPS.Domain.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GPS.Web.Agent.AppCode.Helpers
{
    public static class JsTreeHelper
    {
        public static List<JsTreeObject> FillJsTree(List<WarehouseView> lsWarehouse, string selectedInventoryIds, bool viewOnly, List<long> PermittedInventoryIds)
        {
            List<long> lsSelectedInventories = !string.IsNullOrEmpty(selectedInventoryIds) ? selectedInventoryIds.Split(",").ToList().ConvertAll(x => Int64.Parse(x)) : new List<long>();
            List<JsTreeObject> lsJsTreeObject = new List<JsTreeObject>();

            foreach (var warehouse in lsWarehouse)
            {
                int numPermittedInventories = 0;
                var jsTreeObject = new JsTreeObject();
                jsTreeObject.text = warehouse.Name;
                jsTreeObject.id = warehouse.Id.ToString() + "_warehouse";
                jsTreeObject.state = new State { selected = false, disabled = viewOnly };
                jsTreeObject.type = "warehouse";
                foreach (var inventory in warehouse.Inventories)
                {
                    jsTreeObject.children.Add(new JsTreeObject
                    {
                        text = inventory.Name,
                        id = inventory.Id.ToString(),
                        state = new State { selected = lsSelectedInventories.Any(x => x == inventory.Id), disabled = viewOnly || !PermittedInventoryIds.Any(x => x == inventory.Id) },
                        type = "inventory"
                    });
                    if (PermittedInventoryIds.Any(x => x == inventory.Id))
                    {
                        numPermittedInventories += 1;
                    }
                }
                if(warehouse.Inventories.Count == numPermittedInventories)
                {
                    jsTreeObject.state = new State {disabled = true };
                }
                lsJsTreeObject.Add(jsTreeObject);
            }
            return lsJsTreeObject;
        }
        public static List<JsTreeObject> FillJsTreeWarehousesInventoriesSensors(List<WarehouseView> lsWarehouse, List<long> PermittedInventoryIds)
        {
            List<JsTreeObject> lsJsTreeObject = new List<JsTreeObject>();

            foreach (var warehouse in lsWarehouse)
            {
                var inventoires = warehouse.Inventories.Where(x => PermittedInventoryIds.Any(pid => pid == x.Id)).ToList();
                if (inventoires.Count == 0)
                    continue;
                var jsTreeObject = new JsTreeObject();
                jsTreeObject.text = warehouse.Name;
                jsTreeObject.id = warehouse.Id.ToString() + "_warehouse";
                jsTreeObject.state = new State { selected = false };
                jsTreeObject.type = "warehouse";
                foreach (var inventory in inventoires)
                {
                    var inventorySensors = inventory.InventorySensors;
                    if (inventorySensors.Count == 0)
                        continue;
                    List<JsTreeObject> lsSensorsJsTree = new List<JsTreeObject>();
                    foreach(var inventorySensor in inventorySensors)
                    {
                        lsSensorsJsTree.Add(new JsTreeObject
                        {
                            text = inventorySensor.SensorView.Serial,
                            id = inventorySensor.SensorView.Serial + "_sensor",
                            type = "sensor"
                        });
                    }
                    jsTreeObject.children.Add(new JsTreeObject
                    {
                        text = inventory.Name,
                        id = inventory.Id.ToString() + "_inventory",
                        children = lsSensorsJsTree,
                        type = "inventory"
                    });
                }
                lsJsTreeObject.Add(jsTreeObject);
            }
            return lsJsTreeObject;
        }
    }
    public class JsTreeObject
    {
        public string type { get; set; }
        public string text { get; set; }
        public string id { get; set; }
        public State state { get; set; }
        public List<JsTreeObject> children { get; set; } = new List<JsTreeObject>();
    }
    public class State
    {
        public bool selected;
        public bool disabled;
        public bool opened;
    }
}
