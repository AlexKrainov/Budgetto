﻿using System;

namespace MyProfile.Entity.ModelEntitySave
{
	/// <summary>
	/// Model for BudgetArea.IncludedCollectiveAreas and BudgetSection.IncludedCollectiveSections
	/// </summary>
	public class IncludedCollectiveItem
	{
		public int sectionID { get; set; }
		public Guid personID { get; set; }
	}
}
