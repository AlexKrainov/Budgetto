using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView.Currency;
using System.Collections.Generic;
using System.Globalization;

namespace MyProfile.Entity.ModelView.Account
{
    public class MainAccountModelView
    {
        public long ID { get; set; }
        public AccountTypes AccountType { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }
        public string BalanceString
        {
            get
            {
                return Balance.ToString("C0", CultureInfo.CreateSpecificCulture(Currency.specificCulture));
            }
        }
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
        public bool IsHideCurrentAccount { get; set; }
        public int? CurrencyID { get; set; }
        public CurrencyClientModelView _currency { get; set; }
        public CurrencyClientModelView Currency
        {
            get
            {
                if (_currency != null)
                {
                    return _currency;
                }
                else
                {
                    return new CurrencyClientModelView
                    {
                        id = 1,
                        codeName = "RUB",
                        specificCulture = "ru-RU",
                        icon = "₽",
                    };
                }
            }
            set
            {
                _currency = value;
            }
        }
        public bool ConvertError { get; set; }
    }
}
