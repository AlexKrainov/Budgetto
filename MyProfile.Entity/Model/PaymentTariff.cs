using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
    public enum PaymentTariffTypes
    {
        Undefined = 0,
        Free = 1,
        Standard = 2,
        Freedom = 3,
        Premium = 4,
    }
    public class PaymentTariff
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [MaxLength(32)]
        public string Name { get; set; }
        [MaxLength(32)]
        public string CodeName { get; set; }


        //public virtual IEnumerable<PaymentHistory> PaymentHistories { get; set; }
        public virtual IEnumerable<Payment> Payments { get; set; }
        public virtual IEnumerable<PaymentCounter> PaymentCounters { get; set; }

        public PaymentTariff()
        {
            //  this.PaymentHistories = new HashSet<PaymentHistory>();
            this.Payments = new HashSet<Payment>();
            this.PaymentCounters = new HashSet<PaymentCounter>();
        }
    }
}
