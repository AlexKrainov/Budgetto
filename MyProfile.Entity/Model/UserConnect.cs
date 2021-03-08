using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
    public class UserConnect
    {
        [JsonIgnore]
        [Key]
        [ForeignKey("User")]
        public Guid UserID { get; set; }
        [MaxLength(64)]
        public string TelegramLogin { get; set; }

        public virtual User User { get; set; }

        public virtual IEnumerable<HubConnect> HubConnects { get; set; }
        public virtual IEnumerable<TelegramAccount> TelegramAccounts { get; set; }

        public UserConnect()
        {
            this.HubConnects = new HashSet<HubConnect>();
            this.TelegramAccounts = new HashSet<TelegramAccount>();
        }
    }
}
