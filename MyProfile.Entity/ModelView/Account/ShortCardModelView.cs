using MyProfile.Entity.Model;

namespace MyProfile.Entity.ModelView.Account
{
    public class ShortCardModelView : Select2ModelView
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Logo { get; set; }
        public AccountTypes AccountType { get; set; }
        public string AccountTypeName { get; set; }
        public int BankID { get; set; }
        public string BankName { get; set; }
        public string BankLogoRectangle { get; set; }
    }
}
