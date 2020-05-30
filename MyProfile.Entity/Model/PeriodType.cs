using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
	public enum PeriodTypesEnum
	{
		Undefined,
		Days = 1,
		Weeks = 2,
		Months = 3,
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
