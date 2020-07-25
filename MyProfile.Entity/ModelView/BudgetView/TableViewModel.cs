using MyProfile.Entity.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyProfile.Entity.ModelView.BudgetView
{
    public class TableViewModel
    {
    }
    public class Cell : ICloneable
    {
        public string Value { get; set; }
        public decimal NaturalValue { get; set; }
        public bool IsShow { get; set; } = true;
        public TemplateColumnType TemplateColumnType { get; set; }
        public int dateCounter { get; set; }
        public bool IsWeekend { get; set; }
        public bool IsHoliday { get; set; }
        public DateTime CurrentDate { get; set; }

        /// <summary>
        /// Too long, because boxing and unboxing
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        public Cell CloneObject()
        {
            return new Cell
            {
                Value = this.Value,
                CurrentDate = this.CurrentDate,
                dateCounter = this.dateCounter,
                IsHoliday = this.IsHoliday,
                IsShow = this.IsShow,
                IsWeekend = this.IsWeekend,
                NaturalValue = this.NaturalValue,
                TemplateColumnType = this.TemplateColumnType
            };
        }
    }
    public class FooterCell : Cell
    {
    }


}
