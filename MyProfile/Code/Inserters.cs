using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using MyProfile.Entity.Model;
using MyProfile.Entity.Repository;
using MyProfile.User.Service.PasswordWorker;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MyProfile.Code
{
    public class Inserters
    {
        private DateTime now;
        private IBaseRepository repository;
        private IHostingEnvironment hostingEnvironment;
        private IHttpContextAccessor httpContextAccessor;

        public Inserters(
            IBaseRepository repository,
            IHostingEnvironment hostingEnvironment,
            IHttpContextAccessor httpContextAccessor)
        {
            this.repository = repository;
            this.hostingEnvironment = hostingEnvironment;
            this.httpContextAccessor = httpContextAccessor;
            now = DateTime.Now;


            BanksLoading();
            CreateParentAccount();
            //CreateTelegramBotAccount();
            //CreateTelegramAccountStatus();
            // LoadTimeZone();
        }

        private void BanksLoading()
        {
            if (repository.GetAll<Bank>().Count() <= 100)
            {
                //https://www.banki.ru/banks/
                var oldBanks = new Dictionary<string, string>();

                oldBanks.Add("Сбер", "/resources/banks/sber.svg");
                oldBanks.Add("ВТБ", "/resources/banks/vtb.svg");
                oldBanks.Add("Газпромбанк", "/resources/banks/gasprom.svg");
                oldBanks.Add("Альфа-Банк", "/resources/banks/alfa-logo.svg");
                oldBanks.Add("Банк Открытие", "/resources/banks/open.svg");
                oldBanks.Add("Тинькофф Банк", "/resources/banks/tinkoff-bank.png");
                oldBanks.Add("Национальный Клиринговый Центр", "/resources/banks/moex.svg");
                oldBanks.Add("Россельхозбанк", "/resources/banks/rosselhoz.jfif");
                oldBanks.Add("Московский Кредитный Банк", "/resources/banks/mkb.svg");
                oldBanks.Add("Совкомбанк", "/resources/banks/sovkombank.svg");
                oldBanks.Add("Росбанк", "/resources/banks/ros.svg");
                oldBanks.Add("Райффайзенбанк", "/resources/banks/rasf.svg");
                oldBanks.Add("Ситибанк", "/resources/banks/citi.svg");

                using (StreamReader reader = new StreamReader(hostingEnvironment.WebRootPath + @"\\json\\banks.json"))
                using (StreamReader reader2 = new StreamReader(hostingEnvironment.WebRootPath + @"\\json\\banks-raiting.json"))
                using (WebClient client = new WebClient())
                {
                    List<BankInfo> bankInfos = (List<BankInfo>)JsonConvert.DeserializeObject<List<BankInfo>>(reader.ReadToEnd());
                    List<BankRaiting> bankRatings = (List<BankRaiting>)JsonConvert.DeserializeObject<List<BankRaiting>>(reader2.ReadToEnd());
                    List<Bank> newBanks = new List<Bank>();

                    var banks = repository.GetAll<Bank>().ToList();

                    foreach (var bankInfo in bankInfos)
                    {
                        Bank bank = new Bank();
                        bool isNew = true;
                        try
                        {
                            BankRaiting bankRaiting = bankRatings.FirstOrDefault(x => x.name == bankInfo.bank_name);

                            if (banks.Any(x => x.Name == bankInfo.bank_name))
                            {
                                bank = banks.FirstOrDefault(x => x.Name == bankInfo.bank_name);
                                bank.LogoCircle = oldBanks[bank.Name];
                                isNew = false;
                            }
                            else
                            {
                                bank.Name = bankInfo.bank_name;
                            }

                            bank.bankiruID = bankInfo.bank_id;
                            bank.BankTypeID = (int)BankTypes.Bank;
                            bank.Licence = bankInfo.licence;
                            bank.NameEn = bankInfo.name_eng;
                            bank.Raiting = bankRaiting != null ? int.Parse(bankRaiting.raiting) : 500;
                            bank.Region = bankInfo.region;
                            bank.Tels = bankRaiting != null ? bankRaiting.tels : null;
                        }
                        catch (Exception ex)
                        {
                            continue;
                        }

                        if (!string.IsNullOrEmpty(bankInfo.bank_logo))
                        {
                            try
                            {
                                string n = bankInfo.name_eng.Replace(" ", "_").Replace("-", "_").ToLower();
                                string url = @"C:\Users\t3l3f\source\repos\MyProject\MyProfile\wwwroot\resources\banks\" + n + "_rectangle.gif";
                                client.DownloadFile(new Uri(bankInfo.bank_logo), url);
                                bank.LogoRectangle = @"/resources/banks/" + n + "_rectangle.gif";
                            }
                            catch (Exception ex1)
                            {

                            }
                        }
                        if (isNew)
                        {
                            newBanks.Add(bank);
                        }
                        else
                        {
                            repository.Update(bank);
                        }
                    }

                    repository.CreateRange(newBanks, true);
                }
            }
        }

        private void CreateParentAccount()
        {
            var z = repository.GetAll<Account>(x => x.ParentAccountID != null).Count();
            if (z == 0)
            {
                var userIDs = repository.GetAll<MyProfile.Entity.Model.User>(x => x.Accounts.Count() > 0)
                    .Select(x => x.ID)
                    .ToList();

                foreach (var userID in userIDs)
                {
                    var accounts = repository.GetAll<Account>(x => x.UserID == userID && x.IsDeleted == false)
                        .Select(x => x.BankID)
                        .GroupBy(x => x)
                        .ToList();
                    foreach (var account in accounts)
                    {
                        Account parentAccount = new Account();
                        if (account.Key == null)
                        {
                            parentAccount.AccountTypeID = (int)AccountTypes.Cash;
                            parentAccount.Name = "Все наличные";
                        }
                        else
                        {
                            parentAccount.BankID = account.Key;
                            parentAccount.Name = repository.GetAll<Bank>(x => x.ID == account.Key).FirstOrDefault().Name;
                            parentAccount.AccountTypeID = (int)AccountTypes.Debed;
                        }
                        parentAccount.UserID = userID;
                        parentAccount.DateCreate = now;
                        parentAccount.LastChanges = now;
                        parentAccount.CurrencyID = 1;
                        parentAccount.IsCountTheBalance = false;

                        repository.Create(parentAccount, true);

                        var accountsForUpdate = repository.GetAll<Account>(x => x.UserID == userID && x.BankID == account.Key && x.ID != parentAccount.ID && x.IsDeleted == false).ToList();
                        foreach (var item in accountsForUpdate)
                        {
                            item.ParentAccountID = parentAccount.ID;
                        }
                        repository.Save();
                    }
                }

            }
        }


        private void LoadTimeZone()
        {
            if (repository.GetAll<MyTimeZone>().Any() == false)
            {
                using (StreamReader reader = new StreamReader(hostingEnvironment.WebRootPath + @"\\resources\\timezone.json"))
                {
                    List<TimeZoneLoad> timezons = (List<TimeZoneLoad>)JsonConvert.DeserializeObject<List<TimeZoneLoad>>(reader.ReadToEnd());
                    List<MyTimeZone> dbTimeZone = new List<MyTimeZone>();

                    foreach (var tz in timezons)
                    {
                        MyTimeZone myTimeZone = new MyTimeZone
                        {
                            Abreviatura = tz.abbr,
                            IsDST = tz.isdst,
                            UTCOffsetHours = tz.offset,
                            UTCOffsetMinutes = int.Parse((tz.offset * 60).ToString().Replace(".0", "")),
                            WindowsDisplayName = tz.text,
                            WindowsTimezoneID = tz.value
                        };

                        var olsonTZIDs = new List<OlsonTZID>();
                        for (int i = 0; i < tz.utc.Count; i++)
                        {
                            olsonTZIDs.Add(new OlsonTZID
                            {
                                Name = tz.utc[i]
                            });

                        }
                        myTimeZone.OlsonTZIDs = olsonTZIDs;
                        dbTimeZone.Add(myTimeZone);
                    }
                    repository.CreateRange(dbTimeZone, true);
                }
            }
        }

        private void CreateTelegramAccountStatus()
        {
            List<TelegramAccountStatus> statuses = new List<TelegramAccountStatus>();

            if (!repository.Any<TelegramAccountStatus>(x => x.CodeName == Enum.GetName(typeof(TelegramAccountStatusEnum), TelegramAccountStatusEnum.New)))
            {
                statuses.Add(new TelegramAccountStatus
                {
                    //ID = (int)TelegramAccountStatusEnum.New,
                    Name = "Новый",
                    CodeName = Enum.GetName(typeof(TelegramAccountStatusEnum), TelegramAccountStatusEnum.New),
                });
            }
            if (!repository.Any<TelegramAccountStatus>(x => x.CodeName == Enum.GetName(typeof(TelegramAccountStatusEnum), TelegramAccountStatusEnum.Connected)))
            {
                statuses.Add(new TelegramAccountStatus
                {
                    //ID = (int)TelegramAccountStatusEnum.Connected,
                    Name = "Подключен",
                    CodeName = Enum.GetName(typeof(TelegramAccountStatusEnum), TelegramAccountStatusEnum.Connected),
                });
            }
            if (!repository.Any<TelegramAccountStatus>(x => x.CodeName == Enum.GetName(typeof(TelegramAccountStatusEnum), TelegramAccountStatusEnum.InPause)))
            {
                statuses.Add(new TelegramAccountStatus
                {
                    //ID = (int)TelegramAccountStatusEnum.InPause,
                    Name = "На паузе",
                    CodeName = Enum.GetName(typeof(TelegramAccountStatusEnum), TelegramAccountStatusEnum.InPause),
                });
            }
            if (!repository.Any<TelegramAccountStatus>(x => x.CodeName == Enum.GetName(typeof(TelegramAccountStatusEnum), TelegramAccountStatusEnum.Locked)))
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
                        AccountTypeID = (int)AccountTypes.Cash,
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

        public class TimeZoneLoad
        {
            public string value { get; set; }
            public string abbr { get; set; }
            public decimal offset { get; set; }
            public bool isdst { get; set; }
            public string text { get; set; }
            public List<string> utc { get; set; }
        }

        public class BankInfo
        {
            public string bank_id { get; set; }
            public string bank_name { get; set; }
            public string bank_logo { get; set; }
            public string region { get; set; }
            public int show_on_banki { get; set; }
            public string licence { get; set; }
            public string name_eng { get; set; }
        }
        public class BankRaiting
        {
            public string name { get; set; }
            public string tels { get; set; }
            public string raiting { get; set; }
        }
    }
}
