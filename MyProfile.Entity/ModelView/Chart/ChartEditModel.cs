﻿using MyProfile.Entity.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyProfile.Entity.ModelView.Chart
{
	public class ChartEditModel
	{
		public int ID { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public bool IsShowBudgetMonth { get; set; }
		public bool IsShowBudgetYear { get; set; }

		public ChartTypesEnum ChartTypeID { get; set; }
		public List<ChartFieldItem> Fields { get; set; }
		public string ChartTypeCodeName { get; set; }
		public string ChartTypeName { get; set; }
		public DateTime DateCreate { get; set; }
		public DateTime LastDateEdit { get; set; }
	}

	public class ChartFieldItem
	{
		public int ID { get; set; }
		public string Name { get; set; }
		public string CssColor { get; set; }
		public List<int> Sections { get; set; }
	}
}