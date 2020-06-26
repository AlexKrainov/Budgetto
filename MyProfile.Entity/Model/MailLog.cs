using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyProfile.Entity.Model
{
    public class MailLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; }
        public DateTime SentDateTime { get; set; }
        public DateTime? CameDateTime { get; set; }
        public string Comment { get; set; }
        public bool IsSuccessful { get; set; }

        [ForeignKey("User")]
        public Guid? UserID { get; set; }
        [ForeignKey("MailType")]
        public int MailTypeID { get; set; }


        public virtual User User { get; set; }
        public virtual MailType MailType { get; set; }
    }
}
