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
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public DateTime? LastDatePayment { get; set; }
        public bool IsPaid { get; set; }
        //[ForeignKey("User")]
        //public Guid UserID { get; set; }
        [ForeignKey("PaymentTariff")]
        public int PaymentTariffID { get; set; }

        public virtual User User { get; set; }
        public virtual PaymentTariff PaymentTariff { get; set; }

        public virtual IEnumerable<PaymentHistory> PaymentHistories { get; set; }

        public Payment()
        {
            this.PaymentHistories = new HashSet<PaymentHistory>();
        }
    }
}
