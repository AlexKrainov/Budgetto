using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
	public class MccCode
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public long ID { get; set; }
		public int Mcc { get; set; }

		[ForeignKey("Company")]
		public int? CompanyID { get; set; }
		[ForeignKey("MccCategory")]
		public int MccCategoryID { get; set; }


		public virtual Company Company { get; set; }
		public virtual MccCategory MccCategory { get; set; }
	}
}
