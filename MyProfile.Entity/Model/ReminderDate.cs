using System;
using System.Collections.Generic;
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

		public virtual IEnumerable<Notification> Notifications { get; set; }

		public ReminderDate()
        {
			this.Notifications = new HashSet<Notification>();
		}
	}
}
