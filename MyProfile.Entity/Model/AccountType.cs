using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
	public enum AccountTypesEnum
	{
		Undefined,
		Cash = 1
	}
	public class AccountType
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ID { get; set; }
		[Required]
		[MaxLength(24)]
		public string Name { get; set; }
		[Required]
		[MaxLength(16)]
		public string CodeName { get; set; }
		[MaxLength(512)]
		public string Description { get; set; }
		[MaxLength(16)]
		public string Icon { get; set; }
	}
}
