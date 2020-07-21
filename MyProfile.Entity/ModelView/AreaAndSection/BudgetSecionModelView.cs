using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MyProfile.Entity.ModelView
{
	public class BudgetSectionModelView
	{
		[NonSerialized, JsonIgnore]
		public Guid UserID;
		public int ID { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string CssIcon { get; set; }
		public string CssColor { get; set; }
		public bool IsShowOnSite { get; set; }
        public bool IsShowInCollective { get; set; }

		public int AreaID { get; set; }
		public string AreaName { get; set; }
		public bool IsUpdated { get; set; }
		public bool isShow_Filtered { get; set; } = true;

		public string Owner { get; set; }
		public bool CanEdit { get; set; }

		/// <summary>
		/// For CollectiveBudget
		/// </summary>
		public IEnumerable<BudgetSectionModelView> CollectiveSections { get; set; } = new List<BudgetSectionModelView>();
		public int? SectionTypeID { get; set; }
		public string SectionTypeName { get; set; }
        public int RecordCount { get; set; }
        public bool IsShow { get; set; }
        public bool HasRecords { get; set; }
    }
}
