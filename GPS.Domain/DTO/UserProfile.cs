using GPS.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS.Domain.DTO
{
    public class UserProfile
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string NameEn { get; set; }
        public int? AgentId { get; set; }
        public long? FleetId { get; set; }
        public string FleetName { get; set; }
        public long? GroupId { get; set; }
        public string GroupName { get; set; }
        public long? AccountId { get; set; }
        public string AccountName { get; set; }
        public List<int> PrivilegeTypeIds { get; set; }
        public List<LookupModel> UserWarehouses { get; set; }
        public List<long> UserInventoryIds { get; set; }
    }
}
