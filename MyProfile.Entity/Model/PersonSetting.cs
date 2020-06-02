﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
	public class PersonSetting
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid ID { get; set; }
		[Required]
		[ForeignKey("User")]
		public Guid UserID { get; set; }
		public string SpecificCulture { get; set; }

		public virtual User User { get; set; }
	}
}
