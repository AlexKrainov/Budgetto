using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
    public class PromoCodeLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        [Required]
        [MaxLength(32)]
        public string InputPromoCode { get; set; }
        public DateTime CurrentDateTime { get; set; }
        public bool IsSuccess { get; set; }
        public bool IsApplied { get; set; }

        [ForeignKey("PromoCode")]
        public int? PromoCodeID { get; set; }
        [ForeignKey("PaymentLog")]
        public Guid PaymentLogID { get; set; }

        public virtual PromoCode PromoCode { get; set; }
        public virtual PaymentLog PaymentLog { get; set; }

    }
}
