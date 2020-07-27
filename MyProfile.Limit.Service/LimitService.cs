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
using MyProfile.User.Service;

namespace MyProfile.Limit.Service
{
    public class LimitService
    {
        private IBaseRepository repository;
        private CollectionUserService collectionUserService;
        private BudgetRecordService budgetRecordService;
        private SectionService sectionService;

        public LimitService(IBaseRepository repository,
            CollectionUserService collectionUserService)
        {
            this.repository = repository;
            this.collectionUserService = collectionUserService;
            this.budgetRecordService = new BudgetRecordService(repository);
            this.sectionService = new SectionService(repository);
        }

        public async Task<LimitModelView> UpdateOrCreate(LimitModelView limit)
        {

            limit.UserID = UserInfo.Current.ID;
            limit.SectionGroupLimits = limit.NewSections.Distinct().Select(x => new SectionGroupLimit { BudgetSectionID = x.ID, LimitID = limit.ID }).ToList();

            if (limit.ID > 0)
            {
                var dbLimit = await repository.GetAll<Entity.Model.Limit>(x => x.ID == limit.ID).FirstOrDefaultAsync();
                repository.DeleteRange(dbLimit.SectionGroupLimits, true);

                dbLimit.Name = limit.Name;
                dbLimit.Description = limit.Description;
                dbLimit.LimitMoney = limit.LimitMoney;
                dbLimit.PeriodTypeID = limit.PeriodTypeID;
                dbLimit.SectionGroupLimits = limit.SectionGroupLimits;
                dbLimit.VisibleElement.IsShowInCollective = limit.IsShowInCollective;
                dbLimit.VisibleElement.IsShowOnDashboards = limit.IsShowOnDashboard;

                await repository.UpdateAsync(dbLimit, true);
            }
            else
            {
                limit.VisibleElement = new VisibleElement
                {
                    IsShowOnDashboards = limit.IsShowOnDashboard,
                    IsShowInCollective = limit.IsShowInCollective,
                };
                await repository.CreateAsync(limit, true);
            }

            return await repository.GetAll<MyProfile.Entity.Model.Limit>(x => x.ID == limit.ID)
                .Select(x => new LimitModelView
                {
                    ID = limit.ID,
                    Description = limit.Description,
                    LimitMoney = limit.LimitMoney,
                    Name = limit.Name,
                    IsShowOnDashboard = limit.IsShowOnDashboard,
                    IsShowInCollective = limit.IsShowInCollective,
                    PeriodName = x.PeriodType.Name,
                    PeriodTypeID = x.PeriodTypeID,
                    IsFinishLimit = limit.IsFinished,
                    IsOwner = limit.UserID == x.UserID,
                    UserName = x.User.Name + " " + x.User.LastName,
                    ImageLink = x.User.ImageLink,
                    Sections = x.SectionGroupLimits.Select(y => new Entity.ModelView.BudgetSectionModelView //.OrderBy(z => z.BudgetSection.BudgetRecords)
                    {
                        ID = y.BudgetSectionID,
                        Name = y.BudgetSection.Name,
                        CssColor = y.BudgetSection.CssColor,
                        CssIcon = y.BudgetSection.CssIcon,
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="isRemove">== true - remove, == false - recovery</param>
        /// <returns></returns>
        public async Task<bool> RemoveOrRecovery(LimitModelView limit, bool isRemove)
        {
            var db_limit = await repository.GetAll<Entity.Model.Limit>(x => x.ID == limit.ID && x.UserID == UserInfo.Current.ID).FirstOrDefaultAsync();

            if (db_limit != null)
            {
                db_limit.IsDeleted = isRemove;
                //db_limit.date = DateTime.Now.ToUniversalTime();
                await repository.UpdateAsync(db_limit, true);
                return true;
            }
            return false;
        }

        public async Task<List<LimitModelView>> GetLimitListView(Expression<Func<Entity.Model.Limit, bool>> expression = null)
        {
            var currentUser = UserInfo.Current;
            var predicate = PredicateBuilder.True<Entity.Model.Limit>();

            //if (currentUser.UserSettings.LimitPage_IsShow_Collective)
            //{
            //    var userIDs = await collectionUserService.GetAllCollectiveUserIDsAsync();

            //    predicate = predicate.And(x => userIDs.Contains(x.UserID)
            //        && (x.UserID != currentUser.ID ? x.User.IsAllowCollectiveBudget : true)
            //        && (x.UserID != currentUser.ID ? x.VisibleElement.IsShowInCollective : true)
            //        && (currentUser.UserSettings.LimitPage_Show_IsFinished ? true : x.DateEnd == null || x.DateEnd != null && x.DateEnd.Value > DateTime.Now)
            //        && x.IsDeleted == false);
            //}
            //else
            //{
            predicate = predicate.And(x => currentUser.ID == x.UserID
            && x.IsDeleted == false);
            //}

            if (expression != null) { predicate = predicate.And(expression); }

            return await repository.GetAll(predicate)
                .Select(x => new LimitModelView
                {
                    ID = x.ID,
                    Description = x.Description,
                    LimitMoney = x.LimitMoney,
                    Name = x.Name,
                    IsShowOnDashboard = x.VisibleElement.IsShowOnDashboards,
                    PeriodName = x.PeriodType.Name,
                    PeriodTypeID = x.PeriodTypeID,
                    IsShowInCollective = x.VisibleElement.IsShowInCollective,
                    IsFinishLimit = x.IsFinished,
                    IsOwner = currentUser.ID == x.UserID,
                    UserName = x.User.Name + " " + x.User.LastName,
                    ImageLink = x.User.ImageLink,
                    Sections = x.SectionGroupLimits.Select(y => new Entity.ModelView.BudgetSectionModelView
                    {
                        ID = y.BudgetSectionID,
                        Name = y.BudgetSection.Name,
                        CssColor = y.BudgetSection.CssColor,
                        CssIcon = y.BudgetSection.CssIcon,
                    })
                })
                .ToListAsync();
        }

        public async Task<List<LimitChartModelView>> GetChartData(DateTime start, DateTime finish, PeriodTypesEnum periodTypesEnum)
        {
            List<LimitChartModelView> limitCharts = new List<LimitChartModelView>();
            var currentUser = UserInfo.Current;

            var limits = await GetLimitListView(x =>
                x.PeriodTypeID == (int)periodTypesEnum
                && x.VisibleElement.IsShowOnDashboards);


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

                var totalDays = 1 + (finish - start).Days;
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
                    IsShow = currentUser.UserSettings.Month_LimitWidgets,
                    //Sections
                });
            }

            return limitCharts;
        }
    }
}
