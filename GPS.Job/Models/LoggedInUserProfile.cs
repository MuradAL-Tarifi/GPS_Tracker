using GPS.Helper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace GPS.Job.Models
{
    public class LoggedInUserProfile
    {
        public readonly IHttpContextAccessor _context;

        public LoggedInUserProfile(IHttpContextAccessor context)
        {
            _context = context;

        }

        public string UserId
        {
            get
            {
                return _context.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            }
        }

        public string UserName
        {
            get
            {
                return _context.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            }
        }

        public string Name
        {
            get
            {
                return _context.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower() == "full_name")?.Value;
            }
        }

        public string UserPrivilegesTypeIdsString
        {
            get
            {
                return _context.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower() == "user_privileges_type_ids")?.Value;
            }
        }

        public List<int> UserPrivilegesTypeIds
        {
            get
            {
                return !string.IsNullOrEmpty(UserPrivilegesTypeIdsString) ? UserPrivilegesTypeIdsString.Split(",").ToList().ConvertAll(x => int.Parse(x)) : new List<int>();
            }
        }

        public int? AgentId
        {
            get
            {
                return GPSHelper.ToNullableInt(_context.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower() == "agent_id")?.Value);
            }
        }

        public long? FleetId
        {
            get
            {
                return GPSHelper.ToNullableLong(_context.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower() == "fleet_id")?.Value);
            }
        }

        public string FleetName
        {
            get
            {
                return IsEnglish ? _context.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower() == "fleet_name_en")?.Value :
                   _context.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower() == "fleet_name")?.Value;
            }
        }

        public long? GroupId
        {
            get
            {
                return GPSHelper.ToNullableLong(_context.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower() == "group_id")?.Value);
            }
        }

        public string GroupName
        {
            get
            {
                return IsEnglish ? _context.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower() == "group_name_en")?.Value :
                  _context.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower() == "group_name")?.Value;
            }
        }

        public long? AccountId
        {
            get
            {
                return GPSHelper.ToNullableLong(_context.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower() == "account_id")?.Value);
            }
        }

        public string AccountName
        {
            get
            {
                return _context.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower() == "account_name")?.Value;
            }
        }

        public long? WarehouseId
        {
            get
            {
                return GPSHelper.ToNullableLong(_context.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower() == "warehouse_id")?.Value);
            }
        }

        public string WarehouseName
        {
            get
            {
                return _context.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower() == "warehouse_name")?.Value;
            }
        }

        private bool IsEnglish
        {
            get
            {
                return Thread.CurrentThread.CurrentCulture.Name.Equals("en-US");
            }
        }
    }
}
