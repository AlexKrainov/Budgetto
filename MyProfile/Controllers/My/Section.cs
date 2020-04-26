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
				var areas = await sectionService.GetSectionForEdit();
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
					PersonID = UserInfo.PersonID
				};
				if (budgetArea.ID > 0)
				{
					await repository.UpdateAsync(budgetArea, true);
				}
				else
				{
					await repository.CreateAsync(budgetArea, true);
				}

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
		public async Task<IActionResult> SaveSection([FromBody] BudgetSecionModelView section)
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
					PersonID = UserInfo.PersonID
				};
				if (budgetSection.ID > 0)
				{
					await repository.UpdateAsync(budgetSection, true);
				}
				else
				{
					await repository.CreateAsync(budgetSection, true);
				}

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
		public async Task<IActionResult> GetAllSectionByPerson()
		{
			var sections = await repository.GetAll<BudgetArea>(x => x.PersonID == UserInfo.PersonID)
				.Select(x => new
				{
					x.ID,
					x.Name,
					BudgetSections = x.BudgetSectinos
						.Where(y => y.PersonID == UserInfo.PersonID)
						.Select(y => new
						{
							y.ID,
							y.Name,
						})
				})
				.ToListAsync();

			return Json(new { isOk = true, sections });
		}

		[HttpGet]
		public async Task<IActionResult> GetSectins()
		{
			var sections = await repository.GetAll<BudgetSection>(x => x.PersonID == UserInfo.PersonID)
				.OrderByDescending(x => x.BudgetRecords.Count())
				.Select(x => new
				{
					x.ID,
					x.Name,
					x.Description,
					x.CssIcon,
					x.CssColor,
					x.IsByDefault,
					AreaName = x.BudgetArea.Name,
					AreaID = x.BudgetAreaID,
					RecordCount = x.BudgetRecords.Count(),
					isShow = true,
				})
				.ToListAsync();

			return Json(new { isOk = true, sections });
		}

		#region Delete

		[HttpDelete]
		public async Task<IActionResult> RemoveArea(int id)
		{
			var budgetArea = repository.GetByID<BudgetArea>(id);

			if (budgetArea.PersonID != null && budgetArea.PersonID == UserInfo.PersonID && (budgetArea.BudgetSectinos == null || budgetArea.BudgetSectinos.Count() == 0))
			{
				try
				{
					repository.Delete(budgetArea, true);
				}
				catch (Exception ex)
				{
					return Json(new { isOk = false,id, wasDeleted = false, text = "Не удалось удалить !" });
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