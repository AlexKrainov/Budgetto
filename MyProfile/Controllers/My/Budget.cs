using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyProfile.Budget.Service;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView;
using MyProfile.Entity.Repository;
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
			//repository.CreateRange(new List<BudgetRecord>
			//{
			//	new BudgetRecord
			//	{
			//		Total = 240,
			//		BudgetSectionID = 3,
			//		DateTimeCreate = DateTime.Now,
			//		DateTimeEdit = DateTime.Now,
			//		DateTimeOfPayment = new DateTime(2020,03,03),
			//		Description = "descritioin",
			//		PersonID = Guid.Parse("EA02C872-0C3C-4112-7231-08D7BDD8901D")
			//	},
			//	new BudgetRecord
			//	{
			//		Total = 260,
			//		BudgetSectionID = 3,
			//		DateTimeCreate = DateTime.Now,
			//		DateTimeEdit = DateTime.Now,
			//		DateTimeOfPayment = new DateTime(2020,03,03),
			//		Description = "descritioin",
			//		PersonID = Guid.Parse("EA02C872-0C3C-4112-7231-08D7BDD8901D")
			//	},
			//		new BudgetRecord
			//	{
			//		Total = 260,
			//		BudgetSectionID = 3,
			//		DateTimeCreate = DateTime.Now,
			//		DateTimeEdit = DateTime.Now,
			//		DateTimeOfPayment = new DateTime(2020,03,04),
			//		Description = "descritioin",
			//		PersonID = Guid.Parse("EA02C872-0C3C-4112-7231-08D7BDD8901D")
			//	},
			//	new BudgetRecord
			//	{
			//		Total = 340,
			//		BudgetSectionID = 3,
			//		DateTimeCreate = DateTime.Now,
			//		DateTimeEdit = DateTime.Now,
			//		Description = "descritioin",
			//		DateTimeOfPayment = new DateTime(2020,03,04),
			//		PersonID = Guid.Parse("EA02C872-0C3C-4112-7231-08D7BDD8901D")
			//	},
			//		new BudgetRecord
			//	{
			//		Total = 340,
			//		BudgetSectionID = 3,
			//		DateTimeCreate = DateTime.Now,
			//		DateTimeEdit = DateTime.Now,
			//		DateTimeOfPayment = new DateTime(2020,03,04),
			//		Description = "descritioin",
			//		PersonID = Guid.Parse("EA02C872-0C3C-4112-7231-08D7BDD8901D")
			//	},
			//	new BudgetRecord
			//	{
			//		Total = 1240,
			//		BudgetSectionID = 4,
			//		DateTimeCreate = DateTime.Now,
			//		DateTimeEdit = DateTime.Now,
			//		DateTimeOfPayment = new DateTime(2020,03,04),
			//		Description = "descritioin",
			//		PersonID = Guid.Parse("EA02C872-0C3C-4112-7231-08D7BDD8901D")
			//	},
			//	new BudgetRecord
			//	{
			//		Total = 240,
			//		BudgetSectionID = 5,
			//		DateTimeCreate = DateTime.Now,
			//		DateTimeEdit = DateTime.Now,
			//		DateTimeOfPayment = new DateTime(2020,03,04),
			//		Description = "descritioin",
			//		PersonID = Guid.Parse("EA02C872-0C3C-4112-7231-08D7BDD8901D")
			//	},
			//	new BudgetRecord
			//	{
			//		Total = 240,
			//		BudgetSectionID = 5,
			//		DateTimeCreate = DateTime.Now,
			//		DateTimeEdit = DateTime.Now,
			//		DateTimeOfPayment = new DateTime(2020,03,03),
			//		Description = "descritioin",
			//		PersonID = Guid.Parse("EA02C872-0C3C-4112-7231-08D7BDD8901D")
			//	},
			//	new BudgetRecord
			//	{
			//		Total = 240,
			//		BudgetSectionID = 5,
			//		DateTimeCreate = DateTime.Now,
			//		DateTimeEdit = DateTime.Now,
			//		DateTimeOfPayment = new DateTime(2020,03,04),
			//		Description = "descritioin",
			//		PersonID = Guid.Parse("EA02C872-0C3C-4112-7231-08D7BDD8901D")
			//	},
			//	new BudgetRecord
			//	{
			//		Total = 245,
			//		BudgetSectionID = 6,
			//		DateTimeCreate = DateTime.Now,
			//		DateTimeEdit = DateTime.Now,
			//		DateTimeOfPayment = new DateTime(2020,03,03),
			//		Description = "descritioin",
			//		PersonID = Guid.Parse("EA02C872-0C3C-4112-7231-08D7BDD8901D")
			//	},
			//	new BudgetRecord
			//	{
			//		Total = 300,
			//		BudgetSectionID = 7,
			//		DateTimeCreate = DateTime.Now,
			//		DateTimeEdit = DateTime.Now,
			//		DateTimeOfPayment = new DateTime(2020,03,04),
			//		Description = "descritioin",
			//		PersonID = Guid.Parse("EA02C872-0C3C-4112-7231-08D7BDD8901D")
			//	},
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

		public IActionResult Index()
		{
			return View();
		}

		public JsonResult GetBudget()
		{
			DateTime start = new DateTime(2020, 03, 01);
			DateTime finish = new DateTime(2020, 03, 31);

			var template = templateService.GetTemplateByID(templateID: 3);
			var budgetDataForTable = budgetService.GetBudgetDataByMonth(start, finish, template);


			return Json(new { isOk = true, rows = budgetDataForTable.Item1, footerRow = budgetDataForTable.Item2, template });
		}
	}
}