using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
	public class Bank
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ID { get; set; }
		[Required]
		[MaxLength(32)]
		public string Name { get; set; }
		public string ImageSrc { get; set; }
		

		public virtual ICollection<Account> Accounts { get; set; }

		public Bank()
		{
			this.Accounts = new HashSet<Account>();
		}

	}
}
