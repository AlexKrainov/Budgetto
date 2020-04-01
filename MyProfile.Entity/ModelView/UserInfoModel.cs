using System;
using System.Collections.Generic;
using System.Text;

namespace MyProfile.Entity.ModelView
{
	public class UserInfoModel
	{
		public static Guid PersonID { get { return Guid.Parse("EA02C872-0C3C-4112-7231-08D7BDD8901D"); } }
		public string SpecificCulture { get; set; } = "ru-RU";

	}
}
