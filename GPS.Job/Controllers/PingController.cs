﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GPS.Job.Controllers
{
    [AllowAnonymous]
    public class PingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
