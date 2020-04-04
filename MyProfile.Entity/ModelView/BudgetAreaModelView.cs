using System;
using System.Collections.Generic;
using System.Text;

namespace MyProfile.Entity.ModelView
{
	public class BudgetAreaModelView
	{
		public int ID { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string CssIcon { get; set; }
		public bool IsGlobal { get; set; }

		public bool IsShow { get; set; } = true;
		public bool IsUpdated { get; set; } = false;

		public IEnumerable<BudgetSecionModelView> Sections { get; set; } = new List<BudgetSecionModelView>();
	}
}
