using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
	public class UserSettings
	{
		[Key]
		[ForeignKey("User")]
		public Guid ID { get; set; }

		/// <summary>
		/// The pages Budget/Month, Budget/Year ... consider money with collective or without
		/// </summary>
		public bool BudgetPages_WithCollective { get; set; }
		/// <summary>
		/// The pages Budget/Month, Budget/Year ... show/hide Earning Chart
		/// </summary>
		public bool BudgetPages_EarningChart { get; set; }
		/// <summary>
		/// The pages Budget/Month, Budget/Year ... show/hide Spending Chart
		/// </summary>
		public bool BudgetPages_SpendingChart { get; set; }
		/// <summary>
		/// The pages Budget/Month, Budget/Year ... show/hide Invest Chart
		/// </summary>
		public bool BudgetPages_InvestingChart { get; set; }


		public virtual User User { get; set; }

	}
}
