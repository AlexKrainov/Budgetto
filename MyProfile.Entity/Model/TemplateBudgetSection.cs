using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
	public class TemplateBudgetSection
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public long ID { get; set; }

		[ForeignKey("BudgetSection")]
		public long BudgetSectionID { get; set; }
		[ForeignKey("TemplateColumn")]
		public long TemplateColumnID { get; set; }

		public virtual BudgetSection BudgetSection { get; set; }
		public virtual TemplateColumn TemplateColumn { get; set; }
	}
}
