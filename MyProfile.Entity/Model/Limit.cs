using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
	public class Limit
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ID { get; set; }
		[Column(TypeName = "Money")]
		public decimal Money { get; set; }
		

		[ForeignKey("Person")]
		public Guid PersonID { get; set; }
		[ForeignKey("PeriodType")]
		public int PeriodTypeID { get; set; }
		[ForeignKey("BudgetArea")]
		public int? BudgetAreaID { get; set; }
		[ForeignKey("BudgetSection")]
		public int? BudgetSectionID { get; set; }

		public virtual Person Person { get; set; }
		public virtual BudgetArea BudgetArea { get; set; }
		public virtual BudgetSection BudgetSection { get; set; }
		public virtual PeriodType PeriodType { get; set; }

	}
}
