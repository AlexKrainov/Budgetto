using System;
using System.Collections.Generic;
using System.Text;

namespace MyProfile.Entity.ModelView.Goal
{
	public class GoalModelView : Entity.Model.Goal
	{
		public string OwenerName { get; set; }
		public decimal TotalMoney { get; set; }
		public decimal Percent { get; set; }
        public bool IsShow_BudgetMonth { get; set; }
        public bool IsShow_BudgetYear { get; set; }
        public bool IsShowInCollective { get; set; }

        public IEnumerable<RecordItem> Records { get; set; } = new List<RecordItem>();
		public string ChartID { get; set; }
		public decimal Percent2 { get; set; }
		public bool IsShow { get; set; }
        public bool IsOwner { get; set; }
        public string UserName { get; set; }
        public string ImageLink { get; set; }
        public decimal? LeftMoney { get; set; }
    }

	public class RecordItem
	{
		public int ID { get; set; }
		public int GoalID { get; set; }
		public decimal Total { get; set; }
		public DateTime? DateTimeOfPayment { get; set; }
		public bool IsOwner { get; set; }
		public string UserName { get; set; }
		public string ImageLink { get; set; }
		public DateTime? CreateDateTime { get; set; }
        public bool IsShow { get; set; }
    }
}
