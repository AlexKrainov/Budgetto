using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
	public class BudgetArea
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ID { get; set; }
		[Required]
		public string Name { get; set; }
		public string Description { get; set; }
		[MaxLength(64)]
		public string CssIcon { get; set; }

		public Guid? PersonID { get; set; }

		public virtual Person Person { get; set; }
		public virtual IEnumerable<BudgetSection> BudgetSectinos { get; set; }

		public BudgetArea()
		{
			this.BudgetSectinos = new HashSet<BudgetSection>();
		}
	}
}
