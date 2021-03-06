using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
    public class Record
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        /// <summary>
        /// This prop is ready for count 
        /// </summary>
        [Required]
        [Column(TypeName = "Money")]
        public decimal Total { get; set; }
        [Required]
        [Column(TypeName = "Money")]
        public decimal Cashback { get; set; }
        /// <summary>
        /// It needs to understand what data an user wrote
        /// </summary>
        public string RawData { get; set; }
        public string Description { get; set; }
        public DateTime DateTimeOfPayment { get; set; }
        public DateTime DateTimeCreate { get; set; }
        public DateTime DateTimeEdit { get; set; }
        public DateTime? DateTimeDelete { get; set; }
        public bool IsDeleted { get; set; }
        /// <summary>
        /// Consider when count or not
        /// </summary>
        public bool IsHide { get; set; }
        /// <summary>
        /// Hide this record for all collection but not this user 
        /// </summary>
        public bool IsShowForCollection { get; set; }
        [Column(TypeName = "Money")]
        public decimal? CurrencyRate { get; set; }
        public int CurrencyNominal { get; set; }

        [ForeignKey("User")]
        public Guid UserID { get; set; }
        [ForeignKey("BudgetSection")]
        public long BudgetSectionID { get; set; }
        [ForeignKey("Currency")]
        public int? CurrencyID { get; set; }
        [ForeignKey("Account")]
        public long? AccountID { get; set; }

        public virtual User User { get; set; }
        public virtual BudgetSection BudgetSection { get; set; }
        public virtual Currency Currency { get; set; }
        public virtual Account Account { get; set; }

        public virtual ICollection<RecordTag> Tags { get; set; }
        public virtual ICollection<RecordHistory> RecordHistories { get; set; }

        public Record()
        {
            this.Tags = new HashSet<RecordTag>();
            this.RecordHistories = new HashSet<RecordHistory>();
        }
    }
}
