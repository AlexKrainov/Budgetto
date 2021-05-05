using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
    public class CardPaymentSystem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [ForeignKey("Card")]
        public long CardID { get; set; }
        [ForeignKey("PaymentSystem")]
        public int PaymentSystemID { get; set; }


        public virtual Card Card { get; set; }

        public virtual PaymentSystem PaymentSystem { get; set; }
    }
}
