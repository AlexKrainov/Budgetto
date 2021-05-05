using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
    public class UserSummarySection
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ID { get; set; }

        [ForeignKey("UserSummary")]
		public int UserSummaryID { get; set; }
		[ForeignKey("Section")]
		public long SectionID { get; set; }

		public virtual UserSummary UserSummary { get; set; }
		public virtual BudgetSection Section { get; set; }


	}
}
