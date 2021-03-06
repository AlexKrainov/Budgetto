using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
    public class AccountHistoryActionType
    {
        public static readonly string Create = "Create";
        public static readonly string Edit = "Edit";
        public static readonly string Toggle = "Toggle";
        public static readonly string Delete = "Delete";
        public static readonly string Recovery = "Recovery";
        public static readonly string MoveMoney = "MoveMoney";
        public static readonly string ResetCacback = "ResetCacback";
        public static readonly string AddedPercent = "AddedPercent";
    }
    public class AccountHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        [Required]
        [MaxLength(16)]
        public string ActionType { get; set; }
        /// <summary>
        /// All actions by this row
        /// </summary>
        [MaxLength(1024)]
        public string Actions { get; set; }
        public string OldAccountStateJson { get; set; }
        public string NewAccountStateJson { get; set; }

        [Column(TypeName = "Money")]
        public decimal? ValueTo { get; set; }
        [Column(TypeName = "Money")]
        public decimal? ValueFrom { get; set; }
        [Column(TypeName = "Money")]
        public decimal? OldBalance { get; set; }
        [Column(TypeName = "Money")]
        public decimal? NewBalance { get; set; }
        [Column(TypeName = "Money")]
        public decimal? CurrencyValue { get; set; }
        [Column(TypeName = "Money")]
        public decimal? CachbackBalance { get; set; }

        public DateTime CurrentDate { get; set; }
        [MaxLength(2048)]
        public string Comment { get; set; }

        public string StateField { get; set; }

        [ForeignKey("Account")]
        public long AccountID { get; set; }
        [ForeignKey("AccountFrom")]
        /// <summary>
        /// Second account with action, for transfer money
        /// </summary>
        public long? AccountIDFrom { get; set; }

        public virtual Account Account { get; set; }
        public virtual Account AccountFrom { get; set; }

    }
}
