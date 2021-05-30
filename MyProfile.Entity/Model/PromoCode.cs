using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
    public class PromoCode
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required]
        [MaxLength(16)]
        public string CodeName { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        /// <summary>
        /// How many times tried
        /// </summary>
        public int TryCounter { get; set; }
        public int LimitCounter { get; set; }
        public int Percent { get; set; }


        public virtual IEnumerable<PromoCodeLog> PromoCodeLogs{ get; set; }

        public PromoCode()
        {
            this.PromoCodeLogs = new HashSet<PromoCodeLog>();
        }

    }
}
