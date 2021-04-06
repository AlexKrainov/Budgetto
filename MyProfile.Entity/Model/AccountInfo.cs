using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
    public enum TimeList
    {
        Undefined = 0,
        Daily = 1,
        Weekly = 2,
        Monthly = 3,
        Quarterly = 4,
        HalfYearly = 5,
        Yearly = 6
    }

    public class AccountInfo
    {
        [Key]
        [ForeignKey("Account")]
        public int AccountID { get; set; }

        #region Deposit
        public decimal? InterestRate { get; set; }
        public decimal? InterestBalance { get; set; }
        /// <summary>
        /// Последняя дата начисления процентов
        /// </summary>
        public DateTime? LastInterestAccrualDate { get; set; }
        /// <summary>
        /// Следующая дата начисления процентов
        /// </summary>
        public DateTime? InterestNextDate { get; set; }
        /// <summary>
        /// TimeList (Enum)
        /// </summary>
        public int CapitalizationTimeListID { get; set; }
        public bool IsFinishedDeposit { get; set; }
        public decimal? InterestBalanceForEndOfDeposit { get; set; }
        public bool IsCapitalization { get; set; }
        #endregion


        public virtual Account Account { get; set; }

    }
}
