using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
	public class UserSummary
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ID { get; set; }
		[Required]
		[MaxLength(256)]
		public string Name { get; set; }
		public string Value { get; set; }
		public DateTime CurrentDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsChart { get; set; }

        [ForeignKey("VisibleElement")]
		public long? VisibleElementID { get; set; }
		[ForeignKey("User")]
		public Guid UserID { get; set; }
		[ForeignKey("Summary")]
		public int SummaryID { get; set; }

		public virtual VisibleElement VisibleElement { get; set; }
		public virtual User User { get; set; }
		public virtual Summary Summary { get; set; }

		public virtual ICollection<UserSummarySection> Sections { get; set; }

        public UserSummary()
		{
			this.Sections = new HashSet<UserSummarySection>();
        }

	}
}
