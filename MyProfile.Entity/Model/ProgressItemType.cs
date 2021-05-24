using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
    public enum ProgressItemTypeEnum
    {
        //ProgressTypeEnum.Introductory,
        CreateRecord = 1,
        CreateLimit = 2,
        CreateNotification = 3,
        CreateReminder = 4,
        CreateOrEditTemplate = 5,
        CreateSection = 6,
        CreateArea = 7,
        CreateAccount = 8,

        //FinancialLiteracy,
        Investing10Percent = 9,
        EarnMoreThanSpend = 10,
        CreateRecords70PercentAMonth = 11,

        //Gamification = 3
    }
    public class ProgressItemType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required]
        [MaxLength(32)]
        public string CodeName { get; set; }

    }
}
