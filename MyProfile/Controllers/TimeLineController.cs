using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyProfile.Entity.ModelView;
using MyProfile.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyProfile.Controllers
{
    [Authorize]
    public partial class BudgetController : Controller
    {
        public async Task<IActionResult> TimeLine()
        {
            TimeLineViewModel model = new TimeLineViewModel();

            model.Years = (await budgetRecordService.GetAllYears()).Select(x => new YearsAndCount { year = x }).ToList();

            await userLogService.CreateUserLogAsync(UserInfo.Current.UserSessionID, UserLogActionType.TimeLine_Page);

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
            filter.UserID = UserInfo.Current.ID;

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
            IList<BudgetRecordModelView> result = null;
            filter.UserID = UserInfo.Current.ID;
            try
            {
                result = await budgetRecordService.GetBudgetRecordsByFilterAsync(filter);
            }
            catch (Exception ex)
            {

            }

            return Json(new { isOk = true, data = result, take = result.Count, isEnd = true });
        }

        [HttpPost]
        public async Task<JsonResult> LoadingRecordsForTableView([FromBody] CalendarFilterModels filter)
        {
            var currentUser = UserInfo.Current;
            if (currentUser.IsAllowCollectiveBudget)
            {
                filter.Sections.AddRange(await sectionService.GetCollectionSectionIDsBySectionID(filter.Sections));
            }

            filter.StartDate = new DateTime(filter.StartDate.Year, filter.StartDate.Month, filter.StartDate.Day, 0, 0, 0);
            filter.EndDate = new DateTime(filter.EndDate.Year, filter.EndDate.Month, filter.EndDate.Day, 23, 59, 59);
            filter.UserID = currentUser.ID;
            filter.IsConsiderCollection = currentUser.IsAllowCollectiveBudget && currentUser.UserSettings.BudgetPages_WithCollective;

            var result = await budgetRecordService.GetBudgetRecordsByFilterAsync(filter);

            #region For select sections
            var sections = (await sectionService.GetAllSectionByUserAsync()).ToList();

            for (int i = 0; i < sections.Count(); i++)
            {
                sections[i].Selected = filter.Sections.Any(x => x == sections[i].ID);
            }
            #endregion

            return Json(new
            {
                isOk = true,
                data = result,
                take = result.Count,
                isEnd = result.Count < 10,
                sections,
                dateStart = filter.StartDate,
                dateEnd = filter.EndDate
            });
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
            return Json(new { isOk = await budgetRecordService.RemoveRecord(record), record.DateTimeOfPayment });
        }

        [HttpPost]
        public async Task<JsonResult> RecoveryRecord([FromBody] BudgetRecordModelView record)
        {
            return Json(new { isOk = await budgetRecordService.RecoveryRecord(record), record.DateTimeOfPayment });
        }
    }
}
