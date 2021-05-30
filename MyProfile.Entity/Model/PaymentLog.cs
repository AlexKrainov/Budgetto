using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
    public class PaymentLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { get; set; }
        public DateTime DateEnterPage { get; set; }
        public DateTime? DatePayment { get; set; }
        public DateTime DateClickToPay { get; set; }
        public DateTime DateFinishToPay { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        [ForeignKey("PaymentTariff")]
        public int? PaymentTariffID { get; set; }
        [ForeignKey("PromoCode")]
        public int? PromoCodeID { get; set; }
        [ForeignKey("User")]
        public Guid UserID { get; set; }

        public virtual PaymentTariff PaymentTariff { get; set; }
        public virtual PromoCode PromoCode { get; set; }
        public virtual User User { get; set; }
    }
}
