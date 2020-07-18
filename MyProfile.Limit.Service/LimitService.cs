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
            if (limit.PeriodTypeID == (int)PeriodTypesEnum.Year)
            {
                try
                {
                    limit.DateStart = new DateTime(int.Parse(limit.YearStart), 01, 01, 0, 0, 1);
                }
                catch (Exception)
                {
                    limit.DateStart = new DateTime(DateTime.Now.Year, 01, 01);
                }

                try
                {
                    if (!string.IsNullOrEmpty(limit.YearEnd))
                    {
                        limit.DateEnd = new DateTime(int.Parse(limit.YearEnd), 12, 31, 23, 59, 59);
                    }
                }
                catch (Exception)
                {
                    // limit.DateEnd = new DateTime(DateTime.Now.Year, 01, 01);
                }
            }

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

            return await repository.GetAll<MyProfile.Entity.Model.Limit>(x => x.ID == limit.ID)
                .Select(x => new LimitModelView
                {
                    ID = limit.ID,
                    DateEnd = limit.DateEnd,
                    DateStart = limit.DateStart,
                    Description = limit.Description,
                    LimitMoney = limit.LimitMoney,
                    Name = limit.Name,
                    IsShow = limit.IsShow,
                    PeriodName = x.PeriodType.Name,
                    PeriodTypeID = x.PeriodTypeID,
                    IsShowInCollective = x.IsShowInCollective,
                    IsFinishLimit = limit.DateEnd != null && limit.DateEnd.Value < DateTime.Now ? true : false,
                    YearEnd = limit.DateEnd != null ? limit.DateEnd.Value.Year.ToString() : null,
                    YearStart = limit.DateStart != null ? limit.DateStart.Value.Year.ToString() : null,
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

            if (currentUser.UserSettings.LimitPage_IsShow_Collective)
            {
                var userIDs = await collectionUserService.GetAllCollectiveUserIDsAsync();

                predicate = predicate.And(x => userIDs.Contains(x.UserID)
                    && (x.UserID != currentUser.ID ? x.User.IsAllowCollectiveBudget : true)
                    && (x.UserID != currentUser.ID ? x.IsShowInCollective : true)
                    && (currentUser.UserSettings.LimitPage_Show_IsFinished ? true : x.DateEnd == null || x.DateEnd != null && x.DateEnd.Value > DateTime.Now)
                    && x.IsDeleted == false);
            }
            else
            {
                predicate = predicate.And(x => currentUser.ID == x.UserID
                && (currentUser.UserSettings.LimitPage_Show_IsFinished ? true : x.DateEnd == null || x.DateEnd != null && x.DateEnd.Value > DateTime.Now)
                && x.IsDeleted == false);
            }

            if (expression != null) { predicate = predicate.And(expression); }

            return await repository.GetAll(predicate)
                .Select(x => new LimitModelView
                {
                    DateEnd = x.DateEnd,
                    DateStart = x.DateStart,
                    YearEnd = x.DateEnd != null ? x.DateEnd.Value.Year.ToString() : null,
                    YearStart = x.DateStart != null ? x.DateStart.Value.Year.ToString() : null,
                    ID = x.ID,
                    Description = x.Description,
                    LimitMoney = x.LimitMoney,
                    Name = x.Name,
                    IsShow = x.IsShow,
                    PeriodName = x.PeriodType.Name,
                    PeriodTypeID = x.PeriodTypeID,
                    IsShowInCollective = x.IsShowInCollective,
                    IsFinishLimit = x.DateEnd != null && x.DateEnd.Value < DateTime.Now ? true : false,
                    IsOwner = currentUser.ID == x.UserID,
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
