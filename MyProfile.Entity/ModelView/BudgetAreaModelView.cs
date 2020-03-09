using System;
using System.Collections.Generic;
using System.Text;

namespace MyProfile.Entity.ModelView
{
	public class BudgetAreaModelView
	{
		public int ID { get; set; }
		public Guid PersonID { get; set; } //?
		public string Name { get; set; }
		public string CodeName { get; set; }
		public string Currency { get; set; }
		public decimal CurrencyPrice { get; set; }
		public string Color { get; set; }
		public string CssIcon { get; set; }
		public bool IsGlobal { get; set; }
	}
}
