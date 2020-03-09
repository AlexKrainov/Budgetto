using System;
using System.Collections.Generic;
using System.Text;

namespace MyProfile.Entity.ModelView
{
	public class BudgetSecionModelView
	{
		public int ID { get; set; }
		public string Name { get; set; }
		public string CodeName { get; set; }
		public string Type_RecordType { get; set; }
		public string CssIcon { get; set; }
		public bool IsGlobal { get; set; }

		public List<BudgetAreaModelView> BudgetAreas { get; set; }
	}
}
