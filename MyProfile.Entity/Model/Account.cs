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
        public decimal? InterestRate { get; set; }
        public decimal? CachbackForAllPercent { get; set; }
        public bool IsCachback { get; set; }
		/// <summary>
		/// Is cachback return money or rocket-ruble or miles and etc/
		/// </summary>
        public bool IsCachbackMoney { get; set; }
        public bool IsOverdraft { get; set; }
        public bool IsDefault { get; set; }
        public bool IsHide { get; set; }
        public bool IsDeleted { get; set; }
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


		public virtual User User { get; set; }
		public virtual AccountType AccountType { get; set; }
		public virtual Currency Currency { get; set; }
		public virtual Bank Bank { get; set; }

		public virtual ICollection<BudgetRecord> BudgetRecords { get; set; }
        public virtual ICollection<AccountRecordHistory> AccountRecordHistories { get; set; }

        public Account()
		{
			this.BudgetRecords = new HashSet<BudgetRecord>();
            this.AccountRecordHistories = new HashSet<AccountRecordHistory>();
        }

	}
}
