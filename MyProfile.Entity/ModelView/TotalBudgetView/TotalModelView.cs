using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView.AreaAndSection;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyProfile.Entity.ModelView.TotalBudgetView
{
	public class TotalModelView
	{
        public string ChartID { get; set; }
        public string Name { get; set; }
		public SectionTypeEnum SectionTypeEnum { get; set; }
		public string Total { get; set; }
		public bool IsGood { get; set; }
		public decimal Percent { get; set; }
		public decimal[] data { get; set; }
		public string[] labels { get; set; }
		public bool IsShow { get; set; } = true;
		public List<SectionLightModelView> Sections { get; set; } = new List<SectionLightModelView>();
        public string BorderColor { get; set; }
        public string BackgroundColor { get; set; }
        public string Title { get; set; }
        public bool IsSelected { get; set; }
    }
}
