using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView;
using MyProfile.Identity;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyProfile.Controllers.My
{
	public partial class BudgetController : Controller
	{
		public async Task<IActionResult> Records()
		{
			TimeLineViewModel model = new TimeLineViewModel();

			model.Years = await repository.GetAll<BudgetRecord>(x => x.PersonID == UserInfo.PersonID)
				.Select(x => x.DateTimeOfPayment.Year)
				.GroupBy(x => x)
				.Select(x => new YearsAndCount { year = x.Key })
				.ToListAsync();
			model.Sections = await sectionService.GetSectionsForSelect2();

			return View(model);
		}

		[HttpPost]
		public async Task<JsonResult> GetCountRecordsByYear([FromBody] CalendarFilterModels filter)
		{
			List<DateForCalendar> dates = new List<DateForCalendar>();

			var result = budgetService.GetBudgetRecords(
				new DateTime(filter.Year, 1, 1),
				new DateTime(filter.Year, 12, 31),
				x => x.DateTimeOfPayment.Day,
				y => filter.Sections.Contains(y.BudgetSectionID));

			if (filter.IsAmount)
			{
				dates = result.Select(x => new DateForCalendar
				{
					date = x.FirstOrDefault().DateTimeOfPayment,
					count = x.Sum(y => y.Total)
				}).ToList();
			}
			else
			{
				dates = result.Select(x => new DateForCalendar
				{
					date = x.FirstOrDefault().DateTimeOfPayment,
					count = x.Count()
				}).ToList();
			}

			var legend = dates.OrderBy(x => x.count).Select(x => x.count).GroupBy(x => x).Select(x => x.Key).ToArray();

			return Json(new { isOk = true, dates, year = filter.Year, legend });
		}
		[HttpPost]
		public async Task<JsonResult> LoadingRecordsForCalendar([FromBody] CalendarFilterModels filter)
		{
			var result = await budgetService.GetBudgetRecordsByPeriod(x => x.PersonID == UserInfo.PersonID
				  && filter.StartDate <= x.DateTimeOfPayment && filter.EndDate >= x.DateTimeOfPayment
				  && filter.Sections.Contains(x.BudgetSectionID)
				  && x.IsDeleted == false);

			return Json(new { isOk = true, data = result, take = result.Count, isEnd = result.Count < 10 });
		}

		[HttpPost]
		public async Task<JsonResult> LoadingRecordsForTableView([FromBody] CalendarFilterModels filter)
		{
			if (UserInfo.Current.IsAllowCollectiveBudget)
			{
				filter.Sections.AddRange(await sectionService.GetCollectionSectionBySectionID(filter.Sections));

			}
			//x.PersonID == UserInfo.PersonID
			var result = await budgetService.GetBudgetRecordsByPeriod(x => 
				   filter.StartDate <= x.DateTimeOfPayment && filter.EndDate >= x.DateTimeOfPayment
				  && filter.Sections.Contains(x.BudgetSectionID)
				  && x.IsDeleted == false);

			return Json(new { isOk = true, data = result, take = result.Count, isEnd = result.Count < 10 });
		}
		//LoadingRecords
	}
}
