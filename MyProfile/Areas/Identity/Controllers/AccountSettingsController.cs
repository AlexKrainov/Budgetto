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
	public partial class AccountController : Controller
	{
		[HttpGet]
		[AllowAnonymous]
		public IActionResult AccountSettings()
		{
			return View();
		}

	}

}
