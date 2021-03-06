using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
    public class PaymentHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        [ForeignKey("Payment")]
        public Guid PaymentID { get; set; }
        [ForeignKey("PaymentTariff")]
        public int? PaymentTariffID { get; set; }
        [ForeignKey("PromoCode")]
        public int? PromoCodeID { get; set; }

        public virtual PaymentTariff PaymentTariff { get; set; }
        public virtual Payment Payment { get; set; }
        public virtual PromoCode PromoCode { get; set; }
    }
}
