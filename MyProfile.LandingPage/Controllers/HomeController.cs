using Microsoft.AspNetCore.Mvc;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView.User;
using MyProfile.Identity;
using MyProfile.User.Service;
using System;
using System.Threading.Tasks;

namespace MyProfile.LandingPage.Controllers
{
    public class HomeController : Controller
    {
        private UserLogService userLogService;

        public HomeController(UserLogService userLogService)
        {
            this.userLogService = userLogService;
        }

        public async Task<IActionResult> Index()
        {
            Guid userSessionID = await userLogService.CreateSession(isLandingPage: true);
            userLogService.CreateUserLog(userSessionID, UserLogActionType.LandingPage_Enter);
            Response.Cookies.Append(UserInfo.USER_SESSION_ID, userSessionID.ToString());


            return View(userSessionID);
        }
        public async Task<IActionResult> UpdateUserSession([FromBody] UserStatViewModel personData)
        {
            var userSessionID = await userLogService.UpdateSession(personData);

            Response.Cookies.Append(UserInfo.USER_SESSION_ID, userSessionID.ToString());

            return Json(new { isOk = true, userSessionID });
        }
        public IActionResult GoToBudgetto(Guid id)
        {
            userLogService.CreateUserLog(id, UserLogActionType.LandingPage_MovedToAppBudgetto);
            return Json(new { isOk = true, id });
        }


    }
}
