using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView.Currency;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyProfile.Entity.ModelView.Account
{
    public class AccountViewModel
    {
        public int ID { get; set; }
        public AccountTypesEnum AccountType { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }
        public decimal CachBackBalance { get; set; }
        public string Description { get; set; }
        public decimal? InterestRate { get; set; }
        public decimal? CashBackForAllPercent { get; set; }
        public bool IsCachback { get; set; }
        /// <summary>
        /// Is cachback return money or rocket-ruble or miles and etc/
        /// </summary>
        public bool IsCachBackMoney { get; set; }
        public bool IsOverdraft { get; set; }
        public bool IsDefault { get; set; }
        public bool IsHide { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public DateTime? ResetCashBackDate { get; set; }

        public int CurrencyID { get; set; } = 1;// ruble
        public int? BankID { get; set; }
        public string BankImage { get; set; }
        public string BankName { get; set; }
        public string AccountIcon { get; set; }
        public string AccountTypeName { get; set; }
        public bool IsShow { get; set; }

        public CurrencyClientModelView Currency { get; set; }
        public string CurrencyIcon { get; set; }
        public decimal BalanceEarnings { get; set; }
        public decimal BalanceInvestments { get; set; }
        public bool IsPast { get; set; }
        public decimal BalanceSpendings { get; set; }
        public decimal BalancePastCachback { get; set; }
    }
}
