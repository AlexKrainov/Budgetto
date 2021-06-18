using MyProfile.Entity.ModelView.AreaAndSection;
using MyProfile.Entity.ModelView.TotalBudgetView;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyProfile.Entity.ModelView.Chart
{
    public class UniversalChartWidgetViewModel
    {
        public List<string> Labels { get; set; }
        public List<IChartDataSet> DataSets { get; set; }
        public decimal MaxTotal { get; set; }
        public List<UniversalChartSectionViewModel> Sections { get; set; }
        public List<TotalModelView> TotalView { get; set; }
        public List<UniversalChartTagViewModel> Tags { get; set; }
        public bool IsShow { get; set; }
    }

    public class UniversalChartSectionViewModel : SectionLightModelView
    {
        public decimal Total { get; set; }
        public bool IsShow { get; set; }
        public decimal Percent { get; set; }
        public List<UniversalChartTagViewModel> Tags { get; set; }
    }

    public class UniversalChartTagViewModel : RecordTag
    {
        public decimal EarningSum { get; set; }
        public decimal SpendingSum { get; set; }
        public decimal Percent { get; set; }

        public decimal Total
        {
            get
            {
                return IsSpending ? SpendingSum : EarningSum;
            }
        }

        public bool IsEarning
        {
            get
            {
                return EarningSum != 0;
            }
        }
        public bool IsSpending
        {
            get
            {
                return SpendingSum != 0;
            }
        }
    }
}
