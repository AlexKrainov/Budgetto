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

namespace MyProfile.Controllers.My
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

		//TotalModelView
		[HttpGet]
		public async Task<IActionResult> Load(DateTime to)
		{
			DateTime from = to.AddMonths(-11);
			to = new DateTime(to.Year, to.Month, DateTime.DaysInMonth(to.Year, to.Month));

			var tuple = budgetTotalService.GetChartTotalByMonth(from, to, SectionTypeEnum.Spendings);
			TotalModelView spendingData = new TotalModelView
			{
				data = tuple.Item1.ToArray(),
				labels = tuple.Item2.ToArray(),
				Name = "Расходы",
				SectionTypeEnum = SectionTypeEnum.Spendings,
				Total = (tuple.Item1[tuple.Item1.Count - 1]).ToString("C", CultureInfo.CreateSpecificCulture("ru_RU")),
				IsShow = UserInfo.Current.UserSettings.BudgetPages_SpendingChart
			};

			if (!(DateTime.Now.Year == to.Year && DateTime.Now.Month == to.Month))
			{
				if (spendingData.data[10] == decimal.Zero)
				{
					spendingData.Percent = 100;
				}
				else
				{
					spendingData.Percent = Math.Round(((spendingData.data[11] - spendingData.data[10]) / spendingData.data[10] * 100), 1);
				}
				spendingData.IsGood = spendingData.Percent < 0;

				if (spendingData.Percent < 0)
				{
					spendingData.Percent *= -1;
				}
			}

			tuple = budgetTotalService.GetChartTotalByMonth(from, to, SectionTypeEnum.Earnings);
			TotalModelView earningData = new TotalModelView
			{
				data = tuple.Item1.ToArray(),
				labels = tuple.Item2.ToArray(),
				Name = "Доходы",
				SectionTypeEnum = SectionTypeEnum.Spendings,
				Total = (tuple.Item1[tuple.Item1.Count - 1]).ToString("C", CultureInfo.CreateSpecificCulture("ru_RU")),
				IsShow = UserInfo.Current.UserSettings.BudgetPages_EarningChart
			};

			if (!(DateTime.Now.Year == to.Year && DateTime.Now.Month == to.Month))
			{
				if (earningData.data[10] == decimal.Zero)
				{
					earningData.Percent = 100;
				}
				else
				{
					earningData.Percent = Math.Round(((earningData.data[11] - earningData.data[10]) / earningData.data[10] * 100), 1);
				}
				earningData.IsGood = earningData.Percent > 0;

				if (earningData.Percent < 0)
				{
					earningData.Percent *= -1;
				}
			}

			TotalModelView investinData = new TotalModelView
			{
				Name = "Инвестиции",
				IsShow = UserInfo.Current.UserSettings.BudgetPages_InvestingChart
			};

			return Json(new { EarningData = earningData, SpendingData = spendingData, InvestingData =  investinData});
		}
	}
}