using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView;
using MyProfile.Identity;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyProfile.Controllers
{
    public partial class BudgetController : Controller
    {
        public async Task<IActionResult> TimeLine ()
        {
            TimeLineViewModel model = new TimeLineViewModel();

            model.Years = await repository.GetAll<BudgetRecord>(x => x.UserID == UserInfo.Current.ID)
                .Select(x => x.DateTimeOfPayment.Year)
                .GroupBy(x => x)
                .Select(x => new YearsAndCount { year = x.Key })
                .ToListAsync();
            model.Sections = await sectionService.GetAllSectionByPerson();



            return View(model);
        }

        [HttpPost]
        public async Task<JsonResult> GetCountRecordsByYear([FromBody] CalendarFilterModels filter)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            List<DateForCalendar> dates = new List<DateForCalendar>();

            filter.StartDate = new DateTime(filter.Year, 1, 1, 00, 00, 01);
            filter.EndDate = new DateTime(filter.Year, 12, 31, 23, 59, 59);

            var result = await budgetRecordService.GetBudgetRecordsGroupByDate(filter);

            if (filter.IsAmount)
            {
                dates = result.Select(x => new DateForCalendar
                {
                    date = x.Key,
                    count = x.Sum(y => y.Total)
                }).ToList();
            }
            else
            {
                dates = result.Select(x => new DateForCalendar
                {
                    date = x.Key,
                    count = x.Count()
                }).ToList();
            }

            var legend = dates.OrderBy(x => x.count).Select(x => x.count).GroupBy(x => x).Select(x => x.Key).ToArray();

            stopwatch.Stop();

            return Json(new { isOk = true, dates, year = filter.Year, legend, resultTime = stopwatch.ElapsedMilliseconds / 60 });
        }
        [HttpPost]
        public async Task<JsonResult> LoadingRecordsForCalendar([FromBody] CalendarFilterModels filter)
        {
            var result = await budgetRecordService.GetBudgetRecordsByFilter(filter);

            return Json(new { isOk = true, data = result, take = result.Count, isEnd = result.Count < 10 });
        }

        [HttpPost]
        public async Task<JsonResult> LoadingRecordsForTableView([FromBody] CalendarFilterModels filter)
        {
            var currentUser = UserInfo.Current;
            if (currentUser.IsAllowCollectiveBudget)
            {
                filter.Sections.AddRange(await sectionService.GetCollectionSectionBySectionID(filter.Sections));

            }
            filter.IsConsiderCollection = currentUser.IsAllowCollectiveBudget && currentUser.UserSettings.BudgetPages_WithCollective;

            var result = await budgetRecordService.GetBudgetRecordsByFilter(filter);

            return Json(new { isOk = true, data = result, take = result.Count, isEnd = result.Count < 10 });
        }
        //LoadingRecords

        public async Task<JsonResult> GetLastRecords(int last)
        {
            var result = await budgetRecordService.GetLast(last);

            return Json(new { isOk = true, data = result });
        }


        [HttpPost]
        public async Task<JsonResult> RemoveRecord([FromBody] BudgetRecordModelView record)
        {
            return Json(new { isOk = await budgetRecordService.RemoveRecord(record), record.DateTimeOfPayment});
        }

        [HttpPost]
        public async Task<JsonResult> RecoveryRecord([FromBody] BudgetRecordModelView record)
        {
            return Json(new { isOk = await budgetRecordService.RecoveryRecord(record), record.DateTimeOfPayment });
        }
    }
}
