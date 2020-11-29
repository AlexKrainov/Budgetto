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
        public IActionResult GoToBudgetto(Guid id, string linkName)
        {
            if (linkName == "HeaderButton")
            {
                userLogService.CreateUserLog(id, UserLogActionType.LandingPage_MovedToAppBudgetto_HeaderButton);
            }
            else if (linkName == "FirstViewButton")
            {
                userLogService.CreateUserLog(id, UserLogActionType.LandingPage_MovedToAppBudgetto_FirstViewButton);
            }
            else if (linkName == "SecondViewButton")
            {
                userLogService.CreateUserLog(id, UserLogActionType.LandingPage_MovedToAppBudgetto_SecondViewButton);
            }
            else if (linkName == "SectionViewButton")
            {
                userLogService.CreateUserLog(id, UserLogActionType.LandingPage_MovedToAppBudgetto_SectionViewButton);
            }
            else if (linkName == "RecordsViewButton")
            {
                userLogService.CreateUserLog(id, UserLogActionType.LandingPage_MovedToAppBudgetto_RecordsViewButton);
            }
            else if (linkName == "TableViewButton")
            {
                userLogService.CreateUserLog(id, UserLogActionType.LandingPage_MovedToAppBudgetto_TableViewButton);
            }
            else if (linkName == "LimitViewButton")
            {
                userLogService.CreateUserLog(id, UserLogActionType.LandingPage_MovedToAppBudgetto_LimitViewButton);
            }
            else if (linkName == "GoalsViewButton")
            {
                userLogService.CreateUserLog(id, UserLogActionType.LandingPage_MovedToAppBudgetto_GoalsViewButton);
            }
            else if (linkName == "ChartViewButton")
            {
                userLogService.CreateUserLog(id, UserLogActionType.LandingPage_MovedToAppBudgetto_ChartViewButton);
            }
            else if (linkName == "ReminderViewButton")
            {
                userLogService.CreateUserLog(id, UserLogActionType.LandingPage_MovedToAppBudgetto_ReminderViewButton);
            }
            else if (linkName == "OneForAllViewButton")
            {
                userLogService.CreateUserLog(id, UserLogActionType.LandingPage_MovedToAppBudgetto_OneForAllViewButton);
            }
            else if (linkName == "FreePriceButton")
            {
                userLogService.CreateUserLog(id, UserLogActionType.LandingPage_MovedToAppBudgetto_FreePriceButton);
            }
            else if (linkName == "OneYearPriceButton")
            {
                userLogService.CreateUserLog(id, UserLogActionType.LandingPage_MovedToAppBudgetto_OneYearPriceButton);
            }
            else if (linkName == "ThreeYearsPriceButton")
            {
                userLogService.CreateUserLog(id, UserLogActionType.LandingPage_MovedToAppBudgetto_ThreeYearsPriceButton);
            }
            else
            {
                userLogService.CreateUserLog(id, UserLogActionType.LandingPage_MovedToAppBudgetto);
            }

            return Json(new { isOk = true, id });
        }


    }
}
