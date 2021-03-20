using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
    public enum BankTypes
    {
        Undefined,
        Bank = 1,
        Broker = 2,
        OnlineWallet = 3,
        Microloans = 4,
        Forex = 5
    }
    public class BankType
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
        public bool IsVisible { get; set; }

        public virtual ICollection<Bank> Banks { get; set; }
        public virtual ICollection<AccountType> AccountTypes { get; set; }

        public BankType()
        {
            this.Banks = new HashSet<Bank>();
            this.AccountTypes = new HashSet<AccountType>();
        }
    }
}
