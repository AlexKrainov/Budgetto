using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
    public class RecordTag
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        public DateTime DateSet { get; set; }
        [ForeignKey("Record")]
        public long RecordID { get; set; }
        [ForeignKey("UserTag")]
        public long UserTagID { get; set; }

        public virtual UserTag UserTag { get; set; }
        public virtual Record Record { get; set; }
    }
}
