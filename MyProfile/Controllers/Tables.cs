using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyProfile.Models;

namespace MyProfile.Controllers
{
    public class TablesController : Controller
    {
        public IActionResult BootstrapSortable()
        {
            return View();
        }

        public IActionResult BootstrapTable()
        {
            return View();
        }

        public IActionResult Bootstrap()
        {
            return View();
        }

        public IActionResult Datatables()
        {
            return View();
        }
    }
}
