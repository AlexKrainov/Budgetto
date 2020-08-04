using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
	public class Chat
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ID { get; set; }
		[Required]
		[MaxLength(512)]
		public string Title { get; set; }
		public DateTime DateCreate { get; set; }
		public DateTime DateEdit { get; set; }
		public bool IsDeleted { get; set; }

		public virtual ICollection<ChatUser> ChatUsers { get; set; }
		public virtual ICollection<Message> Messages { get; set; }

        public virtual Feedback Feedback { get; set; }

        public Chat()
		{
			this.ChatUsers = new HashSet<ChatUser>();
			this.Messages = new HashSet<Message>();
		}

	}
}
