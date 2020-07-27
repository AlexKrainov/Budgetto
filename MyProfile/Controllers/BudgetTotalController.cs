using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyProfile.Budget.Service;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView;
using MyProfile.Entity.ModelView.BudgetView;
using MyProfile.Entity.ModelView.TotalBudgetView;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
using MyProfile.Template.Service;

namespace MyProfile.Controllers
{
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
        public async Task<IActionResult> LoadByMonth(DateTime to)
        {
            var values = await budgetTotalService.GetDataByMonth(to);
            
            return Json(new { EarningData = values.Item1, SpendingData = values.Item2, InvestingData = values.Item3 });
        }

        [HttpGet]
        public async Task<IActionResult> LoadByYear(int year)
        {
            var values = await budgetTotalService.GetDataByYear(year);

            return Json(new { EarningData = values.Item1, SpendingData = values.Item2, InvestingData = values.Item3 });
        }
    }
}