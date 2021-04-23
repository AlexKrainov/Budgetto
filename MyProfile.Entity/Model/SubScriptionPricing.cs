using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
    public enum PricingPeriodType
    {
        Undefined = 0,
        Month1 = 1,
        Month2,
        Month3,
        Month4,
        Month5,
        Month6,
        Month7,
        Month8,
        Month9,
        Month10,
        Month11,
        Month12,
        Month24 = 24,
        Month36 = 36,
    }

    public class SubScriptionPricing
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int PricingPeriodTypeID { get; set; }
        [Required]
        [Column(TypeName = "Money")]
        public decimal Price { get; set; }
        [Required]
        [Column(TypeName = "Money")]
        public decimal PricePerMonth { get; set; }

        [ForeignKey("SubScriptionOption")]
        public int SubScriptionOptionID { get; set; }

        public virtual SubScriptionOption SubScriptionOption { get; set; }

        public virtual ICollection<UserSubScription> UserSubScriptions { get; set; }

        public SubScriptionPricing()
        {
            this.UserSubScriptions = new HashSet<UserSubScription>();
        }
    }
}
