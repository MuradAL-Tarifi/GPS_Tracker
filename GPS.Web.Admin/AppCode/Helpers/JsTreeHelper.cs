using GPS.Domain.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GPS.Web.Admin.AppCode.Helpers
{
    public static class JsTreeHelper
    {
        public static List<JsTreeObject> FillJsTree(List<WarehouseView> lsWarehouse, string selectedInventoryIds,bool viewOnly)
        {
            List<long> lsSelectedInventories = !string.IsNullOrEmpty(selectedInventoryIds) ? selectedInventoryIds.Split(",").ToList().ConvertAll(x => Int64.Parse(x)) : new List<long>();
            List<JsTreeObject> lsJsTreeObject = new List<JsTreeObject>();

            foreach(var warehouse in lsWarehouse)
            {
                var jsTreeObject = new JsTreeObject();
                jsTreeObject.text = warehouse.Name;
                jsTreeObject.id = warehouse.Id.ToString() + "_warehouse";
                jsTreeObject.state = new State { selected = false, disabled = viewOnly };
                foreach (var inventory in warehouse.Inventories)
                {
                    jsTreeObject.children.Add(new JsTreeObject
                    {
                        text = inventory.Name,
                        id = inventory.Id.ToString(),
                        state = new State { selected = lsSelectedInventories.Any(x => x == inventory.Id), disabled = viewOnly }
                    });
                }
                lsJsTreeObject.Add(jsTreeObject);
            }
            return lsJsTreeObject;
        }
    }
    public class JsTreeObject
    {
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
