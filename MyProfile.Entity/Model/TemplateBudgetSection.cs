using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
	public class TemplateBudgetSection
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ID { get; set; }

		[ForeignKey("BudgetSection")]
		public int BudgetSectionID { get; set; }
		[ForeignKey("TemplateColumn")]
		public int TemplateColumnID { get; set; }

		public virtual BudgetSection BudgetSection { get; set; }
		public virtual TemplateColumn TemplateColumn { get; set; }
	}
}
