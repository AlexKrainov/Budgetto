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
			var dbUserSettings = await repository.GetAll<UserSettings>(x => x.ID == UserInfo.Current.ID).FirstOrDefaultAsync();
			var user = UserInfo.Current;

			if (userSettings.PageName == "Budget/Month" || userSettings.PageName == "Budget/Year")
			{
				user.UserSettings.BudgetPages_WithCollective = dbUserSettings.BudgetPages_WithCollective = userSettings.BudgetPages_WithCollective;

				user.UserSettings.BudgetPages_EarningChart = dbUserSettings.BudgetPages_EarningChart = userSettings.BudgetPages_EarningChart;
				user.UserSettings.BudgetPages_SpendingChart = dbUserSettings.BudgetPages_SpendingChart = userSettings.BudgetPages_SpendingChart;
				user.UserSettings.BudgetPages_InvestingChart = dbUserSettings.BudgetPages_InvestingChart = userSettings.BudgetPages_InvestingChart;

				user.UserSettings.BudgetPages_IsShow_Limits = dbUserSettings.BudgetPages_IsShow_Limits = userSettings.BudgetPages_IsShow_Limits;

				user.UserSettings.BudgetPages_IsShow_Goals = dbUserSettings.BudgetPages_IsShow_Goals = userSettings.BudgetPages_IsShow_Goals;

				user.UserSettings.BudgetPages_IsShow_BigCharts = dbUserSettings.BudgetPages_IsShow_BigCharts = userSettings.BudgetPages_IsShow_BigCharts;
			}
			else if (userSettings.PageName == "Goal/List")
			{
				user.UserSettings.GoalPage_IsShow_Collective = dbUserSettings.GoalPage_IsShow_Collective = userSettings.GoalPage_IsShow_Collective;
				user.UserSettings.GoalPage_IsShow_Finished = dbUserSettings.GoalPage_IsShow_Finished = userSettings.GoalPage_IsShow_Finished;
			}


			await UserInfo.AddOrUpdate_Authenticate(user);

			await repository.UpdateAsync(dbUserSettings);

			return Json(new { isOk = true });
		}
	}
}
