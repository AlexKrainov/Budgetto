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
		public string CodeName { get; set; }
		public string Currency { get; set; }
		[Required]
		[Column(TypeName = "Money")]
		public decimal CurrencyPrice { get; set; }
		public string Color { get; set; }
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
