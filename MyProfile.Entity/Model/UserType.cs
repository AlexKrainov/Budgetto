using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
	public enum UserTypeEnum
	{
		User = 1,
		Admin = 2,
		Tester = 3,
	}
	public class UserType
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ID { get; set; }
		[Required, MaxLength(8)]
		public string CodeName { get; set; }
	}
}
