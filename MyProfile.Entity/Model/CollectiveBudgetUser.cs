using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyProfile.Entity.Model
{
	public enum CollectiveUserStatusType
	{
		Accepted,
		Poused,
		Gone// left a collective budget
	}
	public class CollectiveBudgetUser
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ID { get; set; }
		[Required]
		[MaxLength(16)]
		public string Status { get; set; }
		public DateTime DateAdded { get; set; }
		public DateTime DateUpdate { get; set; }
		/// <summary>
		/// whom ask
		/// </summary>
		[ForeignKey("User")]
		public Guid UserID { get; set; }
		[ForeignKey("CollectiveBudget")]
		public Guid CollectiveBudgetID { get; set; }

		public virtual User User { get; set; }
		public virtual CollectiveBudget CollectiveBudget { get; set; }

	}
}
