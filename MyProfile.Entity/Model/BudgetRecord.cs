﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
	public class BudgetRecord
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ID { get; set; }
		[Required]
		[Column(TypeName = "Money")]
		public decimal Total { get; set; }
		/// <summary>
		/// It needs to understand what data an user wrote
		/// </summary>
		public string RawData { get; set; } 
		public string Description { get; set; }
		public DateTime DateTimeOfPayment { get; set; }
		public DateTime DateTimeCreate { get; set; }
		public DateTime DateTimeEdit { get; set; }
		public DateTime? DateTimeDelete { get; set; }
		public bool IsDeleted { get; set; }
		/// <summary>
		/// Consider when count or not
		/// </summary>
		public bool IsHide { get; set; }

		[ForeignKey("User")]
		public Guid UserID { get; set; }
		[ForeignKey("BudgetSection")]
		public int BudgetSectionID { get; set; }

		public virtual User User { get; set; }
		public virtual BudgetSection BudgetSection { get; set; }


		

	}
}
