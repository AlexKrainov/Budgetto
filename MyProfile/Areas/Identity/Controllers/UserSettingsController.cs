using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
using MyProfile.User.Service;
using MyProfile.UserLog.Service;
using Newtonsoft.Json;

namespace MyProfile.Areas.Identity.Controllers
{
    public class UserSettingsController : Controller
    {
        private IBaseRepository repository;
        private UserLogService userLogService;

        public UserSettingsController(IBaseRepository repository,
            UserLogService userLogService)
        {
            this.repository = repository;
            this.userLogService = userLogService;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SaveSettings([FromBody] UserSettingsModelView userSettings)
        {
            var user = UserInfo.Current;
            var dbUserSettings = await repository.GetAll<UserSettings>(x => x.ID == user.ID).FirstOrDefaultAsync();

            if (userSettings.PageName == "Budget/Month")
            {
                user.UserSettings.BudgetPages_WithCollective = dbUserSettings.BudgetPages_WithCollective = userSettings.BudgetPages_WithCollective;

                user.UserSettings.Month_EarningWidget = dbUserSettings.Month_EarningWidget = userSettings.Month_EarningWidget;
                user.UserSettings.Month_SpendingWidget = dbUserSettings.Month_SpendingWidget = userSettings.Month_SpendingWidget;
                user.UserSettings.Month_InvestingWidget = dbUserSettings.Month_InvestingWidget = userSettings.Month_InvestingWidget;

                user.UserSettings.Month_LimitWidgets = dbUserSettings.Month_LimitWidgets = userSettings.Month_LimitWidgets;

                user.UserSettings.Month_GoalWidgets = dbUserSettings.Month_GoalWidgets = userSettings.Month_GoalWidgets;

                user.UserSettings.Month_BigCharts = dbUserSettings.Month_BigCharts = userSettings.Month_BigCharts;

                user.UserSettings.Month_Summary = dbUserSettings.Month_Summary = userSettings.Month_Summary;
                user.UserSettings.Month_Accounts = dbUserSettings.Month_Accounts = userSettings.Month_Accounts;

                user.UserSettings.Month_ToDoLists = dbUserSettings.Month_ToDoLists = userSettings.Month_ToDoLists;

                user.UserSettings.Month_ProgressBar = dbUserSettings.Month_ProgressBar = userSettings.Month_ProgressBar;
            }
            else if (userSettings.PageName == "Budget/Year")
            {
                user.UserSettings.BudgetPages_WithCollective = dbUserSettings.BudgetPages_WithCollective = userSettings.BudgetPages_WithCollective;

                user.UserSettings.Year_EarningWidget = dbUserSettings.Year_EarningWidget = userSettings.Year_EarningWidget;
                user.UserSettings.Year_SpendingWidget = dbUserSettings.Year_SpendingWidget = userSettings.Year_SpendingWidget;
                user.UserSettings.Year_InvestingWidget = dbUserSettings.Year_InvestingWidget = userSettings.Year_InvestingWidget;

                user.UserSettings.Year_LimitWidgets = dbUserSettings.Year_LimitWidgets = userSettings.Year_LimitWidgets;

                user.UserSettings.Year_GoalWidgets = dbUserSettings.Year_GoalWidgets = userSettings.Year_GoalWidgets;

                user.UserSettings.Year_BigCharts = dbUserSettings.Year_BigCharts = userSettings.Year_BigCharts;

                user.UserSettings.Year_Summary = dbUserSettings.Year_Summary = userSettings.Year_Summary;
                user.UserSettings.Year_Accounts = dbUserSettings.Year_Accounts = userSettings.Year_Accounts;

                user.UserSettings.Year_ToDoLists = dbUserSettings.Year_ToDoLists = userSettings.Year_ToDoLists;
            }
            else if (userSettings.PageName == "Goal/List")
            {
                user.UserSettings.GoalPage_IsShow_Collective = dbUserSettings.GoalPage_IsShow_Collective = userSettings.GoalPage_IsShow_Collective;
                user.UserSettings.GoalPage_IsShow_Finished = dbUserSettings.GoalPage_IsShow_Finished = userSettings.GoalPage_IsShow_Finished;
            }
            else if (userSettings.PageName == "Limit/List")
            {
                user.UserSettings.LimitPage_IsShow_Collective = dbUserSettings.LimitPage_IsShow_Collective = userSettings.LimitPage_IsShow_Collective;
                user.UserSettings.LimitPage_Show_IsFinished = dbUserSettings.LimitPage_Show_IsFinished = userSettings.LimitPage_Show_IsFinished;
            }

            await UserInfo.AddOrUpdate_Authenticate(user);

            await repository.UpdateAsync(dbUserSettings, true);

            return Json(new { isOk = true });
        }

        [HttpGet]
        public async Task<IActionResult> NotShowEnterHint()
        {
            var user = UserInfo.Current;
            var dbUserSettings = await repository.GetAll<UserSettings>(x => x.ID == user.ID).FirstOrDefaultAsync();

            user.UserSettings.IsShowFirstEnterHint = dbUserSettings.IsShowFirstEnterHint = false;

            await UserInfo.AddOrUpdate_Authenticate(user);
            await repository.UpdateAsync(dbUserSettings, true);
            await userLogService.CreateUserLogAsync(user.UserSessionID, UserLogActionType.User_EnterHintOff);

            return Json(new { isOk = true });
        }

        [HttpGet]
        public async Task<IActionResult> ShowCookieOff()
        {
            var user = UserInfo.Current;
            var dbUserSettings = await repository.GetAll<UserSettings>(x => x.ID == user.ID).FirstOrDefaultAsync();

            user.UserSettings.IsShowCookie = dbUserSettings.IsShowCookie = false;

            await UserInfo.AddOrUpdate_Authenticate(user);
            await repository.UpdateAsync(dbUserSettings, true);
            await userLogService.CreateUserLogAsync(user.UserSessionID, UserLogActionType.User_CookieOff);

            return Json(new { isOk = true });
        }


        [HttpGet]
        public async Task<IActionResult> ShowDocument(string name)
        {
            var user = UserInfo.Current;
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
                Guid userSessionID = user?.UserSessionID ?? Guid.Parse(Request.Cookies[UserInfo.USER_SESSION_ID].ToString());

                await userLogService.CreateUserLogAsync(userSessionID, userLogActionType);
            }
            catch (Exception ex)
            {
            }

            return Json(new { isOk = true });
        }
    }
}
