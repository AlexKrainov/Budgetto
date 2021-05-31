using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
	public enum BudgettoEntityType
	{
		Undefined,
		Limits = 1,
		Reminders = 2,
		ToDoLists = 3,
		Templates = 4,
	}
	public class EntityType
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ID { get; set; }
		[Required]
		[MaxLength(32)]
		public string CodeName { get; set; }
	}
}
