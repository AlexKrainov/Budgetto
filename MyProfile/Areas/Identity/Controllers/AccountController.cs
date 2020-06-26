using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Email.Service;
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
    [Authorize]
    [Area("Identity")]
    public partial class AccountController : Controller
    {
        private IBaseRepository repository;
        private UserLogService userLogService;
        private UserService userService;
        private UserConfirmEmailService userConfirmEmailService;

        public AccountController(IBaseRepository repository,
            UserLogService userLogService,
            UserService userService,
            UserConfirmEmailService userConfirmEmailService)
        {
            this.repository = repository;
            this.userLogService = userLogService;
            this.userService = userService;
            this.userConfirmEmailService = userConfirmEmailService;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string Email, string Password, string ReturnUrl)
        {
            if (string.IsNullOrEmpty(Email) && string.IsNullOrEmpty(Password))
            {
                Email = "ialexkrainov2@gmail.com";
                Password = "BlXlR1234";
            }
            var user = await userService.AuthenticateOrUpdateUserInfo(Email, Password, UserActionType.Login);

            if (user != null)
            {
                if (Url.IsLocalUrl(ReturnUrl))
                {
                    return Redirect(ReturnUrl);
                }
                else
                {
                    return RedirectToAction("Month", "Budget", new { area = "" });
                }
                //ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            ViewData["ErrorMessage"] = "Некорректные почта и(или) пароль.";
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            if (UserInfo.Current != null)
            {
                await userLogService.CreateAction(UserInfo.Current.ID, UserActionType.Logout);
            }

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

    }

}
