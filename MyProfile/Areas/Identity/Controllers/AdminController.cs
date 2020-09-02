using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyProfile.Entity.Model;
using MyProfile.Identity;

namespace MyProfile.Areas.Identity.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Identity")]
    public class AdminController : Controller
    {
        public AdminController()
        {
            if (UserInfo.Current.UserTypeID != (int)UserTypeEnum.Admin)
            {
                throw new Exception("The page close for this type of user");
            }
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
