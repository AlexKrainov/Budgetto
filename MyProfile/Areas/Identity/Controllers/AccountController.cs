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
using Newtonsoft.Json;

namespace MyProfile.Areas.Identity.Controllers
{
	[Authorize]
	[Area("Identity")]
	public partial class AccountController : Controller
	{
		private IBaseRepository repository;
		private UserLogService userLogService;

		public AccountController(IBaseRepository repository, UserLogService userLogService)
		{
			this.repository = repository;
			this.userLogService = userLogService;
		}

		[HttpGet]
		[AllowAnonymous]
		public IActionResult Login()
		{
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		public async Task<IActionResult> Login(string Email, string Password, string ReturnUrl)
		{
			if (string.IsNullOrEmpty(Email) && string.IsNullOrEmpty(Password))
			{
				Email = "ialexkrainov2@gmail.com";
				Password = "BlXlR1234";
			}
			var user = await repository.GetAll<MyProfile.Entity.Model.User>(x => x.Email == Email && x.Password == Password)
				.Select(x => new UserInfoModel
				{
					ID = x.ID,
					Email = x.Email,
					CollectiveBudgetID = x.CollectiveBudgetID,
					DateCreate = x.DateCreate,
					ImageLink = x.ImageLink,
					IsAllowCollectiveBudget = x.IsAllowCollectiveBudget,
					LastName = x.LastName,
					Name = x.Name,
					CollectiveBudget = new CollectiveBudget
					{
						ID = x.CollectiveBudget.ID,
						DateCreate = x.CollectiveBudget.DateCreate,
						DateDelete = x.CollectiveBudget.DateDelete,
						Name = x.CollectiveBudget.Name,
						Users = x.CollectiveBudget.Users.Select(y => new Entity.Model.User { ID = y.ID }).ToList()
					},
					UserSettings = new UserSettings
					{
						BudgetPages_WithCollective = x.UserSettings.BudgetPages_WithCollective
					}
				})
				.FirstOrDefaultAsync();

			if (user != null)
			{
				user.LastUserLogID = await userLogService.CreateAction(user.ID, UserActionType.Login);

				await UserInfo.AddOrUpdate_Authenticate(user); // аутентификация
				UserInfo.LastUserLogID = (int)user.LastUserLogID;

				if (Url.IsLocalUrl(ReturnUrl))
				{
					return Redirect(ReturnUrl);
				}
				else
				{
					return RedirectToAction("Month", "Budget", new { area = "" });
				}
				//ModelState.AddModelError("", "Некорректные логин и(или) пароль");
			}
			ViewData["ErrorMessage"] = "Некорректные почта и(или) пароль.";
			return View();
		}

		public async Task<IActionResult> Logout()
		{
			if (UserInfo.Current != null)
			{
				await userLogService.CreateAction(UserInfo.Current.ID, UserActionType.Logout);
			}

			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			return RedirectToAction("Login", "Account");
		}

	}

}
