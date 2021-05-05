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
		public long ID { get; set; }
		public string Title { get; set; }
		public DateTime DateCreate { get; set; }
		public DateTime DateEdit { get; set; }
        public bool IsFavorite { get; set; }
        public bool IsDeleted { get; set; }

		[ForeignKey("ToDoListFolder")]
		public int ToDoListFolderID { get; set; }
		[ForeignKey("VisibleElement")]
		public long VisibleElementID { get; set; }

		public virtual ToDoListFolder ToDoListFolder { get; set; }
		public virtual VisibleElement VisibleElement { get; set; }

		public virtual ICollection<ToDoListItem> ToDoListItems { get; set; }

		public ToDoList()
		{
			this.ToDoListItems = new HashSet<ToDoListItem>();
		}

	}
}
