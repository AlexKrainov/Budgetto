using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyProfile.Models;

namespace MyProfile.Controllers
{
    public class TypographyController : Controller
    {
        public IActionResult Typography()
        {
            return View();
        }
    }
}
