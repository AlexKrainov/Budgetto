using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyProfile.Entity.ModelView.Notification
{
    public class NotificationViewModel :
        ILimitNotification,
        ITelegramNotification,
        IMailNotification,
        IReminderNotification
    {
        public long NotificationID { get; set; }
        public int NotificationTypeID { get; set; }
        public string NotifyType { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string Color { get; set; }
        public bool IsRead { get; set; }
        public string Name { get; set; }
        [JsonIgnore]
        public List<string> UserConnectionIDs { get; set; }
        public DateTime? ReadyDateTime { get; set; }
        [JsonIgnore]
        public Guid UserID { get; set; }

        #region Telegram
        [JsonIgnore]
        public string TelegramName { get; set; }
        [JsonIgnore]
        public string TelegramUserName { get; set; }
        [JsonIgnore]
        public List<TelegramAccountModelView> TelegramAccounts { get; set; }
        #endregion

        #region Limit
        [JsonIgnore]
        public long? LimitID { get; set; }
        [JsonIgnore]
        public string SpecificCulture { get; set; }
        public decimal? Total { get; set; }
        #endregion

        #region Mail
        [JsonIgnore]
        public string Email { get; set; }
        #endregion

        #region Reminder
        public DateTime? ExpirationDateTime { get; set; }
        public string Icon { get; set; }
        public DateTime? ReadDateTime { get; set; }
        public long? ReminderDateID { get; set; }
        public int ReminderUTCOffsetMinutes { get; set; }
        public int UserUTCOffsetMinutes { get; set; }
        #endregion
    }

    public interface ILimitNotification
    {
        [JsonIgnore]
        long? LimitID { get; set; }
        [JsonIgnore]
        string SpecificCulture { get; set; }
        decimal? Total { get; set; }
    }

    public interface ITelegramNotification
    {
        [JsonIgnore]
        string TelegramName { get; set; }
        [JsonIgnore]
        string TelegramUserName { get; set; }
        [JsonIgnore]
        List<TelegramAccountModelView> TelegramAccounts { get; set; }
    }

    public interface IMailNotification
    {
        [JsonIgnore]
        string Email { get; set; }
    }

    public interface IReminderNotification
    {
        DateTime? ExpirationDateTime { get; set; }
        string Icon { get; set; }
    }
    public class TelegramAccountModelView
    {
        public long ChatID { get; set; }
        public int TelegramBotChatUserID { get; set; }
        public string TelegramID { get; set; }
        public int StatusID { get; set; }
    }
}
