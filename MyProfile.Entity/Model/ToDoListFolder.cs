using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
	public class ToDoListFolder
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ID { get; set; }
		public string Title { get; set; }
		[MaxLength(32)]
		public string CssIcon { get; set; }

		[ForeignKey("User")]
		public Guid UserID { get; set; }

		public virtual User User { get; set; }

		public virtual ICollection<ToDoList> ToDoLists { get; set; }

		public ToDoListFolder()
		{
			this.ToDoLists = new HashSet<ToDoList>();
		}

	}
}
