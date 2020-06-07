using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
	public class User
	{
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
		[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		public DateTime DateCreate { get; set; }
		public DateTime? DateDelete { get; set; }
		public bool IsDeleted { get; set; }

		public bool IsAllowCollectiveBudget { get; set; }
		/// <summary>
		/// we have checkbox (IsAgreeToCollectiveBudget) and CollectiveBudgetID (can be null)
		/// 1) User click the ckeckbox
		/// 2) Show input with search email
		/// 3) If we found the email and user has checkbox and CollectiveBudgetID, we write this CollectiveBudgetID to user from (1)
		/// 4) If we don't find create CollectiveBudget
		/// 
		/// </summary>
		[ForeignKey("CollectiveBudget")]
		public Guid? CollectiveBudgetID { get; set; }

		public virtual CollectiveBudget CollectiveBudget { get; set; }
		public virtual UserSettings UserSettings { get; set; }

		public virtual IEnumerable<BudgetArea> BudgetAreas { get; set; }
		public virtual IEnumerable<BudgetRecord> BudgetRecords { get; set; }
		public virtual IEnumerable<BudgetSection> BudgetSections { get; set; }
		public virtual IEnumerable<Template> Templates { get; set; }

		public User()
		{
			this.BudgetAreas = new HashSet<BudgetArea>();
			this.BudgetRecords = new HashSet<BudgetRecord>();
			this.BudgetSections = new HashSet<BudgetSection>();
			this.Templates = new HashSet<Template>();
		}
	}
}
