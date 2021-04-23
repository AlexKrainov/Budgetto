using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
    public class UserSubScription
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        [MaxLength(256)]
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        [Required]
        [Column(TypeName = "Money")]
        public decimal Price { get; set; }
        [Required]
        [Column(TypeName = "Money")]
        public decimal PricePerMonth { get; set; }
        public bool IsPause { get; set; }
        public bool IsDeleted { get; set; }

        [ForeignKey("SubScriptionPricing")]
        public int? SubScriptionPricingID { get; set; }
        [ForeignKey("User")]
        public Guid UserID { get; set; }

        public virtual SubScriptionPricing SubScriptionPricing { get; set; }
        public virtual User User { get; set; }

    }
}
