using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyProfile.Models;

namespace MyProfile.Controllers
{
    public class CompleteUiController : Controller
    {
        public IActionResult Base()
        {
            return View();
        }

        public IActionResult Charts()
        {
            return View();
        }

        public IActionResult Plugins()
        {
            return View();
        }
    }
}
