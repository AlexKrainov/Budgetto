﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MyProfile.Entity.ModelView
{
	public class BudgetRecordModelView
	{
		public int ID { get; set; }
		public decimal Money { get; set; }
		public string Description { get; set; }
		public DateTime DateTimeOfPayment { get; set; }
		public DateTime? DateTimeCreate { get; set; }
		public DateTime? DateTimeEdit { get; set; }
		public DateTime? DateTimeDelete { get; set; }
		/// <summary>
		/// Consider when count or not
		/// </summary>
		public bool IsConsider { get; set; }

		public int SectionID { get; set; }
		public string SectionName { get; set; }
		public int AreaID { get; set; }
		public string AreaName { get; set; }
	}

	/// <summary>
	/// For save from client
	/// </summary>
	public class RecordModelView
	{
		public string Tag { get; set; }
		public string Money { get; set; }
		public int SectionID { get; set; }
		public string SectionName { get; set; }
	}
}
