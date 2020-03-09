using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyProfile.Entity.Model
{
	public class Person
	{
		public Person()
		{
			this.BudgetSections = new HashSet<BudgetSection>();
			this.BudgetAreas = new HashSet<BudgetArea>();
			this.BudgetRecords = new HashSet<BudgetRecord>();
			this.Templates = new HashSet<Template>();
		}

		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid ID { get; set; }
		[Required]
		public string Name { get; set; }
		public string LastName { get; set; }
		[Required]
		[DataType(DataType.EmailAddress)]
		public string Email { get; set; }
		[Required]
		[DataType(DataType.Password)]
		public string Password { get; set; }
		public string ImageLink { get; set; }
		public DateTime DateCreate { get; set; }
		public DateTime? DateDelete { get; set; }
		public bool IsDeleted { get; set; }

		[ForeignKey("CollectiveBudget")]
		public Guid? CollectiveBudgetID { get; set; }

		public virtual CollectiveBudget CollectiveBudget { get; set; }

		public virtual IEnumerable<BudgetArea> BudgetAreas { get; set; }
		public virtual IEnumerable<BudgetRecord> BudgetRecords { get; set; }
		public virtual IEnumerable<BudgetSection> BudgetSections { get; set; }
		public virtual IEnumerable<Template> Templates { get; set; }
	}
}
