using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyProfile.Entity.ModelView
{
	public class BudgetSectionModelView
	{
		[NonSerialized, JsonIgnore]
		public Guid UserID;
		public long ID { get; set; }
        public int? BaseSectionID { get; set; }
        public string BaseSectionName { get; set; }
        public string Name { get; set; }
		public string Description { get; set; }
		public string CssIcon { get; set; }
		public string CssColor { get; set; }
		public bool IsShowOnSite { get; set; }
        public bool IsShowInCollective { get; set; }

        public int? BaseAreaID { get; set; }
        public int AreaID { get; set; }
		public string AreaName { get; set; }
		public bool IsUpdated { get; set; }
        public bool IsSaved { get; set; }
        public bool IsShow_Filtered { get; set; } = true;

		public string Owner { get; set; }
		public bool CanEdit { get; set; }

		/// <summary>
		/// For CollectiveBudget
		/// </summary>
		public IEnumerable<BudgetSectionModelView> CollectiveSections { get; set; } = new List<BudgetSectionModelView>();
		public int SectionTypeID { get; set; }
		public string SectionTypeName { get; set; }
        public int RecordCount { get; set; }
		public bool IsShow { get; set; } = true;
        public bool HasRecords { get; set; }
        public string CssBackground { get; set; }
        public string CodeName { get; set; }
		public bool IsSelected { get; set; } = true;
        public IList<TagSectionModelView> Tags { get; set; }
        public bool IsCashback { get; set; }
        public bool IsRegularPayment { get; set; }
    }
}
