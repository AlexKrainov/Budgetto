using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
	public class SectionTypeView
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public long ID { get; set; }
		public bool IsShow { get; set; }

		[ForeignKey("User")]
		public Guid UserID { get; set; }
		[ForeignKey("PeriodType")]
		public int PeriodTypeID { get; set; }
		[ForeignKey("SectionType")]
		public int SectionTypeID { get; set; }

		public virtual User User { get; set; }
		public virtual PeriodType PeriodType { get; set; }
		public virtual SectionType SectionType { get; set; }

	}
}
