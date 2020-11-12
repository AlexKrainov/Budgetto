using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
    public class PromoCodeHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required]
        [MaxLength(512)]
        public string InputPromoCode { get; set; }
        public DateTime CurrentDateTime { get; set; }
        public bool IsSuccess { get; set; }
        public bool IsApplied { get; set; }

        [ForeignKey("PaymentHistory")]
        public Guid PaymentHistoryID { get; set; }
        [ForeignKey("PromoCode")]
        public int? PromoCodeID { get; set; }

        public virtual PaymentHistory PaymentHistory { get; set; }
        public virtual PromoCode PromoCode { get; set; }

    }
}
