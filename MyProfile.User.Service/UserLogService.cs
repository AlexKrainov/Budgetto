using Microsoft.EntityFrameworkCore;
using MyProfile.Entity.Model;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
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

        /// <summary>
        /// Login/Registration/ Forgot password/ Enter code/ LogOut
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="userSessionActionType"></param>
        /// <returns></returns>
        public async Task<Guid> CreateSession(Guid? UserID, string userSessionActionType, string comment = null)
        {
            UserSession userSession = new UserSession();

            try
            {
                userSession.IP = UserInfo.HttpContext.Connection.RemoteIpAddress.ToString();
                userSession.SessionID = !string.IsNullOrEmpty(UserInfo.HttpContext.Request.Headers["X-Original-For"])
                ? UserInfo.HttpContext.Request.Headers["X-Original-For"].ToString() : "";

                userSession.UserLogs = new List<UserLog>{
                    new UserLog
                    {
                        CurrentDateTime = DateTime.Now.ToUniversalTime(),
                        ActionCodeName = userSessionActionType,
                        Comment = comment
                    }
                };
                userSession.UserID = UserID;

                await repository.CreateAsync(userSession, true);
            }
            catch (Exception ex)
            {

            }

            return userSession.ID;
        }

        public async Task<int> CreateUserLog(Guid userSessionID, string userLogActionType, string comment = null)
        {
            UserLog userLog = new UserLog();

            try
            {
                userLog.CurrentDateTime = DateTime.Now.ToUniversalTime();
                userLog.ActionCodeName = userLogActionType;
                userLog.UserSessionID = userSessionID;
                userLog.Comment = comment;

                await repository.CreateAsync(userLog, true);
            }
            catch (Exception ex)
            {

            }

            return userLog.ID;
        }

        public async Task<int> CreateLog(Guid? userID = null, int? userLogID = null, string where = null, string errorText = null, string comment = null)
        {
            Log log = new Log
            {
                CurrentDateTime = DateTime.Now.ToUniversalTime(),
                Comment = comment,
                ErrorText = errorText,
                UserID = userID,
                Where = where,
                UserLogID = userLogID
            };

            try
            {
                await repository.CreateAsync(log, true);
            }
            catch (Exception ex)
            {

            }

            return log.ID;
        }

        /// <summary>
        /// If user trying to enter more then 20 times in a day, show him code
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CheckLimitEnter()
        {
            var today = DateTime.Now.Date;
            var ip = UserInfo.HttpContext.Connection.RemoteIpAddress.ToString();

            return await repository.GetAll<UserSession>(x => x.CurrentDateTime.Date == today
                && x.IP == ip)
                .CountAsync() > 20;
        }
    }
}
