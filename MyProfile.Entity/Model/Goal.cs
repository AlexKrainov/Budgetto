using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
	public class Goal
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ID { get; set; }
		[Required]
		public string Name { get; set; }
		public string Description { get; set; }
		[Column(TypeName = "Money")]
		public decimal? ExpectationMoney { get; set; }
		public DateTime? DateStart { get; set; }
		public DateTime? DateEnd { get; set; }
		public bool IsFinished { get; set; }
		public bool IsDeleted { get; set; }
		/// <summary>
		/// Created by constructor after registration
		/// </summary>
		public bool IsCreatedByConstructor { get; set; }

		[ForeignKey("User")]
		public Guid UserID { get; set; }
		[ForeignKey("VisibleElement")]
		public long VisibleElementID { get; set; }

		public virtual User User { get; set; }
		public virtual VisibleElement VisibleElement { get; set; }

		public virtual ICollection<GoalRecord> GoalRecords { get; set; }

		public Goal()
		{
			this.GoalRecords = new HashSet<GoalRecord>();
		}

	}
}
