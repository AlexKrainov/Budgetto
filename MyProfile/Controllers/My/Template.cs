﻿using System;
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

			//BudgetArea budgetArea1 = new BudgetArea()
			//{
			//	Name = "Основные раходы",
			//	UserID = UserInfo.UserID
			//};
			//repository.Create(budgetArea1, true);

			//var BudgetSections = new List<BudgetSection>()
			//{
			//	new BudgetSection
			//	{
			//		BudgetAreaID = budgetArea1.ID,
			//		Name = "Расходы (продукты)",
			//		UserID = Guid.Parse("EA02C872-0C3C-4112-7231-08D7BDD8901D"),
			//	},
			//	new BudgetSection
			//	{
			//		BudgetAreaID = budgetArea1.ID,
			//		Name = "Прочие расходы",
			//		UserID = Guid.Parse("EA02C872-0C3C-4112-7231-08D7BDD8901D"),
			//	},
			//	new BudgetSection
			//	{
			//		BudgetAreaID = budgetArea1.ID,
			//		Name = "Траты на ребенка",
			//		UserID = Guid.Parse("EA02C872-0C3C-4112-7231-08D7BDD8901D"),
			//	},
			//		new BudgetSection
			//	{
			//		BudgetAreaID = budgetArea1.ID,
			//		Name = "Коммунальные",
			//		UserID = Guid.Parse("EA02C872-0C3C-4112-7231-08D7BDD8901D"),
			//	},
			//			new BudgetSection
			//	{
			//		BudgetAreaID = budgetArea1.ID,
			//		Name = "Проездной",
			//		UserID = Guid.Parse("EA02C872-0C3C-4112-7231-08D7BDD8901D"),
			//	},
			//				new BudgetSection
			//	{
			//		BudgetAreaID = budgetArea1.ID,
			//		Name = "Регулярные платежы",
			//		UserID = Guid.Parse("EA02C872-0C3C-4112-7231-08D7BDD8901D"),
			//	},
			//};
			//repository.CreateRange(BudgetSections, true);

			//BudgetArea budgetArea2 = new BudgetArea()
			//{
			//	Name = "Прочие раходы",
			//	UserID = UserInfo.UserID
			//};
			//repository.Create(budgetArea2, true);

			//BudgetSections = new List<BudgetSection>()
			//{
			//	new BudgetSection
			//	{
			//		BudgetAreaID = budgetArea2.ID,
			//		Name = "Внебюджетные расходы",
			//		UserID = Guid.Parse("EA02C872-0C3C-4112-7231-08D7BDD8901D"),
			//	},
			//};
			//repository.CreateRange(BudgetSections, true);

			//budgetArea2 = new BudgetArea()
			//{
			//	Name = "Зарплата",
			//	UserID = UserInfo.UserID
			//};
			//repository.Create(budgetArea2, true);

			//BudgetSections = new List<BudgetSection>()
			//{
			//	new BudgetSection
			//	{
			//		BudgetAreaID = budgetArea2.ID,
			//		Name = "Зарплата",
			//		UserID = Guid.Parse("EA02C872-0C3C-4112-7231-08D7BDD8901D"),
			//	},
			//	new BudgetSection
			//	{
			//		BudgetAreaID = budgetArea2.ID,
			//		Name = "Другие доходы",
			//		UserID = Guid.Parse("EA02C872-0C3C-4112-7231-08D7BDD8901D"),
			//	},
			//	new BudgetSection
			//	{
			//		BudgetAreaID = budgetArea2.ID,
			//		Name = "Выплаты",
			//		UserID = Guid.Parse("EA02C872-0C3C-4112-7231-08D7BDD8901D"),
			//	},
			//	new BudgetSection
			//	{
			//		BudgetAreaID = budgetArea2.ID,
			//		Name = "CashBack",
			//		UserID = Guid.Parse("EA02C872-0C3C-4112-7231-08D7BDD8901D"),
			//	},
			//};
			//repository.CreateRange(BudgetSections, true);

			//BudgetArea budgetArea = new BudgetArea()
			//{
			//	Name = "Инвестирование",
			//	UserID = UserInfo.UserID
			//};
			//repository.Create(budgetArea, true);

			//BudgetSections = new List<BudgetSection>()
			//{
			//	new BudgetSection
			//	{
			//		BudgetAreaID = budgetArea.ID,
			//		Name = "Российский счет",
			//		UserID = Guid.Parse("EA02C872-0C3C-4112-7231-08D7BDD8901D"),
			//	},
			//	new BudgetSection
			//	{
			//		BudgetAreaID = budgetArea.ID,
			//		Name = "Американский счет",
			//		UserID = Guid.Parse("EA02C872-0C3C-4112-7231-08D7BDD8901D"),
			//	},
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
			var templates = await templateService.GetTemplates(x => x.UserID == UserInfo.UserID);
			return Json(new { isOk = true, templates });
		}

		public IActionResult Edit(int? id)
		{
			ViewBag.PeriodTypes = repository.GetAll<PeriodType>()
				.Where(x => x.ID == (int)PeriodTypesEnum.Month || x.ID == (int)PeriodTypesEnum.Year)
				.ToList();

			return View();
		}

		[HttpGet]
		public async Task<IActionResult> GetTemplate(int? id)
		{
			TemplateViewModel templateViewModel = new TemplateViewModel();

			if (id != null)
			{
				templateViewModel = await templateService.GetTemplateByID(x => x.UserID == UserInfo.UserID && x.ID == id);
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