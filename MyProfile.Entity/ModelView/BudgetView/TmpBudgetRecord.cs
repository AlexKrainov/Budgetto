using System;
using System.Collections.Generic;

namespace MyProfile.Entity.ModelView.BudgetView
{
	public class TmpBudgetRecord
	{
		public decimal Total { get; set; }
		public DateTime DateTimeOfPayment { get; set; }
		public long SectionID { get; set; }
		public string SectionName { get; set; }
		public int AreaID { get; set; }
		public string AreaName { get; set; }
		public IEnumerable<long> CollectionSectionIDs { get; set; }
        public int SectionTypeID { get; set; }
        public long? AccountID { get; set; }
        public IEnumerable<long> TagIDs { get; set; }
    }
}
