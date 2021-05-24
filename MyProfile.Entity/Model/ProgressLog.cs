using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
    public class ProgressLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public bool IsComplete { get; set; }
        public DateTime? DateComplete{ get; set; }
        [MaxLength(32)]
        public string Value { get; set; }
        public DateTime? UserDateEdit { get; set; }
        public bool IsActive { get; set; }

        [ForeignKey("Progress")]
        public long ProgressID { get; set; }
        
        public virtual Progress Progress { get; set; }
    }
}
