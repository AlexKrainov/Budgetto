using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
    public enum SystemMailingType
    {
        Undefined = 0,
        FeedbackMonth = 1, //last day in a month
        NotActive1DayAfterRegistration = 2, //Not Enter a day after last session
        NotActive2DaysAfterRegistration = 3,
        NotActive3DaysAfterRegistration = 4,
        NotActive4DaysAfterRegistration = 5,
        NotActive5DaysAfterRegistration = 6,
        NotActive6DaysAfterRegistration = 7,
        NotActive7DaysAfterRegistration = 8,
        StatisticsWeek = 9,// statistic for last week (earnings, spendings, investings)
        StatisticsMonth = 10,
        NotEnterRecords2Days = 11,//
        NotEnterRecords3Days = 12,
        NotEnterRecords4Days = 13,
    }
    public class SystemMailing
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required, MaxLength(128)]
        public string Name { get; set; }
        [MaxLength(128)]
        public string Tooltip { get; set; }
        [Required, MaxLength(64)]
        public string CodeName { get; set; }
        public bool IsActive { get; set; }
        [MaxLength(16)]
        public string CronExpression { get; set; }
        [MaxLength(64)]
        public string CronComment { get; set; }
        public int? TotalMinutes { get; set; }
        public bool IsMail { get; set; }
        public bool IsTelegram { get; set; }
        public bool IsSite { get; set; }

    }
}
