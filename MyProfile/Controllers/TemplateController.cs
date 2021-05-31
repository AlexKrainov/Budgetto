using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public TemplateController(IBaseRepository repository,
            TemplateService templateService,
            UserLogService userLogService,
            UserCounterService userCounterService)
        {
            this.repository = repository;
            this.templateService = templateService;
            this.userLogService = userLogService;
            this.userCounterService = userCounterService;
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

    }
}