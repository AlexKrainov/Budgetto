﻿using System;
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
		/// <summary>
		/// Show or not on dashboard
		/// </summary>
		public bool IsShowOnDashBoard { get; set; }
		public bool IsShowInCollective { get; set; }
		public bool IsFinished { get; set; }
		public bool IsDeleted { get; set; }

		[ForeignKey("User")]
		public Guid UserID { get; set; }

		public virtual User User { get; set; }

		public virtual ICollection<GoalRecord> GoalRecords { get; set; }

		public Goal()
		{
			this.GoalRecords = new HashSet<GoalRecord>();
		}

	}
}