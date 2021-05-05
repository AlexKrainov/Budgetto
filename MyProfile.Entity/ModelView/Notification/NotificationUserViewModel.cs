using System;

namespace MyProfile.Entity.ModelView.Notification
{
    public class NotificationUserViewModel
    {
        public long ID { get; set; }
        public bool IsSite { get; set; }
        public bool IsMail { get; set; }
        public bool IsTelegram { get; set; }
        public decimal Price { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? ExpirationDateTime { get; set; }
        public DateTime? ExpirationDateTimeUTC
        {
            get
            {
                if (ExpirationDateTime.HasValue)
                {
                    return ExpirationDateTime.Value.ToUniversalTime();
                }
                return null;
            }
        }
        public bool IsRepeat { get; set; }
        public int RepeatMinutes { get; set; }
    }
}
