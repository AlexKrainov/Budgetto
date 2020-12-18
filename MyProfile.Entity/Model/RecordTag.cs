using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
    public class RecordTag
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public DateTime DateSet { get; set; }
        [ForeignKey("Record")]
        public int RecordID { get; set; }
        [ForeignKey("UserTag")]
        public int UserTagID { get; set; }

        public virtual UserTag UserTag { get; set; }
        public virtual BudgetRecord Record { get; set; }
    }
}
