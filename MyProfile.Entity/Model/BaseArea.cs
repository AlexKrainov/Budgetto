using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
    public class BaseArea
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ID { get; set; }
		[Required]
		[MaxLength(32)]
		public string Name { get; set; }
		[Required]
		[MaxLength(32)]
		public string CodeName { get; set; }

		public virtual ICollection<BaseSection> BaseSections { get; set; }
		public virtual ICollection<Card> Cards { get; set; }

		public BaseArea()
		{
			this.BaseSections = new HashSet<BaseSection>();
		}

	}
}
