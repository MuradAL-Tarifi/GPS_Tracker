using Microsoft.AspNetCore.Mvc;

namespace GPS.Web.Admin.Controllers
{
    public class AlertTrakerController : Controller
    {
        public IActionResult SensorAlerts()
        {
            return View();
        }
    }
}
