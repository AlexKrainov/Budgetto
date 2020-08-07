using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
	public class ToDoList
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ID { get; set; }
		public string Title { get; set; }
		public DateTime DateCreate { get; set; }
		public DateTime DateEdit { get; set; }
		[MaxLength(32)]
		public string CssIcon { get; set; }
		public bool IsDeleted { get; set; }

		[ForeignKey("User")]
		public Guid UserID { get; set; }
		[ForeignKey("PeriodType")]
		public int PeriodTypeID { get; set; }
		[ForeignKey("VisibleElement")]
		public int VisibleElementID { get; set; }

		public virtual User User { get; set; }
		public virtual PeriodType PeriodType { get; set; }
		public virtual VisibleElement VisibleElement { get; set; }

		public virtual ICollection<ToDoListItem> ToDoListItems { get; set; }

		public ToDoList()
		{
			this.ToDoListItems = new HashSet<ToDoListItem>();
		}

	}
}
