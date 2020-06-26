using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
using MyProfile.User.Service;
using Newtonsoft.Json;

namespace MyProfile.Areas.Identity.Controllers
{
    public partial class AccountController : Controller
    {
        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccountSettings()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> LoadUserSettings()
        {
            return Json(new { isOk = true, user = userService.GetUserSettingsAsync() });
        }

        [HttpGet]
        public async Task<IActionResult> ResendConfirmEmail()
        {
            return Json(new { isOk = true, userConfirmEmailService = await userConfirmEmailService.ConfirmEmail(true) });
        }

        [HttpPost]
        public async Task<IActionResult> SaveUserInfo([FromBody]UserInfoModel user)
        {
            return Json(new { isOk = true, user = await userService.SaveUserInfo(user) });
        }

    }

}
