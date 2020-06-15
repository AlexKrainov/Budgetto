using System;
using System.Collections.Generic;
using System.Text;

namespace MyProfile.Entity.ModelView.Chart
{
	public class ChartDoughnutViewModel : IChartDataSet
	{
		public string Label { get; set; }
		public List<string> BackgroundColor { get; set; }
		public List<int> Data { get; set; }
	}
}
