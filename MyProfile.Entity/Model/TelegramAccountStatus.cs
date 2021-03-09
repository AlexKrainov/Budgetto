using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
    public enum TelegramAccountStatusEnum
    {
        New = 1,
        Connected = 2,
        InPause = 3,
        Locked = 4,
    }
    public class TelegramAccountStatus
    {
        [Key]
        public int ID { get; set; }
        [MaxLength(16)]
        public string Name { get; set; }
        [MaxLength(16)]
        public string CodeName { get; set; }

        public virtual IEnumerable<TelegramAccount> TelegramAccounts { get; set; }


        public TelegramAccountStatus()
        {
            this.TelegramAccounts = new HashSet<TelegramAccount>();
        }
    }
}
