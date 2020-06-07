using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
	public enum PeriodTypesEnum
	{
		Undefined,
		Month = 1,
		Weeks = 2,
		Year = 3,
		Years10 = 4
	}
	public class PeriodType
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ID { get; set; }
		[Required]
		public string Name { get; set; }
		[Required]
		public string CodeName { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public bool IsUsing { get; set; }
	}
}
