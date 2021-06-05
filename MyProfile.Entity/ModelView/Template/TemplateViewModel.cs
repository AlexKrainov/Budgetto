using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView.BudgetView;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyProfile.Entity.ModelView
{
    public class TemplateViewModel
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public bool? IsCountCollectiveBudget { get; set; }
        public DateTime DateCreate { get; set; } = DateTime.Now;
        public DateTime? DateEdit { get; set; } = DateTime.Now;
        public DateTime? LastSeenDateTime { get; set; }
        public int MaxRowInAPage { get; set; }
        public List<Column> Columns { get; set; } = new List<Column>();
        public string PeriodName { get; set; }
        public int PeriodTypeID { get; set; }
        public bool IsShow { get; set; }
        public string Description { get; set; }
        public bool IsDefault { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsCreatedByConstructor { get; set; }
        public bool IsCreatedAsPrepared { get; set; }

        public List<List<Cell>> Rows { get; set; } = new List<List<Cell>>();
        public List<List<FooterCell>> FooterRow { get; set; } = new List<List<FooterCell>>();
    }

    public class TemplateViewModel_Short
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public string PeriodName { get; set; }
        public int PeriodTypeID { get; set; }
        public bool IsDefault { get; set; }
    }

    public class Column
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public bool IsShow { get; set; }
        public string Format { get; set; }
        public ushort PlaceAfterCommon { get; set; } = 2;
        public List<FormulaItem> Formula { get; set; } = new List<FormulaItem>();
        public FooterActionType TotalAction { get; set; } = FooterActionType.Sum;
        public TemplateColumnType TemplateColumnType { get; set; } = TemplateColumnType.Undefined;
        public List<TemplateBudgetSection> TemplateBudgetSections { get; set; } = new List<TemplateBudgetSection>();
        public string ColumnSectionType { get; set; }
        public string StyleForTableView
        {
            get
            {
                var style = "";
                if (!string.IsNullOrEmpty(ColumnSectionType))
                {
                    if (ColumnSectionType == "Spendings")
                    {
                        style += "table-danger";
                    }
                    else if (ColumnSectionType == "Earnings")
                    {
                        style += "table-success";
                    }
                    else if (ColumnSectionType == "Investments")
                    {
                        style += "table-dark";
                    }
                }

                if (TemplateColumnType == TemplateColumnType.DaysForMonth)
                {
                    style += " cell-days ";
                }
                if (TemplateColumnType == TemplateColumnType.MonthsForYear)
                {
                    style += " cell-months ";
                }
                return style;
            }
        }
    }

    public class FormulaItem
    {
        /// <summary>
        /// sectionID
        /// </summary>
        public long? ID { get; set; }
        public string Value { get; set; }
        public FormulaFieldType Type { get; set; }
    }

    public class TemplateBudgetSection
    {
        public long ID { get; set; }
        public int BudgetAreaID { get; set; }
        public string BudgetAreaName { get; set; }
        public long SectionID { get; set; }
        public string SectionName { get; set; }
        public int? SectionTypeID { get; set; }


        #region Collection budget
        //public long MainSectionID { get; set; } = -1;
        //public bool IsCollectSection { get; set; } = false;
        //public List<BudgetSectionModelView> CollectionSections { get; set; }
        #endregion
    }
}
