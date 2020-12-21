using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
    public class UserTag
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ID { get; set; }
		[Required]
		[MaxLength(132)]
		public string Title { get; set; }
		[MaxLength(256)]
		public string Image { get; set; }
		[MaxLength(32)]
		public string IconCss { get; set; }
		public DateTime DateCreate { get; set; }
        public bool IsDeleted { get; set; }


        [ForeignKey("Tag")]
		public int? TagID { get; set; }
		[ForeignKey("User")]
		public Guid? UserID { get; set; }

		public virtual User User { get; set; }
		public virtual Tag Tag { get; set; }

		public virtual ICollection<RecordTag> RecordTags { get; set; }

		public UserTag()
		{
			this.RecordTags = new HashSet<RecordTag>();
		}
	}
}
