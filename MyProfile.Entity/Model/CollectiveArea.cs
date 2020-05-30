using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
	public class CollectiveArea
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ID { get; set; }
		[ForeignKey("Area")]
		public int? AreaID { get; set; }

		[ForeignKey("ChildArea")]
		public int? ChildAreaID { get; set; }

		public virtual BudgetArea Area { get; set; }
		public virtual BudgetArea ChildArea { get; set; }
	}
}
