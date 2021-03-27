using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
	public class Bank
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ID { get; set; }
		[Required]
		[MaxLength(512)]
		public string Name { get; set; }
		[MaxLength(512)]
		public string NameEn { get; set; }
		[MaxLength(512)]
		public string LogoCircle { get; set; }
		[MaxLength(512)]
		public string LogoRectangle { get; set; }
		[MaxLength(512)]
		public string URL { get; set; }	
		[MaxLength(512)]
		public string Tels { get; set; }
        public int Raiting { get; set; }
		[MaxLength(256)]
		public string Licence { get; set; }
		[MaxLength(128)]
		public string Region { get; set; }
		[MaxLength(16)]
		/// <summary>
		/// For banki.ru
		/// </summary>
		public string bankiruID { get; set; }
		[MaxLength(16)]
		public string BrandColor { get; set; }


        [ForeignKey("BankType")]
		public int? BankTypeID { get; set; }

		public virtual BankType BankType { get; set; }

		public virtual ICollection<Account> Accounts { get; set; }
		public virtual ICollection<Card> Cards { get; set; }

		public Bank()
		{
			this.Accounts = new HashSet<Account>();
			this.Cards = new HashSet<Card>();
		}

	}
}
