using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DynamicExpresso;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.EntityFrameworkCore;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
using MyProfile.Template.Service;
using TemplateBudgetSection = MyProfile.Entity.Model.TemplateBudgetSection;

namespace MyProfile.Controllers
{
	public class TemplateController : Controller
	{
		private IBaseRepository repository;
		private TemplateService templateService;

		public TemplateController(IBaseRepository repository,
			TemplateService templateService)
		{
			this.repository = repository;
			this.templateService = templateService;

			//CSharpScript.EvaluateAsync("1 + 3").Result

			//Person person = new Person
			//{
			//	DateCreate = DateTime.Now,
			//	Email = "ialexkrainov2@gmail.com",
			//	IsDeleted = false,
			//	LastName = "Alexey",
			//	Name = "Kraynov",
			//	Password = "BlXlR1234"
			//};
			//repository.Create(person, true);


			//BudgetArea budgetArea = new BudgetArea()
			//{
			//	Name = "Investing",
			//	PersonID = Guid.Parse("EA02C872-0C3C-4112-7231-08D7BDD8901D")
			//};
			//repository.Create(budgetArea, true);

			//BudgetArea budgetArea = new BudgetArea()
			//{
			//	Name = "Ремонт",
			//	PersonID = Guid.Parse("EA02C872-0C3C-4112-7231-08D7BDD8901D")
			//};
			//repository.Create(budgetArea, true);
			//var BudgetSections = new List<BudgetSection>()
			//{
			//	new BudgetSection
			//	{
			//		BudgetAreaID = 4,
			//		Name = "Доставка",
			//		PersonID = Guid.Parse("EA02C872-0C3C-4112-7231-08D7BDD8901D"),
			//	},
			//	new BudgetSection
			//	{
			//		BudgetAreaID = 4,
			//		Name = "Основные закупки",
			//		PersonID = Guid.Parse("EA02C872-0C3C-4112-7231-08D7BDD8901D"),
			//	},
			//};
			//repository.CreateRange(BudgetSections, true);

			// BudgetSections = new List<BudgetSection>()
			//{
			//	new BudgetSection
			//	{
			//		BudgetAreaID = 3,
			//		Name = "Tax",
			//		PersonID = Guid.Parse("EA02C872-0C3C-4112-7231-08D7BDD8901D"),
			//	},
			//	new BudgetSection
			//	{
			//		BudgetAreaID = 3,
			//		Name = "Gold",
			//		PersonID = Guid.Parse("EA02C872-0C3C-4112-7231-08D7BDD8901D"),
			//	},
			//	new BudgetSection
			//	{
			//		BudgetAreaID = 3,
			//		Name = "Shears",
			//		PersonID = Guid.Parse("EA02C872-0C3C-4112-7231-08D7BDD8901D"),
			//	},
			//	new BudgetSection
			//	{
			//		BudgetAreaID = 3,
			//		Name = "OFZ",
			//		PersonID = Guid.Parse("EA02C872-0C3C-4112-7231-08D7BDD8901D"),
			//	}
			//};
			//repository.CreateRange(BudgetSections, true);

			//repository.Create<PeriodType>(new PeriodType
			//{
			//	Name = "Month",
			//	CodeName = "Month"
			//}, true);
			//repository.Create<PeriodType>(new PeriodType
			//{
			//	Name = "Year",
			//	CodeName = "Year"
			//}, true);

		}

		public IActionResult List()
		{
			return View();
		}

		public async Task<JsonResult> GetTemplates()
		{
			var templates = await templateService.GetTemplates(x => x.PersonID == UserInfo.PersonID);
			return Json(new { isOk = true, templates });
		}

		public IActionResult Index(int? id)
		{
			ViewBag.TemplateID = id;

			ViewBag.PeriodTypes = repository.GetAll<PeriodType>().ToList();

			return View();
		}

		[HttpGet]
		public async Task<IActionResult> GetTemplate(int? id)
		{
			TemplateViewModel templateViewModel = new TemplateViewModel();

			if (id != null)
			{
				templateViewModel = await templateService.GetTemplateByID(x => x.PersonID == UserInfo.PersonID && x.ID == id);
			}

			return Json(new
			{
				isOk = true,
				template = templateViewModel
			});
		}

		[HttpPost]
		public async Task<IActionResult> Save([FromBody]TemplateViewModel template)
		{

			template = templateService.SaveTemplate(template);

			return Json(new { isOk = true, template });
		}

		[HttpGet]
		public IActionResult Delete(int id)
		{
			repository.Delete<MyProfile.Entity.Model.Template>(id);

			return RedirectToAction("List");
		}
	}
}