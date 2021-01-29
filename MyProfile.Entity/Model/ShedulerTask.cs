using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        AccountRemoveCachback
    }

    public class ShedulerTask
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
        public string Comment { get; set; }


        public virtual ICollection<ShedulerTaskLog> TaskLogs { get; set; }

        public ShedulerTask()
        {
            this.TaskLogs = new HashSet<ShedulerTaskLog>();
        }

    }
}
