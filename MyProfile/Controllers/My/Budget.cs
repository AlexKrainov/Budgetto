using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyProfile.Budget.Service;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView;
using MyProfile.Entity.ModelView.BudgetView;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
using MyProfile.Template.Service;

namespace MyProfile.Controllers.My
{
	public class BudgetController : Controller
	{
		private IBaseRepository repository;
		private TemplateService templateService;
		private BudgetService budgetService;

		public BudgetController(IBaseRepository repository,
			BudgetService budgetService,
			TemplateService templateService)
		{
			this.repository = repository;
			this.templateService = templateService;
			this.budgetService = budgetService;

			//	new BudgetRecord
			//	{
			//		Total = 140,
			//		BudgetSectionID = 8,
			//		DateTimeCreate = DateTime.Now,
			//		DateTimeEdit = DateTime.Now,
			//		DateTimeOfPayment = new DateTime(2020,03,04),
			//		Description = "descritioin",
			//		PersonID = Guid.Parse("EA02C872-0C3C-4112-7231-08D7BDD8901D")
			//	},
			//}, true);

		}

		public IActionResult Index(int id)
		{
			ViewBag.TemplateID = id;
			return View();
		}

		public async Task<JsonResult> GetBudget(int templateID)
		{
			DateTime start = new DateTime(2020, 01, 01);
			DateTime finish = new DateTime(2020, 01, 31);

			//var template = await templateService.GetTemplateByID(x => x.ID == 3 && x.PersonID == UserInfo.PersonID);
			//var budgetDataForTable = budgetService.GetBudgetDataByDays(start, finish, template);

			//DateTime start = new DateTime(2020, 01, 01);
			//DateTime finish = new DateTime(2020, 12, 31);

			var template = await templateService.GetTemplateByID(x => x.ID == templateID  && x.PersonID == UserInfo.PersonID);
			var budgetDataForTable = budgetService.GetBudgetData(start, finish, template);

			return Json(new { isOk = true, rows = budgetDataForTable.Item1, footerRow = budgetDataForTable.Item2, template });
		}

		[HttpGet]
		public async Task<IActionResult> MonthBudget(DateTime month, int? templateID, int periodTypeID)
		{
			BudgetControllerModelView model = new BudgetControllerModelView();
			model.SelectedDateTime = month;
			model.SelectedTemplateID = templateID ?? -1;
			model.Templates = await templateService.GetNameTemplates(x => x.PersonID == UserInfo.PersonID && x.PeriodTypeID == periodTypeID);

			return View(model);
		}

		[HttpPost]
		public async Task<JsonResult> GetMonthBudget(DateTime month, int templateID)
		{
			DateTime start = new DateTime(month.Year, month.Month, 01);
			DateTime finish = new DateTime(month.Year, month.Month, DateTime.DaysInMonth(month.Year, month.Month));

			var template =  await templateService.GetTemplateByID(x => x.ID == templateID && x.PersonID == UserInfo.PersonID);
			var budgetDataForTable = budgetService.GetBudgetData(start, finish, template);

			return Json(new { isOk = true, rows = budgetDataForTable.Item1, footerRow = budgetDataForTable.Item2, template });
		}

		
	}
}