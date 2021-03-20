using MyProfile.Entity.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyProfile.Entity.ModelView.Account
{
    public class AccountTypeModelView
    {
        public AccountTypes accountType;

        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public IEnumerable<ShortBankModelView> Banks { get; set; }
    }
}
