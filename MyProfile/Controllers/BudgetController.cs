using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MyProfile.Budget.Service;
using MyProfile.Code;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView.BudgetView;
using MyProfile.Entity.ModelView.TemplateModelView;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
using MyProfile.Template.Service;
using MyProfile.User.Service;
using MyProfile.UserLog.Service;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MyProfile.Controllers
{
    [Authorize]
    public partial class BudgetController : Controller
    {
        private IBaseRepository repository;
        private TemplateService templateService;
        private BudgetService budgetService;
        private SectionService sectionService;
        private BudgetRecordService budgetRecordService;
        private UserLogService userLogService;
        //private IOptions<ProjectConfig> config;

        public BudgetController(IBaseRepository repository,
            BudgetService budgetService,
            TemplateService templateService,
            SectionService sectionService,
            BudgetRecordService budgetRecordService,
            UserLogService userLogService)
        //,IOptions<ProjectConfig> config)
        {
            this.repository = repository;
            this.templateService = templateService;
            this.budgetService = budgetService;
            this.sectionService = sectionService;
            this.budgetRecordService = budgetRecordService;
            this.userLogService = userLogService;
            //this.config = config;
        }

        [HttpGet]
        public async Task<IActionResult> Month(int? month, int? templateID)
        {
            BudgetControllerModelView model = new BudgetControllerModelView();
            model.SelectedDateTime = month != null ? new DateTime(DateTime.Now.Year, month ?? 1, 1) : new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            model.SelectedTemplateID = templateID ?? -1;
            model.Templates = await templateService.GetNameTemplates(x =>
                x.UserID == UserInfo.Current.ID
                && x.PeriodTypeID == (int)PeriodTypesEnum.Month
                && x.IsDeleted != true);

            if (model.SelectedTemplateID == -1 && model.Templates.Count() > 0)
            {
                if (model.Templates.Any(x => x.IsDefault))
                {
                    model.SelectedTemplateID = model.Templates.FirstOrDefault(x => x.IsDefault).ID;
                }
                else
                {
                    model.SelectedTemplateID = model.Templates[0].ID;
                }
            }

            await userLogService.CreateUserLogAsync(UserInfo.Current.UserSessionID, UserLogActionType.BudgetMonth_Page);

            return View(model);
        }

        [HttpPost]
        public async Task<JsonResult> GetMonthBudget([FromQuery] DateTime month, [FromQuery] int templateID)
        {
            if (templateID > 0)
            {
                DateTime start = new DateTime(month.Year, month.Month, 01, 00, 00, 00);
                DateTime finish = new DateTime(month.Year, month.Month, DateTime.DaysInMonth(month.Year, month.Month), 23, 59, 59);

                var template = await templateService.GetTemplateByID(x => x.ID == templateID && x.UserID == UserInfo.Current.ID);

                if (template != null)
                {
                    var budgetDataForTable = budgetService.GetBudgetData(start, finish, template);
                    return Json(new { isOk = true, rows = budgetDataForTable.Item1, footerRow = budgetDataForTable.Item2, template });
                }
            }
            return Json(new { isOk = false });
        }

        [HttpGet]
        public async Task<IActionResult> Year(int? year, int? templateID)
        {
            BudgetControllerModelView model = new BudgetControllerModelView();
            model.SelectedYear = year ?? DateTime.Now.Year;
            model.SelectedTemplateID = templateID ?? -1;
            model.Templates = await templateService.GetNameTemplates(x =>
               x.UserID == UserInfo.Current.ID
               && x.PeriodTypeID == (int)PeriodTypesEnum.Year
               && x.IsDeleted != true);

            if (model.SelectedTemplateID == -1 && model.Templates.Count() > 0)
            {
                if (model.Templates.Any(x => x.IsDefault))
                {
                    model.SelectedTemplateID = model.Templates.FirstOrDefault(x => x.IsDefault).ID;
                }
                else
                {
                    model.SelectedTemplateID = model.Templates[0].ID;
                }
            }

            model.Years = await budgetRecordService.GetAllYears();

            if (model.Years == null || model.Years.Count == 0)
            {
                model.Years.Add(DateTime.Now.Year);
            }

            await userLogService.CreateUserLogAsync(UserInfo.Current.UserSessionID, UserLogActionType.BudgetYear_Page);

            return View(model);
        }

        [HttpPost]
        public async Task<JsonResult> GetYearBudget(int year, int templateID)
        {
            DateTime start = new DateTime(year, 1, 01);
            DateTime finish = new DateTime(year, 12, 31);

            var template = await templateService.GetTemplateByID(x => x.ID == templateID && x.UserID == UserInfo.Current.ID);
            if (template != null)
            {
                var budgetDataForTable = budgetService.GetBudgetData(start, finish, template);
                return Json(new { isOk = true, rows = budgetDataForTable.Item1, footerRow = budgetDataForTable.Item2, template });
            }
            return Json(new { isOk = false });
        }

        [HttpGet]
        public async Task<IActionResult> Years(int? lastYear, int? templateID)
        {
            BudgetControllerModelView model = new BudgetControllerModelView();
            model.SelectedYear = lastYear ?? DateTime.Now.Year;
            model.SelectedTemplateID = templateID ?? -1;
            model.Templates = await templateService.GetNameTemplates(x => x.UserID == UserInfo.Current.ID && x.PeriodTypeID == (int)PeriodTypesEnum.Year);
            if (model.SelectedTemplateID == -1 && model.Templates.Count() > 0)
            {
                model.SelectedTemplateID = model.Templates[0].ID;
            }
            return View(model);
        }

        [HttpPost]
        public async Task<JsonResult> GetYearsBudget(int lastYear, int templateID)
        {
            DateTime start = new DateTime(lastYear, 1, 01);
            DateTime finish = new DateTime(lastYear, 12, 31);

            var template = await templateService.GetTemplateByID(x => x.ID == templateID && x.UserID == UserInfo.Current.ID);
            if (template != null)
            {
                var budgetDataForTable = budgetService.GetBudgetData(start, finish, template);
                return Json(new { isOk = true, rows = budgetDataForTable.Item1, footerRow = budgetDataForTable.Item2, template });
            }
            return Json(new { isOk = false });
        }

        /// <summary>
        /// http://www.cbr.ru/development/sxml/
        /// </summary>
        /// <param name="link"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<JsonResult> GetRateFromBank(string link, DateTime date)
        {
            string text = string.Empty;
            var _link = link + date.ToString("dd/MM/yyyy");
            WebRequest wr = WebRequest.Create(_link);
            wr.Timeout = 3500;

            try
            {
                var response = await wr.GetResponseAsync();
                var readStream = new StreamReader(((HttpWebResponse)response).GetResponseStream(), Encoding.GetEncoding("windows-1251"));
                text = readStream.ReadToEnd();
            }
            catch (Exception ex)
            {
                await userLogService.CreateErrorLogAsync(UserInfo.Current.UserSessionID, where: "Budget.GetRateFromBank", ex, comment: _link);
            }

            return Json(new { isOk = true, response = text });
        }

        [HttpPost]
        public async Task<IActionResult> TemplateChangeColumns([FromBody] TemplateColumnOrder templateColumnOrder)
        {
            await templateService.ChangeColumnOrder(templateColumnOrder);


            return Json(new { isOk = true });
        }

    }
}