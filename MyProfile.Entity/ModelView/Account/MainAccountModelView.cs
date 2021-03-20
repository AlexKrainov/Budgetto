using MyProfile.Entity.Model;
using System.Collections.Generic;

namespace MyProfile.Entity.ModelView.Account
{
    public class MainAccountModelView
    {
        public int ID { get; set; }
        public AccountTypes AccountType { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }
        public decimal CachBackBalance { get; set; }
        public string Description { get; set; }
        public bool IsHide { get; set; }
        public int? BankID { get; set; }
        public string BankLogo { get; set; }
        public string BankName { get; set; }
        public string AccountIcon { get; set; }
        public bool IsPast { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsSVG
        {
            get
            {
                return !string.IsNullOrEmpty(BankLogo) ? BankLogo.Contains("svg") : false;
            }
        }
        public bool IsCash
        {
            get
            {
                return AccountTypes.Cash == AccountType;
            }
        }
        public List<AccountViewModel> Accounts { get; set; }
        public int? BankTypeID { get; set; }
        public bool IsShow { get; set; }
    }
}
