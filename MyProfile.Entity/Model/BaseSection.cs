using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
    public class BaseSection
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required]
        [MaxLength(128)]
        public string Name { get; set; }
        [Required]
        [MaxLength(32)]
        public string CodeName { get; set; }
        [Required]
        public string KeyWords { get; set; }
        [MaxLength(32)]
        public string Color { get; set; }
        [MaxLength(32)]
        public string Background { get; set; }
        [MaxLength(64)]
        public string Icon { get; set; }


        [ForeignKey("BaseArea")]
        public int BaseAreaID { get; set; }
        [ForeignKey("SectionType")]
        public int SectionTypeID { get; set; }

        public virtual BaseArea BaseArea { get; set; }
        public virtual SectionType SectionType { get; set; }

        //public virtual ICollection<BudgetSection> BudgetSections { get; set; }

        //public BaseSection()
        //{
        //    this.BudgetSections = new HashSet<BudgetSection>();
        //}

    }
}
