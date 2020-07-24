using Email.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
using MyProfile.User.Service;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MyProfile.Areas.Identity.Controllers
{
    [Authorize]
    [Area("Identity")]
    public partial class AccountController : Controller
    {
        private IBaseRepository repository;
        private UserLogService userLogService;
        private UserService userService;
        private UserEmailService userEmailService;
        private CollectionUserService collectionUserService;

        public AccountController(IBaseRepository repository,
            UserLogService userLogService,
            UserService userService,
            UserEmailService userConfirmEmailService,
            CollectionUserService collectionUserService)
        {
            this.repository = repository;
            this.userLogService = userLogService;
            this.userService = userService;
            this.userEmailService = userConfirmEmailService;
            this.collectionUserService = collectionUserService;
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
            //if (string.IsNullOrEmpty(Email) && string.IsNullOrEmpty(Password))
            //{
            //    Email = "ialexkrainov2@gmail.com";
            //    Password = "BlXlR1234";
            //}
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


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Registration(string Email)
        {
            return View("Registration", Email);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Registration([FromBody] test test)
        {
            if (!string.IsNullOrEmpty(test.Email) && !string.IsNullOrEmpty(test.Password))
            {
                if (await repository.AnyAsync<Entity.Model.User>(x => x.Email == test.Email && x.IsDeleted == false))
                {
                    return Json(new { isOk = false, message = $"В системе уже есть пользователь с такой почтой ({ test.Email })." });
                }

                var now = DateTime.Now;

                try
                {
                    await repository.CreateAsync(new Entity.Model.User
                    {
                        DateCreate = now,
                        Email = test.Email,
                        IsAllowCollectiveBudget = false,
                        Name = test.Email,
                        ImageLink = "/img/user-min.png",
                        Password = test.Password,
                        CollectiveBudgetUser = new Entity.Model.CollectiveBudgetUser
                        {
                            DateAdded = now,
                            DateUpdate = now,
                            Status = Entity.Model.CollectiveUserStatusType.Accepted.ToString(),
                            CollectiveBudget = new Entity.Model.CollectiveBudget
                            {
                                Name = test.Email,
                            }
                        },
                        UserSettings = new Entity.Model.UserSettings
                        {
                            BudgetPages_WithCollective = true,
                            BudgetPages_EarningChart = true,
                            BudgetPages_InvestingChart = true,
                            BudgetPages_SpendingChart = true,
                        }
                    }, true);

                    var user = await userService.AuthenticateOrUpdateUserInfo(test.Email, test.Password, UserActionType.Registration);
                    //Maybe send another pool
                    await userEmailService.ConfirmEmail(user);

                    return Json(new { isOk = true, href = "/Budget/Month" });
                }
                catch (Exception ex)
                {
                    return Json(new { isOk = false, message = $"Во время создания пользователя произошла ошибка." });
                }

            }

            return Json(new { isOk = false, message = $"Все поля обязательны для заполнения." });
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(string Email)
        {
            return View("ResetPassword", new ResetPasswordModel { Email = Email });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(string Email, bool isResetPassword)
        {
            ResetPasswordModel model = new ResetPasswordModel { Email = Email, IsOk = true };

            if (ModelState.IsValid && !string.IsNullOrEmpty(Email) && Email.Contains("@") && Email.Contains("."))
            {
                var user = await repository.GetAll<Entity.Model.User>(x => x.Email == Email && x.IsDeleted == false)
                    .Select(x => new Entity.Model.User
                    {
                        ID = x.ID,
                        Email = x.Email,
                    })
                    .FirstOrDefaultAsync();

                if (user != null)
                {
                    await userEmailService.ResetPassword(user);

                    model.Message = "Письмо с ссылкой на восстановление пароля отправлено на почту.";

                    return View(model);
                }
            }

            model.Message = "Не удалось найти пользователя с такой почтой.";
            model.IsOk = false;

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword2(Guid id)
        {
            return View("ResetPassword2", id);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword2(Guid id,  string newPassword)
        {
            string errorMessage = string.Empty;
            if (id != Guid.Empty
                && !string.IsNullOrEmpty(newPassword)
                && newPassword.Length >= 5)
            {
                try
                {
                    var userID = await userEmailService.ResetPassword_Complete(id);
                    await userService.UpdatePassword(newPassword, userID);

                    var userData = await repository.GetAll<Entity.Model.User>(x => x.ID == userID)
                        .Select(x => new
                        {
                            x.Email
                        })
                        .FirstOrDefaultAsync();

                    await userService.AuthenticateOrUpdateUserInfo(userData.Email, newPassword, UserActionType.ResetPassword);

                    return Json(new { isOk = true, href = "/Budget/Month" });
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                }
            }
            return Json(new { isOk = false, message = "Не удалось обновить пароль. Ошибка сервера.", errorMessage });
        }

    }
    public class test
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class ResetPasswordModel
    {
        public bool? IsOk { get; set; } = null;
        public string Message { get; set; }
        public string Email { get; set; }
    }
}
