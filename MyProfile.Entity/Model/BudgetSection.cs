using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
	public class BudgetSection
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
		/// Hide but not delete
		/// </summary>
		public bool IsShow { get; set; } = true;
		public string IncludedCollectiveSections { get; set; }

		[ForeignKey("Person")]
		public Guid? PersonID { get; set; }
		[ForeignKey("BudgetArea")]
		public int BudgetAreaID { get; set; }

		public virtual Person Person { get; set; }
		public virtual BudgetArea BudgetArea { get; set; }

		public virtual IEnumerable<BudgetRecord> BudgetRecords { get; set; }

		public virtual IEnumerable<Limit> Limits { get; set; }

		public BudgetSection()
		{
			this.Limits = new HashSet<Limit>();
			this.BudgetRecords = new HashSet<BudgetRecord>();
		}
	}
}
