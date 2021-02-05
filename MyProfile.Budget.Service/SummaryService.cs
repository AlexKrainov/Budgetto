using Common.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView;
using MyProfile.Entity.ModelView.Account;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
using MyProfile.UserLog.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProfile.Budget.Service
{
    public class SummaryService
    {
        private IBaseRepository repository;
        private IMemoryCache cache;
        private UserLogService userLogService;
        private BudgetRecordService recordService;
        private CurrencyService сurrencyService;
        private BudgetTotalService budgetTotalService;

        public SummaryService(IBaseRepository repository,
            IMemoryCache cache,
            UserLogService userLogService,
            BudgetRecordService recordService,
            BudgetTotalService budgetTotalService,
            CurrencyService сurrencyService)
        {
            this.repository = repository;
            this.cache = cache;
            this.userLogService = userLogService;
            this.recordService = recordService;
            this.сurrencyService = сurrencyService;
            this.budgetTotalService = budgetTotalService;
        }

        public SummaryModelView GetSummaries(SummaryFilter filter)
        {
            var currentUser = UserInfo.Current;
            var summaries = new SummaryModelView
            {
                CurrencyCodeName = currentUser.Currency.CodeName,
                CurrencySpecificCulture = currentUser.Currency.SpecificCulture,
                IsShow = currentUser.UserSettings.Month_Summary,
            };
            filter.UserID = currentUser.ID;


            var bdSummaries = repository.GetAll<Summary>(x => x.IsActive).ToList();

            if (bdSummaries.Any(x => x.ID == (int)SummaryType.EarningsPerHour))
            {
                var earningsPerHour = bdSummaries.FirstOrDefault(x => x.ID == (int)SummaryType.EarningsPerHour);
                summaries.EarningsPerHour = new EarningsPerHourModelView
                {
                    ID = earningsPerHour.ID,
                    ElementID = earningsPerHour.CodeName,
                    IsShow = true,
                    Name = earningsPerHour.Name
                };
                BuildEarningsPerHour(summaries, filter);
            }

            if (bdSummaries.Any(x => x.ID == (int)SummaryType.ExpensesPerDay))
            {
                var expensesPerDay = bdSummaries.FirstOrDefault(x => x.ID == (int)SummaryType.ExpensesPerDay);

                summaries.ExpensesPerDay = new ExpensesPerDayModelView
                {
                    ID = expensesPerDay.ID,
                    ElementID = expensesPerDay.CodeName,
                    IsShow = true,
                    Name = expensesPerDay.Name
                };
                BuildExpensesPerDay(summaries, filter);
            }

            if (bdSummaries.Any(x => x.ID == (int)SummaryType.CashFlow))
            {
                var chashFlow = bdSummaries.FirstOrDefault(x => x.ID == (int)SummaryType.CashFlow);

                summaries.CashFlow = new CashFlowModelView
                {
                    ID = chashFlow.ID,
                    ElementID = chashFlow.CodeName,
                    IsShow = true,
                    IsChart = chashFlow.IsChart,
                    Name = chashFlow.Name
                };
                BuildCashFlow(summaries, filter);
            }

            if (bdSummaries.Any(x => x.ID == (int)SummaryType.AllAccountsMoney))
            {
                var expensesPerDay = bdSummaries.FirstOrDefault(x => x.ID == (int)SummaryType.AllAccountsMoney);

                summaries.AllAccountsMoney = new AllAccountsMoneyModelView
                {
                    ID = expensesPerDay.ID,
                    ElementID = expensesPerDay.CodeName,
                    IsShow = true,
                    Name = expensesPerDay.Name

                };
                BuildAllAccountsMoney(summaries, filter);
            }

            //ExpensesPerDay

            return summaries;
        }

        public bool SetWorkHours(int workHours)
        {
            var currentUser = UserInfo.Current;
            var now = DateTime.Now.ToUniversalTime();
            bool result = true;
            List<int> errorLogIDs = new List<int>();

            var oldUserSummary = repository.GetAll<UserSummary>(x => x.UserID == currentUser.ID
                && x.SummaryID == (int)SummaryType.EarningsPerHour
                && x.IsActive)
                .FirstOrDefault();
            if (oldUserSummary != null)
            {
                oldUserSummary.IsActive = false;
            }

            UserSummary userSummary = new UserSummary
            {
                Name = "Рабочих часов в месяц",
                Value = workHours.ToString(),
                CurrentDate = now,
                IsActive = true,
                UserID = currentUser.ID,
                SummaryID = (int)SummaryType.EarningsPerHour,
                VisibleElement = new VisibleElement
                {
                    IsShow_BudgetMonth = true,
                    IsShow_BudgetYear = true,
                }
            };

            try
            {
                repository.Create(userSummary, true);
                result = true;
            }
            catch (Exception ex)
            {
                errorLogIDs.Add(userLogService.CreateErrorLog(currentUser.UserSessionID, "SumaryService_SaveWorkhours", ex));
                result = false;
            }
            userLogService.CreateUserLog(currentUser.UserSessionID, UserLogActionType.Summary_Set_WorkHours, errorLogIDs: errorLogIDs);

            return result;
        }

        public async Task<bool> SetWorkHoursAsync(int workHours)
        {
            var currentUser = UserInfo.Current;
            var now = DateTime.Now.ToUniversalTime();
            bool result = true;
            List<int> errorLogIDs = new List<int>();

            var oldUserSummary = await repository.GetAll<UserSummary>(x => x.UserID == currentUser.ID
                && x.SummaryID == (int)SummaryType.EarningsPerHour
                && x.IsActive)
                .FirstOrDefaultAsync();

            if (oldUserSummary != null)
            {
                oldUserSummary.IsActive = false;
            }

            UserSummary userSummary = new UserSummary
            {
                Name = oldUserSummary.Name,
                Value = workHours.ToString(),
                CurrentDate = now,
                IsActive = true,
                UserID = currentUser.ID,
                SummaryID = (int)SummaryType.EarningsPerHour,
                VisibleElement = new VisibleElement
                {
                    IsShow_BudgetMonth = true,
                    IsShow_BudgetYear = true,
                }
            };

            try
            {
                await repository.CreateAsync(userSummary, true);
                result = true;
            }
            catch (Exception ex)
            {
                errorLogIDs.Add(await userLogService.CreateErrorLogAsync(currentUser.UserSessionID, "SumaryService_SetWorkhours", ex));
                result = false;
            }
            await userLogService.CreateUserLogAsync(currentUser.UserSessionID, UserLogActionType.Summary_Set_WorkHours, errorLogIDs: errorLogIDs);

            return result;
        }

        #region Build summaries types

        private void BuildEarningsPerHour(SummaryModelView summary, SummaryFilter filter)
        {
            var now = DateTime.Now;
            var userSummary = repository.GetAll<UserSummary>(x => x.UserID == filter.UserID
                      && x.SummaryID == (int)SummaryType.EarningsPerHour
                      && x.IsActive
                      && x.Value != null)
                 .FirstOrDefault();

            if (userSummary != null && !string.IsNullOrEmpty(userSummary.Value) && int.TryParse(userSummary.Value, out int hours))
            {
                if (userSummary.Sections == null || userSummary.Sections.Count == 0)
                {
                    bool isPast = false;
                    var _filter = new SummaryFilter
                    {
                        StartDate = filter.StartDate,
                        EndDate = filter.EndDate,
                        UserID = filter.UserID,
                        SectionTypes = new List<int> { (int)SectionTypeEnum.Earnings }
                    };

                    summary.EarningsPerHour.AllWorkHours = hours;

                    if (filter.PeriodType == PeriodTypesEnum.Year)
                    {
                        hours *= 12;
                    }

                    summary.TotalEarnings = recordService.GetTotalForSummaryByFilter(_filter);
                    summary.EarningsPerHour.AllWorkHoursByPeriod = hours;
                    int totalDays = 0;


                    if (now > filter.StartDate && now <= filter.EndDate)
                    {
                        totalDays = (int)(now.Date - filter.StartDate.Date).TotalDays;
                    }
                    else if (now > filter.EndDate && now > filter.StartDate)
                    {
                        totalDays = (int)(filter.EndDate.Date - filter.StartDate.Date).TotalDays;
                        isPast = true;
                    }
                    else if (now < filter.EndDate && now < filter.StartDate)
                    {
                        totalDays = 0;
                    }

                    if (totalDays != 0 && hours != 0)
                    {
                        decimal workhoursPerDay = summary.EarningsPerHour.AllWorkHours / 30.4m;

                        if (isPast)
                        {
                            summary.EarningsPerHour.WorkedHours = summary.EarningsPerHour.AllWorkHoursByPeriod;
                        }
                        else
                        {
                            summary.EarningsPerHour.WorkedHours = (int)Math.Round(workhoursPerDay * totalDays, 1);
                        }

                    }

                    if (summary.TotalEarnings != 0 && summary.EarningsPerHour.WorkedHours != 0)
                    {
                        summary.EarningsPerHour.Balance = (summary.TotalEarnings ?? 1) / summary.EarningsPerHour.WorkedHours;
                        summary.EarningsPerHour.BalancePerDay = summary.EarningsPerHour.Balance * 24;
                    }
                    else
                    {
                        summary.EarningsPerHour.Balance = 0;
                        summary.EarningsPerHour.BalancePerDay = 0;
                    }
                }
                else
                {
                    //ToDo: by sections
                }
            }
        }

        private void BuildExpensesPerDay(SummaryModelView summary, SummaryFilter filter)
        {
            var now = DateTime.Now;
            var userSummary = repository.GetAll<UserSummary>(x => x.UserID == filter.UserID
                      && x.SummaryID == (int)SummaryType.ExpensesPerDay
                      && x.IsActive)
                 .FirstOrDefault();

            if (userSummary != null)
            {
                //ToDo: if user has his own summary
            }
            else
            {
                var _filter = new SummaryFilter
                {
                    StartDate = filter.StartDate,
                    EndDate = filter.EndDate,
                    UserID = filter.UserID,
                    SectionTypes = new List<int> { (int)SectionTypeEnum.Spendings }
                };

                summary.TotalSpendings = recordService.GetTotalForSummaryByFilter(_filter);
                if (now > filter.StartDate && now <= filter.EndDate)
                {
                    summary.ExpensesPerDay.TotalDays = (int)(now.Date - filter.StartDate.Date).TotalDays;
                }
                else if (now > filter.EndDate && now > filter.StartDate)
                {
                    summary.ExpensesPerDay.TotalDays = (int)(filter.EndDate.Date - filter.StartDate.Date).TotalDays;
                }
                else if (now < filter.EndDate && now < filter.StartDate)
                {
                    summary.ExpensesPerDay.TotalDays = 0;
                }

                if (summary.TotalSpendings != 0 && summary.ExpensesPerDay.TotalDays != 0)
                {
                    summary.ExpensesPerDay.Balance = (summary.TotalSpendings ?? 1) / summary.ExpensesPerDay.TotalDays;
                }
                else
                {
                    summary.ExpensesPerDay.Balance = 0;
                }
            }
        }

        private void BuildCashFlow(SummaryModelView summary, SummaryFilter filter)
        {
            var now = DateTime.Now;
            var userSummary = repository.GetAll<UserSummary>(x => x.UserID == filter.UserID
                      && x.SummaryID == (int)SummaryType.CashFlow
                      && x.IsActive)
                 .FirstOrDefault();

            if (userSummary != null)
            {
                //ToDo: if user has his own summary
            }
            else
            {
                if (summary.TotalEarnings == null || summary.TotalSpendings == null)
                {
                    var _filter = new SummaryFilter
                    {
                        StartDate = filter.StartDate,
                        EndDate = filter.EndDate,
                        UserID = filter.UserID,
                        SectionTypes = new List<int> { (int)SectionTypeEnum.Spendings }
                    };

                    summary.TotalSpendings = recordService.GetTotalForSummaryByFilter(_filter);

                    _filter.SectionTypes = new List<int> { (int)SectionTypeEnum.Earnings };
                    summary.TotalEarnings = recordService.GetTotalForSummaryByFilter(_filter);
                }

                summary.CashFlow.Balance = (summary.TotalEarnings ?? 0) - (summary.TotalSpendings ?? 0);

                if (summary.CashFlow.IsChart)
                {
                    //if (filter.PeriodType == PeriodTypesEnum.Month)
                    var spending = budgetTotalService.GetChartTotalByMonth(filter.StartDate.AddMonths(-11), filter.EndDate, SectionTypeEnum.Spendings);
                    var earnings = budgetTotalService.GetChartTotalByMonth(filter.StartDate.AddMonths(-11), filter.EndDate, SectionTypeEnum.Earnings);

                    summary.CashFlow.data = new decimal[spending.Item1.Count];
                    summary.CashFlow.labels = new string[spending.Item2.Count];

                    for (int i = 0; i < spending.Item1.Count; i++)
                    {
                        summary.CashFlow.data[i] = earnings.Item1[i] - spending.Item1[i];
                        summary.CashFlow.labels[i] = spending.Item2[i];
                    }

                }
            }
        }

        private void BuildAllAccountsMoney(SummaryModelView summary, SummaryFilter filter)
        {
            var now = DateTime.Now;
            var currentUser = UserInfo.Current; ;

            var userSummary = repository.GetAll<UserSummary>(x => x.UserID == filter.UserID
                      && x.SummaryID == (int)SummaryType.AllAccountsMoney
                      && x.IsActive)
                 .FirstOrDefault();

            if (userSummary != null)
            {
                //ToDo: if user has his own summary
            }
            else
            {
                var accounts = repository.GetAll<Account>(x => x.UserID == filter.UserID && x.IsDeleted != true)
                    .Select(x => new
                    {
                        x.Balance,
                        x.CurrencyID,
                        x.Currency.CodeName
                    })
                    .ToList();

                for (int i = 0; i < accounts.Count; i++)
                {
                    try
                    {
                        if (currentUser.CurrencyID != accounts[i].CurrencyID)
                        {
                            var val = сurrencyService.GetRateByCode(now, accounts[i].CodeName, currentUser.UserSessionID);

                            if (val != null && val.Rate != 0)
                            {
                                summary.AllAccountsMoney.Balance += accounts[i].Balance * val.Rate;
                            }
                            else
                            {
                                summary.AllAccountsMoney.ConvertError = true;
                            }
                        }
                        else
                        {
                            summary.AllAccountsMoney.Balance += accounts[i].Balance;
                        }
                    }
                    catch (Exception ex)
                    {
                        summary.AllAccountsMoney.ConvertError = true;
                    }
                }

            }
        }

        #endregion
    }
}
