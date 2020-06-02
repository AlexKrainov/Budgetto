using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MyProfile.Controllers.My
{
	public class SettingsController : Controller
	{
		public IActionResult Settings()
		{
			return View();
		}
	}
}
