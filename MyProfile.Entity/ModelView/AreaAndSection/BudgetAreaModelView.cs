using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyProfile.Entity.ModelView
{
	public class BudgetAreaModelView
	{
		[NonSerialized, JsonIgnore]
		public Guid UserID;
		public int ID { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string CssIcon { get; set; }

		public string Owner { get; set; }
		public bool IsShowOnSite { get; set; } = true;
		public bool IsShowInCollective { get; set; } = true;


		public bool IsShow_Filtered { get; set; } = true;
		public bool IsUpdated { get; set; } = false;

		public IEnumerable<BudgetSectionModelView> Sections { get; set; } = new List<BudgetSectionModelView>();
        public string CodeName { get; set; }
    }
}
