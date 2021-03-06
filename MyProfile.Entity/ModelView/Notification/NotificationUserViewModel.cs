using System;

namespace MyProfile.Entity.ModelView.Notification
{
    public class NotificationUserViewModel
    {
        public int ID { get; set; }
        public bool IsSite { get; set; }
        public bool IsMail { get; set; }
        public bool IsTelegram { get; set; }
        public decimal Price { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime ExpirationDateTime { get; set; }
        public string Icon { get; set; }
    }
}
