using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
	public enum ChartTypesEnum
	{
		Undefined,
		Line = 1,
		Bar,
		Pie,
		Doughnut,
		Bubble,
		GroupedBar
	}
	public class ChartType
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
		/// <summary>
		/// 
		/// </summary>
		public bool IsUsing { get; set; }
	}
}
