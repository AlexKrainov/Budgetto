using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView.Counter;
using MyProfile.Entity.ModelView.Limit;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
using MyProfile.Limit.Service;
using MyProfile.User.Service;
using MyProfile.UserLog.Service;

namespace MyProfile.Controllers
{
    [Authorize]
    public class LimitController : Controller
    {
        private IBaseRepository repository;
        private LimitService limitService;
        private CommonService commonService;
        private UserLogService userLogService;
        private UserCounterService userCounterService;

        public LimitController(IBaseRepository repository,
            LimitService limitService,
            CommonService dictionariesService,
            UserLogService userLogService,
            UserCounterService userCounterService)
        {
            this.repository = repository;
            this.limitService = limitService;
            this.commonService = dictionariesService;
            this.userLogService = userLogService;
            this.userCounterService = userCounterService;
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            await userLogService.CreateUserLogAsync(UserInfo.Current.UserSessionID, UserLogActionType.Limit_Page);
            CounterViewModel counterViewModel = userCounterService.GetCounterByEntity(BudgettoEntityType.Limits);
            return View(counterViewModel);
        }

        public async Task<JsonResult> GetLimits()
        {
            var currentUser = UserInfo.Current;

            if (currentUser.UserSettings.LimitPage_Show_IsFinished)
            {
                return Json(new { isOk = true, limits = await limitService.GetLimitListView(currentUser.ID) });
            }
            else
            {
                return Json(new { isOk = true, limits = await limitService.GetLimitListView(currentUser.ID, x => x.IsFinished == false) });
            }
        }

        public async Task<JsonResult> GetPeriodTypes()
        {
            return Json(new { isOk = true, periodTypes = commonService.GetPeriodTypes() });
        }

        [HttpPost]
        public async Task<JsonResult> Save([FromBody] LimitModelView limit)
        {
            try
            {
                var result = await limitService.UpdateOrCreate(limit);

                if (result.Item2 == 1)
                {
                    return Json(new { isOk = false, Message = "Ошибка при создании. Превышен лимит доступных лимитов." });
                }
                limit = (await limitService.GetLimitListView(UserInfo.Current.ID, x => x.ID == result.Item1)).FirstOrDefault();
            }
            catch (Exception ex)
            {
                return Json(new { isOk = false, Message = "Во время сохранения лимита произошла ошибка." });
            }

            return Json(new { isOk = true, limit });
        }

        [HttpGet]
        public async Task<IActionResult> LoadCharts(DateTime? date, int year, PeriodTypesEnum periodTypesEnum)
        {
            DateTime start, finish;
            if (date != null)
            {
                start = new DateTime(date.Value.Year, date.Value.Month, 01, 00, 00, 00);
                finish = new DateTime(date.Value.Year, date.Value.Month, DateTime.DaysInMonth(date.Value.Year, date.Value.Month), 23, 59, 59);
            }
            else
            {
                start = new DateTime(year, 1, 01, 00, 00, 00);
                finish = new DateTime(year, 12, 31, 23, 59, 59);
            }

            List<LimitChartModelView> limitChartsData = await limitService.GetChartData(start, finish, periodTypesEnum);

            return Json(new { limitsChartsData = limitChartsData });
        }



        [HttpPost]
        public async Task<JsonResult> Remove([FromBody] LimitModelView limit)
        {
            try
            {
              await limitService.RemoveOrRecovery(limit, isRemove: true);
            }
            catch (Exception ex)
            {
                return Json(new { isOk = false, ex.Message });
            }
            return Json(new { isOk = true, limit });
        }

        [HttpPost]
        public async Task<JsonResult> Recovery([FromBody] LimitModelView limit)
        {
            bool result = false;
            try
            {
               result = await limitService.RemoveOrRecovery(limit, isRemove: false);
            }
            catch (Exception ex)
            {
                return Json(new { isOk = false, ex.Message });
            }
            return Json(new { isOk = result, limit });
        }

        [HttpGet]
        public async Task<IActionResult> ToggleLimit(int id, PeriodTypesEnum periodType)
        {
            bool isShow = await limitService.ToggleLimit(id, periodType);

            return Json(new { isOk = true, isShow });
        }

    }
}
