using Email.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyProfile.Budget.Service;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView.Login;
using MyProfile.Entity.ModelView.User;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
using MyProfile.User.Service;
using MyProfile.UserLog.Service;
using System;
using System.IO;
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
        private SummaryService summaryService;

        public AccountController(IBaseRepository repository,
            UserLogService userLogService,
            UserService userService,
            UserEmailService userConfirmEmailService,
            CollectionUserService collectionUserService,
            SummaryService summaryService)
        {
            this.repository = repository;
            this.userLogService = userLogService;
            this.userService = userService;
            this.userEmailService = userConfirmEmailService;
            this.collectionUserService = collectionUserService;
            this.summaryService = summaryService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ReturnUrl"></param>
        /// <param name="id">UserSessionID</param>
        /// <param name="mid">MailLog.ID</param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string ReturnUrl, Guid? id, string email, Guid? mid, bool isRecoveryPassword = false)
        {
            IActionResult returnAction = null;
            Guid userID = Guid.Empty;

            if (await userLogService.CreateAndCheckIP())
            {
                DateTime nowMinus7Day = DateTime.Now.AddDays(-7);

                //AutoAuthorization by link from mail
                if (mid != null && mid != Guid.Empty)
                {
                    userID = await userEmailService.AuthorizationByEmail(mailID: mid ?? Guid.Empty, userSessionID: id ?? Guid.Empty, email);

                    if (userID != Guid.Empty)
                    {
                        returnAction = await AutoAuthorization(ReturnUrl, userID, userSessionID: id ?? Guid.Empty);

                        if (returnAction != null && isRecoveryPassword == false)
                        {
                            return returnAction;
                        }
                    }
                }
                else if (Request.Cookies.ContainsKey(UserInfo.USER_SESSION_ID)//AutoAuthorization by UserSessionID and UserID
                    && Request.Cookies.ContainsKey(UserInfo.USER_ID)
                    && Guid.TryParse(Request.Cookies[UserInfo.USER_SESSION_ID].ToString(), out Guid usID)
                    && Guid.TryParse(Request.Cookies[UserInfo.USER_ID].ToString(), out Guid uID)
                    && await repository.AnyAsync<UserSession>(x => x.ID == usID && x.UserID == uID && x.LogOutDate == null && x.EnterDate >= nowMinus7Day))
                {
                    returnAction = await AutoAuthorization(ReturnUrl, userID: uID, userSessionID: usID);

                    if (returnAction != null)
                    {
                        return returnAction;
                    }
                }

                Response.Cookies.Delete(UserInfo.USER_ID);
                Response.Cookies.Delete(UserInfo.USER_SESSION_ID);

                if (isRecoveryPassword == false && (id == null || id == Guid.Empty))
                {
                    Guid userSessionID = await userLogService.CreateSession();

                    return View(new LoginModelView { UserSessionID = userSessionID, Email = email });
                }
                else
                {
                    if (isRecoveryPassword)
                    {
                        return View(new LoginModelView
                        {
                            UserSessionID = id ?? Guid.Empty,
                            Email = email,
                            loginView = new LoginViewModel
                            {
                                ID = 4, //onSetNewPassword
                                MailLogID = mid ?? Guid.Empty,
                                UserID = userID
                            }
                        });
                    }
                    else
                    {
                        return View(new LoginModelView
                        {
                            UserSessionID = id ?? Guid.Empty,
                            Email = email
                        });
                    }
                }
            }
            else
            {
                return NotFound();
            }
        }

        private async Task<IActionResult> AutoAuthorization(string ReturnUrl, Guid userID, Guid userSessionID)
        {
            var user = await userService.CheckAndGetUser(email: null, userID: userID);

            if (user != null)
            {
                if (await userLogService.IsBedLimitEnter() == false)
                {
                    user.UserSessionID = userSessionID;
                    user = await userService.AuthenticateOrUpdateUserInfo(user, UserLogActionType.AutoAuthorization);
                    await userLogService.UpdateSession_UserID(user.UserSessionID, user.ID);

                    Response.Cookies.Append(UserInfo.USER_SESSION_ID, userSessionID.ToString());
                    Response.Cookies.Append(UserInfo.USER_ID, user.ID.ToString());

                    if (user.UserSettings.IsShowConstructor)
                    {
                        return RedirectToAction("Index", "Start");
                    }
                    else if (Url.IsLocalUrl(ReturnUrl))
                    {
                        return Redirect(ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Month", "Budget");
                    }
                }
            }
            return null;
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Stat([FromBody] UserStatViewModel personData)
        {
            Guid userSessionID = await userLogService.UpdateSession(personData);
            Response.Cookies.Append(UserInfo.USER_SESSION_ID, userSessionID.ToString());

            return Json(new { isOk = true, userSessionID });
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginModel login)
        {
            var user = await userService.CheckAndGetUser(login.Email.Trim(), login.Password.Trim());

            //check limit enter 20 times
            if (user != null && await userLogService.IsBedLimitEnter())
            {
                await userLogService.CreateUserLogAsync(login.UserSessionID, UserLogActionType.LimitLogin);

                var emailID = await userEmailService.LoginConfirmation(user, login.UserSessionID, MailTypeEnum.LoginLimitEnter);

                if (emailID != Guid.Empty)
                {
                    return Json(new { isOk = true, emailID });
                }
            }

            if (user != null)
            {
                user.UserSessionID = login.UserSessionID;
                user = await userService.AuthenticateOrUpdateUserInfo(user, UserLogActionType.Login);
                await userLogService.UpdateSession_UserID(user.UserSessionID, user.ID);

                Response.Cookies.Append(UserInfo.USER_ID, user.ID.ToString());
                Response.Cookies.Append(UserInfo.USER_SESSION_ID, user.UserSessionID.ToString());

                if (user.UserSettings.IsShowConstructor)
                {
                    return Json(new { isOk = true, href = "/Start/Index" });
                }
                else if (Url.IsLocalUrl(login.ReturnUrl))
                {
                    return Json(new { isOk = true, href = login.ReturnUrl });
                }
                else
                {
                    return Json(new { isOk = true, href = "/Budget/Month" });
                }
            }

            await userLogService.CreateUserLogAsync(login.UserSessionID, UserLogActionType.TryLogin);

            return Json(new { isOk = false, textError = "Неверная почта и(или) пароль." });
        }


        public async Task<IActionResult> Logout()
        {
            var currentUser = UserInfo.Current;
            await userLogService.UserSessionLogOut(currentUser.UserSessionID, currentUser.ID);
            await userLogService.CreateUserLogAsync(currentUser.UserSessionID, UserLogActionType.Logout);

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");//, new { userSessionID = currentUser.UserSessionID });
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Registration([FromBody] LoginModel registrationModel)
        {
            if (!string.IsNullOrEmpty(registrationModel.Email) && !string.IsNullOrEmpty(registrationModel.Password))
            {
                if (await repository.AnyAsync<Entity.Model.User>(x => x.Email == registrationModel.Email.Trim() && x.IsDeleted == false))
                {
                    await userLogService.CreateUserLogAsync(registrationModel.UserSessionID, UserLogActionType.TryRegistration, $"Login: {registrationModel.Email.Trim()}, Password:{registrationModel.Password}, Comment= we already have user with this email");
                    return Json(new { isOk = false, message = $"В системе уже есть пользователь с такой почтой ({ registrationModel.Email.Trim() })." });
                }

                try
                {
                    await userService.CreateUser(registrationModel.Email.Trim(), registrationModel.Password.Trim(), registrationModel.UserSessionID);

                    var user = await userService.CheckAndGetUser(registrationModel.Email.Trim(), registrationModel.Password);
                    user.UserSessionID = registrationModel.UserSessionID;

                    //Maybe send another pool
                    try
                    {
                        await userLogService.CreateUserLogAsync(registrationModel.UserSessionID, UserLogActionType.RegistrationSendEmail, $"Email = {user.Email}");

                        var emailID = await userEmailService.ConfirmEmail(user, user.UserSessionID, MailTypeEnum.Registration);
                        if (emailID != Guid.Empty)
                        {
                            return Json(new { isOk = true, isShowCode = true, emailID });
                        }
                    }
                    catch (Exception ex)
                    {
                        await userLogService.CreateUserLogAsync(registrationModel.UserSessionID, UserLogActionType.RegistrationSendEmail, $"Email = {user.Email}, Error = {ex.Message}");
                        await userLogService.CreateErrorLogAsync(user.UserSessionID, where: "AccountController.Registration_1", ex);
                    }

                    await userLogService.UpdateSession_UserID(user.UserSessionID, user.ID);
                    user = await userService.AuthenticateOrUpdateUserInfo(user, UserLogActionType.Registration);
                    return Json(new { isOk = true, isShowCode = false, href = "/Start/Index" });
                }
                catch (Exception ex)
                {
                    await userLogService.CreateErrorLogAsync(registrationModel.UserSessionID, where: "AccountController.Registration_2", ex);
                    return Json(new { isOk = false, message = $"Во время создания пользователя произошла ошибка." });
                }
            }
            return Json(new { isOk = false, message = $"Все поля обязательны для заполнения." });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> RecoveryPassword([FromBody] LoginModel login)
        {
            var user = await userService.CheckAndGetUser(login.Email.Trim());

            if (user == null)
            {
                await userLogService.CreateUserLogAsync(login.UserSessionID, UserLogActionType.RecoveryPassword_Step1, $"The user did not find (email = {login.Email})");

                return Json(new { isOk = false, message = $"Не удалось найти пользователя с такой почтой. Пожалуйста, зарегистрируйтесь" });
            }

            var emailID = await userEmailService.RecoveryPassword(user, login.UserSessionID);


            if (emailID != Guid.Empty)
            {
                await userLogService.CreateUserLogAsync(login.UserSessionID, UserLogActionType.RecoveryPassword_Step1);

                return Json(new { isOk = true, emailID });
            }
            await userLogService.CreateUserLogAsync(login.UserSessionID, UserLogActionType.RecoveryPassword_Step1, $"Problem when send recovery password (email = {login.Email})");

            return Json(new { isOk = false, message = "Извините, произошла ошибка во время восстановления пароля. Пожалуйста, попробуйте позже." });
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Resend([FromBody] ResendCodeModel model)
        {
            if (model.EmailID != Guid.Empty)
            {
                Guid emailID = Guid.Empty;
                var userID = await userEmailService.CancelLastEmail(model.EmailID, model.UserSessionID);
                var user = await userService.CheckAndGetUser(null, null, userID);

                if (model.LastActionID == 0)//Login
                {
                    emailID = await userEmailService.LoginConfirmation(user, model.UserSessionID, MailTypeEnum.ResendByUser);
                }
                else if (model.LastActionID == 1)//Registration
                {
                    emailID = await userEmailService.ConfirmEmail(user, model.UserSessionID, MailTypeEnum.ResendByUser);
                }
                else if (model.LastActionID == 2) //RecoveryPassword
                {
                    emailID = await userEmailService.RecoveryPassword(user, model.UserSessionID, true);
                }

                await userLogService.CreateUserLogAsync(model.UserSessionID, UserLogActionType.ResendEmail);

                return Json(new { isOk = true, emailID, message = "Сообщение отправлено повторно" });
            }

            try
            {
                await userLogService.UpdateSession_UserID(model.UserSessionID, UserInfo.Current.ID);
            }
            catch (Exception ex)
            {
            }

            string href = "/Budget/Month";
            if (UserInfo.Current.UserSettings.IsShowConstructor)
            {
                href = "/Start/Index";
            }
            return Json(new { isOk = false, message = "Не удалось отправить сообщение повторно. Попробуйте позже.", href });
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CheckCode([FromBody] CheckCodeModel checkCodeModel)
        {
            Guid userID;
            try
            {
                userID = await userEmailService.CheckCode(checkCodeModel.EmailID, checkCodeModel.Code, checkCodeModel.UserSessionID);

                if (userID == Guid.Empty)
                {
                    await userLogService.CreateUserLogAsync(checkCodeModel.UserSessionID, UserLogActionType.CheckCode);
                    return Json(new { isOk = false, message = "Неверный код. Попробуйте еще раз." });
                }
            }
            catch (Exception ex)
            {
                await userLogService.CreateUserLogAsync(checkCodeModel.UserSessionID, UserLogActionType.CheckCode, $"Error: {ex.Message}");
                return Json(new { isOk = false, message = "Неверный код. Попробуйте еще раз." });
            }

            if (checkCodeModel.LastActionID == 2)//recoveryPassword
            {
                await userLogService.CreateUserLogAsync(checkCodeModel.UserSessionID, UserLogActionType.CheckCode, $"The user can recovery password");

                return Json(new { isOk = true, canChangePassword = true, userID });
            }

            var user = await userService.CheckAndGetUser(null, null, userID);
            user.IsConfirmEmail = await userService.SetConfirmEmail(userID, true);
            user.UserSessionID = checkCodeModel.UserSessionID;

            await userService.AuthenticateOrUpdateUserInfo(user, UserLogActionType.LoginAfterCode);
            await userLogService.UpdateSession_UserID(user.UserSessionID, user.ID);

            string href = "/Budget/Month";
            if (user.UserSettings.IsShowConstructor)
            {
                href = "/Start/Index";
            }

            return Json(new { isOk = true, href });
        }


        [HttpGet]
        public async Task<IActionResult> CheckCodeAfterChangeEmail(int Code)
        {
            var currentUser = UserInfo.Current;

            try
            {
                if (await userEmailService.CheckCode(currentUser.Email, Code, currentUser.UserSessionID))
                {
                    currentUser.IsConfirmEmail = await userService.SetConfirmEmail(currentUser.ID, true);
                    await userService.AuthenticateOrUpdateUserInfo(currentUser, UserLogActionType.LoginAfterCode);
                }
                else
                {
                    await userLogService.CreateUserLogAsync(currentUser.UserSessionID, UserLogActionType.CheckCodeAfterChangeEmail);
                    return Json(new { isOk = false, message = "Неверный код. Попробуйте еще раз." });
                }
            }
            catch (Exception ex)
            {
                await userLogService.CreateUserLogAsync(currentUser.UserSessionID, UserLogActionType.CheckCodeAfterChangeEmail, $"Error: {ex.Message}");
                return Json(new { isOk = false, message = "Неверный код. Попробуйте еще раз." });
            }

            return Json(new { isOk = true });
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
                    user.UserSessionID = model.UserSessionID;

                    await userService.AuthenticateOrUpdateUserInfo(user, UserLogActionType.LoginAfterResetPassword);
                    await userLogService.UpdateSession_UserID(user.UserSessionID, user.ID);

                    string href = "/Budget/Month";
                    if (user.UserSettings.IsShowConstructor)
                    {
                        href = "/Start/Index";
                    }

                    return Json(new { isOk = true, href });
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                    await userLogService.CreateErrorLogAsync(model.UserSessionID, where: "AccountController.RecoveryPassword2", ex);
                }
            }

            await userLogService.CreateUserLogAsync(model.UserSessionID, UserLogActionType.LoginAfterResetPassword, $"Error = {errorMessage}");

            return Json(new { isOk = false, message = "Не удалось обновить пароль. Ошибка сервера.", errorMessage });
        }

    }
    public class LoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string ReturnUrl { get; set; }
        public Guid UserSessionID { get; set; }
    }


    public class CheckCodeModel
    {
        public Guid EmailID { get; set; }
        public int LastActionID { get; set; }
        public int Code { get; set; }
        public Guid UserSessionID { get; set; }
    }
    public class ResetPasswordModel
    {
        public Guid ID { get; set; }
        public string NewPassword { get; set; }
        public Guid UserSessionID { get; set; }
    }
    public class ResendCodeModel
    {
        public Guid EmailID { get; set; }
        public int LastActionID { get; set; }
        public Guid UserSessionID { get; set; }
    }
}
