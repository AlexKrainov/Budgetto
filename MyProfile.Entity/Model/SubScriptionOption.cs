using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
    public class SubScriptionOption
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [MaxLength(256)]
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime EditDate { get; set; }

        public bool IsActive { get; set; }
        public bool IsPersonally { get; set; }
        public bool IsFamaly { get; set; }
        public bool IsStudent { get; set; }
        public bool IsBoth { get; set; }
        /// <summary>
        /// Subme.ru raiting
        /// </summary>
        public decimal? _raiting { get; set; }

        [ForeignKey("SubScription")]
        public int SubScriptionID { get; set; }

        public virtual SubScription SubScription { get; set; }

        public virtual ICollection<SubScriptionPricing> SubScriptionPricings { get; set; }

        public SubScriptionOption()
        {
            this.SubScriptionPricings = new HashSet<SubScriptionPricing>();
        }
    }
}
