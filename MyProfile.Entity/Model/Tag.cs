using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
	/// <summary>
	/// Default Tag for all users
	/// If user want to use, he can copy from this
	/// </summary>
    public class Tag
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
	}
}
