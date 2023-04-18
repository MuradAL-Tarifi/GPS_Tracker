using Hangfire.Dashboard;

namespace GPS.Job
{
    public class HangFireAuthFilter 
    {
        //public bool Authorize(DashboardContext context)
        //{
        //    var httpContext = context.GetHttpContext();
        //    if (httpContext.User.Identity.IsAuthenticated)
        //    {
        //        if (httpContext.User.Identity.IsAuthenticated && httpContext.User.IsInRole("administrator"))
        //        {
        //            return true;
        //        }
        //    }

        //    httpContext.Response.Redirect("/account/login");
        //    return true;
        //}
    }
}