using Microsoft.AspNetCore.Mvc;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView;
using MyProfile.Identity;
using System;
using System.Threading.Tasks;

namespace MyProfile.Areas.Identity.Controllers
{
    public partial class AccountController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> AccountSettings()
        {
            await userLogService.CreateUserLogAsync(UserInfo.Current.UserSessionID, UserLogActionType.AccountSetting_Page);

            return View();
        }

        #region User info
        [HttpGet]
        public IActionResult LoadUserSettings()
        {
            return Json(new { isOk = true, user = userService.GetUserSettings() });
        }

        [HttpGet]
        public async Task<IActionResult> ResendConfirmEmail()
        {
            var currentUser = UserInfo.Current;
            return Json(new { isOk = true, userConfirmEmailService = await userEmailService.ConfirmEmail(currentUser, currentUser.UserSessionID, MailTypeEnum.ConfirmEmail, returnUrl: "/Identity/Account/AccountSettings") });
        }

        [HttpPost]
        public async Task<IActionResult> SaveUserInfo([FromBody] UserInfoModel user)
        {
            return Json(new { isOk = true, user = await userService.UpdateUser(user) });
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(Guid id)
        {
            await userEmailService.ConfirmEmail_Complete(id, UserInfo.Current.UserSessionID);

            return RedirectToAction("AccountSettings");
        }

        #endregion

        #region Change password

        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromBody] string newPassword)
        {
            return Json(new { isOk = await userService.UpdatePassword(newPassword, UserInfo.Current.ID) });
        }
        #endregion

        #region Collective budget 

        [HttpPost]
        public async Task<IActionResult> ChangeStatusCollectiveBudget([FromBody] bool isAllowCollectiveBudget)
        {
            if (isAllowCollectiveBudget)
            {
                return Json(new
                {
                    isOk = await collectionUserService.ChangeStatusInCollectiveBudget(CollectiveUserStatusType.Accepted.ToString())
                });
            }
            else
            {
                return Json(new
                {
                    isOk = await collectionUserService.ChangeStatusInCollectiveBudget(CollectiveUserStatusType.Poused.ToString())
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> SearchUser(string email)
        {
            return Json(new { isOk = true, user = await collectionUserService.SearchUserByEmail(email) });
        }

        [HttpGet]
        public async Task<IActionResult> SendOffer(string email)
        {
            return Json(new { isOk = await collectionUserService.SendOffer(email) });
        }

        [HttpGet]
        public async Task<IActionResult> RefreshCollectiveList()
        {
            var collectiveRequests = await collectionUserService.GetCollectiveRequests();
            var collectiveUsers = await collectionUserService.GetCollectiveUsersByCurrentUser();

            return Json(new { isOk = true, collectiveRequests, collectiveUsers });
        }


        [HttpGet]
        public async Task<IActionResult> LeftCollectiveBudgetGroup()
        {
            return Json(new
            {
                isOk = await collectionUserService.ChangeStatusInCollectiveBudget(CollectiveUserStatusType.Gone.ToString())
            });
        }

        [HttpGet]
        public async Task<IActionResult> CheckOffers()
        {
            return Json(new
            {
                isOk = true,
                offers = await collectionUserService.CheckOffers()
            });
        }

        [HttpGet]
        public async Task<IActionResult> OfferAction(int offerID, [FromQuery] bool action)
        {
            return Json(new
            {
                isOk = await collectionUserService.OfferAction(offerID, action)
            });
        }
        #endregion

        #region User settings

        [HttpPost]
        public async Task<IActionResult> SaveUserSettings([FromBody] UserSettingsModelView userSettings)
        {
            return Json(new { isOk = true, user = await userService.UpdateUserSettings(userSettings) });
        }

        #endregion
    }

}
