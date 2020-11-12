using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
    public class PaymentHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; }
        public bool IsPaid { get; set; }
        /// <summary>
        /// PaymentTariffs
        /// </summary>
        [MaxLength(16)]
        public string Tariff { get; set; }
        public DateTime? DateClickToPay { get; set; }
        public DateTime? DateFinisthToPay { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        [ForeignKey("Payment")]
        public int PaymentID { get; set; }

        public virtual Payment Payment { get; set; }
        public virtual IEnumerable<PromoCodeHistory> PromoCodeHistories { get; set; }

        public PaymentHistory()
        {
            this.PromoCodeHistories = new HashSet<PromoCodeHistory>();
        }
    }
}
