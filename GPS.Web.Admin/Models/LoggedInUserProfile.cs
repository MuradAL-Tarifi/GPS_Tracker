using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GPS.Web.Admin.Models
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
        public string IsAdminString
        {
            get
            {
                return _context.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower() == "is_admin")?.Value;
            }
        }
        public bool IsAdmin
        {
            get
            {
                return !string.IsNullOrEmpty(IsAdminString) ? bool.Parse(IsAdminString) : false;
            }
        }

        public string IsSuperAdminString
        {
            get
            {
                return _context.HttpContext.User.Claims.FirstOrDefault(x => x.Type.ToLower() == "is_super_admin")?.Value;
            }
        }
        public bool IsSuperAdmin
        {
            get
            {
                return !string.IsNullOrEmpty(IsSuperAdminString) ? bool.Parse(IsSuperAdminString) : false;
            }
        }
    }
}
