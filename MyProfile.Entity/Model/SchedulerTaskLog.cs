using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
    public class SchedulerTaskLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string Comment { get; set; }
        public DateTime Start { get; set; }
        public DateTime? End { get; set; }
        public bool IsError { get; set; }
        public int ChangedItems { get; set; }

        [ForeignKey("Task")]
        public int TaskID { get; set; }

        public virtual SchedulerTask Task { get; set; }
    }
}
