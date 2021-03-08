using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
    public static class TelegramStatus
    {
        public static readonly string New = "New";
        public static readonly string Connected = "Connected";
        public static readonly string Locked = "Locked";

    }
    public class TelegramAccount
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [MaxLength(32)]
        public string Status { get; set; }
        public long TelegramID { get; set; }
        [MaxLength(512)]
        public string Username { get; set; }
        [MaxLength(512)]
        public string FirstName { get; set; }
        [MaxLength(512)]
        public string LastName { get; set; }
        [MaxLength(512)]
        public string Title { get; set; }
        [MaxLength(512)]
        public string Description { get; set; }
        [MaxLength(16)]
        public string LanguageCode { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime DateEdit { get; set; }
        public DateTime LastDateConnect { get; set; }

        /// <summary>
        /// UserID
        /// </summary>
        [ForeignKey("UserConnect")]
        public Guid? UserID { get; set; }
        
        public virtual UserConnect UserConnect { get; set; }

        public virtual IEnumerable<Notification> Notifications { get; set; }
        public virtual IEnumerable<ChatUser> ChatUsers { get; set; }


        public TelegramAccount()
        {
            this.Notifications = new HashSet<Notification>();
            this.ChatUsers = new HashSet<ChatUser>();
        }
    }
}
