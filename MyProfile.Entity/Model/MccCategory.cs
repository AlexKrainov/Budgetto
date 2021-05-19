using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
	public class MccCategory
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ID { get; set; }
		[MaxLength(64)]
		public string Name { get; set; }
        public bool IsSystem { get; set; }
        public int? bankCategoryID { get; set; }
        public int? bankParentCategoryID { get; set; }

        [ForeignKey("Bank")]
		public int BankID { get; set; }

		public virtual Bank Bank { get; set; }

		public virtual ICollection<MccCode> MccCodes { get; set; }

		public MccCategory()
		{
			this.MccCodes = new HashSet<MccCode>();
		}
	}
}
