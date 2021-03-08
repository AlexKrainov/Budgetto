using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
    public enum NotificationType
    {
        Undefined = 0,
        Limit,
        Reminder,
        Telegram,
        Notification,
    }
    public class Notification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int NotificationTypeID { get; set; }
        public bool IsReady { get; set; }
        public DateTime? IsReadyDateTime { get; set; }
        public bool IsSentOnSite { get; set; }
        public bool IsSentOnTelegram { get; set; }
        public bool IsSentOnMail { get; set; }
        public bool IsDone { get; set; }
        public DateTime LastChangeDateTime { get; set; }

        public bool IsRead { get; set; }
        public DateTime? ReadDateTime { get; set; }

        public bool IsSite { get; set; }
        public bool IsMail { get; set; }
        public bool IsTelegram { get; set; }

        /// <summary>
        /// For limit
        /// </summary>
        [Column(TypeName = "Money")]
        public decimal? Total { get; set; }
        /// <summary>
        /// Reminder
        /// </summary>
        public DateTime? ExpirationDateTime { get; set; }
        /// <summary>
        /// Telegram
        /// </summary>
        public string Value { get; set; }
        [MaxLength(64)]
        public string Icon { get; set; }

        [ForeignKey("User")]
        public Guid UserID { get; set; }
        [ForeignKey("Limit")]
        public int? LimitID { get; set; }
        [ForeignKey("Reminder")]
        public int? ReminderID { get; set; }
        [ForeignKey("TelegramAccount")]
        public int? TelegramAccountID { get; set; }

        public virtual User User { get; set; }
        public virtual Limit Limit{ get; set; }
        public virtual Reminder Reminder { get; set; }
        /// <summary>
        /// Notify when the user has connect to telegram bot
        /// </summary>
        public virtual TelegramAccount TelegramAccount { get; set; }

    }
}
