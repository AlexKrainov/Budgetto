using Email.Service;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView;
using MyProfile.Entity.Repository;
using MyProfile.File.Service;
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
        private UserEmailService userConfirmEmailService;
        private FileWorkerService fileWorkerService;

        public UserService(IBaseRepository repository,
            UserLogService userLogService,
            UserEmailService userConfirmEmailService,
            FileWorkerService fileWorkerService)
        {
            this.repository = repository;
            this.userLogService = userLogService;
            this.userConfirmEmailService = userConfirmEmailService;
            this.fileWorkerService = fileWorkerService;
        }

        public UserInfoModel GetUserSettings()
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

        public async Task<bool> UpdatePassword(string newPassword, Guid userID)
        {
            var dbUser = await repository.GetAll<Entity.Model.User>(x => x.ID == userID)
               .FirstOrDefaultAsync();
            dbUser.Password = newPassword;
            await repository.UpdateAsync(dbUser, true);

            return true;
        }


        public async Task<UserInfoModel> SaveUserInfo(UserInfoModel userInfoModel)
        {
            var user = UserInfo.Current;
            var dbUser = await repository.GetAll<Entity.Model.User>(x => x.ID == user.ID)
                .FirstOrDefaultAsync();
            string oldEmail = new string(dbUser.Email);

            dbUser.Name = user.Name = userInfoModel.Name;
            dbUser.LastName = user.LastName = userInfoModel.LastName;
            dbUser.Email = user.Email = userInfoModel.Email;


            if (!string.IsNullOrEmpty(userInfoModel.ImageBase64))
            {
                //сначало создаем/апдейтим файл и дальше сохраняем его в базу

                if (dbUser.Resource == null)
                {
                    dbUser.Resource = new Resource
                    {
                        BodyBase64 = userInfoModel.ImageBase64
                    };
                }
                else
                {
                    dbUser.Resource.BodyBase64 = userInfoModel.ImageBase64;
                }

                if (dbUser.ResourceID == null)
                {
                    fileWorkerService.CreateFileFromBase64(dbUser.Resource, ResourceFolder.Users);
                }
                else if (userInfoModel.ResourceID == null)
                {
                    fileWorkerService.UpdateFileFromBase64(dbUser.Resource, ResourceFolder.Users);
                }
                else
                {

                }
                userInfoModel.ImageLink = user.ImageLink = dbUser.ImageLink = dbUser.Resource.SrcPath;
                userInfoModel.ImageBase64 = null;
            }

            await repository.UpdateAsync(dbUser, true);

            //Check if we have alredy the new email ???

            if (oldEmail != userInfoModel.Email)
            {
                await UserInfo.ReSignInAsync(user);
                await userConfirmEmailService.ConfirmEmail(user);

                user.IsConfirmEmail = dbUser.IsConfirmEmail = false;

                await repository.UpdateAsync(dbUser, true);
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
                    CollectiveBudgetID = x.CollectiveBudgetUser != null ? x.CollectiveBudgetUser.CollectiveBudgetID : Guid.Empty,
                    DateCreate = x.DateCreate,
                    ImageLink = x.ImageLink,
                    IsAllowCollectiveBudget = x.IsAllowCollectiveBudget,
                    LastName = x.LastName,
                    Name = x.Name,
                    UserTypeID = x.UserTypeID,
                    //CollectiveBudget = new CollectiveBudget
                    //{
                    //    ID = x.CollectiveBudget.ID,
                    //    Name = x.CollectiveBudget.Name,
                    //    Users = x.CollectiveBudget.Users.Select(y => new Entity.Model.User { ID = y.ID }).ToList()
                    //},
                    Currency = x.Currency,
                    UserSettings = new UserSettings
                    {
                        BudgetPages_WithCollective = x.UserSettings.BudgetPages_WithCollective,

                        Month_EarningWidget = x.UserSettings.Month_EarningWidget,
                        Month_InvestingWidget = x.UserSettings.Month_InvestingWidget,
                        Month_SpendingWidget = x.UserSettings.Month_SpendingWidget,
                        Month_BigCharts = x.UserSettings.Month_BigCharts,
                        Month_GoalWidgets = x.UserSettings.Month_GoalWidgets,
                        Month_LimitWidgets = x.UserSettings.Month_LimitWidgets,

                        Year_EarningWidget = x.UserSettings.Year_EarningWidget,
                        Year_InvestingWidget = x.UserSettings.Year_InvestingWidget,
                        Year_SpendingWidget = x.UserSettings.Year_SpendingWidget,
                        Year_BigCharts = x.UserSettings.Year_BigCharts,
                        Year_GoalWidgets = x.UserSettings.Year_GoalWidgets,
                        Year_LimitWidgets = x.UserSettings.Year_LimitWidgets,

                        GoalPage_IsShow_Collective = x.UserSettings.GoalPage_IsShow_Collective,
                        GoalPage_IsShow_Finished = x.UserSettings.GoalPage_IsShow_Finished,

                        LimitPage_Show_IsFinished = x.UserSettings.LimitPage_Show_IsFinished,
                        LimitPage_IsShow_Collective = x.UserSettings.LimitPage_IsShow_Collective,
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
