using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
	public class SectionGroupLimit
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ID { get; set; }
		
		[ForeignKey("Limit")]
		public int LimitID { get; set; }
		[ForeignKey("BudgetSection")]
		public int? BudgetSectionID { get; set; }

		public virtual Limit Limit { get; set; }
		public virtual BudgetSection BudgetSection { get; set; }

	}
}
