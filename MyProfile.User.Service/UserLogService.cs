using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Headers;
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
        /// Check ip for DDoS attacks 
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="userSessionActionType"></param>
        /// <returns></returns>
        public async Task<bool> CreateAndCheckIP()
        {
            try
            {
                string IP = UserInfo.HttpContext.Connection.RemoteIpAddress.ToString();
                string SessionID = !string.IsNullOrEmpty(UserInfo.HttpContext.Request.Headers["X-Original-For"])
                                              ? UserInfo.HttpContext.Request.Headers["X-Original-For"].ToString() : "";
                DateTime now = DateTime.Now.ToUniversalTime();

                var ipSetting = await repository.GetAll<IPSetting>(x => x.IP == IP).FirstOrDefaultAsync();
                if (ipSetting == null)
                {
                    await repository.CreateAsync(new IPSetting
                    {
                        IP = IP,
                        SessionID = SessionID,
                        CreateDate = now,
                        LastVisit = now,
                        IsBlock = false,
                        Counter = 0,
                    }, true);
                }
                else
                {
                    ipSetting.LastVisit = now;
                    ipSetting.SessionID = SessionID;
                    ipSetting.Counter += ipSetting.Counter;

                    await repository.UpdateAsync(ipSetting, true);

                    return ipSetting.IsBlock == false;
                }
            }
            catch (Exception ex)
            {
            }

            return true;
        }

        /// <summary>
        /// Login/Registration/ Forgot password/ Enter code/ LogOut
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="userSessionActionType"></param>
        /// <returns></returns>
        public async Task<Guid> CreateSession()
        {
            Guid newUserSessionID = Guid.NewGuid();

            try
            {
                RequestHeaders header = UserInfo.HttpContext.Request.GetTypedHeaders();
                await repository.CreateAsync(new UserSession
                {
                    ID = newUserSessionID,
                    EnterDate = DateTime.Now.ToUniversalTime(),
                    IP = UserInfo.HttpContext.Connection.RemoteIpAddress.ToString(),
                    SessionID = !string.IsNullOrEmpty(header.Headers["X-Original-For"]) ? header.Headers["X-Original-For"].ToString() : "",
                    Referrer = header.Referer?.AbsoluteUri,

                }, true);
            }
            catch (Exception ex1)
            {

            }

            return newUserSessionID;
        }

        /// <summary>
        /// Login/Registration/ Forgot password/ Enter code/ LogOut
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="userSessionActionType"></param>
        /// <returns></returns>
        public async Task<Guid> UpdateSession(UserStatViewModel personData, Guid? UserID = null)
        {
            Guid userSessionID = Guid.NewGuid();
            try
            {
                bool isCreate = true;
                RequestHeaders header = UserInfo.HttpContext.Request.GetTypedHeaders();

                if (personData.userSessionID != Guid.Empty)
                {
                    userSessionID = personData.userSessionID;
                    isCreate = false;
                }

                UserSession userSession = new UserSession
                {
                    ID = userSessionID,
                    UserID = UserID,
                    BrowerName = personData.browser_name,
                    BrowserVersion = personData.browser_version,
                    City = personData.city,
                    Country = personData.country,
                    IsPhone = personData.isMobile,
                    //IsTablet = personData.isMobile
                    Location = personData.location,
                    OS_Name = personData.os_name,
                    Os_Version = personData.os_version,
                    ScreenSize = personData.screen_size,
                    ContinentCode = personData.continent_code,
                    ContinentName = personData.continent_name,
                    Index = personData.index,
                    Info = personData.info,
                    Path = personData.path,
                    ProviderInfo = personData.provider_info,
                    Threat = personData.threat,

                    SessionID = !string.IsNullOrEmpty(header.Headers["X-Original-For"]) ? header.Headers["X-Original-For"].ToString() : "",
                    Referrer = personData.referrer,
                    EnterDate = DateTime.Now.ToUniversalTime(),
                    IP = personData.ip ?? UserInfo.HttpContext.Connection.RemoteIpAddress.ToString(),
                };

                if (isCreate)
                {
                    await repository.CreateAsync(userSession, true);
                }
                else
                {
                    await repository.UpdateAsync(userSession, true);
                }
            }
            catch (Exception ex)
            {
                try
                {
                    await repository.CreateAsync(new UserSession
                    {
                        ID = userSessionID,
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

            return userSessionID;
        }

        public async Task<int> UpdateSession_UserID(Guid userSessionID, Guid UserID)
        {
            var userSession = await repository.GetAll<UserSession>(x => x.ID == userSessionID).FirstOrDefaultAsync();
            userSession.UserID = UserID;
            await repository.UpdateAsync(userSession, true);
            return 1;
        }

        public async Task<int> UserSessionLogOut(Guid userSessionID, Guid userID)
        {
            try
            {
                var now = DateTime.Now.ToUniversalTime();

                var userSession = await repository.GetAll<UserSession>(x => x.ID == userSessionID).FirstOrDefaultAsync();
                if (userSession != null)
                {
                    userSession.LogOutDate = now;
                    return await repository.UpdateAsync(userSession, true);
                }
                else
                {
                    var userSessions = await repository.GetAll<UserSession>(x => x.UserID == userID && x.LogOutDate == null).ToListAsync();

                    foreach (var _userSession in userSessions)
                    {
                        _userSession.LogOutDate = now;
                    }
                    await repository.SaveAsync();
                }
            }
            catch (Exception ex)
            {
                await CreateErrorLogAsync(userSessionID: userSessionID, where: "UserLogService.UserSessionLogOut", ex);
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
        public async Task<int> CreateUserLogAsync(Guid userSessionID, string userLogActionType, string comment = null, List<int> errorLogIDs = null)
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

            try
            {
                if (errorLogIDs != null && errorLogIDs.Count > 0 && userLog.ID != 0)
                {
                    foreach (var errorLogID in errorLogIDs)
                    {
                        await repository.CreateAsync<UserErrorLog>(new UserErrorLog
                        {
                            UserLogID = userLog.ID,
                            ErrorLogID = errorLogID
                        });
                    }
                    await repository.SaveAsync();
                }
            }
            catch (Exception ex)
            {
            }

            return userLog.ID;
        }
        public int CreateUserLog(Guid userSessionID, string userLogActionType, string comment = null, List<int> errorLogIDs = null)
        {
            UserLog userLog = new UserLog();

            try
            {
                userLog.CurrentDateTime = DateTime.Now.ToUniversalTime();
                userLog.ActionCodeName = userLogActionType;
                userLog.UserSessionID = userSessionID;
                userLog.Comment = comment;

                repository.Create(userLog, true);
            }
            catch (Exception ex)
            {

            }

            try
            {
                if (errorLogIDs != null && errorLogIDs.Count > 0 && userLog.ID != 0)
                {
                    foreach (var errorLogID in errorLogIDs)
                    {
                        repository.Create<UserErrorLog>(new UserErrorLog
                        {
                            UserLogID = userLog.ID,
                            ErrorLogID = errorLogID
                        });
                    }
                    repository.Save();
                }
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
        public async Task<int> CreateErrorLogAsync(Guid userSessionID, string where, Exception exception, string comment = null, List<int> userLogIDs = null)
        {
            repository.ResetContextState();

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

            try
            {
                if (userLogIDs != null && userLogIDs.Count > 0 && log.ID != 0)
                {
                    foreach (var userLogID in userLogIDs)
                    {
                        await repository.CreateAsync<UserErrorLog>(new UserErrorLog
                        {
                            UserLogID = userLogID,
                            ErrorLogID = log.ID
                        });
                    }
                    await repository.SaveAsync();
                }
            }
            catch (Exception ex)
            {
            }

            return log.ID;
        }
        public int CreateErrorLog(Guid userSessionID, string where, Exception exception, string comment = null, List<int> userLogIDs = null)
        {
            repository.ResetContextState();

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
                repository.Create(log, true);
            }
            catch (Exception ex)
            {

            }

            try
            {
                if (userLogIDs != null && userLogIDs.Count > 0 && log.ID != 0)
                {
                    foreach (var userLogID in userLogIDs)
                    {
                        repository.Create<UserErrorLog>(new UserErrorLog
                        {
                            UserLogID = userLogID,
                            ErrorLogID = log.ID
                        });
                    }
                    repository.Save();
                }
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
        public async Task<bool> IsBedLimitEnter()
        {
            var today = DateTime.Now.Date;
            var ip = UserInfo.HttpContext.Connection.RemoteIpAddress.ToString();

            return await repository.GetAll<UserSession>(x => x.EnterDate.Date == today
                && x.IP == ip)
                .CountAsync() > 20;
        }
    }
}
