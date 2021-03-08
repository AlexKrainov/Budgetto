using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
	public class Message
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ID { get; set; }
		public string Text { get; set; }
		public DateTime DateCreate { get; set; }
		public DateTime? DateEdit { get; set; }
		public bool IsDeleted { get; set; }

		[ForeignKey("Chat")]
		public int ChatID { get; set; }
		[ForeignKey("ChatUser")]
		public int? ChatUserID { get; set; }

		public virtual ChatUser ChatUser { get; set; }
		public virtual Chat Chat { get; set; }

		public virtual ICollection<ResourceMessage> ResourceMessages { get; set; }

		public Message()
		{
			this.ResourceMessages = new HashSet<ResourceMessage>();
		}

	}
}
