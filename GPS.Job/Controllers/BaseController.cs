using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GPS.Job.Controllers
{
    //[Authorize(Roles = "administrator")]
    public class BaseController : Controller
    {
        public BaseController()
        {
        }
    }
}
