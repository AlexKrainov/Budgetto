using MyProfile.Entity.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyProfile.Entity.ModelView.Chart
{
	public class ChartEditModel
	{
		public int ID { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public bool IsShowBudgetMonth { get; set; }
		public bool IsShowBudgetYear { get; set; }

		public ChartTypesEnum ChartTypeID { get; set; }
		public IEnumerable<ChartFieldItem> Fields { get; set; }
		public string ChartTypeCodeName { get; set; }
		public string ChartTypeName { get; set; }
		public DateTime DateCreate { get; set; }
		public DateTime LastDateEdit { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsBig { get; set; }
        public string href { get; set; }
    }

	public class ChartFieldItem
	{
		public long ID { get; set; }
		public string Name { get; set; }
		public string CssColor { get; set; }
		public string CssBackground { get; set; }
		public IEnumerable<long> Sections { get; set; }
	}
}
