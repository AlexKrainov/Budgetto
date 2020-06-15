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
				var areas = await sectionService.GetFullModel();
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
				var budgetArea = new BudgetArea
				{
					ID = area.ID,
					CssIcon = area.CssIcon,
					Description = area.Description,
					Name = area.Name,
					UserID = UserInfo.UserID
				};
				if (budgetArea.ID > 0)
				{
					await repository.UpdateAsync(budgetArea, true);
				}
				else
				{
					await repository.CreateAsync(budgetArea, true);
				}

				await sectionService.SaveIncludedArea(budgetArea.ID, area.CollectiveAreas.Select(x => x.ID).ToList());

				area.ID = budgetArea.ID;
				area.IsUpdated = true;

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
				var budgetSection = new BudgetSection
				{
					ID = section.ID,
					CssIcon = section.CssIcon,
					CssColor = section.CssColor,
					Description = section.Description,
					Name = section.Name,
					BudgetAreaID = section.AreaID,
					SectionTypeID = section.SectionTypeID
					//UserID = UserInfo.UserID
				};
				if (budgetSection.ID > 0)
				{
					await repository.UpdateAsync(budgetSection, true);
				}
				else
				{
					await repository.CreateAsync(budgetSection, true);
				}

				await sectionService.SaveIncludedSection(section.ID, section.CollectiveSections.Select(x => x.ID).ToList());

				section.ID = budgetSection.ID;
				section.AreaID = budgetSection.BudgetAreaID;

				section.IsUpdated = true;

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
			var areas = await repository.GetAll<BudgetArea>(x => x.UserID == UserInfo.Current.ID)
				.Select(x => new
				{
					x.ID,
					x.Name,
					BudgetSections = x.BudgetSectinos
						.Where(y => y.BudgetArea.UserID == UserInfo.UserID)
						.Select(y => new
						{
							y.ID,
							y.Name,
						})
				})
				.ToListAsync();

			return Json(new { isOk = true, areas });
		}

		[HttpGet]
		public async Task<IActionResult> GetAllSectionByPerson()
		{
			var sections = await repository.GetAll<BudgetSection>(x => x.UserID == UserInfo.Current.ID)
				.Select(x => new
				{
					x.ID,
					x.Name,
					BudgetAreaID = x.BudgetArea.ID,
					BudgetAreaname = x.BudgetArea.Name
				})
				.ToListAsync();

			return Json(new { isOk = true, sections });
		}

		[HttpGet]
		public async Task<IActionResult> GetSectins()
		{
			var sections = await repository.GetAll<BudgetSection>(x => x.BudgetArea.UserID == UserInfo.Current.ID)
				.OrderByDescending(x => x.BudgetRecords.Count())
				.Select(x => new
				{
					ID = x.ID,
					Name = x.Name,
					Description = x.Description,
					CssIcon = x.CssIcon,
					CssColor = x.CssColor,
					AreaName = x.BudgetArea.Name,
					AreaID = x.BudgetAreaID,
					RecordCount = x.BudgetRecords.Count(),
					IsShow = true,
					CollectionSections = x.CollectiveSections.Select(y => new BudgetSectionModelView
					{
						ID = y.ChildSection.ID,
						CanEdit = false,
						Description = y.ChildSection.Description,
						Name = y.ChildSection.Name,
						Owner = y.ChildSection.User.Name,
						IsShow = true,

					}).ToList()
				})
				.ToListAsync();

			return Json(new { isOk = true, sections });
		}

		#region Delete

		[HttpDelete]
		public async Task<IActionResult> RemoveArea(int id)
		{
			var budgetArea = repository.GetByID<BudgetArea>(id);

			if (budgetArea.UserID != null && budgetArea.UserID == UserInfo.UserID && (budgetArea.BudgetSectinos == null || budgetArea.BudgetSectinos.Count() == 0))
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