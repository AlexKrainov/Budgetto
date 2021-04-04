using Common.Service;
using LinqKit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MyProfile.Areas.Admin.Models;
using MyProfile.Entity.Model;
using MyProfile.Entity.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyProfile.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserActionsController : Controller
    {
        private BaseRepository repository;
        private CommonService commonService;
        private IMemoryCache cache;

        public UserActionsController(BaseRepository repository,
            CommonService commonService,
            CurrencyService currencyService,
            IMemoryCache cache)
        {
            this.repository = repository;
            this.commonService = commonService;
            this.cache = cache;
        }

        public IActionResult List()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetData([FromBody] UserActionsFilterModel filter)
        {
            var now = DateTime.Now.ToUniversalTime();
            var dateMinus2Days = now.AddDays(-1);
            var predicate = PredicateBuilder.True<MyProfile.Entity.Model.UserLog>();
            predicate = predicate.And(x => x.UserSession.UserID != null);

            if (filter == null)
            {
                predicate = predicate.And(x => x.CurrentDateTime >= dateMinus2Days);
            }
            else
            {
                filter.RangeStart = filter.RangeStart.ToUniversalTime();
                filter.RangeEnd = filter.RangeEnd.ToUniversalTime();

                predicate = predicate.And(x => x.CurrentDateTime >= filter.RangeStart && x.CurrentDateTime <= filter.RangeEnd);

                if (filter.UserIDs != null && filter.UserIDs.Count > 0)
                {
                    predicate = predicate.And(x => filter.UserIDs.Contains(x.UserSession.UserID ?? Guid.NewGuid()));
                }
            }

            var data = repository.GetAll(predicate)
                .Select(x => new UserActionsModel
                {
                    ID = x.ID,
                    Comment = x.Comment,
                    CurrentDateTime = x.CurrentDateTime,
                    UserName = x.UserSession.User.Name,
                    Email = x.UserSession.User.Email,
                    ActionName = x.ActionCodeName,
                })
                .OrderByDescending(x => x.ID)
                .ToList();

            foreach (var item in data)
            {
                item.CurrentDateTime = item.CurrentDateTime.AddHours(3);
            }

            return Json(new { isOk = true, data = data });
        }

        [HttpGet]
        public JsonResult GetEmails(string searchString, int page)
        {
            var predicate = PredicateBuilder.True<MyProfile.Entity.Model.User>();

            if (string.IsNullOrEmpty(searchString) == false)
            {
                searchString = searchString.ToLower();
                predicate = predicate.And(item =>
                    item.Email.ToLower().Contains(searchString));
            }

            var emails = repository.GetAll(predicate)
                .Select(x => new EmailModel
                {
                    id = x.ID,
                    text = x.Email,
                    UserName = x.Name
                })
                .OrderBy(item => item.text)
                .Skip((page - 1) * 20)
                .Take(20)
                .ToList();

            return Json(new
            {
                Count = repository.GetAll(predicate).Count(),
                Items = emails
            });
        }
    }
}
