using MyProfile.Entity.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyProfile.Entity.ModelView
{
	public class UserInfoModel : User
	{
		public string SpecificCulture { get; set; } = "ru-RU";
		
		public int? LastUserLogID { get; set; }
		public Guid CollectiveBudgetID { get; set; }

		public UserInfoModel()
		{
			//this.CollectiveBudgetID = Guid.Parse("599AD733-8DB6-4689-E345-08D7EAB4CFD5");
			//this.ID = Guid.Parse("EA02C872-0C3C-4112-7231-08D7BDD8901D");
			//this.IsAllowCollectiveBudget = true;
		}
	}
}
