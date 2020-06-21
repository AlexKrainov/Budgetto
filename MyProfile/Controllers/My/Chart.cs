using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyProfile.Chart.Service;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView.Chart;

namespace MyProfile.Controllers.My
{
    public class Chart : Controller
    {
        private ChartService chartService;

        public Chart(ChartService chartService)
        {
            this.chartService = chartService;

        }

        [HttpGet]
        public IActionResult List()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetCharts([FromBody] ChartEditModel chart)
        {
            return Json(new { isOk = true, charts = await chartService.GetChartListView() });
        }


        [HttpGet]
        public IActionResult Edit(int? id)
        {
            return View(id);
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
            return Json(new { isOk = true, chart });
        }

        [HttpGet]
        public async Task<IActionResult> LoadChart(int id)
        {
            var chart = await chartService.GetChartListView(x => x.ID == id);

            return Json(new { chart = chart.FirstOrDefault() });
        }

        [HttpGet]
        public async Task<IActionResult> LoadCharts(DateTime date, PeriodTypesEnum periodType)
        {
            DateTime start = new DateTime(date.Year, date.Month, 01, 00, 00, 01);
            DateTime finish = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month), 23, 59, 59);

            List<ChartViewModel> chartsData = await chartService.GetChartData(start, finish, periodType);

            return Json(new { bigChartsData = chartsData });
        }

    }
}
