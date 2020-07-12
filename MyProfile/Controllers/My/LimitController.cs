﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView.Limit;
using MyProfile.Entity.Repository;
using MyProfile.Limit.Service;
using MyProfile.LittleDictionaries.Service;

namespace MyProfile.Controllers.My
{
	public class LimitController : Controller
	{
		private IBaseRepository repository;
		private LimitService limitService;
		private DictionariesService dictionariesService;

		public LimitController(IBaseRepository repository,
			LimitService limitService,
			LittleDictionaries.Service.DictionariesService dictionariesService)
		{
			this.repository = repository;
			this.limitService = limitService;
			this.dictionariesService = dictionariesService;
		}
		[HttpGet]
		public async Task<IActionResult> List()
		{
			return View();
		}

		public async Task<JsonResult> GetLimits()
		{
			return Json(new { isOk = true, limits = await limitService.GetLimitListView() });
		}

		public async Task<JsonResult> GetPeriodTypes()
		{
			return Json(new { isOk = true, periodTypes = dictionariesService.GetPeriodTypes() });
		}

		[HttpPost]
		public async Task<JsonResult> Save([FromBody] LimitModelView limit)
		{

			try
			{
				limit = await limitService.UpdateOrCreate(limit);
			}
			catch (Exception ex)
			{
				return Json(new { isOk = false, Message = "Вовремя сохранения лимита произошла ошибка." });
			}

			return Json(new { isOk = true, limit });
		}

		[HttpGet]
		public async Task<IActionResult> LoadCharts(DateTime date, PeriodTypesEnum periodTypesEnum)
		{
			DateTime start = new DateTime(date.Year, date.Month, 01, 00, 00, 01);
			DateTime finish = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month), 23, 59, 59);

			List<LimitChartModelView> limitChartsData = await limitService.GetChartData(start, finish, periodTypesEnum);

			return Json(new { limitsChartsData = limitChartsData });
		}

	}
}