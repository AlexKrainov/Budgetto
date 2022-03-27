using Common.Service;
using LinqKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MyProfile.Areas.Admin.Models;
using MyProfile.Entity.Model;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyProfile.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class UserSessionsController : Controller
    {
        private BaseRepository repository;
        private CommonService commonService;
        private IMemoryCache cache;

        public UserSessionsController(BaseRepository repository,
            CommonService commonService,
            CurrencyService currencyService,
            IMemoryCache cache)
        {
            this.repository = repository;
            this.commonService = commonService;
            this.cache = cache;

            if (UserInfo.Current.UserTypeID != (int)UserTypeEnum.Admin)
            {
                this.Redirect("/Home/Month");
            }
        }

        public IActionResult List()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetData([FromBody] UserSessionFilterModel filter)
        {
            var now = DateTime.Now.ToUniversalTime();
            var dateMinus2Days = now.AddDays(-1);
            var predicate = PredicateBuilder.True<UserSession>();

            if (filter == null)
            {
                predicate = predicate.And(x => x.EnterDate >= dateMinus2Days);
            }
            else
            {
                filter.RangeStart = filter.RangeStart.ToUniversalTime();
                filter.RangeEnd = filter.RangeEnd.ToUniversalTime();

                predicate = predicate.And(x => x.EnterDate >= filter.RangeStart && x.EnterDate <= filter.RangeEnd);

                if (filter.UserIDs != null && filter.UserIDs.Count > 0)
                {
                    predicate = predicate.And(x => filter.UserIDs.Contains(x.UserID ?? Guid.NewGuid()));
                }
            }

            var data = repository.GetAll(predicate)
                .Select(x => new UserSessionModel
                {
                    ID = x.ID,
                    EnterDate = x.EnterDate,
                    IP = x.IP,
                    IsLandingPage = x.IsLandingPage,
                    IsPhone = x.IsPhone,
                    IsTablet = x.IsTablet,
                    ScreenSize = x.ScreenSize,
                    Place = x.City + " " + x.Country,
                    BrowerName = x.BrowerName,
                    OS_Name = x.OS_Name,
                    Referrer = x.Referrer,
                    UserName = x.UserID != null ? x.User.Name : null,
                    Email = x.UserID != null ? x.User.Email : null,
                })
                .OrderByDescending(x => x.EnterDate)
                .ToList();

            foreach (var item in data)
            {
                item.EnterDate = item.EnterDate.AddHours(3);

                item.IPCounter = repository.GetAll<IPSetting>(x => x.IP == item.IP).Select(x => x.Counter)?.FirstOrDefault() ?? 1;
            }

            return Json(new { isOk = true, data = data });
        }
    }
}
