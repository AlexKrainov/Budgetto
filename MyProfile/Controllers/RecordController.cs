using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyProfile.Budget.Service;
using MyProfile.Entity.ModelView;
using MyProfile.Entity.Repository;

namespace MyProfile.Controllers
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

		public async Task<IActionResult> Index()
		{

			return View();
		}

		[HttpGet]
		public async Task<JsonResult> GetByID(int id)
		{
			var record = await budgetRecordService.GetByID(id);

			if (record == null)
			{
				return Json(new { isOk = false, id });
			}
			return Json(new { isOk = true, record });
		}

		[HttpPost]
		public async Task<IActionResult> SaveRecords([FromBody]RecordsModelView budgetRecord)
		{
			budgetRecordService.CreateOrUpdate(budgetRecord);
			return Json(new { isOk = true, budgetRecord });
		}
		[HttpPost]
		public async Task<IActionResult> Save([FromBody]BudgetRecordModelView budgetRecord)
		{
			return Json(new { isOk = budgetRecordService.Create(budgetRecord) });
		}
	}
}