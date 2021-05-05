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
		public long ID { get; set; }
		
		[ForeignKey("Limit")]
		public long LimitID { get; set; }
		[ForeignKey("BudgetSection")]
		public long BudgetSectionID { get; set; }

		public virtual Limit Limit { get; set; }
		public virtual BudgetSection BudgetSection { get; set; }

		
	}
}
