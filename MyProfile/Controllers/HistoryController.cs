using LinqKit;
using Microsoft.AspNetCore.Mvc;
using MyProfile.Budget.Service;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView;
using MyProfile.Entity.ModelView.Account;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
using MyProfile.UserLog.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace MyProfile.Controllers
{
    public class HistoryController : Controller
    {
        private IBaseRepository repository;
        private BudgetService budgetService;
        private SectionService sectionService;
        private BudgetRecordService budgetRecordService;
        private UserLogService userLogService;

        //private IOptions<ProjectConfig> config;

        public HistoryController(IBaseRepository repository,
            BudgetService budgetService,
            SectionService sectionService,
            BudgetRecordService budgetRecordService,
            UserLogService userLogService)
        {
            this.repository = repository;
            this.budgetService = budgetService;
            this.sectionService = sectionService;
            this.budgetRecordService = budgetRecordService;
            this.userLogService = userLogService;

        }

        public IActionResult Records()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> LoadingRecordsForByDate(DateTime date)
        {
            CalendarFilterModels filter = new CalendarFilterModels { Sections = new List<long>() };
            filter.StartDate = new DateTime(date.Year, date.Month, date.Day, 00, 00, 00);
            filter.EndDate = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59);
            filter.Sections = (await sectionService.GetAllSectionByUser()).Select(x => x.ID).ToList();
            filter.UserID = UserInfo.Current.ID;

            var result = await budgetRecordService.GetBudgetRecordsByFilterAsync(filter);

            return Json(new { isOk = true, data = result });
        }

        [HttpPost]
        public async Task<JsonResult> GetGroupRecords([FromBody] CalendarFilterModels filter)
        {
            filter.StartDate = new DateTime(filter.StartDate.Year, filter.StartDate.Month, filter.StartDate.Day, 00, 00, 00);
            filter.EndDate = new DateTime(filter.EndDate.Year, filter.EndDate.Month, filter.EndDate.Day, 23, 59, 59);
            if (filter.IsSearchAllUserSections)
            {
                filter.Sections = (await sectionService.GetAllSectionByUser()).Select(x => x.ID).ToList();
            }
            filter.UserID = UserInfo.Current.ID;

            var result = await budgetRecordService.GetBudgetRecordsGroupByDateByFilterAsync(filter);

            return Json(new { isOk = true, data = result });
        }
    }
}
