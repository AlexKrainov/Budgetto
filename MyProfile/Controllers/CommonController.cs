using Common.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
using MyProfile.Tag.Service;
using MyProfile.UserLog.Service;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MyProfile.Controllers
{
    [Authorize]
    public class CommonController : Controller
    {
        private IBaseRepository repository;
        private CommonService commonService;
        private TagService tagService;
        private UserLogService userLogService;

        public CommonController(IBaseRepository repository,
            CommonService commonService,
            TagService tagService,
            UserLogService userLogService)
        {
            this.repository = repository;
            this.commonService = commonService;
            this.tagService = tagService;
            this.userLogService = userLogService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetFooterAction()
        {
            var data = commonService.GetTotalActions();
            return Json(new { isOk = true, data = data });
        }

        [HttpGet]
        public JsonResult GetCurrenciesInfo()
        {
            var data = commonService.GetCurrencyInfo();
            return Json(new { isOk = true, data = data });
        }

        [HttpGet]
        public JsonResult GetUserTags()
        {
            return Json(new { isOk = true, tags = tagService.GetUserTags() });
        }


        /// <summary>
        /// http://www.cbr.ru/development/sxml/
        /// </summary>
        /// <param name="link"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<JsonResult> GetRateFromBank(string codeNameCBR, DateTime date)
        {
            return Json(new { isOk = true, bankCurrencyData = await commonService.GetRatesFromBankAsync(date, codeNameCBR) });
        }
    }
}