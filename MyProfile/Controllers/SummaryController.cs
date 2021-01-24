using Microsoft.AspNetCore.Mvc;
using MyProfile.Budget.Service;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView;
using MyProfile.Entity.ModelView.Account;
using MyProfile.Entity.Repository;
using MyProfile.UserLog.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyProfile.Controllers
{
    public class SummaryController : Controller
    {
        private IBaseRepository repository;
        private BudgetRecordService budgetRecordService;
        private UserLogService userLogService;
        private SummaryService summaryService;

        public SummaryController(IBaseRepository repository,
            SummaryService summaryService,
            BudgetRecordService budgetRecordService,
            UserLogService userLogService)
        {
            this.repository = repository;
            this.budgetRecordService = budgetRecordService;
            this.userLogService = userLogService;
            this.summaryService = summaryService;
        }

        [HttpGet]
        public JsonResult GetSummaries(DateTime? date, int year, PeriodTypesEnum periodType)
        {
            DateTime start;
            DateTime finish;
            DateTime now = DateTime.Now;

            if (date.HasValue)
            {
                start = new DateTime(date.Value.Year, date.Value.Month, 01, 00, 00, 00);
                finish = new DateTime(date.Value.Year, date.Value.Month, DateTime.DaysInMonth(date.Value.Year, date.Value.Month), 23, 59, 59);
            }
            else
            {
                start = new DateTime(year, 1, 01, 00, 00, 00);
                finish = new DateTime(year, 12, 31, 23, 59, 59);
            }

            SummaryFilter filter = new SummaryFilter
            {
                StartDate = start,
                EndDate = finish,
                PeriodType = periodType
            };
            var summaries = summaryService.GetSummaries(filter); // current data of accounts (month is now or year is now)
            bool isPast = now >= start && now >= finish;

            //if (isPast)
            //{
            //    accountService.GetAcountsAllMoney(start, finish, accounts);
            //}

            return Json(new { isOk = true, summaries, isPast });
        }


        [HttpPost]
        public JsonResult Save([FromBody] AccountViewModel account)
        {

            return Json(new { isOk = true });
        }


    }
}
