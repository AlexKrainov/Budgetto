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
        ConfirmEmail = 1,
        ResetPassword = 2,
        Login = 3,
        ResendByUser = 4
    }

    public class MailType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string CodeName { get; set; }
    }
}
