using GPS.Domain.DTO;
using GPS.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;

namespace GPS.Shared.AppCode
{
    public class UserPrivilegeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public AdminPrivilegeTypeEnum Privilege { get; set; }

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