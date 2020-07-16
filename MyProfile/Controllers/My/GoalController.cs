using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView.Goal;
using MyProfile.Entity.ModelView.Limit;
using MyProfile.Entity.Repository;
using MyProfile.Goal.Service;
using MyProfile.Limit.Service;
using MyProfile.LittleDictionaries.Service;

namespace MyProfile.Controllers.My
{
    public class GoalController : Controller
    {
        private IBaseRepository repository;
        private GoalService goalService;

        public GoalController(IBaseRepository repository,
            GoalService goalService)
        {
            this.repository = repository;
            this.goalService = goalService;
        }
        [HttpGet]
        public async Task<IActionResult> List()
        {
            return View();
        }

        public async Task<JsonResult> GetGoals()
        {
            return Json(new { isOk = true, goals = await goalService.GetGoalListView() });
        }

        [HttpPost]
        public async Task<JsonResult> Save([FromBody] GoalModelView goal)
        {
            try
            {
                goal = await goalService.UpdateOrCreate(goal);
            }
            catch (Exception ex)
            {
                return Json(new { isOk = false, ex.Message });
            }
            return Json(new { isOk = true, goal });
        }

        [HttpPost]
        public async Task<JsonResult> SaveRecord([FromBody] RecordItem record)
        {
            try
            {
                await goalService.AddRecord(record);
            }
            catch (Exception ex)
            {
                return Json(new { isOk = false, ex.Message });
            }
            return Json(new { isOk = true, record });
        }

        [HttpGet]
        public async Task<IActionResult> LoadCharts(DateTime date)
        {
            DateTime start = new DateTime(date.Year, date.Month, 01, 00, 00, 01);
            DateTime finish = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month), 23, 59, 59);

            List<GoalModelView> goalChartsData = await goalService.GetChartData(start, finish);

            return Json(new { goalChartsData = goalChartsData });
        }

    }
}
