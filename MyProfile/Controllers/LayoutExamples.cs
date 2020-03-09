using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyProfile.Models;

namespace MyProfile.Controllers
{
    public class LayoutExamplesController : Controller
    {
        public IActionResult Blank()
        {
            return View();
        }

        public IActionResult Helpers()
        {
            return View();
        }

        public IActionResult HorizontalSidenav()
        {
            return View();
        }

        public IActionResult Layout1Flex()
        {
            return View();
        }

        public IActionResult Layout1()
        {
            return View();
        }

        public IActionResult Layout2Flex()
        {
            return View();
        }

        public IActionResult Layout2()
        {
            return View();
        }

        public IActionResult Options()
        {
            return View();
        }

        public IActionResult WithoutNavbarFlex()
        {
            return View();
        }

        public IActionResult WithoutNavbar()
        {
            return View();
        }

        public IActionResult WithoutSidenav()
        {
            return View();
        }
    }
}
