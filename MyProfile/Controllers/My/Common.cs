﻿using Microsoft.AspNetCore.Mvc;
using MyProfile.Entity.Repository;
using MyProfile.LittleDictionaries.Service;

namespace MyProfile.Controllers.My
{
	public class CommonController : Controller
	{
		private IBaseRepository repository;
		private DictionariesService dictionariesService;

		public CommonController(IBaseRepository repository,
			DictionariesService dictionariesService)
		{
			this.repository = repository;
			this.dictionariesService = dictionariesService;
		}

		public IActionResult Index()
		{
			return View();
		}

		[HttpGet]
		public JsonResult GetFoolterAction()
		{
			var data = dictionariesService.GetTotalActions();
			return Json(new { isOk = true, data = data});
		}
	}
}