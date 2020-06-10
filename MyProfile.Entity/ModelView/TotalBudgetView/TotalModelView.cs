using MyProfile.Entity.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyProfile.Entity.ModelView.TotalBudgetView
{
	public class TotalModelView
	{
		public string Name { get; set; }
		public SectionTypeEnum SectionTypeEnum { get; set; }
		public string Total { get; set; }
		public bool IsGood { get; set; }
		public decimal Percent { get; set; }
		public decimal[] data { get; set; }
		public string[] labels { get; set; }
		public bool IsShow { get; set; } = true;
	}
}
