using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
	public class Chart
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ID { get; set; }
		[Required]
		[MaxLength(32)]
		public string Name { get; set; }
		public string Description { get; set; }
		public DateTime DateCreate { get; set; }
		public DateTime LastDateEdit { get; set; }
		public bool IsDeleted { get; set; }

		[ForeignKey("User")]
		public Guid UserID { get; set; }
		[ForeignKey("PeriodType")]
		public int PeriodTypeID { get; set; }
		[ForeignKey("ChartType")]
		public int ChartTypeID { get; set; }
		[ForeignKey("VisibleElement")]
		public int VisibleElementID { get; set; }

		public virtual User User { get; set; }
		public virtual PeriodType PeriodType { get; set; }
		public virtual ChartType ChartType { get; set; }
		public virtual VisibleElement VisibleElement { get; set; }

		public virtual ICollection<ChartField> ChartFields { get; set; }

		public Chart()
		{
			this.ChartFields = new HashSet<ChartField>();
		}

	}
}
