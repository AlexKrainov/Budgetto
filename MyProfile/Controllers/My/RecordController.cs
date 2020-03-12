﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyProfile.Budget.Service;
using MyProfile.Entity.ModelView;
using MyProfile.Entity.Repository;

namespace MyProfile.Controllers.My
{
	public class RecordController : Controller
	{
		private IBaseRepository repository;
		private BudgetRecordService budgetRecordService;

		public RecordController(IBaseRepository repository, BudgetRecordService budgetRecordService)
		{
			this.repository = repository;
			this.budgetRecordService = budgetRecordService;
		}

		[HttpPost]
		public async Task<IActionResult> Save([FromBody]BudgetRecordModelView budgetRecord)
		{
			return Json(new { isOk = budgetRecordService.Create(budgetRecord) });
		}
	}
}