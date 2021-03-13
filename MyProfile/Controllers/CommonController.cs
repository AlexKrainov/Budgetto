using Common.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView.Currency;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
using MyProfile.Limit.Service;
using MyProfile.Tag.Service;
using MyProfile.UserLog.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace MyProfile.Controllers
{
    [Authorize]
    public class CommonController : Controller
    {
        private IBaseRepository repository;
        private CommonService commonService;
        private TagService tagService;
        private UserLogService userLogService;
        private CurrencyService currencyService;
        private IMemoryCache cache;
        private LimitService limitService;

        public CommonController(IBaseRepository repository,
            CommonService commonService,
            TagService tagService,
            UserLogService userLogService,
            CurrencyService currencyService,
            IMemoryCache cache,
            LimitService limitService)
        {
            this.repository = repository;
            this.commonService = commonService;
            this.tagService = tagService;
            this.userLogService = userLogService;
            this.currencyService = currencyService;
            this.cache = cache;
            this.limitService = limitService;
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
            var data = currencyService.GetCurrencyInfo();
            return Json(new { isOk = true, data = data });
        }

        [HttpGet]
        public JsonResult GetUserTags()
        {
            return Json(new { isOk = true, tags = tagService.GetUserTags() });
        }

        /// <summary>
        /// Check after add, edit record
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<JsonResult> Checker()
        {
            var currentUser = UserInfo.Current;

            await limitService.CheckLimitNotificationsAsync(null, currentUser.ID);

            return Json(new { isOk = true });
        }


        /// <summary>
        /// http://www.cbr.ru/development/sxml/
        /// </summary>
        /// <param name="link"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<JsonResult> GetRateFromBank(string charCode, DateTime date)
        {
            return Json(new { isOk = true, bankCurrencyData = await currencyService.GetRateByCodeAsync(date, charCode, UserInfo.Current.UserSessionID) });
        }

        [HttpGet]
        public async Task<JsonResult> GetTimeZone()
        {
            return Json(new { isOk = true, timezone = commonService.GetTimeZones() });
        }

    }
}
