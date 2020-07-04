using MyProfile.Entity.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyProfile.Entity.ModelView.BudgetView
{
    public class TableViewModel
    {
    }
    public class Cell
    {
        public string Value { get; set; }
        public decimal NaturalValue { get; set; }
        public bool IsShow { get; set; } = true;
        public TemplateColumnType TemplateColumnType { get; set; }
        public int dateCounter { get; set; }
        public bool IsWeekend { get; set; }
        public bool IsHoliday { get; set; }
    }
    public class FooterCell : Cell
    {
    }


}
