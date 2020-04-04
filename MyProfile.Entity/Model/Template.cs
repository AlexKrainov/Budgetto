using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
	public class Template
	{
		public Template()
		{
			this.TemplateColumns = new HashSet<TemplateColumn>();
		}

		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ID { get; set; }
		[Required]
		public string Name { get; set; }
		public string Description { get; set; }
		[Required]
		public string CodeName { get; set; }
		/// <summary>
		///  if it is a month we will write 01.01.2020 00:00-31.01.2020 23:59
		/// if it is a Year we will write 01.01.2020 00:00 - 31.12.2020 23:59
		/// </summary>
		public string CurrentPeriod { get; set; }
		public DateTime DateCreate { get; set; }
		public DateTime DateEdit { get; set; }
		public DateTime? DateDelete { get; set; }
		public bool IsCountCollectiveBudget { get; set; }
		/// <summary>
		/// by default if period month 31 (depends what cinde of month), if year - 12
		/// </summary>
		public int MaxRowInAPage { get; set; }
		public bool IsDelete { get; set; }
		

		[ForeignKey("Person")]
		public Guid PersonID { get; set; }
		[ForeignKey("PeriodType")]
		public int PeriodTypeID { get; set; }


		public virtual Person Person { get; set; }
		public virtual PeriodType PeriodType { get; set; }

		public virtual IEnumerable<TemplateColumn> TemplateColumns { get; set; }
	}
}
