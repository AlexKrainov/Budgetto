using System;
using System.Collections.Generic;
using System.Text;

namespace MyProfile.Entity.ModelView.Chart
{
	public class ChartLineViewModel : IChartDataSet
	{
		public string Label { get; set; }
		public string BorderColor { get; set; }
		public List<int> Data { get; set; }
		public bool Fill { get; set; }
	}
}
