using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
	public class PaymentSystem
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ID { get; set; }
		[Required]
		[MaxLength(24)]
		public string Name { get; set; }
		[Required]
		[MaxLength(16)]
		public string CodeName { get; set; }
		[MaxLength(512)]
		public string Logo { get; set; }
        public bool IsVisible { get; set; }

		public virtual ICollection<Account> Accounts { get; set; }
		public virtual ICollection<CardPaymentSystem> CardPaymentSystems { get; set; }

		public PaymentSystem()
		{
			this.Accounts = new HashSet<Account>();
			this.CardPaymentSystems = new HashSet<CardPaymentSystem>();
		}
	}
}
