using Microsoft.EntityFrameworkCore;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView.User;
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
        public async Task<Guid> CreateSession(UserStatViewModel personData, Guid? UserID = null)
        {
            Guid newUserSessionID = Guid.NewGuid();
            try
            {
                UserSession userSession = new UserSession
                {
                    ID = newUserSessionID,
                    UserID = UserID,
                    BrowerName = personData.browser_name,
                    BrowserVersion = personData.browser_version,
                    City = personData.city,
                    Country = personData.country,
                    EnterDate = DateTime.Now.ToUniversalTime(),
                    IP = personData.ip ?? UserInfo.HttpContext.Connection.RemoteIpAddress.ToString(),
                    IsPhone = personData.isMobile,
                    //IsTablet = personData.isMobile
                    Location = personData.location,
                    OS_Name = personData.os_name,
                    Os_Version = personData.os_version,
                    ScreenSize = personData.screen_size,
                    SessionID = !string.IsNullOrEmpty(UserInfo.HttpContext.Request.Headers["X-Original-For"])
                              ? UserInfo.HttpContext.Request.Headers["X-Original-For"].ToString() : "",
                    Referrer = personData.referrer,
                    ContinentCode = personData.continent_code,
                    ContinentName = personData.continent_name,
                    Index = personData.index,
                    Info = personData.info,
                    Path = personData.path,
                    ProviderInfo = personData.provider_info,
                    Threat = personData.threat,
                };
                await repository.CreateAsync(userSession, true);
            }
            catch (Exception ex)
            {
                try
                {
                    await repository.CreateAsync(new UserSession
                    {
                        ID = newUserSessionID,
                        EnterDate = DateTime.Now.ToUniversalTime(),
                        IP = UserInfo.HttpContext.Connection.RemoteIpAddress.ToString(),
                        SessionID = !string.IsNullOrEmpty(UserInfo.HttpContext.Request.Headers["X-Original-For"])
                                     ? UserInfo.HttpContext.Request.Headers["X-Original-For"].ToString() : "",
                        Comment = ex.Message
                    }, true);
                }
                catch (Exception ex1)
                {

                }
            }

            return newUserSessionID;
        }
        public async Task<int> UpdateSession_UserID(Guid userSessionID, Guid UserID)
        {
            var userSession = await repository.GetAll<UserSession>(x => x.ID == userSessionID).FirstOrDefaultAsync();
            userSession.UserID = UserID;
            await repository.UpdateAsync(userSession, true);
            return 1;
        }

        public async Task<int> UserSessionLogOut(Guid userSessionID, Guid? userID)
        {
            try
            {
                var userSession = await repository.GetAll<UserSession>(x => x.ID == userSessionID).FirstOrDefaultAsync();
                if (userSession != null)
                {
                    userSession.LogOutDate = DateTime.Now.ToUniversalTime();
                    return await repository.UpdateAsync(userSession, true);
                }
            }
            catch (Exception ex)
            {
                await CreateErrorLog(userSessionID: userSessionID, where: "UserLogService.UserSessionLogOut", ex);
            }
            return 0;
        }


        /// <summary>
        /// All user actions
        /// </summary>
        /// <param name="userSessionID"></param>
        /// <param name="userLogActionType"></param>
        /// <param name="comment"></param>
        /// <returns></returns>
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

        /// <summary>
        /// All error
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="userLogID"></param>
        /// <param name="where"></param>
        /// <param name="errorText"></param>
        /// <param name="comment"></param>
        /// <returns></returns>
        public async Task<int> CreateErrorLog(Guid userSessionID, string where, Exception exception, string comment = null)
        {
            ErrorLog log = new ErrorLog
            {
                CurrentDate = DateTime.Now.ToUniversalTime(),
                Comment = comment,
                ErrorText = GetExceptionMessages(exception),
                Where = where,
                UserSessionID = userSessionID,
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

        private string GetExceptionMessages(Exception e, string msgs = "")
        {
            if (e == null) return string.Empty;
            if (msgs == "") msgs = e.Message;
            if (e.InnerException != null)
                msgs += "\r\nInnerException: " + GetExceptionMessages(e.InnerException);
            return msgs;
        }
        /// <summary>
        /// If user trying to enter more then 20 times in a day, show him code
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CheckLimitEnter()
        {
            var today = DateTime.Now.Date;
            var ip = UserInfo.HttpContext.Connection.RemoteIpAddress.ToString();

            return await repository.GetAll<UserSession>(x => x.EnterDate.Date == today
                && x.IP == ip)
                .CountAsync() > 20;
        }
    }
}
