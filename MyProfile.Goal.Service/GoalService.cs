using LinqKit;
using Microsoft.EntityFrameworkCore;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView.Goal;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
using MyProfile.User.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MyProfile.Goal.Service
{
	using Goal = Entity.Model.Goal;
	public class GoalService
	{
		private IBaseRepository repository;
		private CollectionUserService collectionUserService;

		public GoalService(IBaseRepository repository,
			CollectionUserService collectionUserService)
		{
			this.repository = repository;
			this.collectionUserService = collectionUserService;
		}

		public async Task<List<GoalModelView>> GetGoalListView(Expression<Func<Goal, bool>> expression = null)
		{
			var currentUser = UserInfo.Current;
			var userIDs = await collectionUserService.GetAllCollectiveUserIDsAsync();
			var predicate = PredicateBuilder.True<Goal>();

			predicate = predicate.And(x => userIDs.Contains(x.UserID)
				&& (x.UserID != currentUser.ID ? x.User.IsAllowCollectiveBudget : true)
				&& (x.UserID != currentUser.ID ? x.IsShowInCollective : true)
				&& (currentUser.UserSettings.GoalPage_IsShow_Finished ? true : x.IsFinished == false)
				&& x.IsDeleted == false);

			if (expression != null) { predicate = predicate.And(expression); }

			var goals = await repository.GetAll<Goal>(predicate)
				.Select(x => new GoalModelView
				{
					OwenerName = x.User.Name,
					ID = x.ID,
					DateEnd = x.DateEnd,
					DateStart = x.DateStart,
					Description = x.Description,
					IsFinished = x.IsFinished,
					IsShowInCollective = x.IsShowInCollective,
					IsShowOnDashBoard = x.IsShowOnDashBoard,
					Name = x.Name,
					ExpectationMoney = x.ExpectationMoney,
					Records = x.GoalRecords.Select(y => new RecordItem
					{
						ID = y.ID,
						CreateDateTime = y.CreateDateTime,
						DateTimeOfPayment = y.DateTimeOfPayment,
						OwenerName = y.User.Name,
						Total = y.Total,
					}).ToList()
				})
				.ToListAsync();

			for (int i = 0; i < goals.Count; i++)
			{
				var goal = goals[i];
				goal.TotalMoney = goal.Records.Sum(x => x.Total);
				var leftMoney = goal.ExpectationMoney - goal.TotalMoney;
				goal.ChartID = "chart_" + (75 + i);

				goal.Percent = 100 - Math.Round((leftMoney / goal.ExpectationMoney * 100) ?? 0, 2);
				goal.Percent2 = 100 + goal.Percent;
			}

			return goals;
		}

		public async Task<int> AddRecord(RecordItem record)
		{
			GoalRecord goalRecord = new GoalRecord
			{
				CreateDateTime = DateTime.Now.ToUniversalTime(),
				DateTimeOfPayment = record.DateTimeOfPayment,
				GoalID = record.GoalID,
				Total = record.Total,
				UserID = UserInfo.Current.ID
			};
			return await repository.CreateAsync(goalRecord, true);
		}

		public async Task<int> UpdateOrCreate(GoalModelView goal)
		{
			goal.UserID = UserInfo.Current.ID;

			if (goal.ID == 0)
			{
				await repository.CreateAsync(goal, true);
			}
			else
			{
				await repository.UpdateAsync(goal, true);
			}

			return goal.ID;
		}

		public async Task<List<GoalModelView>> GetChartData(DateTime start, DateTime finish)
		{
			var currentUser = UserInfo.Current;
			var goals = await GetGoalListView(x => x.IsShowOnDashBoard);
			//(x.DateEnd == null && x.DateStart <= start) || (x.DateStart <= start && x.DateEnd >= finish));

			for (int i = 0; i < goals.Count; i++)
			{
				goals[i].IsShow = currentUser.UserSettings.BudgetPages_IsShow_Goals;
			}

			return goals;
		}
	}
}
