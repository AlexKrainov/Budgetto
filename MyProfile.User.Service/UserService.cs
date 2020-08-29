using Email.Service;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView;
using MyProfile.Entity.Repository;
using MyProfile.File.Service;
using MyProfile.Identity;
using MyProfile.User.Service.PasswordWorker;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public UserService(IBaseRepository repository,
            UserLogService userLogService,
            UserEmailService userConfirmEmailService,
            FileWorkerService fileWorkerService,
            PasswordService passwordService)
        {
            this.repository = repository;
            this.userLogService = userLogService;
            this.userConfirmEmailService = userConfirmEmailService;
            this.fileWorkerService = fileWorkerService;
            this.passwordService = passwordService;
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
                UserSettings = new UserSettings
                {
                    WebSiteTheme_CodeName = currentUser.UserSettings.WebSiteTheme_CodeName
                },
            };
        }

        public async Task<UserInfoModel> CheckAndGetUser(string email, string password = null, Guid? userID = null)
        {
            var predicate = PredicateBuilder.True<Entity.Model.User>();

            if (userID != null && userID != Guid.Empty)
            {
                predicate = predicate.And(x => x.ID == userID);
            }
            else
            {
                predicate = predicate.And(x => x.Email == email);
            }

            var user = await repository.GetAll<Entity.Model.User>(predicate)
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
                     HashPassword = x.HashPassword,
                     SaltPassword = x.SaltPassword,
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

                         WebSiteTheme_CodeName = x.UserSettings.WebSiteTheme_CodeName,
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
                user.UserSessionID = await userLogService.CreateSession(user.ID, userActionType);

                await UserInfo.AddOrUpdate_Authenticate(user); // аутентификация
            }
            return user;
        }

        #region Methods create/update user
        public async Task<int> CreateUser(string email, string password)
        {
            var now = DateTime.Now.ToUniversalTime();
            var passwordSalt = passwordService.GenerateSalt();
            var passwordHash = passwordService.GenerateHashSHA256(password, passwordSalt);

            await repository.CreateAsync(new Entity.Model.User
            {
                DateCreate = now,
                Email = email,
                IsAllowCollectiveBudget = false,
                Name = email,
                ImageLink = "/img/user-min.png",
                SaltPassword = passwordSalt,
                HashPassword = passwordHash,
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
                    WebSiteTheme_CodeName = WebSiteThemeEnum.Light,
                },
                CurrencyID = 1,
                UserTypeID = (int)UserTypeEnum.User,
                ToDoListFolders = new List<ToDoListFolder>
                {
                    new ToDoListFolder
                    {
                        Title = "Краткосрочные покупки",
                        CssIcon = "ion ion-md-cart"
                    },
                    new ToDoListFolder
                    {
                        Title = "Среднесрочные покупки",
                        CssIcon = "ion ion-ios-home"
                    },
                    new ToDoListFolder
                    {
                        Title = "Долгосрочные цели",
                        CssIcon = "ion ion-md-car"
                    },
                },
                BudgetAreas = new List<BudgetArea> {
                new BudgetArea
                {
                    IsShowOnSite = true,
                    IsShowInCollective = true,
                    Name = "Краткосрочные покупки",
                    CodeName = "ShortTermPurchases",
                    Description = null,
                    BudgetSectinos = new List<BudgetSection>
                    {
                        new BudgetSection
                        {
                            Name = "Продукты",
                            Description = "Краткосрочные покупки в магазинах",
                            CodeName = "Products",
                            CssIcon = "fas fa-shopping-cart",
                            CssColor = "#fff",
                            CssBackground = "#f44336",
                            IsShowInCollective = true,
                            IsShowOnSite = true,
                            SectionTypeID = (int)SectionTypeEnum.Spendings,
                        },
                        new BudgetSection
                        {
                            Name = "Фастфуд",
                            CodeName = "FastFood",
                            CssIcon = "fas fa-hamburger",
                            CssColor = "rgba(24,28,33,0.8)",
                            CssBackground = "#ffcdd2",
                            IsShowInCollective = true,
                            IsShowOnSite = true,
                            SectionTypeID = (int)SectionTypeEnum.Spendings,
                        },
                         new BudgetSection
                        {
                            Name = "Общ. транспорт",
                            CodeName = "CommonTransport",
                            CssIcon = "fas fa-bus",
                            CssColor = "rgba(24,28,33,0.8)",
                            CssBackground = "#ffebee",
                            IsShowInCollective = true,
                            IsShowOnSite = true,
                            SectionTypeID = (int)SectionTypeEnum.Spendings,
                        },
                    }
                },
                new BudgetArea
                {
                    IsShowOnSite = true,
                    IsShowInCollective = true,
                    Name = "Общие расходы",
                    CodeName = "GeneralExpenses",
                    Description = null,
                    BudgetSectinos = new List<BudgetSection>
                    {
                        new BudgetSection
                        {
                            Name = "Книги",
                            Description = "Books",
                            CodeName = "Books",
                            CssIcon = "fas fa-book",
                            CssColor = "rgba(24,28,33,0.8)",
                            CssBackground = "#eeeeee",
                            IsShowInCollective = true,
                            IsShowOnSite = true,
                            SectionTypeID = (int)SectionTypeEnum.Spendings,
                        },
                         new BudgetSection
                        {
                            Name = "Налоги",
                            CodeName = "Taxes",
                            CssIcon = "fas fa-landmark",
                            CssColor = "rgba(24,28,33,0.8)",
                            CssBackground = "#bdbdbd",
                            IsShowInCollective = true,
                            IsShowOnSite = true,
                            SectionTypeID = (int)SectionTypeEnum.Spendings,
                        }
                    }
                },
                new BudgetArea
                {
                    IsShowOnSite = true,
                    IsShowInCollective = true,
                    Name = "Долгосрочные расходы",
                    CodeName = "LongTermExpenses",
                    Description = null,
                    BudgetSectinos = new List<BudgetSection>
                    {
                        new BudgetSection
                        {
                            Name = "Одежда",
                            CodeName = "Clothes",
                            CssIcon = "fas fa-tshirt",
                            CssColor = "rgba(24,28,33,0.8)",
                            CssBackground = "#ffcc80",
                            IsShowInCollective = true,
                            IsShowOnSite = true,
                            SectionTypeID = (int)SectionTypeEnum.Spendings,
                        },
                        new BudgetSection
                        {
                            Name = "Крупная бытовая тех.",
                            Description = "Телевизор, пылесос, стиральная машинка и тд.",
                            CodeName = "LargeHouseholdAppliances",
                            CssIcon = "fas fa-tv",
                            CssColor = "rgba(24,28,33,0.8)",
                            CssBackground = "#ffb74d",
                            IsShowInCollective = true,
                            IsShowOnSite = true,
                            SectionTypeID = (int)SectionTypeEnum.Spendings,
                        }
                    }
                },
                new BudgetArea
                {
                    IsShowOnSite = true,
                    IsShowInCollective = true,
                    Name = "Доходы",
                    CodeName = "Income",
                    Description = null,
                    BudgetSectinos = new List<BudgetSection>
                    {
                        new BudgetSection
                        {
                            Name = "Зарплата",
                            CodeName = "Salary",
                            CssIcon = "fas fa-wallet",
                            CssColor = "rgba(24,28,33,0.8)",
                            CssBackground = "#00e676",
                            IsShowInCollective = true,
                            IsShowOnSite = true,
                            SectionTypeID = (int)SectionTypeEnum.Earnings,
                        },
                        new BudgetSection
                        {
                            Name = "Доп. доходы",
                            CodeName = "OtherIncome",
                            CssIcon = "fas fa-donate",
                            CssColor = "rgba(24,28,33,0.8)",
                            CssBackground = "#b9f6ca",
                            IsShowInCollective = true,
                            IsShowOnSite = true,
                            SectionTypeID = (int)SectionTypeEnum.Earnings,
                        },
                        new BudgetSection
                        {
                            Name = "Кэшбэки",
                            CodeName = "Cashback",
                            CssIcon = "fas fa-ruble-sign",
                            CssColor = "rgba(24,28,33,0.8)",
                            CssBackground = "#b9f6ca",
                            IsShowInCollective = true,
                            IsShowOnSite = true,
                            SectionTypeID = (int)SectionTypeEnum.Earnings,
                        },
                         new BudgetSection
                        {
                            Name = "Инвестиции",
                            Description = "Депозиты, акции, облигации и тд",
                            CodeName = "Investing",
                            CssIcon = "fas fa-piggy-bank",
                            CssColor = "rgba(24,28,33,0.8)",
                            CssBackground = "#b9f6ca",
                            IsShowInCollective = true,
                            IsShowOnSite = true,
                            SectionTypeID = (int)SectionTypeEnum.Earnings,
                        }
                    }
                },
                new BudgetArea
                {
                    IsShowOnSite = true,
                    IsShowInCollective = true,
                    Name = "Ремонт",
                    CodeName = "Repairs",
                    Description = "Все возможные ремонты",
                    BudgetSectinos = new List<BudgetSection>
                    {
                        new BudgetSection
                        {
                            Name = "Ремонт на кухне",
                            CodeName = "KitchenRenovation",
                            CssIcon = "fas fa-paint-roller",
                            CssColor = "rgba(24,28,33,0.8)",
                            CssBackground = "#ffb74d",
                            IsShowInCollective = true,
                            IsShowOnSite = true,
                            SectionTypeID = (int)SectionTypeEnum.Spendings,
                        },
                        new BudgetSection
                        {
                            Name = "Ремонт в квартире",
                            CodeName = "ApartmentRenovation",
                            CssIcon = "fas fa-brush",
                            CssColor = "rgba(24,28,33,0.8)",
                            CssBackground = "#ffb74d",
                            IsShowInCollective = true,
                            IsShowOnSite = true,
                            SectionTypeID = (int)SectionTypeEnum.Spendings,
                        },
                        new BudgetSection
                        {
                            Name = "Ремонт на даче",
                            CodeName = "RepairInTheCountry",
                            CssIcon = "fas fa-home",
                            CssColor = "rgba(24,28,33,0.8)",
                            CssBackground = "#ffe0b2",
                            IsShowInCollective = true,
                            IsShowOnSite = true,
                            SectionTypeID = (int)SectionTypeEnum.Spendings,
                        },
                        new BudgetSection
                        {
                            Name = "Ремонт у родителей",
                            CodeName = "RepairsAtParents",
                            CssIcon = "fas fa-paint-roller",
                            CssColor = "rgba(24,28,33,0.8)",
                            CssBackground = "#ffb74d",
                            IsShowInCollective = true,
                            IsShowOnSite = true,
                            SectionTypeID = (int)SectionTypeEnum.Spendings,
                        }

                    }
                },
                new BudgetArea
                {
                    IsShowOnSite = true,
                    IsShowInCollective = true,
                    Name = "Регулярные платежи",
                    CodeName = "RecurringPayments",
                    Description = null,
                    BudgetSectinos = new List<BudgetSection>
                    {
                        new BudgetSection
                        {
                            Name = "Электричество",
                            CodeName = "Electricity",
                            CssIcon = "fas fa-plug",
                            CssColor = "rgba(24,28,33,0.8)",
                            CssBackground = "#eeeeee",
                            IsShowInCollective = true,
                            IsShowOnSite = true,
                            SectionTypeID = (int)SectionTypeEnum.Spendings,
                        },
                        new BudgetSection
                        {
                            Name = "За квартиру",
                            CodeName = "ForTheApartment",
                            CssIcon = "far fa-building",
                            CssColor = "rgba(24,28,33,0.8)",
                            CssBackground = "#e0e0e0",
                            IsShowInCollective = true,
                            IsShowOnSite = true,
                            SectionTypeID = (int)SectionTypeEnum.Spendings,
                        },
                        new BudgetSection
                        {
                            Name = "Связь",
                            CodeName = "Communication",
                            CssIcon = "fas fa-phone",
                            CssColor = "rgba(24,28,33,0.8)",
                            CssBackground = "#eeeeee",
                            IsShowInCollective = true,
                            IsShowOnSite = true,
                            SectionTypeID = (int)SectionTypeEnum.Spendings,
                        },
                        new BudgetSection
                        {
                            Name = "Интернет",
                            CodeName = "WiFi",
                            CssIcon = "fas fa-wifi",
                            CssColor = "rgba(24,28,33,0.8)",
                            CssBackground = "#eeeeee",
                            IsShowInCollective = true,
                            IsShowOnSite = true,
                            SectionTypeID = (int)SectionTypeEnum.Spendings,
                        }
                    }
                },
                new BudgetArea
                {
                    IsShowOnSite = true,
                    IsShowInCollective = true,
                    Name = "Медицина",
                    CodeName = "Medicine",
                    Description = null,
                    BudgetSectinos = new List<BudgetSection>
                    {
                        new BudgetSection
                        {
                            Name = "Стоматология",
                            Description = "Краткосрочные покупки в магазинах",
                            CodeName = "Dentistry",
                            CssIcon = "fas fa-tooth",
                            CssColor = "rgba(24,28,33,0.8)",
                            CssBackground = "#bdbdbd",
                            IsShowInCollective = true,
                            IsShowOnSite = true,
                            SectionTypeID = (int)SectionTypeEnum.Spendings,
                        },
                        new BudgetSection
                        {
                            Name = "Посещение врача",
                            Description = "Прием врача, консультация, массаж, все возможные обследования",
                            CodeName = "VisitDoctor",
                            CssIcon = "fas fa-user-md",
                            CssColor = "#fff",
                            CssBackground = "#9e9e9e",
                            IsShowInCollective = true,
                            IsShowOnSite = true,
                            SectionTypeID = (int)SectionTypeEnum.Spendings,
                        }
                    }
                },
                new BudgetArea
                {
                    IsShowOnSite = true,
                    IsShowInCollective = true,
                    Name = "Отдых",
                    CodeName = "Recreation",
                    Description = null,
                    BudgetSectinos = new List<BudgetSection>
                    {
                        new BudgetSection
                        {
                            Name = "Рестораны",
                            CodeName = "Restaurants",
                            CssIcon = "fas fa-glass-cheers",
                            CssColor = "rgba(24,28,33,0.8)",
                            CssBackground = "#bbdefb",
                            IsShowInCollective = true,
                            IsShowOnSite = true,
                            SectionTypeID = (int)SectionTypeEnum.Spendings,
                        },
                        new BudgetSection
                        {
                            Name = "Путешествия",
                            CodeName = "Travels",
                            CssIcon = "fas fa-plane",
                            CssColor = "#fff",
                            CssBackground = "#2196f3",
                            IsShowInCollective = true,
                            IsShowOnSite = true,
                            SectionTypeID = (int)SectionTypeEnum.Spendings,
                        },
                        new BudgetSection
                        {
                            Name = "Выезд за город",
                            CodeName = "OutOfTown",
                            CssIcon = "fas fa-drumstick-bite",
                            CssColor = "rgba(24,28,33,0.8)",
                            CssBackground = "#bbdefb",
                            IsShowInCollective = true,
                            IsShowOnSite = true,
                            SectionTypeID = (int)SectionTypeEnum.Spendings,
                        }
                    }
                },
                new BudgetArea
                {
                    IsShowOnSite = true,
                    IsShowInCollective = true,
                    Name = "Авто",
                    CodeName = "Auto",
                    Description = null,
                    BudgetSectinos = new List<BudgetSection>
                    {
                        new BudgetSection
                        {
                            Name = "Страховка",
                            Description = "Осаго, каско и тд",
                            CodeName = "CarInsurance",
                            CssIcon = "fas fa-car",
                            CssColor = "rgba(24,28,33,0.8)",
                            CssBackground = "#e0e0e0",
                            IsShowInCollective = true,
                            IsShowOnSite = true,
                            SectionTypeID = (int)SectionTypeEnum.Spendings,
                        },
                        new BudgetSection
                        {
                            Name = "Бензин",
                            CodeName = "Petrol",
                            CssIcon = "fas fa-fill-drip",
                            CssColor = "rgba(24,28,33,0.8)",
                            CssBackground = "#e0e0e0",
                            IsShowInCollective = true,
                            IsShowOnSite = true,
                            SectionTypeID = (int)SectionTypeEnum.Spendings,
                        },
                        new BudgetSection
                        {
                            Name = "Ремонт авто",
                            CodeName = "AutoRepair",
                            CssIcon = "fas fa-hammer",
                            CssColor = "rgba(24,28,33,0.8)",
                            CssBackground = "#e0e0e0",
                            IsShowInCollective = true,
                            IsShowOnSite = true,
                            SectionTypeID = (int)SectionTypeEnum.Spendings,
                        }
                    }
                },
                },

            }, true);

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



        public async Task<UserInfoModel> UpdateUser(UserInfoModel userInfoModel)
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

            await userLogService.CreateUserLog(user.UserSessionID, UserLogActionType.User_Edit);
            return userInfoModel;
        }

        public async Task<int> UpdateUserSettings(UserSettingsModelView userSettings)
        {
            var user = UserInfo.Current;
            var dbUser = await repository.GetAll<Entity.Model.User>(x => x.ID == user.ID)
                .FirstOrDefaultAsync();

            user.UserSettings.WebSiteTheme_CodeName = dbUser.UserSettings.WebSiteTheme_CodeName = userSettings.WebSiteTheme_CodeName;

            await repository.UpdateAsync(dbUser, true);
            await UserInfo.AddOrUpdate_Authenticate(user);

            return 1;
        }
        #endregion
    }
}
