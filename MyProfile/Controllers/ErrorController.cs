using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MyProfile.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult StatusCode()
        {
            if (Response.StatusCode == (int)HttpStatusCode.NotFound)
            {
                return RedirectToAction("Error_404");
            }
            else
            {
                return RedirectToAction("Error_500");
            }

            return View();
        }
        public IActionResult Error_404()
        {
            return View();
        }

        public IActionResult Error_500()
        {
            return View();
        }
    }
}
