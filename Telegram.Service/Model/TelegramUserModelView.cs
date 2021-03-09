using System;
using System.Collections.Generic;
using System.Text;

namespace Telegram.Service.Model
{
    public class TelegramUserModelView
    {
        public Guid? UserID { get; set; }
        public string TelegramLogin { get; set; }
        public int StatusID { get; set; }
        public int ChatID { get; set; }
        public int ChatUserID { get; set; }
        public int ChatTelegramBotUserID { get; set; }
        public int TelegramAccountID { get; internal set; }
        public string UserName { get; internal set; }
    }
}
