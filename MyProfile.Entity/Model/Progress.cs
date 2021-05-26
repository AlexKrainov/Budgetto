using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
    public class Progress
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        public bool IsComplete { get; set; }
        public DateTime? DateComplete { get; set; }
        [MaxLength(32)]
        public string NeedToBeValue { get; set; }
        [MaxLength(32)]
        public string CurrentValue { get; set; }
        public DateTime? UserDateEdit { get; set; }
        public bool IsActive { get; set; }

        [ForeignKey("User")]
        public Guid UserID { get; set; }
        [ForeignKey("ParentProgress")]
        public long? ParentProgressID { get; set; }
        [ForeignKey("ProgressType")]
        public int ProgressTypeID { get; set; }
        [ForeignKey("ProgressItemType")]
        public int? ProgressItemTypeID { get; set; }

        public virtual User User { get; set; }
        public virtual Progress ParentProgress { get; set; }
        public virtual ProgressType ProgressType { get; set; }
        public virtual ProgressItemType ProgressItemType { get; set; }

        public virtual ICollection<ProgressLog> ProgressLogs { get; set; }
        public virtual ICollection<Progress> Progresses { get; set; }

        public Progress()
        {
            this.ProgressLogs = new HashSet<ProgressLog>();
            this.Progresses = new HashSet<Progress>();
        }
    }
}
