using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MyProfile.Controllers
{
    public class ToDoListController : Controller
    {
        public ToDoListController()
        {

        }
        public IActionResult List()
        {
            return View();
        }
    }
}
