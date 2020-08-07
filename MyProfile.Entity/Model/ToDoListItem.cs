using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
    public class ToDoListItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string Text { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime DateEdit { get; set; }
        public bool IsDone { get; set; }

        [ForeignKey("ToDoList")]
        public int ToDoListID { get; set; }
        [ForeignKey("OwnerUser")]
        public Guid? OwnerUserID { get; set; }
        [ForeignKey("DoneUser")]
        public Guid? DoneUserID { get; set; }

        public virtual User OwnerUser { get; set; }
        public virtual User DoneUser { get; set; }
        public virtual ToDoList ToDoList { get; set; }

    }
}
