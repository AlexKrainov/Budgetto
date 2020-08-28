﻿using Email.Service;
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
            var user = await userService.CheckAndGetUser(login.Email, login.Password);

            //check limit enter 20 times
            if (user != null && await userLogService.CheckLimitEnter())
            {
                await userLogService.CreateSession(user.ID, UserLogActionType.LimitLogin);

                var emailID = await userEmailService.LoginConfirmation(user);

                if (emailID != Guid.Empty)
                {
                    return Json(new { isOk = true, emailID });
                }
            }

            if (user != null)
            {
                user = await userService.AuthenticateOrUpdateUserInfo(user, UserLogActionType.Login);

                if (Url.IsLocalUrl(login.ReturnUrl))
                {
                    return Json(new { isOk = true, href = login.ReturnUrl });
                }
                else
                {
                    return Json(new { isOk = true, href = "/Budget/Month" });
                }
            }

            await userLogService.CreateSession(null, UserLogActionType.TryLogin);

            return Json(new { isOk = false, textError = "Неверная почта и(или) пароль." });

        }

        public async Task<IActionResult> Logout()
        {
            if (UserInfo.Current != null)
            {
                await userLogService.CreateUserLog(UserInfo.Current.UserSessionID, UserLogActionType.Logout);
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
                    await userLogService.CreateSession(null, UserLogActionType.TryRegistration, $"Login: {registrationModel.Email}, Password:{registrationModel.Password}, Comment= we already have user with this email");
                    return Json(new { isOk = false, message = $"В системе уже есть пользователь с такой почтой ({ registrationModel.Email })." });
                }

                try
                {
                    await userService.CreateUser(registrationModel.Email, registrationModel.Password);

                    var user = await userService.CheckAndGetUser(registrationModel.Email, registrationModel.Password);

                    //Maybe send another pool
                    try
                    {
                        user.UserSessionID = await userLogService.CreateSession(user.ID, UserLogActionType.RegistrationSendEmail, $"Email = {user.Email}");

                        var emailID = await userEmailService.ConfirmEmail(user);
                        if (emailID != Guid.Empty)
                        {
                            return Json(new { isOk = true, isShowCode = true, emailID });
                        }
                    }
                    catch (Exception ex)
                    {
                        await userLogService.CreateSession(user.ID, UserLogActionType.RegistrationSendEmail, $"Email = {user.Email}, Error = {ex.Message}");
                        await userLogService.CreateLog(userID: user.ID, where: "AccountController.Registration_1", errorText: ex.Message);
                    }

                    user = await userService.AuthenticateOrUpdateUserInfo(user, UserLogActionType.Registration);
                    return Json(new { isOk = true, isShowCode = false, href = "/Budget/Month" });

                }
                catch (Exception ex)
                {
                    await userLogService.CreateLog(where: "AccountController.Registration_2", errorText: ex.Message);
                    return Json(new { isOk = false, message = $"Во время создания пользователя произошла ошибка." });
                }
            }
            return Json(new { isOk = false, message = $"Все поля обязательны для заполнения." });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> RecoveryPassword([FromBody] LoginModel login)
        {
            var user = await userService.CheckAndGetUser(login.Email);

            if (user == null)
            {
                await userLogService.CreateSession(null, UserLogActionType.RecoveryPassword_Step1, $"The user did not find (email = {login.Email})");

                return Json(new { isOk = false, message = $"Не удалось найти пользователя с такой почтой. Пожалуйста зарегистрируйтесь " });
            }

            var emailID = await userEmailService.RecoveryPassword(user);


            if (emailID != Guid.Empty)
            {
                await userLogService.CreateSession(user.ID, UserLogActionType.RecoveryPassword_Step1);

                return Json(new { isOk = true, emailID });
            }
            await userLogService.CreateSession(user.ID, UserLogActionType.RecoveryPassword_Step1, $"Problem when send recovery password (email = {login.Email})");

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
                var user = await userService.CheckAndGetUser(null, null, userID);

                if (model.LastActionID == 0)//Login
                {
                    emailID = await userEmailService.LoginConfirmation(user, true);
                }
                else if (model.LastActionID == 1)//Registration
                {
                    emailID = await userEmailService.ConfirmEmail(user, true);
                }
                else if (model.LastActionID == 2) //RecoveryPassword
                {
                    emailID = await userEmailService.RecoveryPassword(user, true);
                }

                await userLogService.CreateSession(user.ID, UserLogActionType.ResendEmail);

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
                    await userLogService.CreateSession(null, UserLogActionType.CheckCode);
                    return Json(new { isOk = false, message = "Неверный код. Попробуйте еще." });
                }
            }
            catch (Exception ex)
            {
                await userLogService.CreateSession(null, UserLogActionType.CheckCode, $"Error: {ex.Message}");
                return Json(new { isOk = false, message = "Неверный код. Попробуйте еще." });
            }

            if (checkCodeModel.LastActionID == 2)//recoveryPassword
            {
                await userLogService.CreateSession(null, UserLogActionType.CheckCode, $"The user can recovery password");

                return Json(new { isOk = true, canChangePassword = true, userID });
            }

            var user = await userService.CheckAndGetUser(null, null, userID);
            user.IsConfirmEmail = await userService.SetConfirmEmail(userID, true);

            await userService.AuthenticateOrUpdateUserInfo(user, UserLogActionType.LoginAfterCode);

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

                    var user = await userService.CheckAndGetUser(null, null, model.ID);

                    await userService.AuthenticateOrUpdateUserInfo(user, UserLogActionType.LoginAfterResetPassword);

                    return Json(new { isOk = true, href = "/Budget/Month" });
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                    await userLogService.CreateLog(where: "AccountController.RecoveryPassword2", errorText: ex.Message);
                }
            }

            await userLogService.CreateSession(null, UserLogActionType.LoginAfterResetPassword, $"Error = {errorMessage}");

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
