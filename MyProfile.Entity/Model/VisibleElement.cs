using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
	public class VisibleElement
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ID { get; set; }

		public bool IsShow_BudgetMonth { get; set; }
		public bool IsShow_BudgetYear { get; set; }
        public bool IsShowInCollective { get; set; }
		/// <summary>
		/// Show on the pages Budget month and budget year
		/// </summary>
        public bool IsShowOnDashboards { get; set; }

    }
}
