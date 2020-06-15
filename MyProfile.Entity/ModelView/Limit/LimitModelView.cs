﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MyProfile.Entity.ModelView.Limit
{
	public class LimitModelView : Model.Limit
	{
		public List<BudgetSectionModelView> Sections { get; set; } = new List<BudgetSectionModelView>();
		public List<BudgetSectionModelView> NewSections { get; set; } = new List<BudgetSectionModelView>();
		public string PeriodName { get; set; }
		public bool IsFinishLimit { get; set; }
	}
}
