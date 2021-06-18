using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyProfile.Budget.Service;
using MyProfile.Chart.Service;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView.Chart;
using MyProfile.Identity;
using MyProfile.Tag.Service;
using MyProfile.User.Service;
using MyProfile.UserLog.Service;

namespace MyProfile.Controllers
{
    [Authorize]
    public class ChartController : Controller
    {
        private ChartService chartService;
        private UserLogService userLogService;
        private SectionService sectionService;
        private BudgetTotalService budgetTotalService;
        private TagService tagService;

        public ChartController(ChartService chartService, UserLogService userLogService, SectionService sectionService, BudgetTotalService budgetTotalService, TagService tagService)
        {
            this.chartService = chartService;
            this.userLogService = userLogService;
            this.sectionService = sectionService;
            this.budgetTotalService = budgetTotalService;
            this.tagService = tagService;
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
            return Json(new { isOk = true, charts = await chartService.GetLightChartListView() });
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

        [HttpGet]
        public IActionResult StatisticChart(DateTime? date, int year, PeriodTypesEnum periodType)
        {
            DateTime start, finish;

            if (date != null)
            {
                start = new DateTime(date.Value.Year, date.Value.Month, 01, 00, 00, 00);
                finish = new DateTime(date.Value.Year, date.Value.Month, DateTime.DaysInMonth(date.Value.Year, date.Value.Month), 23, 59, 59);
                //start = DateTime.Now.AddMonths(-3);
                //finish = new DateTime(date.Value.Year, date.Value.Month, DateTime.DaysInMonth(date.Value.Year, date.Value.Month), 23, 59, 59);
            }
            else
            {
                start = new DateTime(year, 1, 01, 00, 00, 00);
                finish = new DateTime(year, 12, 31, 23, 59, 59);
            }


            var allSections = sectionService.GetAllSectionByUser()
                .Select(x => new UniversalChartSectionViewModel
                {
                    BudgetAreaID = x.BudgetAreaID,
                    BudgetAreaname = x.BudgetAreaname,
                    CodeName = x.CodeName,
                    CssBackground = x.CssBackground,
                    CssColor = x.CssColor,
                    CssIcon = x.CssIcon,
                    ID = x.ID,
                    IsCashback = x.IsCashback,
                    Name = x.Name,
                    SectionTypeCodeName = x.SectionTypeCodeName,
                    SectionTypeID = x.SectionTypeID,
                    IsShow = true,
                })
                .ToList();
            var allTags = tagService.GetUserTags()
                 .Select(x => new UniversalChartTagViewModel
                 {
                     CompanyID = x.CompanyID,
                     CompanyLogo = x.CompanyLogo,
                     CompanyName = x.CompanyName,
                     ID = x.ID,
                     Title = x.Title
                 })
                 .ToList();

            var chartData = chartService.GetUniversalChartData(start, finish, periodType, allSections);
            budgetTotalService.GetTotalBySections(start, finish, allSections, allTags);
            allTags = budgetTotalService.GetTotalByTags(start, finish, allTags);
            var values = periodType == PeriodTypesEnum.Month ? budgetTotalService.GetDataByMonth(finish, true) : budgetTotalService.GetDataByYear(finish.Year, true);
            #region ID
            values.Item1.ChartID = values.Item1.ChartID + "_statistic";
            values.Item2.ChartID = values.Item2.ChartID + "_statistic";
            values.Item3.ChartID = values.Item3.ChartID + "_statistic";
            #endregion

            UniversalChartWidgetViewModel model = new UniversalChartWidgetViewModel
            {
                Labels = chartData.Labels,
                DataSets = chartData.DataSets,
                MaxTotal = chartData.MaxTotal,//?
                Sections = allSections.Where(x => x.Total != 0).OrderByDescending(x => x.Total).ToList(),
                Tags = allTags.OrderByDescending(x => x.SpendingSum).ThenByDescending(x => x.EarningSum).ToList(),
                TotalView = new List<Entity.ModelView.TotalBudgetView.TotalModelView>
                {
                    values.Item2,
                    values.Item1,
                    values.Item3
                },
                IsShow = true
            };

            return Json(new { isOk = true, model });
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

        [HttpGet]
        public IActionResult ToggleChart(int id, PeriodTypesEnum periodType, bool isBudgetPage = false)
        {
            bool isShow = chartService.ToggleChart(id, periodType, isBudgetPage);

            return Json(new { isOk = true, isShow });
        }

    }
}
