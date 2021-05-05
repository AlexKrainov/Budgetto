using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
    public class ChatUser
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public long ID { get; set; }
        public bool IsChatOwner { get; set; }
		/// <summary>
		/// Left the chat
		/// </summary>
        public bool Left { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime? DateLeft { get; set; }
        public bool IsMute { get; set; }

        [ForeignKey("Chat")]
		public long ChatID { get; set; }
		[ForeignKey("User")]
		public Guid? UserID { get; set; }
		[ForeignKey("TelegramAccount")]
		public int? TelegramAccountID { get; set; }


		public virtual User User { get; set; }
		public virtual Chat Chat { get; set; }
		public virtual TelegramAccount TelegramAccount { get; set; }

	}
}
