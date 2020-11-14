using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
using MyProfile.User.Service;
using System;
using System.Threading.Tasks;

namespace MyProfile.Controllers
{
    public class SettingsController : Controller
    {
        private IBaseRepository repository;
        private UserService userService;
        private UserLogService userLogService;

        public SettingsController(IBaseRepository repository,
            UserService userService,
            UserLogService userLogService)
        {
            this.repository = repository;
            this.userService = userService;
            this.userLogService = userLogService;
        }

        public IActionResult Settings()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> LeaveSite(Guid UserSessionID)
        {
            try
            {
                await userLogService.CreateUserLogAsync(UserSessionID, UserLogActionType.User_LeaveSite);
            }
            catch (Exception ex)
            {
            }

            return Json(new { isOk = true });
        }

        [HttpPost]
        public async Task<IActionResult> CheckAuthenticated([FromBody] CheckUserOnline checkUserOnline)
        {
            if (this.HttpContext.User.Identity.IsAuthenticated == false)
            {
                try
                {
                    if (checkUserOnline != null
                                   && checkUserOnline.Uid != Guid.Empty
                                   && checkUserOnline.Usid != Guid.Empty
                                   && !string.IsNullOrEmpty(checkUserOnline.Ue))
                    {
                        if (await repository.AnyAsync<MyProfile.Entity.Model.User>(x =>
                                x.ID == checkUserOnline.Uid
                                && x.Email == checkUserOnline.Ue))
                        {
                            if (await repository.AnyAsync<MyProfile.Entity.Model.UserSession>(x =>
                                x.ID == checkUserOnline.Usid
                                && x.UserID == checkUserOnline.Uid
                                && x.LogOutDate == null))
                            {
                                var user = await userService.CheckAndGetUser(checkUserOnline.Ue, userID: checkUserOnline.Uid);
                                user.UserSessionID = checkUserOnline.Usid ?? Guid.Empty;
                                user = await userService.AuthenticateOrUpdateUserInfo(user, UserLogActionType.User_AutoAuthorization);
                                return Json(new { isOk = true, IsAuthorized = true });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { isOk = false });
                }
            }

            return Json(new { isOk = true, IsAuthenticated = true });
        }
    }

    public class CheckUserOnline
    {
        /// <summary>
        /// User.ID
        /// </summary>
        public Guid? Uid { get; set; }
        /// <summary>
        /// UserSessionID
        /// </summary>
        public Guid? Usid { get; set; }
        /// <summary>
        /// User.Email
        /// </summary>
        public string Ue { get; set; }
    }
}
