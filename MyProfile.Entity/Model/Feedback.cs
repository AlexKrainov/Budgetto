using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
    public class Feedback
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ID { get; set; }
		[Required]
		[MaxLength(32)]
		public string Status { get; set; }
		[Required]
		[MaxLength(128)]
		public string Topic { get; set; }
		public int Priority { get; set; }
        public int MoodID { get; set; }


        [ForeignKey("Chat")]
		public int ChatID { get; set; }

		public virtual Chat Chat { get; set; }

	}
}
