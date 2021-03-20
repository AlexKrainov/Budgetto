using System.Collections.Generic;

namespace MyProfile.Entity.ModelView.Account
{
    public class ShortBankTypeModelView
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string CodeName { get; set; }
        public List<AccountTypeModelView> AccountTypes { get; set; }
    }
}
