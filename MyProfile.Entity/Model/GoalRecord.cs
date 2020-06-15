using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
	public class GoalRecord
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ID { get; set; }
		[Column(TypeName = "Money")]
		public decimal Total { get; set; }
		public DateTime? DateTimeOfPayment { get; set; }
		public DateTime CreateDateTime { get; set; }
		
		[ForeignKey("User")]
		public Guid? UserID { get; set; }
		[ForeignKey("Goal")]
		public int GoalID { get; set; }
		
		public virtual User User { get; set; }
		public virtual Goal Goal { get; set; }

	}
}
