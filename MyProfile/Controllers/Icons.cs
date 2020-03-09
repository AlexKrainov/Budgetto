using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyProfile.Models;

namespace MyProfile.Controllers
{
    public class IconsController : Controller
    {
        public IActionResult FontAwesome()
        {
            return View();
        }

        public IActionResult Ionicons()
        {
            return View();
        }

        public IActionResult Linearicons()
        {
            return View();
        }

        public IActionResult Openiconic()
        {
            return View();
        }

        public IActionResult Stroke7()
        {
            return View();
        }
    }
}
