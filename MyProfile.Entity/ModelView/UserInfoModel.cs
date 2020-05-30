using MyProfile.Entity.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyProfile.Entity.ModelView
{
	public class UserInfoModel : Person
	{
		public string SpecificCulture { get; set; } = "ru-RU";
		/// <summary>
		/// Collective person without this.ID
		/// </summary>
		public List<Guid> CollectivePersonIDs
		{
			get { return new List<Guid> { Guid.Parse("086D7C26-1D8D-4CC7-E776-08D7EAB4D0ED") }; }
		} //= new List<Guid>();
		public List<Guid> AllCollectivePersonIDs
		{
			get { return new List<Guid> { Guid.Parse("086D7C26-1D8D-4CC7-E776-08D7EAB4D0ED"), Guid.Parse("EA02C872-0C3C-4112-7231-08D7BDD8901D") }; }
		} //= new List<Guid>();

		public UserInfoModel()
		{
			this.CollectiveBudgetID = Guid.Parse("599AD733-8DB6-4689-E345-08D7EAB4CFD5");
			this.ID = Guid.Parse("EA02C872-0C3C-4112-7231-08D7BDD8901D");
			this.IsAllowCollectiveBudget = true;
		}
	}
}
