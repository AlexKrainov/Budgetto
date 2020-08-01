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
        public async Task<IActionResult> Login([FromBody] LoginModel login)
        {
            var user = await userService.CheckUser(login.Email, login.Password);

            if (user != null && await userLogService.CheckLimitEnter(user.ID, UserActionType.Login))
            {
                var emailID = await userEmailService.LoginConfirmation(user);

                if (emailID != Guid.Empty)
                {
                    return Json(new { isOk = true, emailID });
                }
            }

            if (user != null)
            {
                user = await userService.AuthenticateOrUpdateUserInfo(user, UserActionType.Login);

                if (Url.IsLocalUrl(login.ReturnUrl))
                {
                    return Json(new { isOk = true, href = login.ReturnUrl });
                }
                else
                {
                    return Json(new { isOk = true, href = "/Budget/Month" });
                }
            }

            return Json(new { isOk = false, textError = "Некорректные почта и(или) пароль." });

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


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Registration([FromBody] LoginModel registrationModel)
        {
            if (!string.IsNullOrEmpty(registrationModel.Email) && !string.IsNullOrEmpty(registrationModel.Password))
            {
                if (await repository.AnyAsync<Entity.Model.User>(x => x.Email == registrationModel.Email && x.IsDeleted == false))
                {
                    return Json(new { isOk = false, message = $"В системе уже есть пользователь с такой почтой ({ registrationModel.Email })." });
                }

                try
                {
                    await userService.CreateUser(registrationModel.Email, registrationModel.Password);

                    var user = await userService.CheckUser(registrationModel.Email, registrationModel.Password);
                    //Maybe send another pool
                    try
                    {
                        var emailID = await userEmailService.ConfirmEmail(user);
                        if (emailID != Guid.Empty)
                        {
                            return Json(new { isOk = true, isShowCode = true, emailID });
                        }
                    }
                    catch (Exception ex)
                    {
                    }

                    user = await userService.AuthenticateOrUpdateUserInfo(user, UserActionType.Registration);
                    return Json(new { isOk = true, isShowCode = false, href = "/Budget/Month" });

                }
                catch (Exception ex)
                {
                    return Json(new { isOk = false, message = $"Во время создания пользователя произошла ошибка." });
                }
            }
            return Json(new { isOk = false, message = $"Все поля обязательны для заполнения." });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> RecoveryPassword([FromBody] LoginModel login)
        {
            var user = await userService.CheckUser(login.Email);

            if (user == null)
            {
                return Json(new { isOk = false, message = $"Не удалось найти пользователя с такой почтой. Пожалуйста зарегистрируйтесь " });
            }

            var emailID = await userEmailService.RecoveryPassword(user);

            if (emailID != Guid.Empty)
            {
                return Json(new { isOk = true, emailID });
            }

            return Json(new { isOk = false, message = "Извините, произошла ошибка во время восстановления пароля. Пожалуйста попробуйте позже." });
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Resend([FromBody] ResendCodeModel model)
        {
            if (model.EmailID != Guid.Empty)
            {
                Guid emailID = Guid.Empty;
                var userID = await userEmailService.CancelLastEmail(model.EmailID);
                var user = await userService.CheckUser(null, null, userID);

                if (model.LastActionID == 0)//Login
                {
                    emailID = await userEmailService.LoginConfirmation(user, true);
                }
                else if (model.LastActionID == 1)//Registration
                {
                    emailID = await userEmailService.ConfirmEmail(user, true);
                }
                else if (model.LastActionID == 2) //recoveryPassword
                {
                    emailID = await userEmailService.RecoveryPassword(user, true);
                }
                return Json(new { isOk = true, emailID, message = "Сообщение отправлено повторно" });
            }

            return Json(new { isOk = false, message = "Не удалось отправить сообщение повторно. Попробуйте позже.", href = "/Budget/Month" });
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CheckCode([FromBody] CheckCodeModel checkCodeModel)
        {
            Guid userID;
            try
            {
                userID = await userEmailService.CheckCode(checkCodeModel.EmailID, checkCodeModel.Code);

                if (userID == Guid.Empty)
                {
                    return Json(new { isOk = false, message = "Неверный код. Попробуйте еще." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { isOk = false, message = "Неверный код. Попробуйте еще." });
            }

            if (checkCodeModel.LastActionID == 2)//recoveryPassword
            {
                return Json(new { isOk = true, canChangePassword = true, userID });
            }

            var user = await userService.CheckUser(null, null, userID);

            await userService.AuthenticateOrUpdateUserInfo(user, UserActionType.EnterAfterCode);

            return Json(new { isOk = true, href = "/Budget/Month" });
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> RecoveryPassword2([FromBody] ResetPasswordModel model)
        {
            string errorMessage = string.Empty;
            if (model.ID != Guid.Empty
                && !string.IsNullOrEmpty(model.NewPassword)
                && model.NewPassword.Length >= 5)
            {
                try
                {
                    await userService.UpdatePassword(model.NewPassword, model.ID);

                    var user = await userService.CheckUser(null, null, model.ID);

                    await userService.AuthenticateOrUpdateUserInfo(user, UserActionType.ResetPassword);

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
    public class LoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string ReturnUrl { get; set; }
    }


    public class CheckCodeModel
    {
        public Guid EmailID { get; set; }
        public int LastActionID { get; set; }
        public int Code { get; set; }
    }
    public class ResetPasswordModel
    {
        public Guid ID { get; set; }
        public string NewPassword { get; set; }
    }
    public class ResendCodeModel
    {
        public Guid EmailID { get; set; }
        public int LastActionID { get; set; }
    }
}
