using Common.Service;
using Microsoft.EntityFrameworkCore;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView;
using MyProfile.Entity.ModelView.AreaAndSection;
using MyProfile.Entity.ModelView.BudgetView;
using MyProfile.Entity.ModelView.Chart;
using MyProfile.Entity.ModelView.Tag;
using MyProfile.Entity.ModelView.TotalBudgetView;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProfile.Budget.Service
{
    public class BudgetTotalService
    {
        private IBaseRepository repository;
        private BudgetRecordService budgetRecordService;
        private SectionService sectionService;
        private CurrencyService currencyService;

        private Dictionary<DateTime, List<CurrencyRateHistory>> rates;

        public int[] InvestAccountTypes = new int[]
        {
            (int)AccountTypes.Deposit,
            (int)AccountTypes.Investments,
            (int)AccountTypes.InvestmentsIIS,
        };

        public BudgetTotalService(IBaseRepository repository,
            SectionService sectionService,
            BudgetRecordService budgetRecordService,
            CurrencyService currencyService)
        {
            this.repository = repository;
            this.budgetRecordService = budgetRecordService;
            this.sectionService = sectionService;
            this.currencyService = currencyService;

            rates = new Dictionary<DateTime, List<CurrencyRateHistory>>();
        }

        public Tuple<TotalModelView, TotalModelView, TotalModelView> GetDataByYear(int year, bool isStatisticChart = false)
        {
            DateTime from = new DateTime(year, 1, 1, 0, 0, 0);
            DateTime to = new DateTime(year, 12, 31, 23, 59, 59);
            var currentUser = UserInfo.Current;
            var sections = sectionService.GetAllSectionForRecords();

            TotalModelView spendingData = new TotalModelView
            {
                IsShow = isStatisticChart || currentUser.UserSettings.Year_SpendingWidget,
                ChartID = "spendingChart",
                BackgroundColor = "rgba(217, 83, 79, .2)",
                BorderColor = "rgba(217, 83, 79, 1)",
                Title = "Динамика расходов за выбранный год",
                IsSelected = true
            };
            TotalModelView earningData = new TotalModelView
            {
                IsShow = isStatisticChart || currentUser.UserSettings.Year_EarningWidget,
                ChartID = "earningChart",
                BackgroundColor = "rgba(2, 188, 119, .2)",
                BorderColor = "rgba(2, 188, 119, 1)",
                Title = "Динамика доходов за выбранный год",
                IsSelected = true
            };
            TotalModelView investingData = new TotalModelView
            {
                IsShow = isStatisticChart || currentUser.UserSettings.Year_InvestingWidget,
                ChartID = "investmentsChart",
                BackgroundColor = "rgba(136, 151, 170, .2)",
                BorderColor = "rgba(136, 151, 170, 1)",
                Title = "Динамика пополений инвестиционных счетов за выбранный год",
                IsSelected = false
            };

            if (isStatisticChart || currentUser.UserSettings.Year_SpendingWidget)
            {
                var tuple = GetChartTotalByMonth(from, to, SectionTypeEnum.Spendings);

                var thisYear = tuple.Item1.Sum();
                var lastYear = GetChartTotalByMonth(from.AddYears(-1), to.AddYears(-1), SectionTypeEnum.Spendings).Item1.Sum();

                spendingData.data = tuple.Item1.ToArray();
                spendingData.labels = tuple.Item2.ToArray();
                spendingData.Name = "Расходы";
                spendingData.SectionTypeEnum = SectionTypeEnum.Spendings;
                spendingData.Total = (thisYear).ToString("C0", CultureInfo.CreateSpecificCulture(currentUser.Currency.SpecificCulture));
                spendingData.Sections = sections
                    .Where(x => x.SectionTypeID == (int)SectionTypeEnum.Spendings)
                    .Select(x => new Entity.ModelView.AreaAndSection.SectionLightModelView
                    {
                        ID = x.ID,
                        Name = x.Name
                    })
                    .ToList();


                //if (!(DateTime.Now.Year == to.Year && DateTime.Now.Month == to.Month))
                //{
                if (lastYear == decimal.Zero)
                {
                    spendingData.Percent = 100;
                }
                else
                {
                    spendingData.Percent = Math.Round(((thisYear - lastYear) / lastYear * 100), 1);
                }
                spendingData.IsGood = spendingData.Percent < 0;

                if (spendingData.Percent < 0)
                {
                    spendingData.Percent *= -1;
                }
                //}
            }

            if (isStatisticChart || currentUser.UserSettings.Year_EarningWidget)
            {
                var tuple = GetChartTotalByMonth(from, to, SectionTypeEnum.Earnings);

                var thisYear = tuple.Item1.Sum();
                var lastYear = GetChartTotalByMonth(from.AddYears(-1), to.AddYears(-1), SectionTypeEnum.Earnings).Item1.Sum();

                earningData.data = tuple.Item1.ToArray();
                earningData.labels = tuple.Item2.ToArray();
                earningData.Name = "Доходы";
                earningData.SectionTypeEnum = SectionTypeEnum.Earnings;
                earningData.Total = thisYear.ToString("C0", CultureInfo.CreateSpecificCulture(currentUser.Currency.SpecificCulture));
                earningData.Sections = sections
                   .Where(x => x.SectionTypeID == (int)SectionTypeEnum.Earnings)
                   .Select(x => new Entity.ModelView.AreaAndSection.SectionLightModelView
                   {
                       ID = x.ID,
                       Name = x.Name
                   })
                   .ToList();

                // if (!(DateTime.Now.Year == to.Year && DateTime.Now.Month == to.Month))
                // {
                if (lastYear == decimal.Zero)
                {
                    earningData.Percent = 100;
                }
                else
                {
                    earningData.Percent = Math.Round(((thisYear - lastYear) / lastYear * 100), 1);
                }
                earningData.IsGood = earningData.Percent > 0;

                if (earningData.Percent < 0)
                {
                    earningData.Percent *= -1;
                }
                // }
            }

            if (isStatisticChart || currentUser.UserSettings.Year_InvestingWidget)
            {
                var accountHistories = repository.GetAll<AccountHistory>(x => x.Account.UserID == currentUser.ID
                    && (InvestAccountTypes.Contains(x.Account.AccountTypeID)
                            || InvestAccountTypes.Contains(x.AccountFrom.AccountTypeID))
                        && x.Account.IsDeleted == false
                        && x.ActionType == AccountHistoryActionType.MoveMoney
                        && x.CurrentDate >= from && x.CurrentDate <= to)
                    .Select(x => new AccountHistoryTMP
                    {
                        ValueFrom = x.ValueFrom,
                        ValueTo = x.ValueTo,
                        CurrencyToCodeName = x.Account.Currency.CodeName,
                        CurrencyFromCodeName = x.AccountFrom.Currency.CodeName,
                        AccountTypeTo = (AccountTypes)x.Account.AccountTypeID,
                        AccountTypeFrom = (AccountTypes)x.AccountFrom.AccountTypeID,
                        CurrentDate = x.CurrentDate,
                        CurrencyValue = x.CurrencyValue,
                    })
                .ToList();

                var tuple = GetChartInvestingTotalByMonth(from, to);

                investingData.data = tuple.Item1.ToArray();
                investingData.labels = tuple.Item2.ToArray();

                decimal total = CountTotal(accountHistories);

                investingData.Name = "Инвестиции";
                //investingData.SectionTypeEnum = SectionTypeEnum.Investments;
                investingData.Total = (total).ToString("C0", CultureInfo.CreateSpecificCulture(currentUser.Currency.SpecificCulture));

                if (!(DateTime.Now.Year == to.Year && DateTime.Now.Month == to.Month))
                {
                    if (investingData.data[10] == decimal.Zero)
                    {
                        investingData.Percent = 100;
                    }
                    else
                    {
                        investingData.Percent = Math.Round(((investingData.data[11] - investingData.data[10]) / investingData.data[10] * 100), 1);
                    }
                    investingData.IsGood = investingData.Percent > 0;

                    if (investingData.Percent < 0)
                    {
                        investingData.Percent *= -1;
                    }
                }

                //only for title
                investingData.Sections = repository.GetAll<Account>(x => x.UserID == currentUser.ID
                     && x.Bank.BankTypeID == (int)BankTypes.Broker
                     && x.IsDeleted == false)
                        .Select(x => new SectionLightModelView
                        {
                            Name = x.Name + " (" + x.Bank.Name + ")"
                        })
                    .ToList();
            }

            return new Tuple<TotalModelView, TotalModelView, TotalModelView>(spendingData, earningData, investingData);
        }

        public void GetTotalBySections(DateTime start, DateTime finish, List<UniversalChartSectionViewModel> sections, List<UniversalChartTagViewModel> tags)
        {
            var groupBySections = budgetRecordService.GetBudgetRecordsGroup(start, finish, x => x.SectionID).ToList();
            var temp = budgetRecordService.GetBudgetRecordsGroup(start, finish, x => x.SectionTypeID).ToList();
            var totalEarning = temp.Count == 0 ? 0 : temp.FirstOrDefault(x => x.Key == (int)SectionTypeEnum.Earnings).Sum(x => x.Total);
            var totalSpending = temp.Count == 0 ? 0 : temp.FirstOrDefault(x => x.Key == (int)SectionTypeEnum.Spendings).Sum(x => x.Total);

            for (int i = 0; i < groupBySections.Count; i++)
            {
                var section = sections.FirstOrDefault(x => x.ID == groupBySections[i].Key);
                section.Total = groupBySections[i].Sum(x => x.Total);

                if (section.Total != 0)
                {
                    if (section.SectionTypeID == (int)SectionTypeEnum.Earnings)
                    {
                        section.Percent = Math.Round(section.Total / totalEarning * 100, 2);
                    }
                    else
                    {
                        section.Percent = Math.Round(section.Total / totalSpending * 100, 2);
                    }


                    section.Tags = repository.GetAll<Record>(x => x.UserID == UserInfo.Current.ID
                            && x.DateTimeOfPayment >= start && x.DateTimeOfPayment <= finish
                            && x.IsDeleted != true
                            && x.BudgetSectionID == section.ID
                            && x.Tags.Count > 0)
                        .SelectMany(x => x.Tags)
                        .Select(x => new
                        {
                            x.UserTagID,
                            x.Record.Total
                        })
                        .GroupBy(x => x.UserTagID)
                        .Select(x => new
                        {
                            UserTagID = x.Key,
                            Total = x.Sum(y => y.Total)
                        })
                        .Join(tags, x => x.UserTagID, y => y.ID, (x, y) => new UniversalChartTagViewModel
                        {
                            CompanyID = y.CompanyID,
                            CompanyLogo = y.CompanyLogo,
                            CompanyName = y.CompanyName,
                            Title = y.Title,
                            ID = x.UserTagID,
                            SpendingSum = x.Total,
                        })
                        .OrderByDescending(x => x.SpendingSum)
                        .ToList();
                }
            }
        }

        public List<UniversalChartTagViewModel> GetTotalByTags(DateTime start, DateTime finish, List<UniversalChartTagViewModel> tags)
        {
            tags = repository.GetAll<MyProfile.Entity.Model.RecordTag>(x => x.Record.UserID == UserInfo.Current.ID
                && x.Record.DateTimeOfPayment >= start && x.Record.DateTimeOfPayment <= finish
                && x.Record.IsDeleted != true
                && x.UserTag.IsDeleted != true)
                 .Select(x => new
                 {
                     x.UserTagID,
                     x.Record.Total,
                     x.Record.BudgetSection.SectionTypeID,
                 })
                 .GroupBy(x => x.UserTagID)
                 .Select(x => new TmpTagModelView
                 {
                     UserTagID = x.Key,
                     EarningSum = x.Where(y => y.SectionTypeID == (int)SectionTypeEnum.Earnings).Sum(y => y.Total),
                     SpendingSum = x.Where(y => y.SectionTypeID == (int)SectionTypeEnum.Spendings).Sum(y => y.Total),
                 })
                 .Join(tags, x => x.UserTagID, y => y.ID, (x, y) => new UniversalChartTagViewModel
                 {
                     CompanyID = y.CompanyID,
                     CompanyLogo = y.CompanyLogo,
                     CompanyName = y.CompanyName,
                     Title = y.Title,
                     ID = x.UserTagID,
                     EarningSum = x.EarningSum,
                     SpendingSum = x.SpendingSum
                 })
                 .ToList();

            if (tags.Count > 0)
            {
                var earningsTags = new List<UniversalChartTagViewModel>();
                var temp = budgetRecordService.GetBudgetRecordsGroup(start, finish, x => x.SectionTypeID).ToList();
                var totalEarning = temp.Count == 0 ? 0 : temp.FirstOrDefault(x => x.Key == (int)SectionTypeEnum.Earnings).Sum(x => x.Total);
                var totalSpending = temp.Count == 0 ? 0 : temp.FirstOrDefault(x => x.Key == (int)SectionTypeEnum.Spendings).Sum(x => x.Total);

                foreach (var tag in tags)
                {
                    if (tag.IsEarning)
                    {
                        earningsTags.Add(new UniversalChartTagViewModel
                        {
                            CompanyLogo = tag.CompanyLogo,
                            CompanyID = tag.CompanyID,
                            CompanyName = tag.CompanyName,
                            ID = tag.ID,
                            Title = tag.Title,
                            EarningSum = tag.EarningSum,
                            Percent = Math.Round(tag.EarningSum / totalEarning * 100, 2),
                        });
                    }

                    if (tag.IsSpending)
                    {
                        tag.Percent = Math.Round(tag.SpendingSum / totalSpending * 100, 2);
                    }
                }

                tags = tags
                    .Union(earningsTags)
                    .ToList();
            }

            return tags;
        }


        public Tuple<TotalModelView, TotalModelView, TotalModelView> GetDataByMonth(DateTime to, bool isStatisticChart = false)
        {
            DateTime from = to.AddMonths(-11);
            to = new DateTime(to.Year, to.Month, DateTime.DaysInMonth(to.Year, to.Month), 23, 59, 59);
            var currentUser = UserInfo.Current;
            var sections = sectionService.GetAllSectionForRecords();

            TotalModelView spendingData = new TotalModelView
            {
                IsShow = isStatisticChart || currentUser.UserSettings.Month_SpendingWidget,
                ChartID = "spendingChart",
                BackgroundColor = "rgba(217, 83, 79, .2)",
                BorderColor = "rgba(217, 83, 79, 1)",
                Title = "Динамика расходов за последний 12 месяцев",
                IsSelected = true
            };
            TotalModelView earningData = new TotalModelView
            {
                IsShow = isStatisticChart || currentUser.UserSettings.Month_EarningWidget,
                ChartID = "earningChart",
                BackgroundColor = "rgba(2, 188, 119, .2)",
                BorderColor = "rgba(2, 188, 119, 1)",
                Title = "Динамика доходов за последний 12 месяцев",
                IsSelected = true
            };
            TotalModelView investingData = new TotalModelView
            {
                IsShow = isStatisticChart || currentUser.UserSettings.Month_InvestingWidget,
                ChartID = "investmentsChart",
                BackgroundColor = "rgba(136, 151, 170, .2)",
                BorderColor = "rgba(136, 151, 170, 1)",
                Title = "Динамика пополений инвестиционных счетов за последний 12 месяцев",
                IsSelected = false
            };

            if (isStatisticChart || currentUser.UserSettings.Month_SpendingWidget)
            {
                var tuple = GetChartTotalByMonth(from, to, SectionTypeEnum.Spendings);
                spendingData.data = tuple.Item1.ToArray();
                spendingData.labels = tuple.Item2.ToArray();
                spendingData.Name = "Расходы";
                spendingData.SectionTypeEnum = SectionTypeEnum.Spendings;
                spendingData.Total = (tuple.Item1[tuple.Item1.Count - 1]).ToString("C0", CultureInfo.CreateSpecificCulture(currentUser.Currency.SpecificCulture));
                spendingData.Sections = sections
                   .Where(x => x.SectionTypeID == (int)SectionTypeEnum.Spendings)
                   .Select(x => new Entity.ModelView.AreaAndSection.SectionLightModelView
                   {
                       ID = x.ID,
                       Name = x.Name
                   })
                   .ToList();

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
            }

            if (isStatisticChart || currentUser.UserSettings.Month_EarningWidget)
            {
                var tuple = GetChartTotalByMonth(from, to, SectionTypeEnum.Earnings);

                earningData.data = tuple.Item1.ToArray();
                earningData.labels = tuple.Item2.ToArray();
                earningData.Name = "Доходы";
                earningData.SectionTypeEnum = SectionTypeEnum.Earnings;
                earningData.Total = (tuple.Item1[tuple.Item1.Count - 1]).ToString("C0", CultureInfo.CreateSpecificCulture(currentUser.Currency.SpecificCulture));
                earningData.Sections = sections
                   .Where(x => x.SectionTypeID == (int)SectionTypeEnum.Earnings)
                   .Select(x => new Entity.ModelView.AreaAndSection.SectionLightModelView
                   {
                       ID = x.ID,
                       Name = x.Name
                   })
                   .ToList();

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
            }

            if (isStatisticChart || currentUser.UserSettings.Month_InvestingWidget)
            {
                var from2 = to.AddMonths(-1);

                //var accountHistories = repository.GetAll<AccountHistory>(x => x.Account.UserID == currentUser.ID
                //        && (InvestAccountTypes.Contains(x.Account.AccountTypeID)
                //            || InvestAccountTypes.Contains(x.AccountFrom.AccountTypeID))
                //        && x.Account.IsDeleted == false
                //        && x.ActionType == AccountHistoryActionType.MoveMoney
                //        && x.CurrentDate >= from2 && x.CurrentDate <= to)
                //    .Select(x => new AccountHistoryTMP
                //    {
                //        ValueFrom = x.ValueFrom,
                //        ValueTo = x.ValueTo,
                //        CurrencyToCodeName = x.Account.Currency.CodeName,
                //        CurrencyFromCodeName = x.AccountFrom.Currency.CodeName,
                //        AccountTypeTo = (AccountTypes)x.Account.AccountTypeID,
                //        AccountTypeFrom = (AccountTypes)x.AccountFrom.AccountTypeID,
                //        CurrentDate = x.CurrentDate,
                //        CurrencyValue = x.CurrencyValue,
                //    })
                //    .ToList();

                var tuple = GetChartInvestingTotalByMonth(from, to);

                investingData.data = tuple.Item1.ToArray();
                investingData.labels = tuple.Item2.ToArray();

                decimal total = GetTotalInvestByPeriod(from2, to);// (accountHistories);

                investingData.Name = "Инвестиции";
                //investingData.SectionTypeEnum = SectionTypeEnum.Investments;
                investingData.Total = (total).ToString("C0", CultureInfo.CreateSpecificCulture(currentUser.Currency.SpecificCulture));

                if (!(DateTime.Now.Year == to.Year && DateTime.Now.Month == to.Month))
                {
                    if (investingData.data[10] == decimal.Zero)
                    {
                        investingData.Percent = 100;
                    }
                    else
                    {
                        investingData.Percent = Math.Round(((investingData.data[11] - investingData.data[10]) / investingData.data[10] * 100), 1);
                    }
                    investingData.IsGood = investingData.Percent > 0;

                    if (investingData.Percent < 0)
                    {
                        investingData.Percent *= -1;
                    }
                }

                //only for title
                investingData.Sections = repository.GetAll<Account>(x => x.UserID == currentUser.ID
                     && InvestAccountTypes.Contains(x.AccountTypeID)
                     && x.IsDeleted == false)
                        .Select(x => new SectionLightModelView
                        {
                            Name = x.Name
                        })
                    .ToList();
            }

            return new Tuple<TotalModelView, TotalModelView, TotalModelView>(spendingData, earningData, investingData);
        }

        public decimal GetTotalInvestByPeriod(DateTime from, DateTime to)
        {
            var accountHistories = repository.GetAll<AccountHistory>(x => x.Account.UserID == UserInfo.Current.ID
                       && (InvestAccountTypes.Contains(x.Account.AccountTypeID)
                           || InvestAccountTypes.Contains(x.AccountFrom.AccountTypeID))
                       && x.Account.IsDeleted == false
                       && x.ActionType == AccountHistoryActionType.MoveMoney
                       && x.CurrentDate >= from && x.CurrentDate <= to)
                   .Select(x => new AccountHistoryTMP
                   {
                       ValueFrom = x.ValueFrom,
                       ValueTo = x.ValueTo,
                       CurrencyToCodeName = x.Account.Currency.CodeName,
                       CurrencyFromCodeName = x.AccountFrom.Currency.CodeName,
                       AccountTypeTo = (AccountTypes)x.Account.AccountTypeID,
                       AccountTypeFrom = (AccountTypes)x.AccountFrom.AccountTypeID,
                       CurrentDate = x.CurrentDate,
                       CurrencyValue = x.CurrencyValue,
                   })
                   .ToList();

            return CountTotal(accountHistories);
        }

        private decimal CountTotal(List<AccountHistoryTMP> accountHistories)
        {
            decimal input = 0,
                output = 0;
            var currentUser = UserInfo.Current;

            foreach (var history in accountHistories)
            {
                if (InvestAccountTypes.Contains((int)history.AccountTypeTo)) //input
                {
                    if (history.CurrencyToCodeName == "RUB")
                    {
                        input += history.ValueTo ?? 0;
                    }
                    else if (history.CurrencyFromCodeName == "RUB")
                    {
                        input += history.ValueFrom ?? 0;
                    }
                    else
                    {
                        List<CurrencyRateHistory> currencyRates = getRate(history.CurrentDate, currentUser.UserSessionID);

                        input += (history.ValueTo ?? 0) * currencyRates.FirstOrDefault(x => x.CharCode == history.CurrencyToCodeName).Rate;
                    }
                }
                else if (InvestAccountTypes.Contains((int)history.AccountTypeFrom))//output
                {
                    if (history.CurrencyFromCodeName == "RUB")
                    {
                        output += history.ValueFrom ?? 0;
                    }
                    else if (history.CurrencyToCodeName == "RUB")
                    {
                        output += history.ValueTo ?? 0;
                    }
                    else
                    {
                        List<CurrencyRateHistory> currencyRates = getRate(history.CurrentDate, currentUser.UserSessionID);

                        output += (history.ValueFrom ?? 0) * currencyRates.FirstOrDefault(x => x.CharCode == history.CurrencyFromCodeName).Rate;
                    }
                }
            }
            return input - output;
        }

        private List<CurrencyRateHistory> getRate(DateTime currentDate, Guid userSessionID)
        {
            if (rates.ContainsKey(currentDate))
            {
                return rates[currentDate];
            }

            var rate = currencyService.GetRatesByDate(currentDate, userSessionID);
            rates.Add(currentDate, rate);

            return rate;
        }

        public Tuple<List<decimal>, List<string>> GetChartInvestingTotalByMonth(DateTime from, DateTime to)
        {
            var accountHistories = repository.GetAll<AccountHistory>(x => x.Account.UserID == UserInfo.Current.ID
                   && (InvestAccountTypes.Contains(x.Account.AccountTypeID)
                        || InvestAccountTypes.Contains(x.AccountFrom.AccountTypeID))
                   && x.Account.IsDeleted == false
                   && x.ActionType == AccountHistoryActionType.MoveMoney
                  && x.CurrentDate >= from && x.CurrentDate <= to)
              .Select(x => new AccountHistoryTMP
              {
                  ValueFrom = x.ValueFrom,
                  ValueTo = x.ValueTo,
                  CurrencyToCodeName = x.Account.Currency.CodeName,
                  CurrencyFromCodeName = x.AccountFrom.Currency.CodeName,
                  AccountTypeTo = (AccountTypes)x.Account.AccountTypeID,
                  AccountTypeFrom = (AccountTypes)x.AccountFrom.AccountTypeID,
                  CurrentDate = x.CurrentDate,
                  CurrencyValue = x.CurrencyValue,
              })
              .GroupBy(x => x.CurrentDate.Month)
              .ToList();

            List<decimal> datas = new List<decimal>();
            List<string> labels = new List<string>();

            while (to > from)
            {
                labels.Add(from.Month + " " + from.Year);

                //decimal input = accountHistories.Where(x => x.Key == from.Month).SelectMany(x => x).Where(x => x.Actions == "input").Sum(x => x.ValueTo) ?? 0;
                //decimal output = accountHistories.Where(x => x.Key == from.Month).SelectMany(x => x).Where(x => x.Actions == "output").Sum(x => x.ValueFrom) ?? 0;
                var total = CountTotal(accountHistories.Where(x => x.Key == from.Month).SelectMany(x => x).ToList());
                datas.Add(total);// input - output);
                from = from.AddMonths(1);
            }

            return new Tuple<List<decimal>, List<string>>(datas, labels);
        }

        public Tuple<List<decimal>, List<string>> GetChartTotalByMonth(DateTime from, DateTime to, SectionTypeEnum sectionTypeEnum)
        {
            var budgetRecordsGroup = budgetRecordService
                .GetBudgetRecordsGroup(
                    from,
                    to,
                    x => x.DateTimeOfPayment.Month,
                    x => x.BudgetSection.SectionTypeID == (int)sectionTypeEnum)
                .ToList();

            List<decimal> datas = new List<decimal>();
            List<string> labels = new List<string>();

            while (to > from)
            {
                labels.Add(from.Month + " " + from.Year);
                datas.Add(budgetRecordsGroup.Where(x => x.Key == from.Month).Sum(y => y.Sum(z => z.Total)));
                from = from.AddMonths(1);
            }

            return new Tuple<List<decimal>, List<string>>(datas, labels);
        }
    }

    public class AccountHistoryTMP
    {
        public decimal? ValueFrom { get; internal set; }
        public decimal? ValueTo { get; internal set; }
        public string CurrencyToCodeName { get; internal set; }
        public string CurrencyFromCodeName { get; internal set; }
        public DateTime CurrentDate { get; internal set; }
        public decimal? CurrencyValue { get; internal set; }
        public AccountTypes AccountTypeTo { get; internal set; }
        public AccountTypes AccountTypeFrom { get; internal set; }
    }
}
