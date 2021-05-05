using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
    public class PaymentCounter
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        public int CanBeCount { get; set; }
        public DateTime LastChanges { get; set; }

        [ForeignKey("PaymentTariff")]
        public int PaymentTariffID { get; set; }
        [ForeignKey("EntityType")]
        public int EntityTypeID { get; set; }

        public virtual EntityType EntityType { get; set; }
        public virtual PaymentTariff PaymentTariff { get; set; }

    }
}
