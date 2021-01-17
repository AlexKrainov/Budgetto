using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
	public class CurrencyRateHistory
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ID { get; set; }
		public DateTime Date { get; set; }

		public decimal Rate { get; set; }
		public int Nominal { get; set; }
		[Required]
		[MaxLength(8)]
		public string NumCode { get; set; }
		[Required]
		[MaxLength(8)]
		public string CharCode { get; set; }
		[Required]
		[MaxLength(64)]
		public string Name { get; set; }
		[Required]
		[MaxLength(16)]
		public string CodeName_CBR { get; set; }

		public int? CurrencyID { get; set; }

	}
}
