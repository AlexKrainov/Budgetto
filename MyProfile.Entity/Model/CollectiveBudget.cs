using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
	public class CollectiveBudget
	{
		public CollectiveBudget()
		{
			this.CollectiveBudgetRequests = new HashSet<CollectiveBudgetRequest>();
			this.CollectiveBudgetUsers = new HashSet<CollectiveBudgetUser>();
		}

		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid ID { get; set; }
		[Required]
		public string Name { get; set; }


		public virtual IEnumerable<CollectiveBudgetRequest> CollectiveBudgetRequests { get; set; }
		public virtual IEnumerable<CollectiveBudgetUser> CollectiveBudgetUsers { get; set; }
	}
}
