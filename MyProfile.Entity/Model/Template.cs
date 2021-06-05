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
		public long ID { get; set; }
		[Required]
		public string Name { get; set; }
		public string Description { get; set; }
		/// <summary>
		///  if it is a month we will write 01.01.2020 00:00-31.01.2020 23:59
		/// if it is a Year we will write 01.01.2020 00:00 - 31.12.2020 23:59
		/// </summary>
		public string CurrentPeriod { get; set; }
		public DateTime DateCreate { get; set; }
		public DateTime DateEdit { get; set; }
		public DateTime? LastSeenDate { get; set; }
		public DateTime? DateDelete { get; set; }
		public bool IsCountCollectiveBudget { get; set; }
		/// <summary>
		/// by default if period month 31 (depends what cinde of month), if year - 12
		/// </summary>
		public int MaxRowInAPage { get; set; }
		public bool IsDeleted { get; set; }
		/// <summary>
		/// Default template in PeriodType
		/// </summary>
		public bool IsDefault { get; set; }
		/// <summary>
		/// Created by constructor after registration
		/// </summary>
		public bool IsCreatedByConstructor { get; set; }
        public bool IsCreatedByPrepared { get; set; }



        [ForeignKey("User")]
		public Guid UserID { get; set; }
		[ForeignKey("PeriodType")]
		public int PeriodTypeID { get; set; }


		public virtual User User { get; set; }
		public virtual PeriodType PeriodType { get; set; }

		public virtual IEnumerable<TemplateColumn> TemplateColumns { get; set; }
	}
}
