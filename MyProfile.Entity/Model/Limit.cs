using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
	public class Limit
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ID { get; set; }
		[Required]
		public string Name { get; set; }
		public string Description { get; set; }
		[Column(TypeName = "Money")]
		public decimal LimitMoney { get; set; }
		public DateTime? DateStart { get; set; }
		public DateTime? DateEnd { get; set; }
		/// <summary>
		/// Show or not in dashbord
		/// </summary>
		public bool IsShow { get; set; }
		public bool IsShowInCollective { get; set; } = true;
		public bool IsDeleted { get; set; }

		[ForeignKey("User")]
		public Guid UserID { get; set; }
		[ForeignKey("PeriodType")]
		public int PeriodTypeID { get; set; }

		public virtual User User { get; set; }
		public virtual PeriodType PeriodType { get; set; }

		public virtual ICollection<SectionGroupLimit> SectionGroupLimits { get; set; }

		public Limit()
		{
			this.SectionGroupLimits = new HashSet<SectionGroupLimit>();
		}

	}
}
