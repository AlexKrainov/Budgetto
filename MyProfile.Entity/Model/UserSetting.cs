using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
	public class UserSettings
	{
		[Key]
		[ForeignKey("User")]
		public Guid ID { get; set; }
	

		public virtual User User { get; set; }

	}
}
