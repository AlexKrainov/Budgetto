using System;
using System.Collections.Generic;
using System.Text;

namespace MyProfile.Entity.ModelView
{
    public class SummaryModelView
    {
        public decimal? TotalSpendings { get; set; }
        public decimal? TotalEarnings { get; set; }
        public string CurrencyCodeName { get; set; }
        public string CurrencySpecificCulture { get; set; }
        public bool IsShow { get; set; }


        public EarningsPerHourModelView EarningsPerHour { get; set; }
        public ExpensesPerDayModelView ExpensesPerDay { get; set; }
        public CashFlowModelView CashFlow { get; set; }
        public AllAccountsMoneyModelView AllAccountsMoney { get; set; }
    }

    public class SummaryBase
    {
        public int ID { get; set; }
        public string ElementID { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }

        public bool IsShow { get; set; }
    }

    public class EarningsPerHourModelView : SummaryBase
    {
        public int WorkHours { get; set; }
        public DateTime LastChange { get; set; }
    }
    public class ExpensesPerDayModelView : SummaryBase
    {
        public int TotalDays { get; set; }
    }
    public class CashFlowModelView : SummaryBase
    {

    }
    public class AllAccountsMoneyModelView : SummaryBase
    {
        /// <summary>
        /// if while we are tring to get rate from CB by money catch an error
        /// </summary>
        public bool ConvertError { get; set; }
    }
}
