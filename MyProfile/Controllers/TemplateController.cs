using Common.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyProfile.Budget.Service;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView;
using MyProfile.Entity.ModelView.Counter;
using MyProfile.Entity.ModelView.TemplateModelView;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
using MyProfile.Template.Service;
using MyProfile.User.Service;
using MyProfile.UserLog.Service;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MyProfile.Controllers
{
    [Authorize]
    public class TemplateController : Controller
    {
        private IBaseRepository repository;
        private TemplateService templateService;
        private UserLogService userLogService;
        private UserCounterService userCounterService;
        private CommonService commonService;
        private SectionService sectionService;

        public TemplateController(IBaseRepository repository,
            TemplateService templateService,
            UserLogService userLogService,
            UserCounterService userCounterService,
            CommonService commonService,
            SectionService sectionService)
        {
            this.repository = repository;
            this.templateService = templateService;
            this.userLogService = userLogService;
            this.userCounterService = userCounterService;
            this.commonService = commonService;
            this.sectionService = sectionService;
        }

        public async Task<IActionResult> List()
        {
            await userLogService.CreateUserLogAsync(UserInfo.Current.UserSessionID, UserLogActionType.Templates_Page);
            CounterViewModel counterViewModel = userCounterService.GetCounterByEntity(BudgettoEntityType.Templates);
            return View(counterViewModel);
        }

        public async Task<JsonResult> GetTemplates()
        {
            var templates = await templateService.GetTemplates();
            return Json(new { isOk = true, templates });
        }

        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.TemplateID = id;
            ViewBag.PeriodTypes = repository.GetAll<PeriodType>()
                .Where(x => x.ID == (int)PeriodTypesEnum.Month || x.ID == (int)PeriodTypesEnum.Year)
                .ToList();

            await userLogService.CreateUserLogAsync(UserInfo.Current.UserSessionID, UserLogActionType.TemplateEdit_Page);
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetTemplate(int? id)
        {
            TemplateViewModel templateViewModel = new TemplateViewModel();

            if (id != null)
            {
                templateViewModel = await templateService.GetTemplateByID(x => x.UserID == UserInfo.Current.ID && x.ID == id);
            }

            return Json(new
            {
                isOk = true,
                template = templateViewModel
            });
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] TemplateViewModel template)
        {
            TemplateErrorModelView templateResult = new TemplateErrorModelView();

            try
            {
                templateResult = await templateService.SaveTemplate(template, false);
            }
            catch (Exception ex)
            {
                templateResult.IsOk = false;
                await userLogService.CreateErrorLogAsync(UserInfo.Current.UserSessionID, where: "Template.Save", ex);
            }

            return Json(templateResult);
        }

        [HttpPost]
        public async Task<IActionResult> SavePreparedTemplate([FromBody] TemplateViewModel template)
        {
            TemplateErrorModelView templateResult = new TemplateErrorModelView();

            try
            {
                templateResult = await templateService.SaveTemplate(template, false, isPreparedTemplate: true);
            }
            catch (Exception ex)
            {
                templateResult.IsOk = false;
                await userLogService.CreateErrorLogAsync(UserInfo.Current.UserSessionID, where: "Template.SavePreparedTemplate", ex);
            }

            return Json(templateResult);
        }

        [HttpPost]
        public async Task<IActionResult> SavePreparedTemplateAndEdit([FromBody] TemplateViewModel template)
        {
            TemplateErrorModelView templateResult = new TemplateErrorModelView();

            try
            {
                templateResult = await templateService.SaveTemplate(template, false, isPreparedTemplate: true);
                await userLogService.CreateUserLogAsync(UserInfo.Current.UserSessionID, UserLogActionType.PreparedTemplate_GoToEdit);
                templateResult.Href = "/Template/Edit/" + templateResult.Template.ID;
            }
            catch (Exception ex)
            {
                templateResult.IsOk = false;
                await userLogService.CreateErrorLogAsync(UserInfo.Current.UserSessionID, where: "Template.SavePreparedTemplateAndEdit", ex);
            }

            return Json(templateResult);
        }


        [HttpPost]
        public async Task<IActionResult> SaveAs([FromBody] TemplateViewModel template)
        {
            TemplateErrorModelView templateResult = new TemplateErrorModelView();
            var tmpTemplateID = template.ID;
            template.ID = 0;

            try
            {
                templateResult = await templateService.SaveTemplate(template, true);

                if (templateResult.IsOk == false)
                {
                    templateResult.Template.ID = tmpTemplateID;
                }
            }
            catch (Exception ex)
            {
                await userLogService.CreateErrorLogAsync(UserInfo.Current.UserSessionID, where: "Template.SaveAs", ex);
            }

            return Json(templateResult);
        }



        [HttpPost]
        public async Task<JsonResult> Remove([FromBody] TemplateViewModel template)
        {
            try
            {
                await templateService.RemoveOrRecovery(template.ID, isRemove: true);
            }
            catch (Exception ex)
            {
                return Json(new { isOk = false, ex.Message });
            }
            return Json(new { isOk = true, template });
        }

        [HttpPost]
        public async Task<JsonResult> Recovery([FromBody] TemplateViewModel template)
        {
            bool result;
            try
            {
                result = await templateService.RemoveOrRecovery(template.ID, isRemove: false);
            }
            catch (Exception ex)
            {
                return Json(new { isOk = false, ex.Message });
            }
            return Json(new { isOk = result, template });
        }

        /// <summary>
        /// Delete from /Template/Edit page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if (await templateService.RemoveOrRecovery(id, isRemove: true))
            {
                return RedirectToAction("List");
            }
            return RedirectToAction("Edit", id);
        }

        [HttpGet]
        public async Task<IActionResult> ToggleTemplate(int id)
        {
            await templateService.ToggleTemplate(id);
            return Json(new { isOk = true });
        }

        [HttpGet]
        public IActionResult GetPreparedTemplates()
        {
            var currentUser = UserInfo.Current;
            var periodTypes = commonService.GetPeriodTypes();
            var groupedAreaAndSection = sectionService.GetAllAreaAndSectionByPerson(currentUser.ID);

            PreparedTemplateService preparedTemplate
                = new PreparedTemplateService(periodTypes, groupedAreaAndSection);
            var z = preparedTemplate.GetPreparedItems();
            return Json(new { isOk = true, preparedTemplate = z });
        }
    }
}