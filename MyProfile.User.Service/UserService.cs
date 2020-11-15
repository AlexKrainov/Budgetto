﻿using Common.Service;
using Email.Service;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView;
using MyProfile.Entity.ModelView.TemplateModelView;
using MyProfile.Entity.ModelView.User;
using MyProfile.Entity.Repository;
using MyProfile.File.Service;
using MyProfile.Identity;
using MyProfile.User.Service.PasswordWorker;
using Newtonsoft.Json;
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
        private PasswordService passwordService;
        private CommonService commonService;

        public UserService(IBaseRepository repository,
            UserLogService userLogService,
            UserEmailService userConfirmEmailService,
            FileWorkerService fileWorkerService,
            PasswordService passwordService,
            CommonService commonService)
        {
            this.repository = repository;
            this.userLogService = userLogService;
            this.userConfirmEmailService = userConfirmEmailService;
            this.fileWorkerService = fileWorkerService;
            this.passwordService = passwordService;
            this.commonService = commonService;
        }

        public UserInfoClientSide GetUserSettings()
        {
            var currentUser = UserInfo.Current;

            return new UserInfoClientSide
            {
                CollectiveBudgetID = currentUser.CollectiveBudgetID,
                DateCreate = currentUser.DateCreate,
                Email = currentUser.Email,
                ImageLink = currentUser.ImageLink,
                IsAllowCollectiveBudget = currentUser.IsAllowCollectiveBudget,
                LastName = currentUser.LastName,
                Name = currentUser.Name,
                IsConfirmEmail = currentUser.IsConfirmEmail,
                UserSettings = new UserSettingsClientSide
                {
                    WebSiteTheme = currentUser.UserSettings.WebSiteTheme,
                    NewsLetter = currentUser.UserSettings.NewsLetter,
                    IsShowHints = currentUser.UserSettings.IsShowHints,
                    IsShowFirstEnterHint = currentUser.UserSettings.IsShowFirstEnterHint,
                    IsShowCookie = currentUser.UserSettings.IsShowCookie,
                },
                IsAvailable = currentUser.IsAvailable,
                Payment = new PaymentClientSide
                {
                    DateFrom = currentUser.Payment.DateFrom,
                    DateTo = currentUser.Payment.DateTo,
                    Tariff = currentUser.Payment.Tariff
                }
            };
        }

        public async Task<UserInfoModel> CheckAndGetUser(string email, string password = null, Guid? userID = null)
        {
            var now = DateTime.Now.ToUniversalTime();

            var predicate = PredicateBuilder.True<Entity.Model.User>();

            if (userID != null && userID != Guid.Empty)
            {
                predicate = predicate.And(x => x.ID == userID);
            }
            else
            {
                predicate = predicate.And(x => x.Email == email);
            }

            UserInfoModel user = await repository.GetAll<Entity.Model.User>(predicate)
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
                     IsConfirmEmail = x.IsConfirmEmail,
                     CurrencyID = x.CurrencyID,
                     UserType = x.UserType,
                     //CollectiveBudget = new CollectiveBudget
                     //{
                     //    ID = x.CollectiveBudget.ID,
                     //    Name = x.CollectiveBudget.Name,
                     //    Users = x.CollectiveBudget.Users.Select(y => new Entity.Model.User { ID = y.ID }).ToList()
                     //},

                     HashPassword = x.HashPassword,
                     SaltPassword = x.SaltPassword,
                     Currency = x.Currency,
                     IsAvailable = x.Payment.DateFrom <= now && x.Payment.DateTo >= now,
                     Payment = new Payment
                     {
                         DateFrom = x.Payment.DateFrom,
                         DateTo = x.Payment.DateTo,
                         ID = x.Payment.ID,
                         IsPaid = x.Payment.IsPaid,
                         LastDatePayment = x.Payment.LastDatePayment,
                         Tariff = x.Payment.Tariff,
                     },
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

                         WebSiteTheme = x.UserSettings.WebSiteTheme,
                         NewsLetter = x.UserSettings.NewsLetter,
                         IsShowHints = x.UserSettings.IsShowHints,
                         IsShowFirstEnterHint = x.UserSettings.IsShowFirstEnterHint,
                         IsShowConstructor = x.UserSettings.IsShowConstructor,
                         IsShowCookie = x.UserSettings.IsShowCookie,
                     }
                 })
                 .FirstOrDefaultAsync();

            if (user != null)
            {
                if (password != null && user.HashPassword != passwordService.GenerateHashSHA256(password, user.SaltPassword))
                {
                    user = null;
                }
                else
                {
                    user.SaltPassword = null;
                    user.HashPassword = null;
                }
            }
            return user;
        }



        public async Task<UserInfoModel> AuthenticateOrUpdateUserInfo(UserInfoModel user, string userActionType)
        {
            if (user != null)
            {
                await userLogService.CreateUserLogAsync(user.UserSessionID, userActionType);

                await UserInfo.AddOrUpdate_Authenticate(user); // аутентификация
            }
            return user;
        }

        #region Methods create/update user
        public async Task<int> CreateUser(string email, string password, Guid userSessionID)
        {
            var now = DateTime.Now.ToUniversalTime();
            var passwordSalt = passwordService.GenerateSalt();
            var passwordHash = passwordService.GenerateHashSHA256(password, passwordSalt);
            // var newUserID = Guid.NewGuid();

            var newUser = new Entity.Model.User
            {
                //ID = newUserID,
                DateCreate = now,
                Email = email,
                IsAllowCollectiveBudget = false,
                Name = email,
                ImageLink = "/img/user-min.png",
                SaltPassword = passwordSalt,
                HashPassword = passwordHash,
                CurrencyID = 1,
                CollectiveBudgetUser = new Entity.Model.CollectiveBudgetUser
                {
                    DateAdded = now,
                    DateUpdate = now,
                    Status = CollectiveUserStatusType.Accepted.ToString(),
                    CollectiveBudget = new Entity.Model.CollectiveBudget
                    {
                        Name = email,
                    }
                },
                UserSettings = new Entity.Model.UserSettings
                {
                    BudgetPages_WithCollective = true,
                    Month_EarningWidget = true,
                    Month_InvestingWidget = true,
                    Month_SpendingWidget = true,
                    WebSiteTheme = WebSiteThemeEnum.Light,
                    IsShowConstructor = true,
                },
                Payment = new Payment
                {
                    DateFrom = now,
                    DateTo = now.AddMonths(2),
                    IsPaid = false,
                    Tariff = PaymentTariffs.Free,
                    PaymentHistories = new List<PaymentHistory> {
                        new PaymentHistory {
                            DateFrom = now,
                            DateTo = now.AddMonths(2),
                            Tariff = PaymentTariffs.Free,
                        }
                    }
                },
                UserTypeID = (int)UserTypeEnum.User,
            };
            await repository.CreateAsync(newUser, true);

            return 1;
        }


        public async Task<bool> UpdatePassword(string newPassword, Guid userID)
        {
            var dbUser = await repository
                .GetAll<Entity.Model.User>(x => x.ID == userID)
               .FirstOrDefaultAsync();

            var passwordHash = passwordService.GenerateHashSHA256(newPassword, dbUser.SaltPassword);
            //var passwordHash = passwordService.GenerateHashSHA256(newPassword, dbUser.SaltPassword);
            dbUser.HashPassword = passwordHash;

            await repository.UpdateAsync(dbUser, true);

            return true;
        }

        public async Task<bool> SetConfirmEmail(Guid userID, bool isConfirmEmail)
        {
            var dbUser = await repository
                .GetAll<Entity.Model.User>(x => x.ID == userID)
               .FirstOrDefaultAsync();

            dbUser.IsConfirmEmail = isConfirmEmail;

            await repository.UpdateAsync(dbUser, true);

            return isConfirmEmail;
        }



        public async Task<UserInfoModel> UpdateUser(UserInfoModel userInfoModel, bool userSettingsSave = false)
        {
            var user = UserInfo.Current;
            var dbUser = await repository.GetAll<Entity.Model.User>(x => x.ID == user.ID)
                .FirstOrDefaultAsync();
            string oldEmail = new string(dbUser.Email);

            dbUser.Name = user.Name = userInfoModel.Name?.Trim();
            dbUser.LastName = user.LastName = userInfoModel.LastName?.Trim();
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

            if (userSettingsSave)
            {
                dbUser.UserSettings.IsShowConstructor = userInfoModel.UserSettings.IsShowConstructor;
            }

            await repository.UpdateAsync(dbUser, true);

            //Check if we have alredy the new email ???

            if (oldEmail != userInfoModel.Email)
            {
                await UserInfo.ReSignInAsync(user);
                await userConfirmEmailService.ConfirmEmail(user);

                user.IsConfirmEmail = dbUser.IsConfirmEmail = userInfoModel.IsConfirmEmail = false;

                await repository.UpdateAsync(dbUser, true);
                await userLogService.CreateUserLogAsync(user.UserSessionID, UserLogActionType.User_Change_Email);
            }

            await UserInfo.AddOrUpdate_Authenticate(user);
            await userLogService.CreateUserLogAsync(user.UserSessionID, UserLogActionType.User_Edit);

            return userInfoModel;
        }

        public async Task<int> UpdateUserSettings(UserSettingsModelView userSettings)
        {
            var user = UserInfo.Current;
            var dbUser = await repository.GetAll<Entity.Model.User>(x => x.ID == user.ID)
                .FirstOrDefaultAsync();

            user.UserSettings.WebSiteTheme = dbUser.UserSettings.WebSiteTheme = userSettings.WebSiteTheme;
            user.UserSettings.NewsLetter = dbUser.UserSettings.NewsLetter = userSettings.NewsLetter;
            user.UserSettings.IsShowHints = dbUser.UserSettings.IsShowHints = userSettings.IsShowHints;

            await repository.UpdateAsync(dbUser, true);
            await UserInfo.AddOrUpdate_Authenticate(user);

            return 1;
        }
        #endregion
    }
}
