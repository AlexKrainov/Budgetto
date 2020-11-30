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
            else if (linkName == "EmailButton")
            {
                userLogService.CreateUserLog(id, UserLogActionType.LandingPage_MovedToAppBudgetto_EmailButton);
            }
            else
            {
                userLogService.CreateUserLog(id, UserLogActionType.LandingPage_MovedToAppBudgetto);
            }

            return Json(new { isOk = true, id });
        }

        [HttpGet]
        public async Task<IActionResult> ShowDocument(string name)
        {
            Guid usID = Guid.Empty;
            if (Request.Cookies.ContainsKey(UserInfo.USER_SESSION_ID))
            {
                Guid.TryParse(Request.Cookies[UserInfo.USER_SESSION_ID].ToString(), out usID);
            }

            var userLogActionType = string.Empty;

            if (name != null)
            {
                switch (name)
                {
                    case "cookie_policy":
                        userLogActionType = UserLogActionType.Document_CookiePolicy;
                        break;
                    case "personal_data_processing_policy":
                        userLogActionType = UserLogActionType.Document_PersonalDataProcessingPolicy;
                        break;
                    case "terms_of_use":
                        userLogActionType = UserLogActionType.Document_TermsOfUse;
                        break;
                    default:
                        userLogActionType = "documents_";
                        break;
                }
            }

            try
            {
                await userLogService.CreateUserLogAsync(usID, userLogActionType);
            }
            catch (Exception ex)
            {
            }
            return Json(new { isOk = true });
        }

        [HttpGet]
        public async Task<IActionResult> ShowMore(string name)
        {
            Guid usID = Guid.Empty;
            if (Request.Cookies.ContainsKey(UserInfo.USER_SESSION_ID))
            {
                Guid.TryParse(Request.Cookies[UserInfo.USER_SESSION_ID].ToString(), out usID);
            }

            try
            {
                await userLogService.CreateUserLogAsync(usID, UserLogActionType.LandingPage_ShowMore);
            }
            catch (Exception ex)
            {
            }
            return Json(new { isOk = true });
        }
    }
}
