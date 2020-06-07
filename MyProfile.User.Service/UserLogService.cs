using MyProfile.Entity.Model;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
using System;
using System.Threading.Tasks;

namespace MyProfile.User.Service
{
	public class UserLogService
	{
		private IBaseRepository repository;

		public UserLogService(IBaseRepository repository)
		{
			this.repository = repository;
		}

		public async Task<int> CreateAction(Guid? UserID, string userActionType)
		{
			UserLog userLog = new UserLog();

			try
			{
				userLog.IP = UserInfo.HttpContext.Connection.RemoteIpAddress.ToString();
				userLog.SessionID = !string.IsNullOrEmpty(UserInfo.HttpContext.Request.Headers["X-Original-For"])
				? UserInfo.HttpContext.Request.Headers["X-Original-For"].ToString() : "";

				userLog.CurrentDateTime = DateTime.Now.ToUniversalTime();
				userLog.ActionCodeName = userActionType;
				userLog.UserID = UserID;

				if (UserInfo.LastUserLogID != 0)
				{
					userLog.ParentUserLogID = UserInfo.LastUserLogID;
				}

				await repository.CreateAsync(userLog, true);
			}
			catch (Exception ex)
			{

			}

			return (UserInfo.LastUserLogID = userLog.ID);
		}
	}
}
