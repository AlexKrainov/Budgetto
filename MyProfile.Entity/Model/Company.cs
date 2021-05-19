using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
	public class Company
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ID { get; set; }
		[Required]
		[MaxLength(256)]
		public string Name { get; set; }
		[MaxLength(512)]
		public string TagKeyWords { get; set; }
		[MaxLength(512)]
		public string BankKeyWords { get; set; }
		[MaxLength(128)]
        public string Site { get; set; }
		public DateTime CreateDate { get; set; }
        public bool IsChecked { get; set; }
		[MaxLength(64)]
        public string Country { get; set; }
		[MaxLength(64)]
        public string City { get; set; }
		[MaxLength(8)]
		public string BrandColor { get; set; }
		[MaxLength(8)]
		public string TextColor { get; set; }
		[MaxLength(128)]
		public string LogoCircle { get; set; }
		[MaxLength(128)]
        public string LogoSquare { get; set; }
		/// <summary>
		/// id of Tff brand.id
		/// </summary>
        public string t_objectID { get; set; }


        [ForeignKey("ParentCompany")]
		public int? ParentCompanyID { get; set; }

		public virtual Company ParentCompany { get; set; }
		
		public virtual ICollection<UserTag> UserTags { get; set; }

		public Company()
		{
			this.UserTags = new HashSet<UserTag>();
		}

	}
}
