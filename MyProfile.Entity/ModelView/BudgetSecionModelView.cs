using System;
using System.Collections.Generic;
using System.Text;

namespace MyProfile.Entity.ModelView
{
	public class BudgetSecionModelView
	{
		public int ID { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string CssIcon { get; set; }
		public string CssColor { get; set; }
		public bool IsGlobal { get; set; }//?

		public int AreaID { get; set; }
		public string AreaName { get; set; }
		public bool IsUpdated { get; set; }
	}
}
