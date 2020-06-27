using Microsoft.AspNetCore.Mvc;
using MyProfile.Entity.ModelView;
using System;
using System.Threading.Tasks;

namespace MyProfile.Areas.Identity.Controllers
{
    public partial class AccountController : Controller
    {
        [HttpGet]
        public IActionResult AccountSettings()
        {
            return View();
        }

        #region General settings
        [HttpGet]
        public IActionResult LoadUserSettings()
        {
            return Json(new { isOk = true, user = userService.GetUserSettings() });
        }

        [HttpGet]
        public async Task<IActionResult> ResendConfirmEmail()
        {
            return Json(new { isOk = true, userConfirmEmailService = await userConfirmEmailService.ConfirmEmail(true) });
        }

        [HttpPost]
        public async Task<IActionResult> SaveUserInfo([FromBody] UserInfoModel user)
        {
            return Json(new { isOk = true, user = await userService.SaveUserInfo(user) });
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(Guid id)
        {
            await userConfirmEmailService.ConfirmEmail_Complete(id);

            return RedirectToAction("AccountSettings");
        }

        #endregion

        #region Change password

        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromBody] string newPassword)
        {
            return Json(new { isOk = await userService.UpdatePassword(newPassword) });
        } 
        #endregion
    }

}
