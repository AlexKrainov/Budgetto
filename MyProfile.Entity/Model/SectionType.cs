using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
    public enum SectionTypeEnum
    {
        Earnings = 1,
        Spendings = 2,
        //Investments = 3
    }
    public class SectionType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        /// <summary>
        /// Income, Consumption
        /// </summary>
        [Required, MaxLength(16)]
        public string Name { get; set; }
        /// <summary>
        /// Income, Consumption
        /// </summary>
        [Required, MaxLength(16)]
        public string CodeName { get; set; }

        //public virtual IEnumerable<BudgetSection> BudgetSections { get; set; }
        //public virtual IEnumerable<SectionTypeView> SectionTypeViews { get; set; }

        //public SectionType()
        //{
        //	this.BudgetSections = new HashSet<BudgetSection>();
        //	this.SectionTypeViews = new HashSet<SectionTypeView>();

        //}
    }
}
