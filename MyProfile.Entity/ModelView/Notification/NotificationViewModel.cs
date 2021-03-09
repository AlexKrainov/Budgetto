using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyProfile.Entity.ModelView.Notification
{
    public class NotificationViewModel
    {
        public string Icon { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string Color { get; set; }
        public int NotificationID { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ExpirationDateTime { get; set; }
        public decimal? Total { get; set; }
        public int NotificationTypeID { get; set; }
        public string Name { get; set; }
        [JsonIgnore]
        public List<string> UserConnectionIDs { get; set; }
        [JsonIgnore]
        public string SpecificCulture { get; set; }
        public DateTime? ReadyDateTime { get; set; }
        public List<TelegramAccountModelView> TelegramAccounts { get; set; }
        [JsonIgnore]
        public Guid UserID { get; set; }
        [JsonIgnore]
        public string Email { get; set; }
        [JsonIgnore]
        public int? LimitID { get; set; }
    }
    public class TelegramAccountModelView
    {
        public int ChatID { get; set; }
        public int TelegramBotChatUserID { get; set; }
        public string TelegramID { get; set; }
    }
}
