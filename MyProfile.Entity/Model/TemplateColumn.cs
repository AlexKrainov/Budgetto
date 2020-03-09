using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyProfile.Entity.Model
{
	public enum TemplateColumnType
	{
		Undefined = 0,
		BudgetSection,//Money
		DaysForMonth,
		MonthsForYear,
		YearForYear,
		Percent,
		Comment
	}

	public enum FooterActionType
	{
		Undefined = 0,
		Sum,
		Avr,
		Min,
		Max
	}

	public class TemplateColumn
	{
		public TemplateColumn()
		{
			this.TemplateBudgetSections = new HashSet<TemplateBudgetSection>();
		}

		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ID { get; set; }
		[Required]
		public string Name { get; set; }
		public int Order { get; set; }
		public bool IsShow { get; set; }
		[Required]
		public string Formula { get; set; }
		/// <summary>
		/// It can by count money/day/months/year (week??? we need to think)
		/// </summary>
		[Required]
		public int ColumnTypeID { get; set; }
		public int? FooterActionTypeID { get; set; }
		/// <summary>
		/// how many decimal places
		/// </summary>
		public ushort? PlaceAfterCommon { get; set; }

		[ForeignKey("Template")]
		public int TemplateID { get; set; }

		public virtual Template Template { get; set; }

		public virtual IEnumerable<TemplateBudgetSection> TemplateBudgetSections { get; set; }
	}

}
