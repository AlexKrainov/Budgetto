using System;
using System.Collections.Generic;
using System.Text;

namespace MyProfile.Entity.ModelView.Chart
{
	public class ChartLineViewModel : IChartData
	{
		public string Label { get; set; }
		public string BorderColor { get; set; }
		public decimal[] Data { get; set; }
		public bool Fill { get; set; }
	}
}
