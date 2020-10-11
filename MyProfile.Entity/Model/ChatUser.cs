﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
    public class ChatUser
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ID { get; set; }
        public bool IsChatOwner { get; set; }
		/// <summary>
		/// Left the chat
		/// </summary>
        public bool Left { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime? DateLeft { get; set; }
        public bool IsMute { get; set; }

        [ForeignKey("Chat")]
		public int ChatID { get; set; }
		[ForeignKey("User")]
		public Guid UserID { get; set; }

		public virtual User User { get; set; }
		public virtual Chat Chat { get; set; }

	}
}