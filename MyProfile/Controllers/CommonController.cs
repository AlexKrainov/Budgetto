using Common.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MyProfile.Budget.Service;
using MyProfile.Entity.Model;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
using MyProfile.Limit.Service;
using MyProfile.Progress.Service;
using MyProfile.Tag.Service;
using MyProfile.UserLog.Service;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MyProfile.Controllers
{
    [Authorize]
    public class CommonController : Controller
    {
        private IBaseRepository repository;
        private CommonService commonService;
        private TagService tagService;
        private UserLogService userLogService;
        private CurrencyService currencyService;
        private IMemoryCache cache;
        private LimitService limitService;
        private ProgressService progressService;
        private BudgetTotalService budgetTotalService;

        public CommonController(IBaseRepository repository,
            CommonService commonService,
            TagService tagService,
            UserLogService userLogService,
            CurrencyService currencyService,
            IMemoryCache cache,
            LimitService limitService,
            ProgressService progressService,
            BudgetTotalService budgetTotalService)
        {
            this.repository = repository;
            this.commonService = commonService;
            this.tagService = tagService;
            this.userLogService = userLogService;
            this.currencyService = currencyService;
            this.cache = cache;
            this.limitService = limitService;
            this.progressService = progressService;
            this.budgetTotalService = budgetTotalService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetFooterAction()
        {
            var data = commonService.GetTotalActions();
            return Json(new { isOk = true, data = data });
        }

        [HttpGet]
        public JsonResult GetCurrenciesInfo()
        {
            var data = currencyService.GetCurrencyInfo();
            return Json(new { isOk = true, data = data });
        }

        [HttpGet]
        public JsonResult GetUserTags()
        {
            return Json(new { isOk = true, tags = tagService.GetUserTags() });
        }

        /// <summary>
        /// Check after add, edit record
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<JsonResult> Checker()
        {
            var currentUser = UserInfo.Current;

            await limitService.CheckLimitNotificationsAsync(null, currentUser.ID);

            return Json(new { isOk = true });
        }


        /// <summary>
        /// http://www.cbr.ru/development/sxml/
        /// </summary>
        /// <param name="link"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<JsonResult> GetRateFromBank(string charCode, DateTime date)
        {
            return Json(new { isOk = true, bankCurrencyData = await currencyService.GetRateByCodeAsync(date, charCode, UserInfo.Current.UserSessionID) });
        }

        [HttpGet]
        public async Task<JsonResult> GetTimeZone()
        {
            return Json(new { isOk = true, timezone = commonService.GetTimeZones() });
        }

        /// <summary>
        /// Check after add, edit record
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetProgress(DateTime date, PeriodType periodType)
        {
            var currentUser = UserInfo.Current;

            if (currentUser.IsCompleteIntroductoryProgress == false)
            {
                return Json(new { isOk = true, data = progressService.GetIntroductoryProgress() });
            }
            else
            {
                DateTime start = new DateTime(date.Year, date.Month, 01, 00, 00, 00);
                DateTime finish = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month), 23, 59, 59);

                var totalInvest = budgetTotalService.GetTotalInvestByPeriod(start, finish);
                var total = budgetTotalService.GetChartTotalByMonth(start, finish, SectionTypeEnum.Earnings);
                var totalEarnings = total.Item1[total.Item1.Count - 1];
                total = budgetTotalService.GetChartTotalByMonth(start, finish, SectionTypeEnum.Spendings);
                var totalSpendings = total.Item1[total.Item1.Count - 1];
                int count = repository.GetAll<Record>(x => x.UserID == currentUser.ID && x.IsDeleted == false && x.DateTimeOfPayment >= start && x.DateTimeOfPayment <= finish)
                    .GroupBy(x => x.DateTimeOfPayment.Date)
                    .Count();

                //return Json(new { isOk = true});
                return Json(new { isOk = true, data = progressService.SetAndGetFinancialLiteracyMonthProgress(totalEarnings, totalSpendings, totalInvest, count) });
            }



        }

    }
}
