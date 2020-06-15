using System;
using System.Collections.Generic;
using System.Text;

namespace MyProfile.Entity.ModelView.Chart
{
	public class ChartBubbleViewModel : IChartDataSet
	{
		public List<string> Label { get; set; }
		public string BackgroundColor { get; set; }
		public string borderColor{ get; set; }
		public List<ChartBubbleDataItem> Data { get; set; }
	}

	public class ChartBubbleDataItem
	{
		public int X { get; set; }
		public int Y { get; set; }
		public int R { get; set; }
	}
}
