using Common.Service;
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

namespace MyProfile.Budget.Service
{
    public class SummaryService
    {
        private IBaseRepository repository;
        private IMemoryCache cache;
        private UserLogService userLogService;
        private BudgetRecordService recordService;
        private CommonService commonService;

        public SummaryService(IBaseRepository repository,
            IMemoryCache cache,
            UserLogService userLogService,
            BudgetRecordService recordService,
            CommonService commonService)
        {
            this.repository = repository;
            this.cache = cache;
            this.userLogService = userLogService;
            this.recordService = recordService;
            this.commonService = commonService;
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
                var expensesPerDay = bdSummaries.FirstOrDefault(x => x.ID == (int)SummaryType.CashFlow);

                summaries.CashFlow = new CashFlowModelView
                {
                    ID = expensesPerDay.ID,
                    ElementID = expensesPerDay.CodeName,
                    IsShow = true,
                    Name = expensesPerDay.Name

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

        public bool SaveWorkhours(EarningsPerHourModelView userInfo)
        {
            var currentUser = UserInfo.Current;
            var now = DateTime.Now.ToUniversalTime();
            bool result = true;
            List<int> errorLogIDs = new List<int>();

            var oldUserSummary = repository.GetAll<UserSummary>(x => x.UserID == currentUser.ID
                && x.SummaryID == (int)SummaryType.EarningsPerHour
                && x.IsActive).FirstOrDefault();
            oldUserSummary.IsActive = false;

            UserSummary userSummary = new UserSummary
            {
                Name = oldUserSummary.Name,
                Value = userInfo.WorkHours.ToString(),
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
            userLogService.CreateUserLog(currentUser.UserSessionID, UserLogActionType.Summary_Edit_WorkHours, errorLogIDs: errorLogIDs);

            return result;
        }

        #region Build summaries types

        private void BuildEarningsPerHour(SummaryModelView summary, SummaryFilter filter)
        {
            var userSummary = repository.GetAll<UserSummary>(x => x.UserID == filter.UserID
                      && x.SummaryID == (int)SummaryType.EarningsPerHour
                      && x.IsActive
                      && x.Value != null)
                 .FirstOrDefault();

            if (userSummary != null && !string.IsNullOrEmpty(userSummary.Value) && int.TryParse(userSummary.Value, out int hours))
            {
                if (userSummary.Sections == null || userSummary.Sections.Count == 0)
                {
                    var _filter = new SummaryFilter
                    {
                        StartDate = filter.StartDate,
                        EndDate = filter.EndDate,
                        UserID = filter.UserID,
                        SectionTypes = new List<int> { (int)SectionTypeEnum.Earnings }
                    };

                    if (filter.PeriodType == PeriodTypesEnum.Year)
                    {
                        hours *= 12;
                    }

                    summary.TotalEarnings = recordService.GetTotalForSummaryByFilter(_filter);
                    summary.EarningsPerHour.WorkHours = hours;

                    if (summary.TotalEarnings != 0 && hours != 0)
                    {
                        summary.EarningsPerHour.Balance = (summary.TotalEarnings ?? 1) / hours;
                    }
                    else
                    {
                        summary.EarningsPerHour.Balance = 0;
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
            }
        }

        private void BuildAllAccountsMoney(SummaryModelView summary, SummaryFilter filter)
        {
            var now = DateTime.Now;
            var userCurrencyID = UserInfo.Current.CurrencyID;

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
                        x.Currency.CodeName_CBR
                    })
                    .ToList();

                for (int i = 0; i < accounts.Count; i++)
                {
                    try
                    {
                        if (userCurrencyID != accounts[i].CurrencyID)
                        {
                            var val = commonService.GetRatesFromBank(now, accounts[i].CodeName_CBR);

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
