﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MyProfile.Entity.ModelView.BudgetView
{
	public class BudgetControllerModelView
	{
		public int SelectedTemplateID { get; set; }
		public List<TemplateViewModel_Short> NameTemplates { get; set; } = new List<TemplateViewModel_Short>();
		public DateTime SelectedDateTime { get; set; }
		public int SelectedYear{ get; set; }
	}
}
