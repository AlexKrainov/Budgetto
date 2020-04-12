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
	public partial class BudgetController : Controller
	{
		private IBaseRepository repository;
		private TemplateService templateService;
		private BudgetService budgetService;
		private SectionService sectionService;

		public BudgetController(IBaseRepository repository,
			BudgetService budgetService,
			TemplateService templateService,
			SectionService sectionService)
		{
			this.repository = repository;
			this.templateService = templateService;
			this.budgetService = budgetService;
			this.sectionService = sectionService;
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

		public async Task<JsonResult> GetBudget(int templateID)
		{
			DateTime start = new DateTime(2020, 01, 01);
			DateTime finish = new DateTime(2020, 01, 31);

			//var template = await templateService.GetTemplateByID(x => x.ID == 3 && x.PersonID == UserInfo.PersonID);
			//var budgetDataForTable = budgetService.GetBudgetDataByDays(start, finish, template);

			//DateTime start = new DateTime(2020, 01, 01);
			//DateTime finish = new DateTime(2020, 12, 31);

			var template = await templateService.GetTemplateByID(x => x.ID == templateID && x.PersonID == UserInfo.PersonID);
			var budgetDataForTable = budgetService.GetBudgetData(start, finish, template);

			return Json(new { isOk = true, rows = budgetDataForTable.Item1, footerRow = budgetDataForTable.Item2, template });
		}

		[HttpGet]
		public async Task<IActionResult> DaysBudget(int? month, int? templateID)
		{
			BudgetControllerModelView model = new BudgetControllerModelView();
			model.SelectedDateTime = month != null ? new DateTime(DateTime.Now.Year, month ?? 1, 1) : DateTime.Now;
			model.SelectedTemplateID = templateID ?? -1;
			model.NameTemplates = await templateService.GetNameTemplates(x => x.PersonID == UserInfo.PersonID && x.PeriodTypeID == (int)PeriodTypesEnum.Days);

			return View(model);
		}

		[HttpPost]
		public async Task<JsonResult> GetDaysBudget([FromQuery] DateTime month, [FromQuery] int templateID)
		{
			DateTime start = new DateTime(month.Year, month.Month, 01);
			DateTime finish = new DateTime(month.Year, month.Month, DateTime.DaysInMonth(month.Year, month.Month));

			var template = await templateService.GetTemplateByID(x => x.ID == templateID && x.PersonID == UserInfo.PersonID);

			if (template != null)
			{
				var budgetDataForTable = budgetService.GetBudgetData(start, finish, template);
				return Json(new { isOk = true, rows = budgetDataForTable.Item1, footerRow = budgetDataForTable.Item2, template });
			}
			return Json(new { isOk = false });
		}

		[HttpGet]
		public async Task<IActionResult> MonthsBudget(int? year, int? templateID)
		{
			BudgetControllerModelView model = new BudgetControllerModelView();
			model.SelectedYear = year ?? DateTime.Now.Year;
			model.SelectedTemplateID = templateID ?? -1;
			model.NameTemplates = await templateService.GetNameTemplates(x => x.PersonID == UserInfo.PersonID && x.PeriodTypeID == (int)PeriodTypesEnum.Months);

			return View(model);
		}

		[HttpPost]
		public async Task<JsonResult> GetMonthsBudget(int year, int templateID)
		{
			DateTime start = new DateTime(year, 1, 01);
			DateTime finish = new DateTime(year, 12, 31);

			var template = await templateService.GetTemplateByID(x => x.ID == templateID && x.PersonID == UserInfo.PersonID);
			if (template != null)
			{
				var budgetDataForTable = budgetService.GetBudgetData(start, finish, template);
				return Json(new { isOk = true, rows = budgetDataForTable.Item1, footerRow = budgetDataForTable.Item2, template });
			}
			return Json(new { isOk = false });
		}


	}
}