using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
	public class CollectiveBudgetRequestOwner
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ID { get; set; }
	
		/// <summary>
		/// who ask
		/// </summary>
		[ForeignKey("User")]
		public Guid UserID { get; set; }
		//[ForeignKey("CollectiveBudgetRequest")]
		//public int CollectiveBudgetRequestID { get; set; }


		public virtual User User { get; set; }
		//public virtual CollectiveBudgetRequest CollectiveBudgetRequest { get; set; }
	}
}
