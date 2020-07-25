using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyProfile.Budget.Service;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
using Newtonsoft.Json;

namespace MyProfile.Controllers
{
	public class SectionController : Controller
	{
		private IBaseRepository repository;
		private SectionService sectionService;

		public SectionController(IBaseRepository repository, SectionService sectionService)
		{
			this.repository = repository;
			this.sectionService = sectionService;

			//CollectiveBudget collectiveBudget = new CollectiveBudget
			//{
			//	Name = "CollectiveBudget",
			//	DateCreate = DateTime.Now
			//};
			//repository.Create(collectiveBudget, true);

			//var person = new Person
			//{
			//	CollectiveBudgetID = collectiveBudget.ID,
			//	Email = "test@gmail.com",
			//	Password = "test",
			//	IsAllowCollectiveBudget = true,
			//	Name = "Test account",
			//	LastName = "Test account",

			//};
			//repository.Create(person, true);

			//var area = new BudgetArea
			//{
			//	Name = "Area for test account",
			//	UserID = person.ID
			//};
			//repository.Create(area, true);

			//var section = new BudgetSection
			//{
			//	BudgetAreaID = 12,
			//	Name = "Test for test account",
			//	SectionTypeCodeName = "Income",
			//};
			//repository.Create(section, true);
		}

		[HttpGet]
		public async Task<IActionResult> Edit()
		{
			return View();
		}

		public async Task<IActionResult> GetAllSectionForEdit()
		{
			try
			{
				var areas =  sectionService.GetFullModelByUserID();
				return Json(new { isOk = true, areas });
			}
			catch (Exception ex)
			{

			}

			return Json(new { isOk = false });
		}

		[HttpPost]
		public async Task<IActionResult> SaveArea([FromBody] BudgetAreaModelView area)
		{
			try
			{
				await sectionService.CreateOrUpdateArea(area);
				
				return Json(new { isOk = true, area });
			}
			catch (Exception ex)
			{

			}

			return Json(new { isOk = false });
		}

		[HttpPost]
		public async Task<IActionResult> SaveSection([FromBody] BudgetSectionModelView section)
		{
			try
			{
				await sectionService.CreateOrUpdateSection(section);

				return Json(new { isOk = true, section });
			}
			catch (Exception ex)
			{

			}

			return Json(new { isOk = false });
		}

		[HttpGet]
		public async Task<IActionResult> GetAllAreaAndSectionByPerson()
		{
			var areas = await sectionService.GetAllAreaAndSectionByPerson();

			return Json(new { isOk = true, areas });
		}

		[HttpGet]
		public async Task<IActionResult> GetAllSectionByPerson()
		{
			var sections = await sectionService.GetAllSectionByPerson(); 

			return Json(new { isOk = true, sections });
		}

		[HttpGet]
		public async Task<IActionResult> GetSectins()
		{
			var sections = await sectionService.GetAllSectionForRecords(); 

			return Json(new { isOk = true, sections });
		}

		#region Delete

		[HttpDelete]
		public async Task<IActionResult> RemoveArea(int id)
		{
			var budgetArea = repository.GetByID<BudgetArea>(id);

			if (budgetArea.UserID != null && budgetArea.UserID == UserInfo.Current.ID && (budgetArea.BudgetSectinos == null || budgetArea.BudgetSectinos.Count() == 0))
			{
				try
				{
					repository.Delete(budgetArea, true);
				}
				catch (Exception ex)
				{
					return Json(new { isOk = false, id, wasDeleted = false, text = "Не удалось удалить !" });
				}
				return Json(new { isOk = true, id, wasDeleted = true, text = "Удаление прошло успешно" });
			}
			return Json(new { isOk = true, id, wasDeleted = false, text = "Не удалось удалить !" });
		}

		[HttpDelete]
		public async Task<IActionResult> RemoveSection(int id)
		{
			var budgetSection = repository.GetByID<BudgetSection>(id);

			if (budgetSection.BudgetRecords == null || budgetSection.BudgetRecords.Count() == 0)
			{
				try
				{
					repository.Delete(budgetSection, true);
				}
				catch (Exception ex)
				{
					return Json(new { isOk = false, id, wasDeleted = false, text = "Не удалось удалить !" });
				}
				return Json(new { isOk = true, id, wasDeleted = true, text = "Удаление прошло успешно" });
			}
			return Json(new { isOk = true, id, wasDeleted = false, text = "Не удалось удалить !" });
		}

		#endregion
	}

}