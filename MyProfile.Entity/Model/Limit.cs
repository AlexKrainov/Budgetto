﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
	public class Limit
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ID { get; set; }
		public string Name { get; set; }
		[Column(TypeName = "Money")]
		public decimal LimitMoney { get; set; }
		public DateTime? DateStart { get; set; }
		public DateTime? DateEnd { get; set; }
		public bool IsShow { get; set; }

		[ForeignKey("Person")]
		public Guid PersonID { get; set; }
		[ForeignKey("PeriodType")]
		public int PeriodTypeID { get; set; }

		public virtual Person Person { get; set; }
		public virtual PeriodType PeriodType { get; set; }

	}
}
