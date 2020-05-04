using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyProfile.Entity.ModelView
{
	public class BudgetAreaModelView
	{
		[NonSerialized, JsonIgnore]
		public Guid PersonID;
		public int ID { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string CssIcon { get; set; }

		public string Owner { get; set; }
		public bool CanEdit { get; set; }

		public bool IsShow { get; set; } = true;
		public bool IsUpdated { get; set; } = false;
		public string IncludedCollectiveAreas_Raw { get; set; }

		public List<BudgetSectionModelView> Sections { get; set; } = new List<BudgetSectionModelView>();
		/// <summary>
		/// For CollectiveBudget
		/// </summary>
		public List<BudgetAreaModelView> IncludedCollectiveAreas { get; set; } = new List<BudgetAreaModelView>();
	}
}
