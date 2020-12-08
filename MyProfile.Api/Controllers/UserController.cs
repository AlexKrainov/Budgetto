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
using MyProfile.User.Service;
using MyProfile.UserLog.Service;

namespace MyProfile.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IBaseRepository repository;
        private UserLogService userLogService;
        private UserService userService;
        private UserEmailService userEmailService;
        private CollectionUserService collectionUserService;

        public UserController(IBaseRepository repository,
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
        [HttpGet("getusers")]
        public async Task<ActionResult<IEnumerable<UserSettings>>> GetUsers()
        {
            return await repository.GetAll<MyProfile.Entity.Model.User>()
                .Select(x => new UserSettings
                {
                    Name = x.Name,
                    Email = x.Email
                })
                .ToListAsync();
        }
    }
}
