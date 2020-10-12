using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
    public class Reminder
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required]
        [MaxLength(256)]
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? DateReminder { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime DateEdit { get; set; }
        public bool IsRepeat { get; set; }
        [MaxLength(16)]
        public string RepeatEvery { get; set; }
        public bool IsDeleted { get; set; }
        [MaxLength(32)]
        public string CssIcon { get; set; }
        public bool IsReminderByMail { get; set; }

        [ForeignKey("User")]
        public Guid UserID { get; set; }

        public virtual User User { get; set; }

        public virtual IEnumerable<ReminderDate> ReminderDates { get; set; }

        public Reminder()
        {
            this.ReminderDates = new HashSet<ReminderDate>();
        }
    }
}
