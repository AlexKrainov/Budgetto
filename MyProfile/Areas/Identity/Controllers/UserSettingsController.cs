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
using Newtonsoft.Json;

namespace MyProfile.Areas.Identity.Controllers
{
	public class UserSettingsController : Controller
	{
		private IBaseRepository repository;

		public UserSettingsController(IBaseRepository repository)
		{
			this.repository = repository;
		}

		[HttpPost]
		[AllowAnonymous]
		public async Task<IActionResult> SaveSettings([FromBody] UserSettingsModelView userSettings)
		{
			var user = UserInfo.Current;
			var dbUserSettings = await repository.GetAll<UserSettings>(x => x.ID == user.ID).FirstOrDefaultAsync();

			if (userSettings.PageName == "Budget/Month" || userSettings.PageName == "Budget/Year")
			{
				user.UserSettings.BudgetPages_WithCollective = dbUserSettings.BudgetPages_WithCollective = userSettings.BudgetPages_WithCollective;

				user.UserSettings.Month_EarningWidget = dbUserSettings.Month_EarningWidget = userSettings.Month_EarningWidget;
				user.UserSettings.Month_SpendingWidget = dbUserSettings.Month_SpendingWidget = userSettings.Month_SpendingWidget;
				user.UserSettings.Month_InvestingWidget = dbUserSettings.Month_InvestingWidget = userSettings.Month_InvestingWidget;

				user.UserSettings.Month_LimitWidgets = dbUserSettings.Month_LimitWidgets = userSettings.Month_LimitWidgets;

				user.UserSettings.Month_GoalWidgets = dbUserSettings.Month_GoalWidgets = userSettings.Month_GoalWidgets;

				user.UserSettings.Month_BigCharts = dbUserSettings.Month_BigCharts = userSettings.Month_BigCharts;
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
	}
}
