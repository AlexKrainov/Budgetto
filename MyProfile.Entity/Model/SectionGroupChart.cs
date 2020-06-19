using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
	public class SectionGroupChart
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ID { get; set; }
		
		[ForeignKey("PartChart")]
		public int PartChartID { get; set; }
		[ForeignKey("BudgetSection")]
		public int BudgetSectionID { get; set; }

		public virtual ChartField PartChart { get; set; }
		public virtual BudgetSection BudgetSection { get; set; }

		
	}
}
