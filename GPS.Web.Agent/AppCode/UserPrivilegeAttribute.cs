using GPS.Domain.DTO;
using GPS.Web.Agent.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GPS.Web.Agent.AppCode
{
    public class UserPrivilegeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public AgentPrivilegeTypeEnum Privilege { get; set; }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var loggedInUser = (LoggedInUserProfile)context.HttpContext.RequestServices.GetService(typeof(LoggedInUserProfile));

            if (loggedInUser.UserPrivilegesTypeIds.Any(x => x == (int)Privilege))
            {
                return; // User Authorized
            }

            context.Result = new RedirectResult("~/");
            return;
        }
    }
}
