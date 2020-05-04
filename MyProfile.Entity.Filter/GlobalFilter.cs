using MyProfile.Entity.Model;
using MyProfile.Identity;
using System;
using System.Linq.Expressions;

namespace MyProfile.Entity.Filter
{
	public static class GlobalFilter
	{
		public static Expression<Func<BudgetArea, bool>> CommonFilter(this Expression<Func<BudgetArea, bool>> predicate)
		{
			//if (UserInfo.Current.)
			//{

			//}
			


			return predicate;
		}
	}
}
