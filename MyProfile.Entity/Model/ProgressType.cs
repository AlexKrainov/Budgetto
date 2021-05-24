using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
    public enum ProgressTypeEnum
    {
        Introductory = 1,
        FinancialLiteracyMonth = 2,
        Gamification = 3
    }
    public class ProgressType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required]
        [MaxLength(32)]
        public string CodeName { get; set; }
    }
}
