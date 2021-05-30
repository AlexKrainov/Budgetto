using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
    /// <summary>
    /// Use this object for error on the site
    /// </summary>
    public class Payment
    {
        [Key]
        [ForeignKey("User")]
        public Guid ID { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public DateTime? LastDatePayment { get; set; }
        [ForeignKey("PaymentTariff")]
        public int PaymentTariffID { get; set; }
        [ForeignKey("PromoCode")]
        public int? PromoCodeID { get; set; }

        public virtual PaymentTariff PaymentTariff { get; set; }
        public virtual PromoCode PromoCode { get; set; }
        public virtual User User{ get; set; }

        //public virtual IEnumerable<PaymentHistory> PaymentHistories { get; set; }

        //public Payment()
        //{
        //    this.PaymentHistories = new HashSet<PaymentHistory>();
        //}
    }
}
