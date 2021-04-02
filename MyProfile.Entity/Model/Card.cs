using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
    public class Card
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required]
        [MaxLength(512)]
        public string Name { get; set; }
        [MaxLength(256)]
        public string SmallLogo { get; set; }
        [MaxLength(256)]
        public string BigLogo { get; set; }
        public bool IsInterest { get; set; }
        public decimal Interest { get; set; }
        public decimal ServiceCostTo { get; set; }
        public decimal ServiceCostFrom { get; set; }
        public decimal Cashback { get; set; }
        public bool IsCashback { get; set; }
        public bool IsCustomDesign { get; set; }
        /// <summary>
        /// Процентная ставка
        /// </summary>
        public decimal Rate { get; set; }
        /// <summary>
        /// true = от 20%, false = до 20%, null = 20%, only for credit cards
        /// </summary>
        public bool? IsRateTo { get; set; }
        public decimal CreditLimit { get; set; }
        public int GracePeriod { get; set; }

        public int Raiting { get; set; }
        public string bonuses { get; set; }
        public string paymentSystems { get; set; }
        public int bankiruCardID { get; set; }
        /// <summary>
        /// Property for searching, it's like additional informations
        /// </summary>
        public string SearchString { get; set; }
        public string bankName { get; set; }


        [ForeignKey("AccountType")]
        public int AccountTypeID { get; set; }
        [ForeignKey("Bank")]
        public int? BankID { get; set; }

        public virtual Bank Bank { get; set; }
        public virtual AccountType AccountType { get; set; }

        public virtual ICollection<Account> Accounts { get; set; }
        public virtual ICollection<CardPaymentSystem> CardPaymentSystems { get; set; }

        public Card()
        {
            this.Accounts = new HashSet<Account>();
            this.CardPaymentSystems = new HashSet<CardPaymentSystem>();
        }

    }
}
