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
            var values = budgetTotalService.GetDataByMonth(to);

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