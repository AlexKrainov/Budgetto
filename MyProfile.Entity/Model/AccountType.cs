using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
    public enum AccountTypes
    {
        Undefined,
        Cash = 1,
        /// <summary>
        /// Депозит
        /// </summary>
        Debed = 2,
        Credit = 3,
        Installment = 4, //Карта рассрочки
        OnlineWallet = 5, //Счет (Электронный кошелек)
        Investments = 6, //брокерский счет
        Deposit = 7,//Вклад
        InvestmentsIIS = 8, //брокерский счет (ИИС)
        
    }
    public class AccountType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required]
        [MaxLength(64)]
        public string Name { get; set; }
        [Required]
        [MaxLength(16)]
        public string CodeName { get; set; }
        [MaxLength(512)]
        public string Description { get; set; }
        [MaxLength(64)]
        public string Icon { get; set; }
        public bool IsVisible { get; set; }
        public bool IsPaymentSystem { get; set; }

        [ForeignKey("BankType")]
        public int? BankTypeID { get; set; }

        public virtual BankType BankType { get; set; }

    }
}
