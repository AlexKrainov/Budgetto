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
            var predicate = PredicateBuilder.True<Goal>();

            if (currentUser.UserSettings.GoalPage_IsShow_Collective)
            {
                var userIDs = await collectionUserService.GetAllCollectiveUserIDsAsync();
                predicate = predicate.And(x => userIDs.Contains(x.UserID)
                    && (x.UserID != currentUser.ID ? x.User.IsAllowCollectiveBudget : true)
                    && (x.UserID != currentUser.ID ? x.VisibleElement.IsShowInCollective : true)
                    && (currentUser.UserSettings.GoalPage_IsShow_Finished ? true : x.IsFinished == false)
                    && x.IsDeleted == false);
            }
            else
            {
                predicate = predicate.And(x => x.UserID == currentUser.ID
                  && (currentUser.UserSettings.GoalPage_IsShow_Finished ? true : x.IsFinished == false)
                  && x.IsDeleted == false);
            }

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
                    IsShowInCollective = x.VisibleElement.IsShowInCollective,
                    IsShow_BudgetMonth = x.VisibleElement.IsShow_BudgetMonth,
                    IsShow_BudgetYear = x.VisibleElement.IsShow_BudgetYear,
                    Name = x.Name,
                    ExpectationMoney = x.ExpectationMoney,
                    IsOwner = currentUser.ID == x.UserID,
                    UserName = x.User.Name + " " + x.User.LastName,
                    ImageLink = x.User.ImageLink,
                    Records = x.GoalRecords.Select(y => new RecordItem
                    {
                        ID = y.ID,
                        CreateDateTime = y.CreateDateTime,
                        DateTimeOfPayment = y.DateTimeOfPayment,
                        UserName = y.User.Name + " " + y.User.LastName,
                        IsOwner = y.UserID == currentUser.ID,
                        ImageLink = y.User.ImageLink,
                        Total = y.Total,
                    })
                    .OrderByDescending(h => h.DateTimeOfPayment)
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

        public async Task<GoalModelView> UpdateOrCreate(GoalModelView goal)
        {
            goal.UserID = UserInfo.Current.ID;

            if (goal.ID == 0)
            {
                goal.VisibleElement = new VisibleElement
                {
                    IsShow_BudgetMonth = goal.IsShow_BudgetMonth,
                    IsShow_BudgetYear = goal.IsShow_BudgetYear,
                };

                await repository.CreateAsync(goal, true);
            }
            else
            {
                var dbGoal = await repository.GetAll<Goal>(x => x.ID == goal.ID).FirstOrDefaultAsync();
                dbGoal.DateEnd = goal.DateEnd;
                dbGoal.DateStart = goal.DateStart;
                dbGoal.Description = goal.Description;
                dbGoal.ExpectationMoney = goal.ExpectationMoney;
                dbGoal.IsFinished = goal.IsFinished;
                dbGoal.Name = goal.Name;
                dbGoal.VisibleElement.IsShow_BudgetMonth = goal.IsShow_BudgetMonth;
                dbGoal.VisibleElement.IsShow_BudgetYear = goal.IsShow_BudgetYear;

                await repository.UpdateAsync(dbGoal, true);
            }

            return await repository.GetAll<Goal>(x => x.ID == goal.ID)
                .Select(x => new GoalModelView
                {
                    OwenerName = x.User.Name,
                    ID = x.ID,
                    DateEnd = x.DateEnd,
                    DateStart = x.DateStart,
                    Description = x.Description,
                    IsFinished = x.IsFinished,
                    IsShowInCollective = x.VisibleElement.IsShowInCollective,
                    IsShow_BudgetMonth = x.VisibleElement.IsShow_BudgetMonth,
                    IsShow_BudgetYear = x.VisibleElement.IsShow_BudgetYear,
                    Name = x.Name,
                    ExpectationMoney = x.ExpectationMoney,
                    IsOwner = true,
                    UserName = x.User.Name + " " + x.User.LastName,
                    ImageLink = x.User.ImageLink,
                    Records = x.GoalRecords.Select(y => new RecordItem
                    {
                        ID = y.ID,
                        CreateDateTime = y.CreateDateTime,
                        DateTimeOfPayment = y.DateTimeOfPayment,
                        UserName = y.User.Name + " " + y.User.LastName,
                        IsOwner = y.UserID == goal.UserID,
                        ImageLink = y.User.ImageLink,
                        Total = y.Total,
                    })
                    .OrderByDescending(h => h.DateTimeOfPayment)
                    .ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<List<GoalModelView>> GetChartData(DateTime start, DateTime finish, PeriodTypesEnum periodTypesEnum)
        {
            var currentUser = UserInfo.Current;
            List<GoalModelView> goals = new List<GoalModelView>();
            if (periodTypesEnum == PeriodTypesEnum.Month)
            {
                goals = await GetGoalListView(x => x.VisibleElement.IsShow_BudgetMonth);
            }
            else if (periodTypesEnum == PeriodTypesEnum.Year)
            {
                goals = await GetGoalListView(x => x.VisibleElement.IsShow_BudgetYear);
            }
            //(x.DateEnd == null && x.DateStart <= start) || (x.DateStart <= start && x.DateEnd >= finish));

            for (int i = 0; i < goals.Count; i++)
            {
                goals[i].IsShow = currentUser.UserSettings.BudgetPages_IsShow_Goals;
            }

            return goals;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="goal"></param>
        /// <param name="isRemove">== true - remove, == false - recovery</param>
        /// <returns></returns>
        public async Task<bool> RemoveOrRecovery(GoalModelView goal, bool isRemove)
        {
            var db_item = await repository.GetAll<Entity.Model.Goal>(x => x.ID == goal.ID && x.UserID == UserInfo.Current.ID).FirstOrDefaultAsync();

            if (db_item != null)
            {
                db_item.IsDeleted = isRemove;
                //db_item. = DateTime.Now.ToUniversalTime();
                await repository.UpdateAsync(db_item, true);
                return true;
            }
            return false;
        }
    }
}
