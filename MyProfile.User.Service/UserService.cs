using Common.Service;
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
                await userLogService.CreateUserLog(user.UserSessionID, userActionType);

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
                ToDoListFolders = new List<ToDoListFolder>
                {
                    new ToDoListFolder
                    {
                        Title = "Покупки",
                        CssIcon = "ion ion-md-cart"
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
                                CssColor = "rgba(24,28,33,0.8)",
                                CssBackground = "rgb(255, 235, 238)",
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
                                CssBackground = "rgb(255, 235, 238)",
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
                                CssBackground = "rgb(255, 235, 238)",
                                IsShowInCollective = true,
                                IsShowOnSite = true,
                                SectionTypeID = (int)SectionTypeEnum.Spendings,
                            },
                            new BudgetSection
                            {
                                Name = "Рестораны",
                                CodeName = "Restorans",
                                CssIcon = "fas fa-glass-cheers",
                                CssColor = "rgba(24,28,33,0.8)",
                                CssBackground = "rgb(255, 235, 238)",
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
                                Name = "Подарки",
                                Description = "",
                                CodeName = "Gifts",
                                CssIcon = "fas fa-birthday-cake",
                                CssColor = "rgba(24,28,33,0.8)",
                                CssBackground = "rgb(255, 205, 210)",
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
                                CssBackground = "rgb(255, 205, 210)",
                                IsShowInCollective = true,
                                IsShowOnSite = true,
                                SectionTypeID = (int)SectionTypeEnum.Spendings,
                            },
                              new BudgetSection
                            {
                                Name = "Одежда",
                                CodeName = "Clothes",
                                CssIcon = "fas fa-tshirt",
                                CssColor = "rgba(24,28,33,0.8)",
                                CssBackground = "rgb(255, 205, 210)",
                                IsShowInCollective = true,
                                IsShowOnSite = true,
                                SectionTypeID = (int)SectionTypeEnum.Spendings,
                            },
                               new BudgetSection
                            {
                                Name = "Красота",
                                Description="Парикмахерские, салоны красоты и тд.",
                                CodeName = "Beauty",
                                CssIcon = "far fa-eye",
                                CssColor = "rgba(24,28,33,0.8)",
                                CssBackground = "rgb(255, 205, 210)",
                                IsShowInCollective = true,
                                IsShowOnSite = true,
                                SectionTypeID = (int)SectionTypeEnum.Spendings,
                            },
                                  new BudgetSection
                            {
                                Name = "Развлечения",
                                CodeName = "Entertainment",
                                CssIcon = "fas fa-biking",
                                CssColor = "rgba(24,28,33,0.8)",
                                CssBackground = "rgb(255, 205, 210)",
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
                        Name = "Долгосрочные расходы",
                        CodeName = "LongTermExpenses",
                        Description = null,
                        BudgetSectinos = new List<BudgetSection>
                        {
                           new BudgetSection
                            {
                                Name = "Ремонт",
                                Description = "",
                                CodeName = "Renovation",
                                CssIcon = "fas fa-paint-roller",
                                CssColor = "rgba(24,28,33,0.8)",
                                CssBackground = "rgb(239, 154, 154)",
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
                                CssBackground = "rgb(239, 154, 154)",
                                IsShowInCollective = true,
                                IsShowOnSite = true,
                                SectionTypeID = (int)SectionTypeEnum.Spendings,
                            },
                             new BudgetSection
                            {
                                Name = "Все для дома",
                                Description = "Телевизор, пылесос, стиральная машинка и тд.",
                                CodeName = "AllForHouse",
                                CssIcon = "fas fa-couch",
                                CssColor = "rgba(24,28,33,0.8)",
                                CssBackground = "rgb(239, 154, 154)",
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
                                Name = "Дивиденды",
                                Description = "Дивиденды от депозитов, акций, облигаций и тд",
                                CodeName = "Dividends",
                                CssIcon = "fas fa-piggy-bank",
                                CssColor = "rgba(24,28,33,0.8)",
                                CssBackground = "rgb(185, 246, 202)",
                                IsShowInCollective = true,
                                IsShowOnSite = true,
                                SectionTypeID = (int)SectionTypeEnum.Investments,
                            },
                            new BudgetSection
                            {
                                Name = "Кэшбэки",
                                CodeName = "Cashback",
                                CssIcon = "fas fa-ruble-sign",
                                CssColor = "rgba(24,28,33,0.8)",
                                CssBackground = "rgb(185, 246, 202)",
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
                                CssBackground = "rgb(185, 246, 202)",
                                IsShowInCollective = true,
                                IsShowOnSite = true,
                                SectionTypeID = (int)SectionTypeEnum.Earnings,
                            },
                           new BudgetSection
                            {
                                Name = "Зарплата",
                                CodeName = "Salary",
                                CssIcon = "fas fa-wallet",
                                CssColor = "rgba(24,28,33,0.8)",
                                CssBackground = "rgb(185, 246, 202)",
                                IsShowInCollective = true,
                                IsShowOnSite = true,
                                SectionTypeID = (int)SectionTypeEnum.Earnings,
                            },
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
                                CssBackground = "rgb(238, 238, 238)",
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
                                CssBackground = "rgb(238, 238, 238)",
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
                                CssBackground = "rgb(238, 238, 238)",
                                IsShowInCollective = true,
                                IsShowOnSite = true,
                                SectionTypeID = (int)SectionTypeEnum.Spendings,
                            },
                            new BudgetSection
                            {
                                Name = "Интернет",
                                CodeName = "Internet",
                                CssIcon = "fas fa-wifi",
                                CssColor = "rgba(24,28,33,0.8)",
                                CssBackground = "rgb(238, 238, 238)",
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
                                CodeName = "Dentistry",
                                CssIcon = "fas fa-tooth",
                                CssColor = "rgba(24,28,33,0.8)",
                                CssBackground = "rgb(224, 224, 224)",
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
                                CssColor = "rgba(24,28,33,0.8)",
                                CssBackground = "rgb(224, 224, 224)",
                                IsShowInCollective = true,
                                IsShowOnSite = true,
                                SectionTypeID = (int)SectionTypeEnum.Spendings,
                            },
                             new BudgetSection
                            {
                                Name = "Аптеки",
                                CodeName = "Pharmacy",
                                CssIcon = "fas fa-prescription-bottle-alt",
                                CssColor = "rgba(24,28,33,0.8)",
                                CssBackground = "rgb(224, 224, 224)",
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
                                Name = "Путешествия",
                                CodeName = "Travels",
                                CssIcon = "fas fa-plane",
                                CssColor = "#fff",
                                CssBackground = "#2196f3",
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
                                CssBackground = "rgb(189, 189, 189)",
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
                                CssBackground = "rgb(189, 189, 189)",
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
                                CssBackground = "rgb(189, 189, 189)",
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
                        Name = "Инвестиции",
                        CodeName = "Investments",
                        Description = null,
                        BudgetSectinos = new List<BudgetSection>
                        {
                            new BudgetSection
                            {
                                Name = "Инвестиции",
                                CodeName = "Investment",
                                CssIcon = "fas fa-donate",
                                CssColor = "rgba(24,28,33,0.8)",
                                CssBackground = "rgb(187, 222, 251)",
                                IsShowInCollective = true,
                                IsShowOnSite = true,
                                SectionTypeID = (int)SectionTypeEnum.Investments,
                            },
                             new BudgetSection
                            {
                                Name = "Комиссия брокера",
                                CodeName = "BrokerCommission",
                                CssIcon = "fas fa-landmark",
                                CssColor = "rgba(24,28,33,0.8)",
                                CssBackground = "rgb(187, 222, 251)",
                                IsShowInCollective = true,
                                IsShowOnSite = true,
                                SectionTypeID = (int)SectionTypeEnum.Spendings,
                            },
                        }
                    },
                },
            };
            await repository.CreateAsync(newUser, true);

            #region Create Template for month
            var budgetSections = newUser.BudgetAreas.SelectMany(x => x.BudgetSectinos).ToList();
            var foodSections = budgetSections
                .Where(x => x.CodeName == "Products" || x.CodeName == "Restorans" || x.CodeName == "FastFood")
                .Select(x => new TemplateBudgetSectionPlusViewModel { BudgetSectionID = x.ID, CodeName = x.CodeName, Name = x.Name })
                .ToList();
            string foodFormula = commonService.GenerateFormulaBySections(foodSections);

            var RecurringPaymentsSections = budgetSections
                .Where(x => x.CodeName == "Internet" || x.CodeName == "Communication" || x.CodeName == "ForTheApartment" || x.CodeName == "Electricity" || x.CodeName == "Beauty" || x.CodeName == "Taxes")
                .Select(x => new TemplateBudgetSectionPlusViewModel { BudgetSectionID = x.ID, CodeName = x.CodeName, Name = x.Name })
                .ToList();
            string RecurringPaymentsFormula = commonService.GenerateFormulaBySections(RecurringPaymentsSections);

            var RecreationSections = budgetSections
                .Where(x => x.CodeName == "Clothes" || x.CodeName == "Entertainment" || x.CodeName == "Gifts" || x.CodeName == "CommonTransport")
                .Select(x => new TemplateBudgetSectionPlusViewModel { BudgetSectionID = x.ID, CodeName = x.CodeName, Name = x.Name })
                .ToList();
            string RecreationFormula = commonService.GenerateFormulaBySections(RecreationSections);


            var MedicineSections = budgetSections
                .Where(x => x.CodeName == "Dentistry" || x.CodeName == "Pharmacy" || x.CodeName == "VisitDoctor")
                .Select(x => new TemplateBudgetSectionPlusViewModel { BudgetSectionID = x.ID, CodeName = x.CodeName, Name = x.Name })
                .ToList();
            string MedicineFormula = commonService.GenerateFormulaBySections(MedicineSections);

            var IncomeSections = budgetSections
                .Where(x => x.CodeName == "Dividends" || x.CodeName == "Cashback" || x.CodeName == "OtherIncome" || x.CodeName == "Salary")
                .Select(x => new TemplateBudgetSectionPlusViewModel { BudgetSectionID = x.ID, CodeName = x.CodeName, Name = x.Name })
                .ToList();
            string IncomeFormula = commonService.GenerateFormulaBySections(IncomeSections);


            var NotBudgetSections = budgetSections
                .Where(x => x.CodeName == "Renovation" || x.CodeName == "AllForHouse" || x.CodeName == "LargeHouseholdAppliances" || x.CodeName == "Travels")
                .Select(x => new TemplateBudgetSectionPlusViewModel { BudgetSectionID = x.ID, CodeName = x.CodeName, Name = x.Name })
                .ToList();
            string NotBudgetFormula = commonService.GenerateFormulaBySections(NotBudgetSections);

            try
            {
                await repository.CreateAsync(
                          new Template
                          {
                              UserID = newUser.ID,
                              DateCreate = now,
                              DateEdit = now,
                              Description = "",
                              IsCountCollectiveBudget = true,
                              IsDefault = true,
                              MaxRowInAPage = 30,
                              Name = "Шаблон на месяц",
                              PeriodTypeID = (int)PeriodTypesEnum.Month,
                              IsDeleted = false,
                              TemplateColumns = new List<TemplateColumn>
                              {
                                new TemplateColumn
                                {
                                    Name = "Дни",
                                    Order = 0,
                                    IsShow = true,
                                    Formula = "[]",
                                    ColumnTypeID = 2,
                                    Format = "dd",
                                    FooterActionTypeID = 0,
                                    PlaceAfterCommon = 0,
                                },
                                new TemplateColumn
                                {
                                    Name = "Еда",
                                    Order = 1,
                                    IsShow = true,
                                    Formula = foodFormula,
                                    ColumnTypeID = (int)TemplateColumnType.BudgetSection,
                                    Format = "",
                                    FooterActionTypeID = (int)FooterActionType.Sum,
                                    PlaceAfterCommon = 2,
                                    TemplateBudgetSections = foodSections.Select(x => new Entity.Model.TemplateBudgetSection{ BudgetSectionID = x.BudgetSectionID}).ToList()
                                },
                                new TemplateColumn
                                {
                                    Name = "Прочие расходы",
                                    Order = 2,
                                    IsShow = true,
                                    Formula = RecreationFormula,
                                    ColumnTypeID = (int)TemplateColumnType.BudgetSection,
                                    Format = "",
                                    FooterActionTypeID = (int)FooterActionType.Sum,
                                    PlaceAfterCommon = 2,
                                    TemplateBudgetSections = RecreationSections.Select(x => new Entity.Model.TemplateBudgetSection{ BudgetSectionID = x.BudgetSectionID}).ToList()
                                },
                                new TemplateColumn
                                {
                                    Name = "Регулярные платежи",
                                    Order = 3,
                                    IsShow = true,
                                    Formula = RecurringPaymentsFormula,
                                    ColumnTypeID = (int)TemplateColumnType.BudgetSection,
                                    Format = "",
                                    FooterActionTypeID = (int)FooterActionType.Sum,
                                    PlaceAfterCommon = 2,
                                    TemplateBudgetSections = RecurringPaymentsSections.Select(x => new Entity.Model.TemplateBudgetSection{ BudgetSectionID = x.BudgetSectionID}).ToList()
                                },
                                new TemplateColumn
                                {
                                    Name = "Медицина",
                                    Order = 4,
                                    IsShow = true,
                                    Formula = MedicineFormula,
                                    ColumnTypeID = (int)TemplateColumnType.BudgetSection,
                                    Format = "",
                                    FooterActionTypeID = (int)FooterActionType.Sum,
                                    PlaceAfterCommon = 2,
                                    TemplateBudgetSections = MedicineSections.Select(x => new Entity.Model.TemplateBudgetSection{ BudgetSectionID = x.BudgetSectionID}).ToList()
                                },
                                new TemplateColumn
                                {
                                    Name = "Доходы",
                                    Order = 5,
                                    IsShow = true,
                                    Formula = IncomeFormula,
                                    ColumnTypeID = (int)TemplateColumnType.BudgetSection,
                                    Format = "",
                                    FooterActionTypeID = (int)FooterActionType.Sum,
                                    PlaceAfterCommon = 2,
                                    TemplateBudgetSections = IncomeSections.Select(x => new Entity.Model.TemplateBudgetSection{ BudgetSectionID = x.BudgetSectionID}).ToList()
                                },
                                new TemplateColumn {
                                    Name = "Внебюджет",
                                    Order = 6,
                                    IsShow = true,
                                    Formula = NotBudgetFormula,
                                    ColumnTypeID = (int)TemplateColumnType.BudgetSection,
                                    Format = "",
                                    FooterActionTypeID = (int)FooterActionType.Sum,
                                    PlaceAfterCommon = 2,
                                    TemplateBudgetSections = NotBudgetSections.Select(x => new Entity.Model.TemplateBudgetSection{ BudgetSectionID = x.BudgetSectionID}).ToList()
                                }
                              }
                          }, true);
            }
            catch (Exception ex)
            {
                await userLogService.CreateErrorLog(userSessionID, where: "UserSevice.CreateUser.CreateTemplate", errorText: ex.Message);
            }
            #endregion

            #region Create Template for year
            foodSections = budgetSections
               .Where(x => x.CodeName == "Products" || x.CodeName == "Restorans" || x.CodeName == "FastFood")
               .Select(x => new TemplateBudgetSectionPlusViewModel { BudgetSectionID = x.ID, CodeName = x.CodeName, Name = x.Name })
               .ToList();
            foodFormula = commonService.GenerateFormulaBySections(foodSections);

            RecurringPaymentsSections = budgetSections
                .Where(x => x.CodeName == "Internet" || x.CodeName == "Communication" || x.CodeName == "ForTheApartment" || x.CodeName == "Electricity" || x.CodeName == "Beauty" || x.CodeName == "Taxes")
                .Select(x => new TemplateBudgetSectionPlusViewModel { BudgetSectionID = x.ID, CodeName = x.CodeName, Name = x.Name })
                .ToList();
            RecurringPaymentsFormula = commonService.GenerateFormulaBySections(RecurringPaymentsSections);

            RecreationSections = budgetSections
               .Where(x => x.CodeName == "Clothes" || x.CodeName == "Entertainment" || x.CodeName == "Gifts" || x.CodeName == "CommonTransport")
               .Select(x => new TemplateBudgetSectionPlusViewModel { BudgetSectionID = x.ID, CodeName = x.CodeName, Name = x.Name })
               .ToList();
            RecreationFormula = commonService.GenerateFormulaBySections(RecreationSections);


            MedicineSections = budgetSections
               .Where(x => x.CodeName == "Dentistry" || x.CodeName == "Pharmacy" || x.CodeName == "VisitDoctor")
               .Select(x => new TemplateBudgetSectionPlusViewModel { BudgetSectionID = x.ID, CodeName = x.CodeName, Name = x.Name })
               .ToList();
            MedicineFormula = commonService.GenerateFormulaBySections(MedicineSections);

            IncomeSections = budgetSections
               .Where(x => x.CodeName == "Dividends" || x.CodeName == "Cashback" || x.CodeName == "OtherIncome" || x.CodeName == "Salary")
               .Select(x => new TemplateBudgetSectionPlusViewModel { BudgetSectionID = x.ID, CodeName = x.CodeName, Name = x.Name })
               .ToList();
            IncomeFormula = commonService.GenerateFormulaBySections(IncomeSections);


            NotBudgetSections = budgetSections
               .Where(x => x.CodeName == "Renovation" || x.CodeName == "AllForHouse" || x.CodeName == "LargeHouseholdAppliances" || x.CodeName == "Travels")
               .Select(x => new TemplateBudgetSectionPlusViewModel { BudgetSectionID = x.ID, CodeName = x.CodeName, Name = x.Name })
               .ToList();
            NotBudgetFormula = commonService.GenerateFormulaBySections(NotBudgetSections);

            var CarSections = budgetSections
               .Where(x => x.CodeName == "CarInsurance" || x.CodeName == "Petrol" || x.CodeName == "AutoRepair")
               .Select(x => new TemplateBudgetSectionPlusViewModel { BudgetSectionID = x.ID, CodeName = x.CodeName, Name = x.Name })
               .ToList();
            var CarFormula = commonService.GenerateFormulaBySections(CarSections);

            var InvestSections = budgetSections
              .Where(x => x.CodeName == "Investment")
              .Select(x => new TemplateBudgetSectionPlusViewModel { BudgetSectionID = x.ID, CodeName = x.CodeName, Name = x.Name })
              .ToList();
            var InvestFormula = commonService.GenerateFormulaBySections(InvestSections);

            try
            {
                await repository.CreateAsync(
                          new Template
                          {
                              UserID = newUser.ID,
                              DateCreate = now,
                              DateEdit = now,
                              Description = "",
                              IsCountCollectiveBudget = true,
                              IsDefault = true,
                              MaxRowInAPage = 30,
                              Name = "Шаблон на год",
                              PeriodTypeID = (int)PeriodTypesEnum.Year,
                              IsDeleted = false,
                              TemplateColumns = new List<TemplateColumn>
                              {
                                new TemplateColumn
                                {
                                    Name = "Дни",
                                    Order = 0,
                                    IsShow = true,
                                    Formula = "[]",
                                    ColumnTypeID = 2,
                                    Format = "dd",
                                    FooterActionTypeID = 0,
                                    PlaceAfterCommon = 0,
                                },
                                new TemplateColumn
                                {
                                    Name = "Еда",
                                    Order = 1,
                                    IsShow = true,
                                    Formula = foodFormula,
                                    ColumnTypeID = (int)TemplateColumnType.BudgetSection,
                                    Format = "",
                                    FooterActionTypeID = (int)FooterActionType.Sum,
                                    PlaceAfterCommon = 2,
                                    TemplateBudgetSections = foodSections.Select(x => new Entity.Model.TemplateBudgetSection{ BudgetSectionID = x.BudgetSectionID}).ToList()
                                },
                                new TemplateColumn
                                {
                                    Name = "Прочие расходы",
                                    Order = 2,
                                    IsShow = true,
                                    Formula = RecreationFormula,
                                    ColumnTypeID = (int)TemplateColumnType.BudgetSection,
                                    Format = "",
                                    FooterActionTypeID = (int)FooterActionType.Sum,
                                    PlaceAfterCommon = 2,
                                    TemplateBudgetSections = RecreationSections.Select(x => new Entity.Model.TemplateBudgetSection{ BudgetSectionID = x.BudgetSectionID}).ToList()
                                },
                                new TemplateColumn
                                {
                                    Name = "Регулярные платежи",
                                    Order = 3,
                                    IsShow = true,
                                    Formula = RecurringPaymentsFormula,
                                    ColumnTypeID = (int)TemplateColumnType.BudgetSection,
                                    Format = "",
                                    FooterActionTypeID = (int)FooterActionType.Sum,
                                    PlaceAfterCommon = 2,
                                    TemplateBudgetSections = RecurringPaymentsSections.Select(x => new Entity.Model.TemplateBudgetSection{ BudgetSectionID = x.BudgetSectionID}).ToList()
                                },
                                new TemplateColumn
                                {
                                    Name = "Медицина",
                                    Order = 4,
                                    IsShow = true,
                                    Formula = MedicineFormula,
                                    ColumnTypeID = (int)TemplateColumnType.BudgetSection,
                                    Format = "",
                                    FooterActionTypeID = (int)FooterActionType.Sum,
                                    PlaceAfterCommon = 2,
                                    TemplateBudgetSections = MedicineSections.Select(x => new Entity.Model.TemplateBudgetSection{ BudgetSectionID = x.BudgetSectionID}).ToList()
                                },
                                new TemplateColumn
                                {
                                    Name = "Доходы",
                                    Order = 5,
                                    IsShow = true,
                                    Formula = IncomeFormula,
                                    ColumnTypeID = (int)TemplateColumnType.BudgetSection,
                                    Format = "",
                                    FooterActionTypeID = (int)FooterActionType.Sum,
                                    PlaceAfterCommon = 2,
                                    TemplateBudgetSections = IncomeSections.Select(x => new Entity.Model.TemplateBudgetSection{ BudgetSectionID = x.BudgetSectionID}).ToList()
                                },
                                new TemplateColumn {
                                    Name = "Внебюджет",
                                    Order = 6,
                                    IsShow = true,
                                    Formula = NotBudgetFormula,
                                    ColumnTypeID = (int)TemplateColumnType.BudgetSection,
                                    Format = "",
                                    FooterActionTypeID = (int)FooterActionType.Sum,
                                    PlaceAfterCommon = 2,
                                    TemplateBudgetSections = NotBudgetSections.Select(x => new Entity.Model.TemplateBudgetSection{ BudgetSectionID = x.BudgetSectionID}).ToList()
                                },
                                 new TemplateColumn {
                                    Name = "Траты на машину",
                                    Order = 7,
                                    IsShow = true,
                                    Formula = CarFormula,
                                    ColumnTypeID = (int)TemplateColumnType.BudgetSection,
                                    Format = "",
                                    FooterActionTypeID = (int)FooterActionType.Sum,
                                    PlaceAfterCommon = 2,
                                    TemplateBudgetSections = CarSections.Select(x => new Entity.Model.TemplateBudgetSection{ BudgetSectionID = x.BudgetSectionID}).ToList()
                                },
                                  new TemplateColumn {
                                    Name = "Инвестиции",
                                    Order = 8,
                                    IsShow = true,
                                    Formula = InvestFormula,
                                    ColumnTypeID = (int)TemplateColumnType.BudgetSection,
                                    Format = "",
                                    FooterActionTypeID = (int)FooterActionType.Sum,
                                    PlaceAfterCommon = 2,
                                    TemplateBudgetSections = InvestSections.Select(x => new Entity.Model.TemplateBudgetSection{ BudgetSectionID = x.BudgetSectionID}).ToList()
                                },

                              }
                          }, true);
            }
            catch (Exception ex)
            {
                await userLogService.CreateErrorLog(userSessionID, where: "UserSevice.CreateUser.CreateTemplate", errorText: ex.Message);
            }
            #endregion

            #region Create Chart for year

            try
            {
                await repository.CreateAsync(new Chart
                {
                    ChartTypeID = (int)ChartTypesEnum.Line,
                    DateCreate = now,
                    LastDateEdit = now,
                    Name = "График доходов и расходов",
                    UserID = newUser.ID,
                    VisibleElement = new VisibleElement
                    {
                        IsShow_BudgetYear = true,
                        IsShowOnDashboards = false,
                        IsShow_BudgetMonth = false,
                    },
                    ChartFields = new List<ChartField>
                    {
                        new ChartField
                            {
                                CssColor = "#2ECC71",
                                Name = "Доходы",
                                SectionGroupCharts = budgetSections
                                    .Where(x => x.CodeName == "Dividends" || x.CodeName == "Cashback" || x.CodeName == "OtherIncome" || x.CodeName == "Salary")
                                    .Select(x => new SectionGroupChart
                                    {
                                        BudgetSectionID = x.ID,
                                    })
                                    .ToList()
                            },
                        new ChartField
                        {
                            CssColor = "#E74C3C",
                            Name = "Расходы",
                            SectionGroupCharts = budgetSections
                                    .Where(x => !(x.CodeName == "Dividends" || x.CodeName == "Cashback" || x.CodeName == "OtherIncome" || x.CodeName == "Salary" || x.CodeName == "Investment"))
                                    .Select(x => new SectionGroupChart
                                    {
                                        BudgetSectionID = x.ID,
                                    })
                                    .ToList()
                    }
                }
                }, true);
            }
            catch (Exception ex)
            {
                await userLogService.CreateErrorLog(userSessionID, where: "UserSevice.CreateUser.CreateChart", errorText: ex.Message);
            }
            #endregion

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

            dbUser.Name = user.Name = userInfoModel.Name.Trim();
            dbUser.LastName = user.LastName = userInfoModel.LastName.Trim();
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

                user.IsConfirmEmail = dbUser.IsConfirmEmail = userInfoModel.IsConfirmEmail = false;

                await repository.UpdateAsync(dbUser, true);
                await userLogService.CreateUserLog(user.UserSessionID, UserLogActionType.User_Change_Email);
            }

            await UserInfo.AddOrUpdate_Authenticate(user);
            await userLogService.CreateUserLog(user.UserSessionID, UserLogActionType.User_Edit);

            return userInfoModel;
        }

        public async Task<int> UpdateUserSettings(UserSettingsModelView userSettings)
        {
            var user = UserInfo.Current;
            var dbUser = await repository.GetAll<Entity.Model.User>(x => x.ID == user.ID)
                .FirstOrDefaultAsync();

            user.UserSettings.WebSiteTheme = dbUser.UserSettings.WebSiteTheme = userSettings.WebSiteTheme;
            user.UserSettings.NewsLetter = dbUser.UserSettings.NewsLetter = userSettings.NewsLetter;

            await repository.UpdateAsync(dbUser, true);
            await UserInfo.AddOrUpdate_Authenticate(user);

            return 1;
        }
        #endregion
    }
}
