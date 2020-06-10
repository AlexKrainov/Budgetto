using Microsoft.EntityFrameworkCore;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProfile.User.Service
{
	public class CollectionUserService
	{
		private IBaseRepository repository;

		public CollectionUserService(IBaseRepository repository)
		{
			this.repository = repository;
		}

		public List<Guid> GetAllCollectiveUserIDs()
		{
			Guid currentUserID = UserInfo.Current.ID;
			List<Guid> allCollectiveUserIDs = new List<Guid>();

			if (UserInfo.Current.IsAllowCollectiveBudget)
			{
				allCollectiveUserIDs.AddRange(repository.GetAll<MyProfile.Entity.Model.User>(x => x.ID == currentUserID)
					.SelectMany(x => x.CollectiveBudget.Users)
					.Where(x => x.IsAllowCollectiveBudget)
					.Select(x => x.ID)
					.ToList());
			}
			else
			{
				allCollectiveUserIDs.Add(currentUserID);
			}
			return allCollectiveUserIDs;
		}

		public async Task<List<Guid>> GetAllCollectiveUserIDsAsync()
		{
			Guid currentUserID = UserInfo.Current.ID;
			List<Guid> allCollectiveUserIDs = new List<Guid>();

			if (UserInfo.Current.IsAllowCollectiveBudget)
			{
				allCollectiveUserIDs.AddRange(await repository.GetAll<MyProfile.Entity.Model.User>(x => x.ID == currentUserID)
					.SelectMany(x => x.CollectiveBudget.Users)
					.Where(x => x.IsAllowCollectiveBudget)
					.Select(x => x.ID)
					.ToListAsync());
			}
			else
			{
				allCollectiveUserIDs.Add(currentUserID);
			}
			return allCollectiveUserIDs;
		}
	}
}
