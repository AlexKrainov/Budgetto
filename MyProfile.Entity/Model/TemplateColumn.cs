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
		BudgetSection = 1,//Money
		DaysForMonth = 2,
		MonthsForYear = 3,
		YearsFor10Year = 4,
		Percent = 5,
		Comment = 6,
		WeeksForMonth = 7,
		Error = 8,
	}

	public enum FooterActionType
	{
		Undefined = 0,
		Sum,
		Avr,
		Min,
		Max
	}

	public enum FormulaFieldType
	{
		Undefined = 0,
		Section = 1,
		Number = 2,
		Mark = 3,
		Parentheses = 4,
		Days = 5,
		Month = 6
	}

	public class TemplateColumn
	{
		public TemplateColumn()
		{
			this.TemplateBudgetSections = new HashSet<TemplateBudgetSection>();
		}

		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public long ID { get; set; }
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
		/// <summary>
		/// Output formtat, for example for [Days] it can be like 1,2,3,4 or 1.03.2020,2.03.2020
		/// </summary>
		public string Format { get; set; }
		public int? FooterActionTypeID { get; set; }
		/// <summary>
		/// how many decimal places
		/// </summary>
		public ushort? PlaceAfterCommon { get; set; }

		[ForeignKey("Template")]
		public long TemplateID { get; set; }

		public virtual Template Template { get; set; }

		public virtual IEnumerable<TemplateBudgetSection> TemplateBudgetSections { get; set; }
	}

}
