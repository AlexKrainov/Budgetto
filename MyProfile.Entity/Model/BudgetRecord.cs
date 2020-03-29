using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
	public class BudgetRecord
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ID { get; set; }
		[Required]
		[Column(TypeName = "Money")]
		public decimal Total { get; set; }
		/// <summary>
		/// It needs to understand what data an user wrote
		/// </summary>
		public string RawData { get; set; } 
		public string Description { get; set; }
		public DateTime DateTimeOfPayment { get; set; }
		public DateTime DateTimeCreate { get; set; }
		public DateTime DateTimeEdit { get; set; }
		public DateTime? DateTimeDelete { get; set; }
		public bool IsDeleted { get; set; }
		/// <summary>
		/// Consider when count or not
		/// </summary>
		public bool IsConsider { get; set; }

		[ForeignKey("Person")]
		public Guid PersonID { get; set; }
		[ForeignKey("BudgetSection")]
		public int BudgetSectionID { get; set; }

		public virtual Person Person { get; set; }
		public virtual BudgetSection BudgetSection { get; set; }
	}
}
