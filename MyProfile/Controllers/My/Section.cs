using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyProfile.Entity.Model;
using MyProfile.Entity.Repository;
using MyProfile.Identity;

namespace MyProfile.Controllers
{
	public class SectionController : Controller
	{
		private IBaseRepository repository;

		public SectionController(IBaseRepository repository)
		{
			this.repository = repository;
		}

		[HttpGet]
		public async Task<IActionResult> GetAllSectionByPerson()
		{
			Guid personID = Guid.Parse("EA02C872-0C3C-4112-7231-08D7BDD8901D");

			var sections = await repository.GetAll<BudgetArea>(x => x.PersonID == personID)
				.Select(x => new
				{
					x.ID,
					x.Name,
					BudgetSections = x.BudgetSectinos
						.Where(y => y.PersonID == personID)
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
					x.Type_RecordType,
					x.CssIcon,
					x.IsByDefault,
					AreaName = x.BudgetArea.Name,
					AreaID = x.BudgetAreaID,
					RecordCount = x.BudgetRecords.Count(),
					isShow = true,
				})
				.ToListAsync();

			return Json(new { isOk = true, sections });
		}
	}
}