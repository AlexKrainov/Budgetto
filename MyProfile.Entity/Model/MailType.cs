using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
    public enum MailTypeEnum
    {
        Undefined = 0,
        Registration = 1,
        EmailUpdate = 2,
        PasswordReset = 3,
        LoginLimitEnter = 4,
        ResendByUser = 5,
        NotificationLimit = 6,
        NotificationReminder = 7,
        NotificationSystemMailing = 8,
    }

    public class MailType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public string CodeName { get; set; }
    }
}
