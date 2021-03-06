using System;
using System.Collections.Generic;
using System.Text;

namespace MyProfile.Entity.ModelView.Login
{
    public class LoginModelView
    {
        public Guid UserSessionID { get; set; }
        public string Email { get; set; }

        public LoginViewModel loginView { get; set; }
    }

    public class LoginViewModel
    {
        public int ID { get; set; }
        public Guid MailLogID { get; set; }
        public Guid UserID { get; set; }
    }
}
