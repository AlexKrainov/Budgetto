using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView.Goal;
using MyProfile.Entity.ModelView.Limit;
using MyProfile.Entity.Repository;
using MyProfile.Goal.Service;
using MyProfile.Identity;
using MyProfile.Limit.Service;
using MyProfile.LittleDictionaries.Service;
using MyProfile.User.Service;

namespace MyProfile.Controllers
{
    [Authorize]
    public class GoalController : Controller
    {
        private IBaseRepository repository;
        private GoalService goalService;
        private UserLogService userLogService;

        public GoalController(IBaseRepository repository,
            GoalService goalService,
            UserLogService userLogService)
        {
            this.repository = repository;
            this.goalService = goalService;
            this.userLogService = userLogService;
        }
        [HttpGet]
        public async Task<IActionResult> List()
        {
            await userLogService.CreateUserLogAsync(UserInfo.Current.UserSessionID, UserLogActionType.Goal_Page);
            return View();
        }

        public async Task<JsonResult> GetGoals()
        {
            return Json(new { isOk = true, goals = await goalService.GetGoalListView() });
        }
        [HttpGet]
        public async Task<JsonResult> GetGoal(int goalID)
        {
            return Json(new
            {
                isOk = true,
                goal = (await goalService.GetGoalListView(x => x.ID == goalID)).FirstOrDefault()
            });
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
            List<GoalModelView> goalChartsData = await goalService.GetChartData(start, finish, periodTypesEnum);

            return Json(new { goalChartsData });
        }

        [HttpPost]
        public async Task<JsonResult> Remove([FromBody] GoalModelView goal)
        {
            try
            {
                await goalService.RemoveOrRecovery(goal, isRemove: true);
            }
            catch (Exception ex)
            {
                return Json(new { isOk = false, ex.Message });
            }
            return Json(new { isOk = true, goal });
        }

        [HttpPost]
        public async Task<JsonResult> Recovery([FromBody] GoalModelView goal)
        {
            try
            {
                await goalService.RemoveOrRecovery(goal, isRemove: false);
            }
            catch (Exception ex)
            {
                return Json(new { isOk = false, ex.Message });
            }
            return Json(new { isOk = true, goal });
        }


    }
}
