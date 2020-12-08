using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyProfile.Budget.Service;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
using MyProfile.User.Service;
using MyProfile.UserLog.Service;
using Newtonsoft.Json;

namespace MyProfile.Controllers
{
    [Authorize]
    public class SectionController : Controller
    {
        private IBaseRepository repository;
        private SectionService sectionService;
        private UserLogService userLogService;

        public SectionController(IBaseRepository repository, SectionService sectionService, UserLogService userLogService)
        {
            this.repository = repository;
            this.sectionService = sectionService;
            this.userLogService = userLogService;
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            await userLogService.CreateUserLogAsync(UserInfo.Current.UserSessionID, UserLogActionType.Section_Page);
            return View();
        }

        public async Task<IActionResult> GetAllSectionForEdit()
        {
            try
            {
                var areas = sectionService.GetFullModelByUserID();
                return Json(new { isOk = true, areas });
            }
            catch (Exception ex)
            {

            }

            return Json(new { isOk = false });
        }

        [HttpPost]
        public async Task<IActionResult> SaveArea([FromBody] BudgetAreaModelView area)
        {
            try
            {
                await sectionService.CreateOrUpdateArea(area);

                return Json(new { isOk = true, area });
            }
            catch (Exception ex)
            {

            }

            return Json(new { isOk = false });
        }

        [HttpPost]
        public async Task<IActionResult> SaveSection([FromBody] BudgetSectionModelView section)
        {
            try
            {
                await sectionService.CreateOrUpdateSection(section);

                return Json(new { isOk = true, section });
            }
            catch (Exception ex)
            {

            }

            return Json(new { isOk = false });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAreaAndSectionByPerson()
        {
            var areas = await sectionService.GetAllAreaAndSectionByPerson();

            return Json(new { isOk = true, areas });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSectionByPerson()
        {
            var sections = await sectionService.GetAllSectionByUser();

            return Json(new { isOk = true, sections });
        }

        [HttpGet]
        public async Task<IActionResult> GetSectins()
        {
            var sections = await sectionService.GetAllSectionForRecords();

            return Json(new { isOk = true, sections });
        }

        #region Delete

        [HttpPost]
        public async Task<IActionResult> RemoveArea(int id)
        {
            var tuple = await sectionService.DeleteArea(id);

            return Json(new { isOk = tuple.Item1, id, wasDeleted = tuple.Item2, text = tuple.Item3 });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveSection(int id)
        {
            var tuple = await sectionService.DeleteSection(id);

            return Json(new { isOk = tuple.Item1, id, wasDeleted = tuple.Item2, text = tuple.Item3 });
        }

        #endregion
    }

}