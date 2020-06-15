using MyProfile.Entity.ModelView.Limit;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;
using MyProfile.Entity.Model;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using LinqKit;
using MyProfile.Budget.Service;

namespace MyProfile.Limit.Service
{
	public class LimitService
	{
		private IBaseRepository repository;
		private BudgetRecordService budgetRecordService;
		private SectionService sectionService;

		public LimitService(IBaseRepository repository)
		{
			this.repository = repository;
			this.budgetRecordService = new BudgetRecordService(repository);
			this.sectionService = new SectionService(repository);
		}

		public async Task<int> UpdateOrCreate(LimitModelView limit)
		{
			//var newLimit = new Entity.Model.Limit
			//{
			//	ID = limit.ID,
			//	DateEnd = limit.DateEnd,
			//	DateStart = limit.DateStart,
			//	Description = limit.Description,
			//	IsShow = limit.IsShow,
			//	LimitMoney = limit.LimitMoney,
			//	Name = limit.Name,
			//	PeriodTypeID = limit.PeriodTypeID,
			//	SectionGroupLimits = limit.NewSections.Distinct().Select(x => new SectionGroupLimit { BudgetSectionID = x.ID, LimitID = limit.ID }).ToList(),
			//	UserID = UserInfo.Current.ID,
			//};
			limit.UserID = UserInfo.Current.ID;
			limit.SectionGroupLimits = limit.NewSections.Distinct().Select(x => new SectionGroupLimit { BudgetSectionID = x.ID, LimitID = limit.ID }).ToList();
			

			if (limit.ID > 0)
			{
				repository.DeleteRange(
					await repository.GetAll<Entity.Model.Limit>(x => x.ID == limit.ID)
						.SelectMany(y => y.SectionGroupLimits)
						.ToListAsync(), true);
				await repository.UpdateAsync(limit, true);
			}
			else
			{
				await repository.CreateAsync(limit, true);
			}

			//limit.Sections = await repository.GetAll<Entity.Model.SectionGroupLimit>(x => x.LimitID == newLimit.ID)
			//	.Select(x => new Entity.ModelView.BudgetSectionModelView
			//	{
			//		ID = x.BudgetSectionID,
			//		Name = x.BudgetSection.Name,
			//		CssColor = x.BudgetSection.CssColor,
			//		CssIcon = x.BudgetSection.CssIcon
			//	})
			//	.ToListAsync(); 

			return limit.ID;
		}

		public async Task<List<LimitModelView>> GetLimitListView(Expression<Func<Entity.Model.Limit, bool>> expression = null)
		{
			var predicate = PredicateBuilder.True<Entity.Model.Limit>();
			predicate = predicate.And(x => x.UserID == UserInfo.Current.ID);

			if (expression != null) { predicate = predicate.And(expression); }

			return await repository.GetAll<Entity.Model.Limit>(predicate)
				.Select(x => new LimitModelView
				{
					DateEnd = x.DateEnd,
					DateStart = x.DateStart,
					ID = x.ID,
					Description = x.Description,
					LimitMoney = x.LimitMoney,
					Name = x.Name,
					IsShow = x.IsShow,
					PeriodName = x.PeriodType.Name,
					PeriodTypeID = x.PeriodTypeID,
					IsFinishLimit = x.DateEnd != null && x.DateEnd.Value < DateTime.Now ? true : false,
					Sections = x.SectionGroupLimits.Select(y => new Entity.ModelView.BudgetSectionModelView //.OrderBy(z => z.BudgetSection.BudgetRecords)
					{
						ID = y.BudgetSectionID,
						Name = y.BudgetSection.Name,
						CssColor = y.BudgetSection.CssColor,
						CssIcon = y.BudgetSection.CssIcon,
					}).ToList()
				})
				.ToListAsync();
		}

		public async Task<List<LimitChartModelView>> GetChartData(DateTime start, DateTime finish, PeriodTypesEnum periodTypesEnum)
		{
			List<LimitChartModelView> limitCharts = new List<LimitChartModelView>();

			var limits = await GetLimitListView(x =>
				x.PeriodTypeID == (int)periodTypesEnum
				&& ((x.DateEnd == null && x.DateStart <= start) || (x.DateStart <= start && x.DateEnd >= finish)));

			var currentUser = UserInfo.Current;

			for (int i = 0; i < limits.Count; i++)
			{
				var limit = limits[i];

				var filter = new Entity.ModelView.CalendarFilterModels
				{
					StartDate = start,
					EndDate = finish,
					Sections = limit.Sections.Select(x => x.ID).ToList()
				};
				filter.IsConsiderCollection = currentUser.IsAllowCollectiveBudget && currentUser.UserSettings.BudgetPages_WithCollective;
				if (filter.IsConsiderCollection)
				{
					filter.Sections.AddRange(await sectionService.GetCollectionSectionBySectionID(filter.Sections));
				}

				var totalSpended = await budgetRecordService.GetTotalSpendsForLimitByFilter(filter);

				//leftMoneyInADay
				decimal leftMoneyInADay = decimal.Zero;
				decimal percent2 = decimal.Zero;
				bool isThisMonth = finish.Month == DateTime.Now.Month && finish.Year == DateTime.Now.Year;

				var totalDays = (finish - start).Days;
				var leftDays = (finish - DateTime.Now).Days;
				var leftMoneyToSpend = limit.LimitMoney - totalSpended;

				if (leftMoneyToSpend > 0)
				{
					if (isThisMonth)
					{
						leftMoneyInADay = leftMoneyToSpend / leftDays;
					}
					else
					{
						leftMoneyInADay = totalSpended / totalDays;
					}
					if (totalSpended > 0)
					{
						percent2 = Math.Round(leftMoneyToSpend / limit.LimitMoney * 100, 2);
					}
				}
				else
				{
					leftMoneyInADay = totalSpended / totalDays;
				}

				limitCharts.Add(new LimitChartModelView
				{
					ChartID = "limitChart_" + i,
					Name = limit.Name,
					SpendedMoney = totalSpended,
					LimitMoney = limit.LimitMoney,
					LeftMoneyInADay = leftMoneyInADay,
					Percent2 = percent2,
					Percent1 = 100 - percent2,
					IsThisMonth = isThisMonth,
					IsShow = currentUser.UserSettings.BudgetPages_IsShow_Limits,
					//Sections
				});
			}

			return limitCharts;
		}
	}
}
