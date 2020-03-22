using MyProfile.Entity.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyProfile.Entity.ModelView
{
	public class TemplateViewModel
	{
		public int ID { get; set; }
		public string Name { get; set; }
		public bool? IsCountCollectiveBudget { get; set; }
		public DateTime DateCreate { get; set; }
		public DateTime? DateEdit { get; set; }
		public DateTime? LastSeenDateTime { get; set; }
		public int MaxRowInAPage { get; set; }
		public List<Column> Columns { get; set; } = new List<Column>();
		public string PeriodName { get; set; }
		public int PeriodTypeID { get; set; }
	}

	public class Column
	{
		public int ID { get; set; }
		public string Name { get; set; }
		public int Order { get; set; }
		public bool IsShow { get; set; }
		public string Format { get; set; }
		public ushort PlaceAfterCommon { get; set; } = 2;
		public List<FormulaItem> Formula { get; set; } = new List<FormulaItem>();
		public FooterActionType TotalAction { get; set; } = FooterActionType.Undefined;
		public TemplateColumnType TemplateColumnType { get; set; } = TemplateColumnType.Undefined;
		public List<TemplateAreaType> TemplateBudgetSections { get; set; } = new List<TemplateAreaType>();

	}

	public class FormulaItem
	{
		/// <summary>
		/// sectionID
		/// </summary>
		public int? ID { get; set; }
		public string Value { get; set; }
		public FormulaFieldType Type { get; set; }
	}

	public class TemplateAreaType
	{
		public int ID { get; set; }
		public int BudgetAreaID { get; set; }
		public string BudgetAreaName { get; set; }
		public int SectionID { get; set; }
		public string SectionName { get; set; }
		public string SectionCodeName { get; set; }
	}
}
