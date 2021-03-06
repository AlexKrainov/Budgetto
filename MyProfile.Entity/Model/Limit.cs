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
        public bool IsFinished { get; set; }
        public bool IsDeleted { get; set; }
		/// <summary>
		/// Created by constructor after registration
		/// </summary>
		public bool IsCreatedByConstructor { get; set; }

		[ForeignKey("User")]
		public Guid UserID { get; set; }
		[ForeignKey("PeriodType")]
		public int PeriodTypeID { get; set; }
		[ForeignKey("VisibleElement")]
		public int VisibleElementID { get; set; }

		public virtual User User { get; set; }
		public virtual PeriodType PeriodType { get; set; }
		public virtual VisibleElement VisibleElement { get; set; }

		public virtual ICollection<SectionGroupLimit> SectionGroupLimits { get; set; }
		public virtual ICollection<Notification> Notifications { get; set; }

		public Limit()
		{
			this.SectionGroupLimits = new HashSet<SectionGroupLimit>();
			this.Notifications = new HashSet<Notification>();
		}

	}
}
