using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DynamicExpresso;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView;
using MyProfile.Entity.Repository;
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

		public IActionResult Index()
		{
			//var z1 = CSharpScript.EvaluateAsync("1 + 2").Result;
			//var z2 = CSharpScript.EvaluateAsync("1 - 2").Result;
			//var z3 = CSharpScript.EvaluateAsync("2 * 2").Result;
			//var z4 = CSharpScript.EvaluateAsync("2 / 2").Result;
			//var z5 = CSharpScript.EvaluateAsync("2 + (1 + 2)").Result;
			//var z6 = CSharpScript.EvaluateAsync("2 + 2 * 4").Result;
			////var z7 = CSharpScript.EvaluateAsync("20 * 5%");//.Result;
			////var z8 = CSharpScript.Create("20 * 5%");
			////var z9 = CSharpScript.RunAsync("20 * 5%");
			//var interpreter = new Interpreter();
			//var result = interpreter.Eval("1 + 2");
			//var z10 = interpreter.Eval("1 - 2");
			//var z11 = interpreter.Eval("2 * 2.2");
			//var z13 = interpreter.Eval("2 / 2");
			//var z14 = interpreter.Eval("2 * 2");
			////var z15 = interpreter.Eval("20 + 5%");
			//var z16 = interpreter.Eval("2 + (1 + 2)");
			//var z17 = interpreter.Eval("2 + 2 * 4");



			return View();
		}
		[HttpGet]
		public async Task<IActionResult> GetData(int? id)
		{
			TemplateViewModel templateViewModel = templateService.GetTemplateByID(id);

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
	}
}