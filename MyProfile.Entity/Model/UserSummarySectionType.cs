using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
    public class UserSummarySectionType
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ID { get; set; }

        [ForeignKey("UserSummary")]
		public int UserSummaryID { get; set; }
		[ForeignKey("SectionType")]
		public int SectionTypeID { get; set; }

		public virtual UserSummary UserSummary { get; set; }
		public virtual SectionType SectionType { get; set; }


	}
}
