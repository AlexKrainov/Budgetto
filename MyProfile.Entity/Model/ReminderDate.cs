using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
    public class ReminderDate
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ID { get; set; }
        public DateTime DateReminder { get; set; }
        public bool IsDone { get; set; }

		[ForeignKey("Reminder")]
		public int ReminderID { get; set; }
		
		public virtual Reminder Reminder { get; set; }
		
	}
}
