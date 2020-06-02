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
		/// Hide but not delete
		/// </summary>
		public bool IsShow { get; set; } = true;
		/// <summary>
		/// Can see only owner
		/// </summary>
		public bool IsPrivate { get; set; } 

		[ForeignKey("User")]
		public Guid? UserID { get; set; }
		[ForeignKey("BudgetArea")]
		public int BudgetAreaID { get; set; }
		[ForeignKey("SectionType")]
		public int? SectionTypeID { get; set; }


		public virtual User User { get; set; }
		public virtual BudgetArea BudgetArea { get; set; }
		public virtual SectionType SectionType { get; set; }


		public virtual IEnumerable<BudgetRecord> BudgetRecords { get; set; }
		public virtual IEnumerable<CollectiveSection> CollectiveSections { get; set; }

		public virtual IEnumerable<Limit> Limits { get; set; }

		public BudgetSection()
		{
			this.Limits = new HashSet<Limit>();
			this.BudgetRecords = new HashSet<BudgetRecord>();
			this.CollectiveSections = new HashSet<CollectiveSection>();
		}
	}
}
