using System;
using System.Collections.Generic;

namespace MyProfile.Entity.ModelView.BudgetView
{
	public class TmpBudgetRecord
	{
		public decimal Total { get; set; }
		public DateTime DateTimeOfPayment { get; set; }
		public int SectionID { get; set; }
		public string SectionName { get; set; }
		public int AreaID { get; set; }
		public string AreaName { get; set; }
		public List<int> CollectionSectionIDs { get; set; }
	}
}
