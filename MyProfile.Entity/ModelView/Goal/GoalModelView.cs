﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MyProfile.Entity.ModelView.Goal
{
	public class GoalModelView : Entity.Model.Goal
	{
		public string OwenerName { get; set; }
		public decimal TotalMoney { get; set; }
		public decimal Percent { get; set; }


		public List<RecordItem> Records { get; set; } = new List<RecordItem>();
		public string ChartID { get; set; }
		public decimal Percent2 { get; set; }
		public bool IsShow { get; set; }
	}

	public class RecordItem
	{
		public int ID { get; set; }
		public int GoalID { get; set; }
		public decimal Total { get; set; }
		public DateTime? DateTimeOfPayment { get; set; }
		public string OwenerName { get; set; }
		public DateTime? CreateDateTime { get; set; }
	}
}