using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
    public class SectionGroupChart
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ID { get; set; }
		
		[ForeignKey("ChartField")]
		public int ChartFieldID { get; set; }
		[ForeignKey("BudgetSection")]
		public int BudgetSectionID { get; set; }

		public virtual ChartField ChartField { get; set; }
		public virtual BudgetSection BudgetSection { get; set; }

		
	}
}
