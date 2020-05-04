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
		/// <summary>
		/// It needs for isolate the section created by the system
		/// </summary>
		public string CodeName { get; set; }
		/// <summary>
		/// Hide but not delete
		/// </summary>
		public bool IsShow { get; set; } = true;
		public string Description { get; set; }
		[MaxLength(64)]
		public string CssIcon { get; set; }
		public string IncludedCollectiveAreas { get; set; }

		[ForeignKey("Person")]
		public Guid? PersonID { get; set; }

		public virtual Person Person { get; set; }
		public virtual IEnumerable<BudgetSection> BudgetSectinos { get; set; }

		public BudgetArea()
		{
			this.BudgetSectinos = new HashSet<BudgetSection>();
		}
	}
}
