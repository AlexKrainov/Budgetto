using Microsoft.EntityFrameworkCore;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView;
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

        public BudgetTotalService(IBaseRepository repository)
        {
            this.repository = repository;
            this.budgetRecordService = new BudgetRecordService(repository);
        }

        public Tuple<TotalModelView, TotalModelView, TotalModelView> GetDataByYear(int year)
        {
            DateTime from = new DateTime(year, 1, 1, 0, 0, 0);
            DateTime to = new DateTime(year, 12, 31, 23, 59, 59);
            var currentUser = UserInfo.Current;

            TotalModelView spendingData = new TotalModelView { IsShow = currentUser.UserSettings.Year_SpendingWidget };
            TotalModelView earningData = new TotalModelView { IsShow = currentUser.UserSettings.Year_EarningWidget };
            TotalModelView investinData = new TotalModelView { IsShow = currentUser.UserSettings.Year_InvestingWidget };

            if (currentUser.UserSettings.Year_SpendingWidget)
            {
                var tuple = GetChartTotalByMonth(from, to, SectionTypeEnum.Spendings);

                var thisYear = tuple.Item1.Sum();
                var lastYear = GetChartTotalByMonth(from.AddYears(-1), to.AddYears(-1), SectionTypeEnum.Spendings).Item1.Sum();

                spendingData.data = tuple.Item1.ToArray();
                spendingData.labels = tuple.Item2.ToArray();
                spendingData.Name = "Расходы";
                spendingData.SectionTypeEnum = SectionTypeEnum.Spendings;
                spendingData.Total = (thisYear).ToString("C0", CultureInfo.CreateSpecificCulture(currentUser.Currency.SpecificCulture));


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

            if (currentUser.UserSettings.Year_EarningWidget)
            {
                var tuple = GetChartTotalByMonth(from, to, SectionTypeEnum.Earnings);

                var thisYear = tuple.Item1.Sum();
                var lastYear = GetChartTotalByMonth(from.AddYears(-1), to.AddYears(-1), SectionTypeEnum.Earnings).Item1.Sum();

                earningData.data = tuple.Item1.ToArray();
                earningData.labels = tuple.Item2.ToArray();
                earningData.Name = "Доходы";
                earningData.SectionTypeEnum = SectionTypeEnum.Spendings;
                earningData.Total = thisYear.ToString("C0", CultureInfo.CreateSpecificCulture(currentUser.Currency.SpecificCulture));

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

            if (currentUser.UserSettings.Year_InvestingWidget)
            {
                var tuple = GetChartTotalByMonth(from, to, SectionTypeEnum.Investments);

                var thisYear = tuple.Item1.Sum();
                var lastYear = GetChartTotalByMonth(from.AddYears(-1), to.AddYears(-1), SectionTypeEnum.Investments).Item1.Sum();

                investinData.data = tuple.Item1.ToArray();
                investinData.labels = tuple.Item2.ToArray();
                investinData.Name = "Инвестиции";
                investinData.SectionTypeEnum = SectionTypeEnum.Investments;
                investinData.Total = (thisYear).ToString("C0", CultureInfo.CreateSpecificCulture(currentUser.Currency.SpecificCulture));

                //if (!(DateTime.Now.Year == to.Year && DateTime.Now.Month == to.Month))
                //{
                if (lastYear == decimal.Zero)
                {
                    investinData.Percent = 100;
                }
                else
                {
                    investinData.Percent = Math.Round(((thisYear - lastYear) / lastYear * 100), 1);
                }
                investinData.IsGood = investinData.Percent > 0;

                if (investinData.Percent < 0)
                {
                    investinData.Percent *= -1;
                }
                // }
            }

            return new Tuple<TotalModelView, TotalModelView, TotalModelView>(spendingData, earningData, investinData);
        }

        public Tuple<TotalModelView, TotalModelView, TotalModelView> GetDataByMonth(DateTime to)
        {
            DateTime from = to.AddMonths(-11);
            to = new DateTime(to.Year, to.Month, DateTime.DaysInMonth(to.Year, to.Month));
            var currentUser = UserInfo.Current;

            TotalModelView spendingData = new TotalModelView { IsShow = currentUser.UserSettings.Month_SpendingWidget };
            TotalModelView earningData = new TotalModelView { IsShow = currentUser.UserSettings.Month_EarningWidget };
            TotalModelView investingData = new TotalModelView { IsShow = currentUser.UserSettings.Month_InvestingWidget };

            if (currentUser.UserSettings.Month_SpendingWidget)
            {
                var tuple = GetChartTotalByMonth(from, to, SectionTypeEnum.Spendings);
                spendingData.data = tuple.Item1.ToArray();
                spendingData.labels = tuple.Item2.ToArray();
                spendingData.Name = "Расходы";
                spendingData.SectionTypeEnum = SectionTypeEnum.Spendings;
                spendingData.Total = (tuple.Item1[tuple.Item1.Count - 1]).ToString("C", CultureInfo.CreateSpecificCulture(currentUser.Currency.SpecificCulture));

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

            if (currentUser.UserSettings.Month_EarningWidget)
            {
                var tuple = GetChartTotalByMonth(from, to, SectionTypeEnum.Earnings);

                earningData.data = tuple.Item1.ToArray();
                earningData.labels = tuple.Item2.ToArray();
                earningData.Name = "Доходы";
                earningData.SectionTypeEnum = SectionTypeEnum.Spendings;
                earningData.Total = (tuple.Item1[tuple.Item1.Count - 1]).ToString("C", CultureInfo.CreateSpecificCulture(currentUser.Currency.SpecificCulture));

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

            if (currentUser.UserSettings.Month_InvestingWidget)
            {
                var tuple = GetChartTotalByMonth(from, to, SectionTypeEnum.Investments);

                investingData.data = tuple.Item1.ToArray();
                investingData.labels = tuple.Item2.ToArray();
                investingData.Name = "Инвестиции";
                investingData.SectionTypeEnum = SectionTypeEnum.Investments;
                investingData.Total = (tuple.Item1[tuple.Item1.Count - 1]).ToString("C", CultureInfo.CreateSpecificCulture(currentUser.Currency.SpecificCulture));

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
                    investingData.IsGood = earningData.Percent > 0;

                    if (investingData.Percent < 0)
                    {
                        investingData.Percent *= -1;
                    }
                }
            }

            return new Tuple<TotalModelView, TotalModelView, TotalModelView>(spendingData, earningData, investingData);
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
}
