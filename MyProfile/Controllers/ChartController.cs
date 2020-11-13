using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyProfile.Chart.Service;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView.Chart;
using MyProfile.Identity;
using MyProfile.User.Service;

namespace MyProfile.Controllers
{
    [Authorize]
    public class ChartController : Controller
    {
        private ChartService chartService;
        private UserLogService userLogService;

        public ChartController(ChartService chartService, UserLogService userLogService)
        {
            this.chartService = chartService;
            this.userLogService = userLogService;
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            await userLogService.CreateUserLogAsync(UserInfo.Current.UserSessionID, UserLogActionType.BigCharts_Page);
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetCharts([FromBody] ChartEditModel chart)
        {
            return Json(new { isOk = true, charts = await chartService.GetChartListView() });
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int? id, string href)
        {
            await userLogService.CreateUserLogAsync(UserInfo.Current.UserSessionID, UserLogActionType.BigChartEdit_Page);
            return View(new ChartEditViewModel { ID = id, href = href });
        }


        [HttpPost]
        public async Task<JsonResult> Save([FromBody] ChartEditModel chart)
        {
            try
            {
                await chartService.CreateOrUpdate(chart);
            }
            catch (Exception ex)
            {
                return Json(new { isOk = false, ex.Message });
            }
            return Json(new { isOk = true, chart, href = chart.href ?? "/Chart/List" });
        }

        [HttpGet]
        public async Task<IActionResult> LoadChart(int id)
        {
            var chart = await chartService.GetChartListView(x => x.ID == id);

            return Json(new { chart = chart.FirstOrDefault() });
        }

        [HttpGet]
        public async Task<IActionResult> LoadCharts(DateTime? date, int year, PeriodTypesEnum periodType)
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

            List<ChartViewModel> chartsData = await chartService.GetChartData(start, finish, periodType);

            return Json(new { bigChartsData = chartsData });
        }

        [HttpPost]
        public async Task<JsonResult> Remove([FromBody] ChartEditModel chart)
        {
            try
            {
                await chartService.RemoveOrRecovery(chart, isRemove: true);
            }
            catch (Exception ex)
            {
                return Json(new { isOk = false, ex.Message });
            }
            return Json(new { isOk = true, chart });
        }

        [HttpPost]
        public async Task<JsonResult> Recovery([FromBody] ChartEditModel chart)
        {
            try
            {
                await chartService.RemoveOrRecovery(chart, isRemove: false);
            }
            catch (Exception ex)
            {
                return Json(new { isOk = false, ex.Message });
            }
            return Json(new { isOk = true, chart });
        }
    }
}
