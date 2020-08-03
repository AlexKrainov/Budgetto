using System;
using System.Collections.Generic;
using System.Text;

namespace MyProfile.Entity.ModelView.Chart
{
    public interface IChartData : IChartDataSet
    {
        decimal[] Data { get; set; }
        decimal MinValue { get; set; }
        decimal MaxValue { get; set; }
        decimal AvgValue { get; set; }
        decimal TotalValue { get; set; }

    }
}
