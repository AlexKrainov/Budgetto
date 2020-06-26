using Email.Service;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProfile.User.Service
{
    public class UserService
    {
        private IBaseRepository repository;
        private UserLogService userLogService;
        private UserConfirmEmailService userConfirmEmailService;

        public UserService(IBaseRepository repository,
            UserLogService userLogService,
            UserConfirmEmailService userConfirmEmailService)
        {
            this.repository = repository;
            this.userLogService = userLogService;
            this.userConfirmEmailService = userConfirmEmailService;
        }

        public UserInfoModel GetUserSettingsAsync()
        {
            var currentUser = UserInfo.Current;

            return new UserInfoModel
            {
                CollectiveBudgetID = currentUser.CollectiveBudgetID,
                DateCreate = currentUser.DateCreate,
                Email = currentUser.Email,
                ImageLink = currentUser.ImageLink,
                IsAllowCollectiveBudget = currentUser.IsAllowCollectiveBudget,
                LastName = currentUser.LastName,
                Name = currentUser.Name,
                IsConfirmEmail = currentUser.IsConfirmEmail,
            };
        }

        public async Task<UserInfoModel> SaveUserInfo(UserInfoModel userInfoModel)
        {
            var user = UserInfo.Current;
            var oldUser = await repository.GetAll<Entity.Model.User>(x => x.ID == user.ID)
                .FirstOrDefaultAsync();
            string oldEmail = new string(oldUser.Email);

            oldUser.Name = user.Name = userInfoModel.Name;
            oldUser.LastName = user.LastName = userInfoModel.LastName;
            oldUser.Email = user.Email = userInfoModel.Email;

            await repository.UpdateAsync(oldUser, true);

            if (oldEmail != userInfoModel.Email)
            {
                await UserInfo.ReSignInAsync(user);
                await userConfirmEmailService.ConfirmEmail(false);
            }
            else
            {
                await UserInfo.AddOrUpdate_Authenticate(user);
            }

            return userInfoModel;
        }

        public async Task<UserInfoModel> AuthenticateOrUpdateUserInfo(string email, string password, string userActionType)
        {
            var user = await repository.GetAll<Entity.Model.User>(x => x.Email == email && x.Password == password)
                .Select(x => new UserInfoModel
                {
                    ID = x.ID,
                    Email = x.Email,
                    CollectiveBudgetID = x.CollectiveBudgetID,
                    DateCreate = x.DateCreate,
                    ImageLink = x.ImageLink,
                    IsAllowCollectiveBudget = x.IsAllowCollectiveBudget,
                    LastName = x.LastName,
                    Name = x.Name,
                    CollectiveBudget = new CollectiveBudget
                    {
                        ID = x.CollectiveBudget.ID,
                        DateCreate = x.CollectiveBudget.DateCreate,
                        DateDelete = x.CollectiveBudget.DateDelete,
                        Name = x.CollectiveBudget.Name,
                        Users = x.CollectiveBudget.Users.Select(y => new Entity.Model.User { ID = y.ID }).ToList()
                    },
                    UserSettings = new UserSettings
                    {
                        BudgetPages_WithCollective = x.UserSettings.BudgetPages_WithCollective
                    }
                })
                .FirstOrDefaultAsync();

            if (user != null)
            {
                user.LastUserLogID = await userLogService.CreateAction(user.ID, userActionType);

                await UserInfo.AddOrUpdate_Authenticate(user); // аутентификация
                UserInfo.LastUserLogID = (int)user.LastUserLogID;

            }
            return user;
        }
    }
}
