using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyProfile.Entity.ModelView.Payment;
using MyProfile.Payment.Service;

namespace MyProfile.Controllers
{
    public class StoreController : Controller
    {
        private PaymentService paymentService;

        public StoreController(PaymentService paymentService)
        {
            this.paymentService = paymentService;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> PayStart(PaymentViewModel model)
        {
            var paymentHistoryID = await paymentService.CreatePaymentHistory(model);
            return Json(new { isOk = true, paymentHistoryID });
        }
        [HttpGet]
        public async Task<IActionResult> PayFinish(Guid paymentID)
        {
            var _paymentID = await paymentService.Paid(paymentID);
            return Json(new { isOk = _paymentID > 0, paymentID = _paymentID });
        }


    }
}
