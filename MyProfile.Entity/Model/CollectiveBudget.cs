using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
	public class CollectiveBudget
	{
		public CollectiveBudget()
		{
			this.People = new HashSet<Person>();
		}

		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid ID { get; set; }
		[Required]
		public string Name { get; set; }
		public DateTime DateCreate { get; set; }
		public DateTime? DateDelete { get; set; }


		public virtual IEnumerable<Person> People { get; set; }
	}
}
