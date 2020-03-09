using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using MyProfile.Models;

namespace MyProfile.Controllers
{
    public class DashboardsController : Controller
    {
        private readonly ILogger<DashboardsController> _logger;

        public DashboardsController(ILogger<DashboardsController> logger)
        {
            _logger = logger;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Dashboard2()
        {
            return View();
        }

        public IActionResult Dashboard3()
        {
            return View();
        }

        public IActionResult Dashboard4()
        {
            return View();
        }

        public IActionResult Dashboard5()
        {
            return View();
        }

        public IActionResult Dashboard1()
        {
            return View();
        }
    }
}
