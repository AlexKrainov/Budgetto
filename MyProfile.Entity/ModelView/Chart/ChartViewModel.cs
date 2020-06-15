using MyProfile.Entity.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyProfile.Entity.ModelView.Chart
{
	public class ChartViewModel
	{
		public string ChartID { get; set; }
		public string Name { get; set; }
		public ChartTypesEnum ChartTypesEnum { get; set; }
		public string ChartTypeCodeName { get; set; }
		public List<string> Labels { get; set; } = new List<string>();
		public List<IChartDataSet> DataSets { get; set; }
	}
}
