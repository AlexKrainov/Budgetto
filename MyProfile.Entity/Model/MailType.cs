using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MyProfile.Entity.Model
{
    public enum MailTypeEnum
    {
        Undefined = 0,
        Registration = 1,
        EmailUpdate = 2,
        PasswordReset = 3,
        LoginLimitEnter = 4,
        ResendByUser = 5
    }

    public class MailType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string CodeName { get; set; }
    }
}
