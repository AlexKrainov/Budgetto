using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyProfile.Budget.Service;
using MyProfile.Entity.Model;
using MyProfile.Entity.Repository;
using System;
using System.Threading.Tasks;

namespace MyProfile.Controllers
{
    [Authorize]
    public partial class BudgetTotalController : Controller
    {
        private IBaseRepository repository;
        private BudgetTotalService budgetTotalService;

        public BudgetTotalController(IBaseRepository repository,
            BudgetTotalService budgetTotalService)
        {
            this.repository = repository;
            this.budgetTotalService = budgetTotalService;
        }

        [HttpGet]
        public IActionResult LoadByMonth(DateTime to)
        {
            var from  = new DateTime(to.Year, to.Month, 01, 00, 00, 00);
            to = new DateTime(to.Year, to.Month, DateTime.DaysInMonth(to.Year, to.Month), 23, 59, 59);
            var values = budgetTotalService.GetDataByMonth(from, to);

            return Json(new { SpendingData = values.Item1, EarningData = values.Item2, InvestingData = values.Item3 });
        }

        [HttpGet]
        public IActionResult LoadByYear(int year)
        {
            var values = budgetTotalService.GetDataByYear(year);

            return Json(new { SpendingData = values.Item1, EarningData = values.Item2, InvestingData = values.Item3 });
        }


    }
}