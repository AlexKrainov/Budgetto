using LinqKit;
using Microsoft.EntityFrameworkCore;
using MyProfile.Budget.Service;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView.Limit;
using MyProfile.Entity.ModelView.Notification;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
using MyProfile.Notification.Service;
using MyProfile.User.Service;
using MyProfile.UserLog.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MyProfile.Limit.Service
{
    public partial class LimitService
    {
        private BaseRepository repository;
        private CollectionUserService collectionUserService;
        private BudgetRecordService budgetRecordService;
        private SectionService sectionService;
        private UserLogService userLogService;
        private NotificationService notificationService;

        public LimitService(BaseRepository repository,
            CollectionUserService collectionUserService,
            SectionService sectionService,
            BudgetRecordService budgetRecordService,
            NotificationService notificationService)
        {
            this.repository = repository;
            this.collectionUserService = collectionUserService;
            this.budgetRecordService = budgetRecordService;
            this.sectionService = sectionService;
            this.userLogService = new UserLogService(repository);
            this.notificationService = notificationService;
        }

        public async Task<LimitModelView> UpdateOrCreate(LimitModelView limit)
        {
            var currentUser = UserInfo.Current;
            limit.UserID = currentUser.ID;
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
                await userLogService.CreateUserLogAsync(currentUser.UserSessionID, UserLogActionType.Limit_Edit);
            }
            else
            {
                limit.CurrencyID = 1;

                limit.VisibleElement = new VisibleElement
                {
                    IsShowOnDashboards = limit.IsShowOnDashboard,
                    IsShowInCollective = limit.IsShowInCollective,
                };
                await repository.CreateAsync(limit, true);
                await userLogService.CreateUserLogAsync(currentUser.UserSessionID, UserLogActionType.Limit_Create);
            }

            if (limit.Notifications.Any())
            {
                try
                {
                    var anyChanges = await notificationService.CreateOrUpdate<Entity.Model.Limit>(limit.Notifications, limit.ID, currentUser.ID);

                    #region Force checke
                    if (anyChanges)
                    {
                        await CheckLimitNotificationsAsync(limit.ID);
                        await userLogService.CreateUserLogAsync(currentUser.UserSessionID, UserLogActionType.Limit_Notification);
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    await userLogService.CreateErrorLogAsync(currentUser.UserSessionID, "LimitService_UpdateOrCreate_Notifications", ex);
                }
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
                    IsFinished = limit.IsFinished,
                    IsOwner = limit.UserID == x.UserID,
                    UserName = x.User.Name + " " + x.User.LastName,
                    ImageLink = x.User.ImageLink,
                    Sections = x.SectionGroupLimits.Select(y => new Entity.ModelView.BudgetSectionModelView //.OrderBy(z => z.BudgetSection.BudgetRecords)
                    {
                        ID = y.BudgetSectionID,
                        Name = y.BudgetSection.Name,
                        CssColor = y.BudgetSection.CssColor,
                        CssBackground = y.BudgetSection.CssBackground,
                        CssIcon = y.BudgetSection.CssIcon,
                        IsShow_Filtered = true,
                        IsShow = true,
                        AreaName = y.BudgetSection.BudgetArea.Name,
                    }).ToList(),
                    Notifications = x.Notifications != null ?
                        x.Notifications.Select(y => new NotificationUserViewModel
                        {
                            ID = y.ID,
                            IsMail = y.IsMail,
                            IsSite = y.IsSite,
                            IsTelegram = y.IsTelegram,
                            Price = y.Total ?? 0
                        }).ToList() : null
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
            var currentUser = UserInfo.Current;
            var db_limit = await repository.GetAll<Entity.Model.Limit>(x => x.ID == limit.ID && x.UserID == currentUser.ID).FirstOrDefaultAsync();

            if (db_limit != null)
            {
                db_limit.IsDeleted = isRemove;
                //db_limit.date = DateTime.Now.ToUniversalTime();
                await repository.UpdateAsync(db_limit, true);
                await userLogService.CreateUserLogAsync(currentUser.UserSessionID, UserLogActionType.Limit_Delete);
                return true;
            }
            return false;
        }

        public async Task<List<LimitModelView>> GetLimitListView(Guid userID, Expression<Func<Entity.Model.Limit, bool>> expression = null)
        {
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
            predicate = predicate.And(x => userID == x.UserID && x.IsDeleted == false);
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
                    IsFinished = x.IsFinished,
                    IsOwner = userID == x.UserID,
                    UserName = x.User.Name + " " + x.User.LastName,
                    ImageLink = x.User.ImageLink,
                    Currency = new Currency
                    {
                        ID = x.Currency.ID,
                        CodeName = x.Currency.CodeName,
                        SpecificCulture = x.Currency.SpecificCulture,
                        Icon = x.Currency.Icon,
                    },
                    Sections = x.SectionGroupLimits.Select(y => new Entity.ModelView.BudgetSectionModelView
                    {
                        ID = y.BudgetSectionID,
                        Name = y.BudgetSection.Name,
                        CssColor = y.BudgetSection.CssColor,
                        CssBackground = y.BudgetSection.CssBackground,
                        CssIcon = y.BudgetSection.CssIcon,
                        IsShow_Filtered = true,
                        IsShow = true,
                        AreaName = y.BudgetSection.BudgetArea.Name,
                    }),
                    Notifications = x.Notifications != null ?
                        x.Notifications.Select(y => new NotificationUserViewModel
                        {
                            ID = y.ID,
                            IsMail = y.IsMail,
                            IsSite = y.IsSite,
                            IsTelegram = y.IsTelegram,
                            Price = y.Total ?? 0
                        }).ToList() : null
                })
                .ToListAsync();
        }

        public async Task<List<LimitChartModelView>> GetChartData(DateTime start, DateTime finish, PeriodTypesEnum periodTypesEnum)
        {
            List<LimitChartModelView> limitCharts = new List<LimitChartModelView>();
            var currentUser = UserInfo.Current;
            bool isShow = true;
            var now = DateTime.Now;
            int totalDays = 0;
            string text = "";
            bool isThis = false;
            bool isPast = start < now && finish < now;// month/year
            bool isFuture = start > now && finish > now;// month/year
            var limits = await GetLimitListView(currentUser.ID, x =>
                x.PeriodTypeID == (int)periodTypesEnum
                && x.VisibleElement.IsShowOnDashboards);

            if (periodTypesEnum == PeriodTypesEnum.Month)
            {
                isShow = currentUser.UserSettings.Month_LimitWidgets;
                totalDays = 1 + (finish - start).Days;

                isThis = finish.Month == now.Month && finish.Year == now.Year;// month/year
            }
            else if (periodTypesEnum == PeriodTypesEnum.Year)
            {
                isShow = currentUser.UserSettings.Year_LimitWidgets;
                totalDays = 12;

                isThis = finish.Year == now.Year;// month/year
            }

            for (int i = 0; i < limits.Count; i++)
            {
                var limit = limits[i];

                if (periodTypesEnum == PeriodTypesEnum.Month)
                {
                    if (isThis)
                    {
                        text = "Примерно осталось расходов в день:";
                    }
                    else if (isPast)
                    {
                        text = "Примерно было расходов в день:";
                    }
                    else
                    {
                        text = "Примерно возможных расходов в день:";
                    }
                }
                else if (periodTypesEnum == PeriodTypesEnum.Year)
                {
                    if (isThis)
                    {
                        text = "Примерно осталось расходов в месяц:";
                    }
                    else if (isPast)
                    {
                        text = "Примерно было расходов в месяц:";
                    }
                    else
                    {
                        text = "Примерно возможных расходов в месяц:";
                    }
                }

                var filter = new Entity.ModelView.CalendarFilterModels
                {
                    StartDate = start,
                    EndDate = finish,
                    Sections = limit.Sections.Select(x => x.ID).ToList(),
                    UserID = currentUser.ID
                };

                filter.IsConsiderCollection = currentUser.IsAllowCollectiveBudget && currentUser.UserSettings.BudgetPages_WithCollective;

                if (filter.IsConsiderCollection)
                {
                    filter.Sections.AddRange(await sectionService.GetCollectionSectionIDsBySectionID(filter.Sections));
                }

                var totalSpended = await budgetRecordService.GetTotalSpendsForLimitByFilter(filter);

                //leftMoneyInADay
                decimal leftMoneyInADay = decimal.Zero;
                decimal percent2 = decimal.Zero;

                var leftMoneyToSpend = limit.LimitMoney - totalSpended;

                if (isThis && leftMoneyToSpend < 0)
                {
                    text = "Вы превысили лимит на";
                    leftMoneyInADay = leftMoneyToSpend * -1;
                }
                else if (isThis)
                {
                    if (periodTypesEnum == PeriodTypesEnum.Month)
                    {
                        var leftDays = (finish - now).Days + 1;
                        leftMoneyInADay = leftMoneyToSpend / leftDays;
                    }
                    else if (periodTypesEnum == PeriodTypesEnum.Year)
                    {
                        leftMoneyInADay = leftMoneyToSpend / totalDays;
                    }
                }
                else if (isPast)
                {
                    leftMoneyInADay = totalSpended / totalDays;
                }
                else
                {
                    leftMoneyInADay = limit.LimitMoney / totalDays;
                }


                if (totalSpended >= 0)
                {
                    percent2 = Math.Round(leftMoneyToSpend / limit.LimitMoney * 100, 2);
                }

                limitCharts.Add(new LimitChartModelView
                {
                    ID = limit.ID,
                    ChartID = "limitChart_" + i,
                    Name = limit.Name,
                    SpendedMoney = totalSpended,
                    LimitMoney = limit.LimitMoney,
                    LeftMoneyInADay = leftMoneyInADay,
                    Percent2 = percent2 < 0 ? 0 : percent2,
                    Percent1 = 100 - percent2,
                    IsThis = isThis,
                    IsPast = isPast,
                    IsFuture = isFuture,
                    IsShow = isShow,
                    PeriodTypeID = (int)periodTypesEnum,
                    Text = text,
                    Currency = new Entity.ModelView.Currency.CurrencyClientModelView
                    {
                        id = limit.CurrencyID ?? 0,
                        codeName = limit.Currency.CodeName,
                        specificCulture = limit.Currency.SpecificCulture,
                        icon = limit.Currency.Icon,
                    },
                    Sections = limit.Sections.Select(x => new Entity.ModelView.AreaAndSection.SectionLightModelView
                    {
                        ID = x.ID,
                        Name = x.Name
                    })
                    .ToList()
                });
            }

            return limitCharts;
        }

        public async Task<bool> ToggleLimit(int limitID, PeriodTypesEnum periodType)
        {
            var currentUser = UserInfo.Current;
            var db_limit = await repository.GetAll<Entity.Model.Limit>(x => x.ID == limitID && x.UserID == currentUser.ID)
                .FirstOrDefaultAsync();

            if (db_limit != null)
            {
                db_limit.VisibleElement.IsShowOnDashboards = !db_limit.VisibleElement.IsShowOnDashboards;
                //db_limit.date = DateTime.Now.ToUniversalTime();
                await repository.UpdateAsync(db_limit, true);

                if (periodType == PeriodTypesEnum.Undefined)
                {//if toggle on the limit page
                    await userLogService.CreateUserLogAsync(currentUser.UserSessionID, UserLogActionType.Limit_Toggle);
                }
                else
                {//if hide on the budget page 
                    await userLogService.CreateUserLogAsync(currentUser.UserSessionID, UserLogActionType.BudgetPage_HideLimit);
                }
                return db_limit.VisibleElement.IsShowOnDashboards;
            }
            return false;
        }
    }
}
