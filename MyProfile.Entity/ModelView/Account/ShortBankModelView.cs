using System;
using System.Collections.Generic;
using System.Text;

namespace MyProfile.Entity.ModelView.Account
{
    public class ShortBankModelView : Select2ModelView
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string LogoCircle { get; set; }
        public string LogoRectangle { get; set; }
        public int? BankTypeID { get; set; }
    }
}
