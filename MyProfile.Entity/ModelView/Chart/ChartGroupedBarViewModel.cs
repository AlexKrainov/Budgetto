using System;
using System.Collections.Generic;
using System.Text;

namespace MyProfile.Entity.ModelView.Chart
{
    public class ChartGroupedBarViewModel : IChartData
    {
        public string Label { get; set; }
        public string BackgroundColor { get; set; }
        public decimal[] Data { get; set; }
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }
        public decimal AvgValue { get; set; }
        public decimal TotalValue { get; set; }
        public double BarPercentage { get; set; } = 0.5;
        public double BarThickness { get; set; } = 6;
    }
}
