using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
	[Authorize]
	public partial class BudgetController : Controller
	{
		private IBaseRepository repository;
		private TemplateService templateService;
		private BudgetService budgetService;
		private SectionService sectionService;
		private BudgetRecordService budgetRecordService;

		public BudgetController(IBaseRepository repository,
			BudgetService budgetService,
			TemplateService templateService,
			SectionService sectionService,
			BudgetRecordService budgetRecordService)
		{
			this.repository = repository;
			this.templateService = templateService;
			this.budgetService = budgetService;
			this.sectionService = sectionService;
			this.budgetRecordService = budgetRecordService;
			//	new BudgetRecord
			//	{
			//		Total = 140,
			//		BudgetSectionID = 8,
			//		DateTimeCreate = DateTime.Now,
			//		DateTimeEdit = DateTime.Now,
			//		DateTimeOfPayment = new DateTime(2020,03,04),
			//		Description = "descritioin",
			//		UserID = Guid.Parse("EA02C872-0C3C-4112-7231-08D7BDD8901D")
			//	},
			//}, true);
		}

		public async Task<JsonResult> GetBudget(int templateID)
		{
			DateTime start = new DateTime(2020, 01, 01);
			DateTime finish = new DateTime(2020, 01, 31);

			//var template = await templateService.GetTemplateByID(x => x.ID == 3 && x.UserID == UserInfo.UserID);
			//var budgetDataForTable = budgetService.GetBudgetDataByDays(start, finish, template);

			//DateTime start = new DateTime(2020, 01, 01);
			//DateTime finish = new DateTime(2020, 12, 31);

			var template = await templateService.GetTemplateByID(x => x.ID == templateID && x.UserID == UserInfo.UserID);
			var budgetDataForTable = budgetService.GetBudgetData(start, finish, template);

			return Json(new { isOk = true, rows = budgetDataForTable.Item1, footerRow = budgetDataForTable.Item2, template });
		}

		[HttpGet]
		public async Task<IActionResult> Month(int? month, int? templateID)
		{
			BudgetControllerModelView model = new BudgetControllerModelView();
			model.SelectedDateTime = month != null ? new DateTime(DateTime.Now.Year, month ?? 1, 1) : new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
			model.SelectedTemplateID = templateID ?? -1;
			model.Templates = await templateService.GetNameTemplates(x => x.UserID == UserInfo.UserID && x.PeriodTypeID == (int)PeriodTypesEnum.Month);

			if (model.SelectedTemplateID == -1 && model.Templates.Count() > 0)
			{
				model.SelectedTemplateID = model.Templates[0].ID;
			}

			return View(model);
		}

		[HttpPost]
		public async Task<JsonResult> GetMonthBudget([FromQuery] DateTime month, [FromQuery] int templateID)
		{
			if (templateID > 0)
			{
				DateTime start = new DateTime(month.Year, month.Month, 01, 00, 00, 01);
				DateTime finish = new DateTime(month.Year, month.Month, DateTime.DaysInMonth(month.Year, month.Month), 23, 59, 59);

				var template = await templateService.GetTemplateByID(x => x.ID == templateID && x.UserID == UserInfo.UserID);

				if (template != null)
				{
					var budgetDataForTable = budgetService.GetBudgetData(start, finish, template);
					return Json(new { isOk = true, rows = budgetDataForTable.Item1, footerRow = budgetDataForTable.Item2, template });
				}
			}
			return Json(new { isOk = false });
		}

		[HttpGet]
		public async Task<IActionResult> Year(int? year, int? templateID)
		{
			BudgetControllerModelView model = new BudgetControllerModelView();
			model.SelectedYear = year ?? DateTime.Now.Year;
			model.SelectedTemplateID = templateID ?? -1;
			model.Templates = await templateService.GetNameTemplates(x => x.UserID == UserInfo.UserID && x.PeriodTypeID == (int)PeriodTypesEnum.Year);
			if (model.SelectedTemplateID == -1 && model.Templates.Count() > 0)
			{
				model.SelectedTemplateID = model.Templates[0].ID;
			}
			return View(model);
		}

		[HttpPost]
		public async Task<JsonResult> GetYearBudget(int year, int templateID)
		{
			DateTime start = new DateTime(year, 1, 01);
			DateTime finish = new DateTime(year, 12, 31);

			var template = await templateService.GetTemplateByID(x => x.ID == templateID && x.UserID == UserInfo.UserID);
			if (template != null)
			{
				var budgetDataForTable = budgetService.GetBudgetData(start, finish, template);
				return Json(new { isOk = true, rows = budgetDataForTable.Item1, footerRow = budgetDataForTable.Item2, template });
			}
			return Json(new { isOk = false });
		}

		[HttpGet]
		public async Task<IActionResult> Years(int? lastYear, int? templateID)
		{
			BudgetControllerModelView model = new BudgetControllerModelView();
			model.SelectedYear = lastYear ?? DateTime.Now.Year;
			model.SelectedTemplateID = templateID ?? -1;
			model.Templates = await templateService.GetNameTemplates(x => x.UserID == UserInfo.UserID && x.PeriodTypeID == (int)PeriodTypesEnum.Year);
			if (model.SelectedTemplateID == -1 && model.Templates.Count() > 0)
			{
				model.SelectedTemplateID = model.Templates[0].ID;
			}
			return View(model);
		}

		[HttpPost]
		public async Task<JsonResult> GetYearsBudget(int lastYear, int templateID)
		{
			DateTime start = new DateTime(lastYear, 1, 01);
			DateTime finish = new DateTime(lastYear, 12, 31);

			var template = await templateService.GetTemplateByID(x => x.ID == templateID && x.UserID == UserInfo.UserID);
			if (template != null)
			{
				var budgetDataForTable = budgetService.GetBudgetData(start, finish, template);
				return Json(new { isOk = true, rows = budgetDataForTable.Item1, footerRow = budgetDataForTable.Item2, template });
			}
			return Json(new { isOk = false });
		}


	}
}