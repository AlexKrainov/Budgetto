using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView.Currency;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace MyProfile.Entity.ModelView.Account
{
    public class AccountViewModel
    {
        public int ID { get; set; }
        public int? ParentID { get; set; }
        public AccountTypes AccountType { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }
        public string BalanceString { get
            {
                return Balance.ToString("C0", CultureInfo.CreateSpecificCulture(Currency.specificCulture));
            }
        }
        public decimal CachBackBalance { get; set; }
        public string CachBackBalanceString
        {
            get
            {
                return CachBackBalance.ToString("C0", CultureInfo.CreateSpecificCulture(Currency.specificCulture));
            }
        }
        public string Description { get; set; }
        public decimal? InterestRate { get; set; }
        public decimal? CachBackForAllPercent { get; set; }
        public bool IsCachback { get; set; }
        /// <summary>
        /// Is cachback return money or rocket-ruble or miles and etc/
        /// </summary>
        public bool IsCachBackMoney { get; set; }
        public bool IsOverdraft { get; set; }
        public bool IsDefault { get; set; }
        public bool IsHideCurrentAccount { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsCash { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public DateTime? ResetCashBackDate { get; set; }

        public int CurrencyID { get; set; } = 1;// ruble
        public int? BankID { get; set; }
        public int? BankTypeID { get; set; }

        public string BankLogo { get; set; }
        public string BankName { get; set; }
        public string AccountIcon { get; set; }
        public string AccountTypeName { get; set; }
        public bool IsShow { get; set; }
        /// <summary>
        /// For Transfer money
        /// </summary>
        public bool IsSelected { get; set; }
        /// <summary>
        /// For Transfer money
        /// </summary>
        public bool IsDisabled { get; set; }

        public CurrencyClientModelView Currency { get; set; }
        public string CurrencyIcon { get; set; }
        public decimal BalanceEarnings { get; set; }
        public decimal BalanceInvestments { get; set; }
        public bool IsPast { get; set; }
        public decimal BalanceSpendings { get; set; }
        public decimal BalancePastCachback { get; set; }
        public bool IsCountTheBalance { get; set; }
        public int? PaymentSystemID { get; set; }
        public DateTime? ResetCachbackDate { get; set; }
        public DateTime? DateStart { get; set; }
        public double? Percent { get; set; }
        public decimal? Input { get; set; }
        public decimal? Output { get; set; }
        public bool IsCountBalanceInMainAccount { get; set; }
        public string PaymentLogo { get; set; }
        public int? CardID { get; set; }
        public string CardName { get; set; }
        public string CardLogo { get; set; }
        public int TimeListID { get; set; }
        public decimal? InterestBalanceForEnd { get; set; }
        public bool IsFinishedDeposit { get; set; }
        public decimal? InterestBalance { get; set; }
        public decimal? InterestBalanceForPeriod { get; set; }
        public bool IsCapitalization { get; set; }
    }
}
