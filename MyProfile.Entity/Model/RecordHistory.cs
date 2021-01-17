using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
    public class ActionTypeCode
    {
       public static readonly string Create = "Create";
       public static readonly string Edit = "Edit";
       public static readonly string Delete = "Delete";
       public static readonly string Recovery = "Recovery";
    }

    public class RecordHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required]
        public string ActionTypeCode { get; set; }
        [Required]
        [Column(TypeName = "Money")]
        public decimal RecordTotal { get; set; }
        [Column(TypeName = "Money")]
        public decimal RecordCachback { get; set; }

        /// <summary>
        /// new value after action
        /// </summary>
        [Column(TypeName = "Money")]
        public decimal AccountNewBalance { get; set; }
        /// <summary>
        /// new value after action
        /// </summary>
        [Column(TypeName = "Money")]
        public decimal AccountNewBalanceCashback { get; set; }
        /// <summary>
        /// how much was written off
        /// </summary>
        [Column(TypeName = "Money")]
        public decimal AccountTotal { get; set; }
        /// <summary>
        /// how much was written off
        /// </summary>
        [Column(TypeName = "Money")]
        public decimal AccountCashback { get; set; }

        [Column(TypeName = "Money")]
        public decimal? RacordCurrencyRate { get; set; }
        public int RecordCurrencyNominal { get; set; }

        [Column(TypeName = "Money")]
        public decimal? AccountCurrencyRate { get; set; }
        public int AccountCurrencyNominal { get; set; }

        [MaxLength(264)]
        public string Comment { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime DateTimeOfPayment { get; set; }


        [ForeignKey("Record")]
        public int RecordID { get; set; }
        [ForeignKey("Account")]
        public int? AccountID { get; set; }
        [ForeignKey("RecordCurrency")]
        public int? RecordCurrencyID { get; set; }
        [ForeignKey("AccountCurrency")]
        public int? AccountCurrencyID { get; set; }
        [ForeignKey("Section")]
        public int? SectionID { get; set; }


        public virtual Record Record { get; set; }
        public virtual Account Account { get; set; }
        public virtual Currency RecordCurrency { get; set; }
        public virtual Currency AccountCurrency { get; set; }
        public virtual BudgetSection Section { get; set; }
    }
}
