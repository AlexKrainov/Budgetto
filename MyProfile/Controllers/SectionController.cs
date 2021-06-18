using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyProfile.Budget.Service;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView;
using MyProfile.Entity.ModelView.Section;
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

        public IActionResult GetAllSectionForEdit()
        {
            try
            {
                return Json(new { isOk = true, areas = sectionService.GetFullModelByUserID() });
            }
            catch (Exception ex)
            {

            }

            return Json(new { isOk = false });
        }

        //[HttpPost]
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
        public IActionResult GetAllAreaAndSectionByPerson()
        {
            var areas = sectionService.GetAllAreaAndSectionByPerson();

            return Json(new { isOk = true, areas });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSectionByPerson()
        {
            var sections = await sectionService.GetAllSectionByUserAsync();

            return Json(new { isOk = true, sections });
        }

        [HttpGet]
        public IActionResult GetSectins()
        {
            var sections = sectionService.GetAllSectionForRecords();

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

        [HttpGet]
        public JsonResult GetBaseSection(string searchString, int page)
        {
            var predicate = PredicateBuilder.True<BaseSectionModelView>();

            if (string.IsNullOrEmpty(searchString) == false)
            {
                searchString = searchString.ToLower();
                predicate = predicate.And(item =>
                    item.KeyWords.ToLower().Contains(searchString)
                    || item.SectionName.ToLower().Contains(searchString)
                    || item.AreaName.ToLower().Contains(searchString));
            }

            var banks = sectionService.GetBaseSections()
                .AsQueryable()
                .Where(predicate);

            var result = banks
                .OrderBy(item => item.AreaID)
                .Skip((page - 1) * 20)
                .Take(20)
                .ToList();

            return Json(new
            {
                Count = banks.Count(),
                Items = result
            });
        }

    }

}