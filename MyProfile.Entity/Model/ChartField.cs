using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
	public class ChartField
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ID { get; set; }
		[Required]
		[MaxLength(32)]
		public string Name { get; set; }
		[MaxLength(24)]
		public string CssColor { get; set; }

		[ForeignKey("Chart")]
		public int ChartID { get; set; }

		public virtual Chart Chart { get; set; }

		public virtual ICollection<SectionGroupChart> SectionGroupCharts { get; set; }

		public ChartField()
		{
			this.SectionGroupCharts = new HashSet<SectionGroupChart>();
		}

	}
}
