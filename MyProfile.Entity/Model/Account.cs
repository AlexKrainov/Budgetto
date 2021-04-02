using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
	public class Account
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ID { get; set; }
		[Required]
		[MaxLength(128)]
		public string Name { get; set; }
		[Required]
		[Column(TypeName = "Money")]
		public decimal Balance { get; set; }
		[Column(TypeName = "Money")]
		public decimal CachbackBalance { get; set; }

		[MaxLength(264)]
		public string Description { get; set; }
        public decimal? CachbackForAllPercent { get; set; }
        public bool IsCachback { get; set; }
		/// <summary>
		/// Is cachback return money or rocket-ruble or miles and etc/
		/// </summary>
        public bool IsCachbackMoney { get; set; }
        public bool IsOverdraft { get; set; }
        public bool IsDefault { get; set; }
		/// <summary>
		/// Count this balance with all accounts.balance
		/// </summary>
        public bool IsCountTheBalance { get; set; }
		/// <summary>
		/// Count this balance in a main account
		/// </summary>
        public bool IsCountBalanceInMainAccount { get; set; }
        public bool IsHide { get; set; }
        public bool IsDeleted { get; set; }
		/// <summary>
		/// Start period
		/// </summary>
        public DateTime? DateStart { get; set; }
		/// <summary>
		/// End period
		/// </summary>
        public DateTime? ExpirationDate { get; set; }


		public DateTime? ResetCachbackDate { get; set; }
		public DateTime DateCreate { get; set; }
		public DateTime LastChanges { get; set; }


		[ForeignKey("AccountType")]
		public int AccountTypeID { get; set; }
		[ForeignKey("User")]
		public Guid UserID { get; set; }
		[ForeignKey("Currency")]
		public int? CurrencyID { get; set; }
		[ForeignKey("Bank")]
		public int? BankID { get; set; }
		[ForeignKey("PaymentSystem")]
		public int? PaymentSystemID { get; set; }
		[ForeignKey("ParentAccount")]
		public int? ParentAccountID { get; set; }
		[ForeignKey("Card")]
		public int? CardID { get; set; }


		public virtual User User { get; set; }
		public virtual AccountType AccountType { get; set; }
		public virtual Currency Currency { get; set; }
		public virtual Bank Bank { get; set; }
		public virtual PaymentSystem PaymentSystem { get; set; }
		public virtual Account ParentAccount { get; set; }
		public virtual Card Card { get; set; }
		public virtual AccountInfo AccountInfo { get; set; }

		public virtual ICollection<Record> BudgetRecords { get; set; }
        public virtual ICollection<RecordHistory> RecordHistories { get; set; }
        public virtual ICollection<AccountHistory> AccountHistories { get; set; }
        public virtual ICollection<AccountHistory> AccountHistories2 { get; set; }
		public virtual ICollection<Account> ChildAccounts { get; set; }

		public Account()
		{
			this.BudgetRecords = new HashSet<Record>();
            this.RecordHistories = new HashSet<RecordHistory>();
            this.AccountHistories = new HashSet<AccountHistory>();
            this.AccountHistories2 = new HashSet<AccountHistory>();
            this.ChildAccounts = new HashSet<Account>();
        }

	}
}
