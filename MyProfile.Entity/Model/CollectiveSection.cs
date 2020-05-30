﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
	public class CollectiveSection
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ID { get; set; }
		[ForeignKey("Section")]
		public int? SectionID { get; set; }
		[ForeignKey("ChildSection")]
		public int? ChildSectionID { get; set; }


		public virtual BudgetSection Section { get; set; }
		public virtual BudgetSection ChildSection { get; set; }
	}
}
