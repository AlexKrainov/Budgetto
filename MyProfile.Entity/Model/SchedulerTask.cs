using System;
using System.ComponentModel.DataAnnotations;

namespace MyProfile.Entity.Model
{
    public enum TaskStatus
    {
        Undefined = 0,
        New,
        InProcess,
        Stop,
        Error
    }

    public enum TaskType
    {
        Undefined = 0,
        AccountRemoveCachback = 1,
        SetDoneToReminderDates = 2,
        CurrencyHistoryTask = 3,
        ResetHubConnectTask = 4,
        NotificationLimitCheckerTask = 5,
        NotificationSiteTask = 6,
        NotificationTelegramTask = 7,
        NotificationMailTask = 8,
        NotificationReminderCheckerTask = 9,
        NotificationReset = 10,

        Test,
    }

    public class SchedulerTask
    {
        [Key]
        public int ID { get; set; }
        [Required]
        [MaxLength(64)]
        public string Name { get; set; }
        public DateTime? FirstStart { get; set; }
        public DateTime? LastStart { get; set; }
        [Required]
        [MaxLength(16)]
        public string TaskStatus { get; set; }
        [Required]
        [MaxLength(64)]
        public string TaskType { get; set; }
        [MaxLength(16)]
        public string CronExpression { get; set; }
        [MaxLength(64)]
        public string CronComment { get; set; }
        public string Comment { get; set; }


        //public virtual ICollection<SchedulerTaskLog> TaskLogs { get; set; }

        //public SchedulerTask()
        //{
        //    this.TaskLogs = new HashSet<SchedulerTaskLog>();
        //}

    }
}
