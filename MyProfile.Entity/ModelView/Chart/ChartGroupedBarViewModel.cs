﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MyProfile.Entity.ModelView.Chart
{
    public class ChartGroupedBarViewModel : IChartData
    {
        public string Label { get; set; }
        public string BackgroundColor { get; set; }
        public decimal[] Data { get; set; }
    }
}
