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
		public string CodeName { get; set; }
		/// <summary>
		/// What kind of record plus or minus 
		/// </summary>
		public string Type_RecordType { get; set; }
		public string CssIcon { get; set; }
		/// <summary>
		/// Default sectino then the person write records
		/// </summary>
		public bool IsByDefault { get; set; }

		public Guid? PersonID { get; set; }
		[ForeignKey("BudgetArea")]
		public int BudgetAreaID { get; set; }

		public virtual Person Person { get; set; }
		public virtual BudgetArea BudgetArea { get; set; }
		
		public virtual IEnumerable<BudgetRecord> BudgetRecords { get; set; }
	}
}
