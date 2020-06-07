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
			}

			await UserInfo.AddOrUpdate_Authenticate(user);

			await repository.UpdateAsync(dbUserSettings);

			return Json(new { isOk = true });
		}
	}
}
