using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
    public class PaymentTariffs
    {
        public const string Free = "Free";
        public const string Standart_Year = "Standart_Year";
    }
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
        [MaxLength(16)]
        public string Tariff { get; set; }

        //[ForeignKey("User")]
        //public Guid UserID { get; set; }

        public virtual User User { get; set; }

        public virtual IEnumerable<PaymentHistory> PaymentHistories { get; set; }

        public Payment()
        {
            this.PaymentHistories = new HashSet<PaymentHistory>();
        }
    }
}
