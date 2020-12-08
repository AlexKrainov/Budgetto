using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Email.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyProfile.API.Model;
using MyProfile.Entity.ModelView.User;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
using MyProfile.User.Service;
using MyProfile.UserLog.Service;

namespace MyProfile.API.Controllers
{
    [Route("v1_0/[controller]")]
    [ApiController]
    public class UserSessionController : ControllerBase
    {
        private IBaseRepository repository;
        private UserLogService userLogService;
        private UserService userService;
        private UserEmailService userEmailService;
        private CollectionUserService collectionUserService;

        public UserSessionController(IBaseRepository repository,
            UserLogService userLogService,
            UserService userService,
            UserEmailService userConfirmEmailService,
            CollectionUserService collectionUserService)
        {
            this.repository = repository;
            this.userLogService = userLogService;
            this.userService = userService;
            this.userEmailService = userConfirmEmailService;
            this.collectionUserService = collectionUserService;
        }

        // GET: api/Accounts
        [HttpPost("UpdateUserSession")]
        public async Task<ActionResult<UserSettings>> UpdateUserSession([FromBody]UserStatViewModel personData)
        {
            UserSettings userSettings = new UserSettings();
            userSettings.IsOk = true;
            userSettings.SessionID = await userLogService.UpdateSession(personData);

            Response.Cookies.Append(UserInfo.USER_SESSION_ID, userSettings.SessionID.ToString());

            return userSettings;
        }

        // GET: api/Accounts
        [HttpGet("TestOK")]
        public async Task<ActionResult<string>> TestOK()
        {
            return "OK";
        }
    }
}
