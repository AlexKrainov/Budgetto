using System;
using System.Collections.Generic;
using System.Text;

namespace MyProfile.Entity.ModelView.Limit
{
	public class LimitChartModelView
	{
		public string Name { get; set; }
		public List<BudgetSectionModelView> Sections { get; set; } = new List<BudgetSectionModelView>();
		public decimal LimitMoney { get; set; }
		public decimal SpendedMoney { get; set; }
		public decimal LeftMoneyInADay { get; set; }
		public string ChartID { get; set; }
		public decimal Percent1 { get; set; }
		public decimal Percent2 { get; set; }
		/// <summary>
		/// Is DateTime.Now == current view date 
		/// </summary>
		public bool IsThis { get; set; }
		public bool IsPast { get; set; }
		public bool IsFuture { get; set; }
		public bool IsShow { get; set; }
        public int PeriodTypeID { get; set; }
        public string Text { get; set; }
    }
}
