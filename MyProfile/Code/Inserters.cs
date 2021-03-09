using MyProfile.Entity.Model;
using MyProfile.Entity.Repository;
using MyProfile.User.Service.PasswordWorker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyProfile.Code
{
    public class Inserters
    {
        private BaseRepository repository;
        private DateTime now;

        public Inserters(BaseRepository repository)
        {
            this.repository = repository;
            now = DateTime.Now;

            CreateTelegramBotAccount();
            CreateTelegramAccountStatus();
        }

        private void CreateTelegramAccountStatus()
        {
            List<TelegramAccountStatus> statuses = new List<TelegramAccountStatus>();

            if (!repository.Any<TelegramAccountStatus>(x => x.ID == (int)TelegramAccountStatusEnum.New))
            {
                statuses.Add(new TelegramAccountStatus
                {
                    //ID = (int)TelegramAccountStatusEnum.New,
                    Name = "Новый",
                    CodeName = Enum.GetName(typeof(TelegramAccountStatusEnum), TelegramAccountStatusEnum.New),
                });
            }
            if (!repository.Any<TelegramAccountStatus>(x => x.ID == (int)TelegramAccountStatusEnum.Connected))
            {
                statuses.Add(new TelegramAccountStatus
                {
                    //ID = (int)TelegramAccountStatusEnum.Connected,
                    Name = "Подключен",
                    CodeName = Enum.GetName(typeof(TelegramAccountStatusEnum), TelegramAccountStatusEnum.Connected),
                });
            }
            if (!repository.Any<TelegramAccountStatus>(x => x.ID == (int)TelegramAccountStatusEnum.InPause))
            {
                statuses.Add(new TelegramAccountStatus
                {
                    //ID = (int)TelegramAccountStatusEnum.InPause,
                    Name = "На паузе",
                    CodeName = Enum.GetName(typeof(TelegramAccountStatusEnum), TelegramAccountStatusEnum.InPause),
                });
            }
            if (!repository.Any<TelegramAccountStatus>(x => x.ID == (int)TelegramAccountStatusEnum.Locked))
            {
                statuses.Add(new TelegramAccountStatus
                {
                    //ID = (int)TelegramAccountStatusEnum.Locked,
                    Name = "Заблокирован",
                    CodeName = Enum.GetName(typeof(TelegramAccountStatusEnum), TelegramAccountStatusEnum.Locked),
                });
            }

            if (statuses.Count > 0)
            {
                repository.CreateRange(statuses, true);
            }
        }

        private void CreateTelegramBotAccount()
        {
            if (repository.Any<Entity.Model.User>(x => x.UserTypeID == (int)UserTypeEnum.TelegramBot))
            {
                return;
            }

            var now = DateTime.Now.ToUniversalTime();
            PasswordService passwordService = new PasswordService();
            var passwordSalt = passwordService.GenerateSalt();
            var passwordHash = passwordService.GenerateHashSHA256("Telegram_bot_07_03_2021", passwordSalt);
            string telegramLogin = "telegram_TelegramBot";

            var newUser = new Entity.Model.User
            {
                //ID = newUserID,
                DateCreate = now,
                Email = "Telegram_bot",
                IsAllowCollectiveBudget = false,
                Name = "Telegram_bot",
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
                        Name = "Telegram_bot",
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
                Payment = new MyProfile.Entity.Model.Payment
                {
                    DateFrom = now,
                    DateTo = now.AddYears(10),
                    //DateTo = now.AddMonths(2),
                    IsPaid = false,
                    Tariff = PaymentTariffs.Free,
                    PaymentHistories = new List<PaymentHistory> {
                        new PaymentHistory {
                            DateFrom = now,
                            DateTo = now.AddYears(1),
                            //DateTo = now.AddMonths(2),
                            Tariff = PaymentTariffs.Free,
                        }
                    }
                },
                Accounts = new List<Account>{
                    new Account
                    {
                        AccountTypeID = (int)AccountTypesEnum.Cash,
                        Balance = 0,
                        CurrencyID = 1, //Rubles
                        DateCreate = now,
                        IsDefault = true,
                        Name = "Наличные",
                    }
                },
                UserSummaries = new List<UserSummary> {
                    new  UserSummary
                    {
                        Name = "Доходы в час",
                        Value = "0",
                        CurrentDate = now,
                        IsActive = true,
                        SummaryID = (int)SummaryType.EarningsPerHour,
                        VisibleElement = new VisibleElement
                        {
                            IsShow_BudgetMonth = true,
                            IsShow_BudgetYear = true,
                        }
                    }
                },
                UserConnect = new UserConnect
                {
                    TelegramLogin = telegramLogin
                },
                UserTypeID = (int)UserTypeEnum.TelegramBot,
            };
            try
            {
                repository.Create(newUser, true);
            }
            catch (Exception ex)
            {

            }
        }

        private void InsertNotification()
        {

            //Entity.Model.Notification notification = new Entity.Model.Notification
            //{
            //    UserID = Guid.Parse("0820C316-7E24-4469-1943-08D86AC5B6E8"),
            //    Total = 5000,
            //    LastChangeDateTime = DateTime.Now,
            //    ExpirationDateTime = DateTime.Now,
            //    NotificationTypeID = (int)NotificationType.Limit,
            //    LimitNotification = new LimitNotification
            //    {
            //        LimitID = 13,
            //    },
            //};


            //repository.Create(notification, true);
        }

        private void InsertSummeries()
        {
            var db_summuries = repository.GetAll<Summary>().Select(x => new { ID = x.ID, CodeName = x.CodeName }).ToList();
            List<Summary> summaries = new List<Summary>();

            //if (!db_summuries.Any(x => x.ID == (int)SummaryType.EarningsPerHour))
            //{
            //    summaries.Add(new Summary
            //    {
            //        Name = "Доходы в час/день",
            //        CodeName = "EarningsPerHour",
            //        CurrentDate = now,
            //        IsActive = true,
            //        VisibleElement = new VisibleElement
            //        {
            //            IsShow_BudgetMonth = true,
            //            IsShow_BudgetYear = true,
            //        }
            //    });
            //}

            //if (!db_summuries.Any(x => x.ID == (int)SummaryType.ExpensesPerDay))
            //{
            //    summaries.Add(new Summary
            //    {
            //        Name = "Расходы день",
            //        CodeName = "ExpensesPerDay",
            //        CurrentDate = now,
            //        IsActive = true,
            //        VisibleElement = new VisibleElement
            //        {
            //            IsShow_BudgetMonth = true,
            //            IsShow_BudgetYear = true,
            //        }
            //    });
            //}

            //if (!db_summuries.Any(x => x.ID == (int)SummaryType.CashFlow))
            //{
            //    summaries.Add(new Summary
            //    {
            //        Name = "Денежный поток",
            //        CodeName = "CashFlow",
            //        CurrentDate = now,
            //        IsActive = true,
            //        IsChart = true,
            //        VisibleElement = new VisibleElement
            //        {
            //            IsShow_BudgetMonth = true,
            //            IsShow_BudgetYear = true,
            //        }
            //    });
            //}

            //if (!db_summuries.Any(x => x.ID == (int)SummaryType.AllAccountsMoney))
            //{
            //    summaries.Add(new Summary
            //    {
            //        Name = "Всего денег на счетах",
            //        CodeName = "AllAccountsMoney",
            //        CurrentDate = now,
            //        IsActive = true,
            //        VisibleElement = new VisibleElement
            //        {
            //            IsShow_BudgetMonth = true,
            //            IsShow_BudgetYear = true,
            //        }
            //    });
            //}

            if (summaries.Count > 0)
            {
                repository.CreateRange(summaries);
                repository.Save();
            }
        }
    }
}
