using Microsoft.AspNetCore.Mvc;
using MyProfile.Entity.ModelView.SubScription;
using MyProfile.Entity.Repository;
using MyProfile.SubScription.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyProfile.Controllers
{
    public class SubscriptionController : Controller
    {
        private IBaseRepository repository;
        private SubScriptionService subScriptionService;

        public SubscriptionController(IBaseRepository repository,
            SubScriptionService subScriptionService)
        {
            this.repository = repository;
            this.subScriptionService = subScriptionService;
        }
        public IActionResult List()
        {
            return View();
        }

        //GetBaseSubScriptions
        [HttpGet]
        public IActionResult GetBaseSubScriptions()
        {
            return Json(new { isOk = true, baseSubScriptions = subScriptionService.GetBaseSubScriptions() });
        }

        [HttpGet]
        public IActionResult GetUserSubScriptions()
        {
            return Json(new { isOk = true, userSubScriptions = subScriptionService.GetUserSubScriptions() });
        }

        //
        [HttpPost]
        public JsonResult Save([FromBody] BaseSubScriptionModelView sub)
        {
            try
            {
                subScriptionService.CraeteOrUpdate(sub);
            }
            catch (Exception ex)
            {
                return Json(new { isOk = false, Message = "Вовремя сохранения  произошла ошибка." });
            }

            return Json(new { isOk = true });
        }

        [HttpPost]
        public JsonResult Remove(int id)
        {
            bool result = true;
            try
            {
                result = subScriptionService.Remove(id);
            }
            catch (Exception ex)
            {
                return Json(new { isOk = false, Message = "Вовремя удаления произошла ошибка." });
            }

            return Json(new { isOk = result });
        }

        [HttpPost]
        public JsonResult Recovery(int id)
        {
            bool result = true;
            try
            {
                result = subScriptionService.Recovery(id);
            }
            catch (Exception ex)
            {
                return Json(new { isOk = false, Message = "Вовремя восстановления произошла ошибка." });
            }

            return Json(new { isOk = result });
        }
    }
}
