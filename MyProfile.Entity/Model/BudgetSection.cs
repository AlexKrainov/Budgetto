using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
	public class BudgetSection
	{
		public BudgetSection()
		{
			this.BudgetRecords = new HashSet<BudgetRecord>();
		}

		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ID { get; set; }
		[Required]
		public string Name { get; set; }
		public string Description { get; set; }
		[MaxLength(64)]
		public string CssIcon { get; set; }
		[MaxLength(24)]
		public string CssColor { get; set; }

		/// <summary>
		/// Income, Consumption, Saving
		/// </summary>
		[Required, MaxLength(16)]
		public string SectionTypeCodeName { get; set; }
		/// <summary>
		/// Default sectino then the person write records
		/// </summary>
		public bool IsByDefault { get; set; }

		[ForeignKey("Person")]
		public Guid? PersonID { get; set; }
		[ForeignKey("BudgetArea")]
		public int BudgetAreaID { get; set; }

		public virtual Person Person { get; set; }
		public virtual BudgetArea BudgetArea { get; set; }

		public virtual IEnumerable<BudgetRecord> BudgetRecords { get; set; }
	}
}
