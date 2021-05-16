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
using SubScriptionModel = MyProfile.Entity.Model.SubScription;


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

            //CardToJson();
            //CardJsonToCard();

            //CreditRCardsLoading();
            //DebitCardsLoading();
            //BanksLoading();
            //CreateParentAccount();
            //CreateTelegramBotAccount();
            //CreateTelegramAccountStatus();
            // LoadTimeZone();
            //SubScriptionLoading();
            // SubScriptionToJson();
            //SubScriptionToProd();
            //InsertSummeries();
            //AddPaymentTariff();
            //AddBudgettoEntityType();
            //AddPaymentCounter();

            CheckAndAddUserEntityCounter();
            CheckAndAddBaseSectionsAndAreas();

        }

        private void CheckAndAddBaseSectionsAndAreas()
        {
            List<BaseAreaAndSection> categories = new List<BaseAreaAndSection>();
            List<BaseArea> baseAreas = repository.GetAll<BaseArea>().ToList();
            List<BaseSection> baseSections = repository.GetAll<BaseSection>().ToList();

            using (StreamReader reader = new StreamReader(hostingEnvironment.WebRootPath + @"\\json\\base-section-area.json"))
            {
                categories = JsonConvert.DeserializeObject<List<BaseAreaAndSection>>(reader.ReadToEnd());

                BaseArea baseArea = new BaseArea();
                BaseSection baseSection = new BaseSection();

                foreach (var item in categories)
                {
                    if (!string.IsNullOrEmpty(item.AreaEng))
                    {
                        baseArea = baseAreas.FirstOrDefault(x => x.CodeName == item.AreaEng);
                        if (baseArea == null)
                        {
                            baseArea = new BaseArea();
                        }

                        baseArea.Name = item.AreaRus;
                        baseArea.CodeName = item.AreaEng;

                        if (baseArea.ID == 0)
                        {
                            repository.Create(baseArea, true);
                        }
                        else
                        {
                            repository.Update(baseArea, true);
                        }
                    }
                    else
                    {
                        baseSection = baseSections.FirstOrDefault(x => x.CodeName == item.CategoryEng);
                        if (baseSection == null)
                        {
                            baseSection = new BaseSection();
                        }

                        baseSection.Name = item.CategoryRus;
                        baseSection.CodeName = item.CategoryEng;
                        baseSection.BaseAreaID = baseArea.ID;
                        baseSection.KeyWords = item.KeyWords;
                        baseSection.Color = item.Color;
                        baseSection.Background = item.Background;
                        baseSection.Icon = item.Icon;

                        if (item.SectionType == Enum.GetName(typeof(SectionTypeEnum), SectionTypeEnum.Spendings))
                        {
                            baseSection.SectionTypeID = (int)SectionTypeEnum.Spendings;
                        }
                        else if (item.SectionType == Enum.GetName(typeof(SectionTypeEnum), SectionTypeEnum.Earnings))
                        {
                            baseSection.SectionTypeID = (int)SectionTypeEnum.Earnings;
                        }

                        if (baseSection.ID == 0)
                        {
                            repository.Create(baseSection, true);
                        }
                        else
                        {
                            repository.Update(baseSection, true);
                        }
                    }

                }
            }
        }

        private void AddBudgettoEntityType()
        {
            var db_EntityType = repository.GetAll<EntityType>().Select(x => new { ID = x.ID }).ToList();
            List<EntityType> entityTypes = new List<EntityType>();

            if (!db_EntityType.Any(x => x.ID == (int)BudgettoEntityType.Limits))
            {
                entityTypes.Add(new EntityType
                {
                    CodeName = Enum.GetName(typeof(BudgettoEntityType), BudgettoEntityType.Limits)
                });
            }

            if (!db_EntityType.Any(x => x.ID == (int)BudgettoEntityType.Reminders))
            {
                entityTypes.Add(new EntityType
                {
                    CodeName = Enum.GetName(typeof(BudgettoEntityType), BudgettoEntityType.Reminders)
                });
            }

            if (!db_EntityType.Any(x => x.ID == (int)BudgettoEntityType.ToDoLists))
            {
                entityTypes.Add(new EntityType
                {
                    CodeName = Enum.GetName(typeof(BudgettoEntityType), BudgettoEntityType.ToDoLists)
                });
            }


            if (entityTypes.Count > 0)
            {
                repository.CreateRange(entityTypes);
                repository.Save();
            }
        }

        private void AddPaymentCounter()
        {
            var now = DateTime.Now.ToUniversalTime();
            var db_PaymentCounter = repository.GetAll<PaymentCounter>().Select(x => new { ID = x.ID, EntityTypeID = x.EntityTypeID, x.PaymentTariffID }).ToList();
            List<PaymentCounter> paymentCounters = new List<PaymentCounter>();

            if (!db_PaymentCounter.Any(x => x.PaymentTariffID == (int)PaymentTariffTypes.Free))
            {
                if (!db_PaymentCounter.Any(x => x.EntityTypeID == (int)BudgettoEntityType.Limits))
                {
                    paymentCounters.Add(new PaymentCounter
                    {
                        PaymentTariffID = (int)PaymentTariffTypes.Free,
                        EntityTypeID = (int)BudgettoEntityType.Limits,
                        CanBeCount = 1,
                        LastChanges = now,
                    });
                }

                if (!db_PaymentCounter.Any(x => x.EntityTypeID == (int)BudgettoEntityType.Reminders))
                {
                    paymentCounters.Add(new PaymentCounter
                    {
                        PaymentTariffID = (int)PaymentTariffTypes.Free,
                        EntityTypeID = (int)BudgettoEntityType.Reminders,
                        CanBeCount = 1,
                        LastChanges = now,
                    });
                }

                if (!db_PaymentCounter.Any(x => x.EntityTypeID == (int)BudgettoEntityType.ToDoLists))
                {
                    paymentCounters.Add(new PaymentCounter
                    {
                        PaymentTariffID = (int)PaymentTariffTypes.Free,
                        EntityTypeID = (int)BudgettoEntityType.ToDoLists,
                        CanBeCount = 1,
                        LastChanges = now,
                    });
                }
            }

            if (!db_PaymentCounter.Any(x => x.PaymentTariffID == (int)PaymentTariffTypes.Standard))
            {
                if (!db_PaymentCounter.Any(x => x.EntityTypeID == (int)BudgettoEntityType.Limits))
                {
                    paymentCounters.Add(new PaymentCounter
                    {
                        PaymentTariffID = (int)PaymentTariffTypes.Standard,
                        EntityTypeID = (int)BudgettoEntityType.Limits,
                        CanBeCount = 3,
                        LastChanges = now,
                    });
                }

                if (!db_PaymentCounter.Any(x => x.EntityTypeID == (int)BudgettoEntityType.Reminders))
                {
                    paymentCounters.Add(new PaymentCounter
                    {
                        PaymentTariffID = (int)PaymentTariffTypes.Standard,
                        EntityTypeID = (int)BudgettoEntityType.Reminders,
                        CanBeCount = 3,
                        LastChanges = now,
                    });
                }

                if (!db_PaymentCounter.Any(x => x.EntityTypeID == (int)BudgettoEntityType.ToDoLists))
                {
                    paymentCounters.Add(new PaymentCounter
                    {
                        PaymentTariffID = (int)PaymentTariffTypes.Standard,
                        EntityTypeID = (int)BudgettoEntityType.ToDoLists,
                        CanBeCount = 3,
                        LastChanges = now,
                    });
                }
            }

            if (!db_PaymentCounter.Any(x => x.PaymentTariffID == (int)PaymentTariffTypes.Freedom))
            {
                if (!db_PaymentCounter.Any(x => x.EntityTypeID == (int)BudgettoEntityType.Limits))
                {
                    paymentCounters.Add(new PaymentCounter
                    {
                        PaymentTariffID = (int)PaymentTariffTypes.Freedom,
                        EntityTypeID = (int)BudgettoEntityType.Limits,
                        CanBeCount = 99,
                        LastChanges = now,
                    });
                }

                if (!db_PaymentCounter.Any(x => x.EntityTypeID == (int)BudgettoEntityType.Reminders))
                {
                    paymentCounters.Add(new PaymentCounter
                    {
                        PaymentTariffID = (int)PaymentTariffTypes.Freedom,
                        EntityTypeID = (int)BudgettoEntityType.Reminders,
                        CanBeCount = 99,
                        LastChanges = now,
                    });
                }

                if (!db_PaymentCounter.Any(x => x.EntityTypeID == (int)BudgettoEntityType.ToDoLists))
                {
                    paymentCounters.Add(new PaymentCounter
                    {
                        PaymentTariffID = (int)PaymentTariffTypes.Freedom,
                        EntityTypeID = (int)BudgettoEntityType.ToDoLists,
                        CanBeCount = 99,
                        LastChanges = now,
                    });
                }
            }

            if (!db_PaymentCounter.Any(x => x.PaymentTariffID == (int)PaymentTariffTypes.Premium))
            {
                if (!db_PaymentCounter.Any(x => x.EntityTypeID == (int)BudgettoEntityType.Limits))
                {
                    paymentCounters.Add(new PaymentCounter
                    {
                        PaymentTariffID = (int)PaymentTariffTypes.Premium,
                        EntityTypeID = (int)BudgettoEntityType.Limits,
                        CanBeCount = 5,
                        LastChanges = now,
                    });
                }

                if (!db_PaymentCounter.Any(x => x.EntityTypeID == (int)BudgettoEntityType.Reminders))
                {
                    paymentCounters.Add(new PaymentCounter
                    {
                        PaymentTariffID = (int)PaymentTariffTypes.Premium,
                        EntityTypeID = (int)BudgettoEntityType.Reminders,
                        CanBeCount = 5,
                        LastChanges = now,
                    });
                }

                if (!db_PaymentCounter.Any(x => x.EntityTypeID == (int)BudgettoEntityType.ToDoLists))
                {
                    paymentCounters.Add(new PaymentCounter
                    {
                        PaymentTariffID = (int)PaymentTariffTypes.Premium,
                        EntityTypeID = (int)BudgettoEntityType.ToDoLists,
                        CanBeCount = 5,
                        LastChanges = now,
                    });
                }
            }

            if (paymentCounters.Count > 0)
            {
                repository.CreateRange(paymentCounters);
                repository.Save();
            }
        }

        private void AddPaymentTariff()
        {
            var db_PaymentTariff = repository.GetAll<PaymentTariff>().Select(x => new { ID = x.ID, CodeName = x.CodeName }).ToList();
            List<PaymentTariff> PaymentTariffs = new List<PaymentTariff>();

            if (!db_PaymentTariff.Any(x => x.ID == (int)PaymentTariffTypes.Free))
            {
                PaymentTariffs.Add(new PaymentTariff
                {
                    Name = "Free",
                    CodeName = "Free"
                });
            }

            if (!db_PaymentTariff.Any(x => x.ID == (int)PaymentTariffTypes.Standard))
            {
                PaymentTariffs.Add(new PaymentTariff
                {
                    Name = "Standard",
                    CodeName = "Standard"
                });
            }

            if (!db_PaymentTariff.Any(x => x.ID == (int)PaymentTariffTypes.Premium))
            {
                PaymentTariffs.Add(new PaymentTariff
                {
                    Name = "Premium",
                    CodeName = "Premium"
                });
            }

            if (!db_PaymentTariff.Any(x => x.ID == (int)PaymentTariffTypes.Freedom))
            {
                PaymentTariffs.Add(new PaymentTariff
                {
                    Name = "Freedom",
                    CodeName = "Freedom"
                });
            }

            if (PaymentTariffs.Count > 0)
            {
                repository.CreateRange(PaymentTariffs);
                repository.Save();
            }
        }

        private void CheckAndAddUserEntityCounter()
        {
            var now = DateTime.Now.ToUniversalTime();
            var userEntityCounters = new List<UserEntityCounter>();

            var users = repository.GetAll<MyProfile.Entity.Model.User>(x => x.IsDeleted == false)
                .Select(x => new
                {
                    x.ID,
                    Counters = x.UserEntityCounters
                     .Select(y => new
                     {
                         y.EntityTypeID,
                         y.AddedCount
                     })
                     .ToList()
                })
                .ToList();

            for (int i = 0; i < users.Count; i++)
            {
                if (!users[i].Counters.Any(x => x.EntityTypeID == (int)BudgettoEntityType.Limits))
                {
                    userEntityCounters.Add(new UserEntityCounter
                    {
                        LastChanges = now,
                        EntityTypeID = (int)BudgettoEntityType.Limits,
                        AddedCount = 0,
                        UserID = users[i].ID,
                    });
                }

            }

            if (userEntityCounters.Count > 0)
            {
                repository.CreateRange(userEntityCounters, true);
            }
        }

        private void SubScriptionToProd()
        {
            if (repository.GetAll<SubScriptionModel>().Any() == false)
            {
                List<SubScriptionCategory> categories = new List<SubScriptionCategory>();

                using (StreamReader reader = new StreamReader(hostingEnvironment.WebRootPath + @"\\json\\SubScriptionCategoryToProd.json"))
                {
                    categories = (List<SubScriptionCategory>)JsonConvert.DeserializeObject<List<SubScriptionCategory>>(reader.ReadToEnd());
                    repository.CreateRange(categories, true);
                }

                using (StreamReader reader = new StreamReader(hostingEnvironment.WebRootPath + @"\\json\\SubScriptionToProd.json"))
                {
                    List<SubScriptionModel> subScriptions = (List<SubScriptionModel>)JsonConvert.DeserializeObject<List<SubScriptionModel>>(reader.ReadToEnd());

                    foreach (var subScription in subScriptions)
                    {
                        subScription.SubScriptionCategoryID = categories.FirstOrDefault(x => x.Title == subScription.Description).ID;
                        subScription.Description = null;
                    }

                    repository.CreateRange(subScriptions, true);
                }
            }
        }

        private void SubScriptionToJson()
        {
            using (StreamWriter writer = new StreamWriter(hostingEnvironment.WebRootPath + @"\\json\\SubScriptionCategoryToProd.json"))
            {
                //SubScriptionCategory = 
                var cards = repository.GetAll<SubScriptionCategory>()
                     .Select(x => new SubScriptionCategory
                     {
                         CodeName = x.CodeName,
                         Title = x.Title
                     })
                     .ToList();
                var s = JsonConvert.SerializeObject(cards);
                writer.Write(s);
                writer.Close();
            }

            using (StreamWriter writer = new StreamWriter(hostingEnvironment.WebRootPath + @"\\json\\SubScriptionToProd.json"))
            {
                //SubScriptionCategory = 
                var cards = repository.GetAll<SubScriptionModel>()
                     .Select(x => new SubScriptionModel
                     {
                         IsActive = true,
                         LogoBig = x.LogoBig,
                         Site = x.Site,
                         Title = x.Title,
                         Description = x.SubScriptionCategory.Title,
                         SubScriptionOptions = x.SubScriptionOptions
                            .Select(y => new SubScriptionOption
                            {
                                EditDate = y.EditDate,
                                IsActive = true,
                                IsBoth = y.IsBoth,
                                IsFamaly = y.IsFamaly,
                                IsPersonally = y.IsPersonally,
                                IsStudent = y.IsStudent,
                                Title = y.Title,
                                _raiting = y._raiting,
                                SubScriptionPricings = y.SubScriptionPricings
                                    .Select(z => new SubScriptionPricing
                                    {
                                        Price = z.Price,
                                        PricePerMonth = z.PricePerMonth,
                                        PricingPeriodTypeID = z.PricingPeriodTypeID,
                                    }).ToList()
                            }).ToList()
                     })
                     .ToList();
                var s = JsonConvert.SerializeObject(cards);
                writer.Write(s);
                writer.Close();
            }
        }

        private void SubScriptionLoading()
        {
            if (repository.GetAll<SubScriptionPricing>().Any() == false)
            {
                using (StreamReader reader = new StreamReader(hostingEnvironment.WebRootPath + @"\\json\\subscription.json"))
                using (WebClient client = new WebClient())
                {
                    List<SubScriptionJson> json = (List<SubScriptionJson>)JsonConvert.DeserializeObject<List<SubScriptionJson>>(reader.ReadToEnd());
                    List<SubScriptionModel> subScriptions = new List<SubScriptionModel>();
                    List<SubScriptionCategory> categories = repository.GetAll<SubScriptionCategory>().ToList();

                    //foreach (var jsonSub in json)
                    //{
                    //    if (categories.Any(x => x.Title == jsonSub.category) == false)
                    //    {
                    //        categories.Add(new SubScriptionCategory
                    //        {
                    //            Title = jsonSub.category,
                    //            CodeName = ""
                    //        });
                    //    }
                    //}

                    //  repository.CreateRange(categories, true);

                    foreach (var jsonSub in json)
                    {
                        SubScriptionModel subScription;
                        bool isBoth = false;
                        bool isFamaly = false;
                        bool isStudent = false;
                        bool isPersanaly = false;
                        string title = "";
                        decimal price;
                        decimal priceForMonth;

                        PricingPeriodType pricingPeriodType = PricingPeriodType.Undefined;
                        try
                        {
                            var arr = jsonSub.title.Split(" – ");

                            if (subScriptions.Any(x => x.Title == arr[0]) == false)
                            {
                                subScription = new SubScriptionModel { SubScriptionCategory = new SubScriptionCategory(), SubScriptionOptions = new List<SubScriptionOption>() };
                                subScription.Title = arr[0];
                                subScription.Site = jsonSub.site;
                            }
                            else
                            {
                                subScription = subScriptions.FirstOrDefault(x => x.Title == arr[0]);
                            }

                            subScription.SubScriptionCategoryID = categories.FirstOrDefault(x => x.Title == jsonSub.category).ID;


                            isBoth = arr[1].ToLower().Contains("для двоих");
                            isFamaly = arr[1].ToLower().Contains("семейная");
                            isStudent = arr[1].ToLower().Contains("студенческая");
                            isPersanaly = isBoth == false && isFamaly == false && isStudent == false;

                            if (jsonSub.title.Contains("Подписка"))
                            {
                                title = arr[1].Replace("Подписка ", "");
                                if (title.IndexOf(" на") >= 0)
                                {
                                    title = title.Substring(0, title.IndexOf(" на"));
                                }
                                else
                                {
                                    title = null;
                                }
                            }

                            price = decimal.Parse(jsonSub.price);

                            if (jsonSub.title.Contains("на 1 год") || jsonSub.title.Contains("на 12 месяцев"))
                            {
                                pricingPeriodType = PricingPeriodType.Month12;
                                priceForMonth = price / 12;

                            }
                            else if (jsonSub.title.Contains("на 3 месяца"))
                            {
                                pricingPeriodType = PricingPeriodType.Month3;
                                priceForMonth = price / 3;
                            }
                            else
                            {
                                pricingPeriodType = PricingPeriodType.Month1;
                                priceForMonth = price;
                            }

                            var subScriptionOption = subScription.SubScriptionOptions.FirstOrDefault(x => x.Title == title);
                            bool isNew = true;

                            if (subScriptionOption != null)
                            {
                                isNew = !(subScriptionOption.IsBoth == isBoth && subScriptionOption.IsFamaly == isFamaly && subScriptionOption.IsPersonally == isPersanaly && subScriptionOption.IsStudent == isStudent);
                            }

                            if (isNew)
                            {
                                subScription.SubScriptionOptions.Add(new SubScriptionOption
                                {
                                    IsActive = true,
                                    IsFamaly = isFamaly,
                                    IsPersonally = isPersanaly,
                                    IsStudent = isStudent,
                                    IsBoth = isBoth,
                                    _raiting = decimal.Parse(jsonSub.raiting),
                                    EditDate = now,
                                    Title = title,
                                    SubScriptionPricings = new List<SubScriptionPricing> { new SubScriptionPricing
                                {
                                    PricingPeriodTypeID = (int)pricingPeriodType,
                                    Price = price,
                                    PricePerMonth = priceForMonth,
                                } }
                                });
                            }
                            else
                            {
                                subScriptionOption.SubScriptionPricings.Add(new SubScriptionPricing
                                {
                                    PricingPeriodTypeID = (int)pricingPeriodType,
                                    Price = price,
                                    PricePerMonth = priceForMonth,
                                });
                            }



                        }
                        catch (Exception ex)
                        {
                            continue;
                        }

                        if (!string.IsNullOrEmpty(jsonSub.src))
                        {
                            try
                            {
                                Random random = new Random();
                                var n = random.Next(10000, 99999).ToString();

                                string url = @"C:\Users\t3l3f\source\repos\MyProject\MyProfile\wwwroot\resources\subscriptions\" + n + "_big.jpg";
                                client.DownloadFile(new Uri(jsonSub.src), url);
                                subScription.LogoBig = @"/resources/subscriptions/" + n + "_big.jpg";
                            }
                            catch (Exception ex1)
                            {

                            }
                        }
                        subScriptions.Add(subScription);
                    }

                    repository.CreateRange(subScriptions, true);
                }

            }
        }

        private void CardJsonToCard()
        {
            if (repository.GetAll<Card>().Any() == false)
            {
                var banks = repository.GetAll<Bank>()
                    .Select(x => new
                    {
                        bankID = x.ID,
                        bankName = x.Name
                    })
                    .ToList();

                using (StreamReader reader = new StreamReader(hostingEnvironment.WebRootPath + @"\\json\\CardToProd.json"))
                {
                    List<Card> cards = (List<Card>)JsonConvert.DeserializeObject<List<Card>>(reader.ReadToEnd());

                    foreach (var card in cards)
                    {
                        try
                        {
                            card.BankID = banks.FirstOrDefault(x => x.bankName == card.bankName).bankID;
                        }
                        catch (Exception ex)
                        {

                        }
                    }

                    repository.CreateRange(cards, true);
                }
            }

        }

        private void CardToJson()
        {
            using (StreamWriter writer = new StreamWriter(hostingEnvironment.WebRootPath + @"\\json\\CardToProd.json"))
            {
                var cards = repository.GetAll<Card>()
                     .Select(x => new Card
                     {
                         AccountTypeID = x.AccountTypeID,
                         BankID = x.BankID,
                         bankName = x.Bank.Name,
                         bankiruCardID = x.bankiruCardID,
                         BigLogo = x.BigLogo,
                         bonuses = x.bonuses,
                         Cashback = x.Cashback,
                         CreditLimit = x.CreditLimit,
                         GracePeriod = x.GracePeriod,
                         Interest = x.Interest,
                         IsCashback = x.IsCashback,
                         IsCustomDesign = x.IsCustomDesign,
                         IsInterest = x.IsInterest,
                         IsRateTo = x.IsRateTo,
                         Name = x.Name,
                         paymentSystems = x.paymentSystems,
                         Raiting = x.Raiting,
                         Rate = x.Rate,
                         SearchString = x.SearchString,
                         ServiceCostFrom = x.ServiceCostFrom,
                         ServiceCostTo = x.ServiceCostTo,
                         SmallLogo = x.SmallLogo,
                         CardPaymentSystems = x.CardPaymentSystems
                             .Select(y => new CardPaymentSystem
                             {
                                 PaymentSystemID = y.PaymentSystemID
                             })
                             .ToList()
                     })
                     .ToList();
                var s = JsonConvert.SerializeObject(cards);
                writer.Write(s);
                writer.Close();
            }
        }

        private void CreditRCardsLoading()
        {
            if (repository.GetAll<Card>().Any(x => x.AccountTypeID == (int)AccountTypes.Installment) == false)
            {
                using (StreamReader reader = new StreamReader(hostingEnvironment.WebRootPath + @"\\json\\rassrochki-credit-cards.json"))
                using (WebClient client = new WebClient())
                {
                    List<CreditCard> cards = (List<CreditCard>)JsonConvert.DeserializeObject<List<CreditCard>>(reader.ReadToEnd());
                    List<Card> newCards = new List<Card>();

                    var banks = repository.GetAll<Bank>().ToList();
                    int visaID = repository.GetAll<PaymentSystem>(x => x.CodeName == "Visa").FirstOrDefault().ID;
                    int mastercardID = repository.GetAll<PaymentSystem>(x => x.CodeName == "Mastercard").FirstOrDefault().ID;
                    int maestroID = repository.GetAll<PaymentSystem>(x => x.CodeName == "Maestro").FirstOrDefault().ID;
                    int mirID = repository.GetAll<PaymentSystem>(x => x.CodeName == "Mir").FirstOrDefault().ID;
                    int payPalID = repository.GetAll<PaymentSystem>(x => x.CodeName == "PayPal").FirstOrDefault().ID;
                    int americanExpressID = repository.GetAll<PaymentSystem>(x => x.CodeName == "AmericanExpress").FirstOrDefault().ID;

                    foreach (var cardInfo in cards)
                    {
                        Card card = new Card();
                        try
                        {
                            card.AccountTypeID = (int)AccountTypes.Installment;
                            card.Raiting = cardInfo.raiting;
                            card.Name = cardInfo.cardName;
                            card.BankID = banks.FirstOrDefault(x => x.Name == cardInfo.bankName)?.ID ?? null;

                            if (!cardInfo.bankiCardID.Contains("specials"))
                            {
                                card.bankiruCardID = int.Parse(cardInfo.bankiCardID.Replace("/", ""));
                            }

                            #region payment system
                            card.CardPaymentSystems = new List<CardPaymentSystem>();

                            foreach (var paymentSystem in cardInfo.paymentSystems)
                            {
                                if (paymentSystem.Contains("Visa"))
                                {
                                    card.CardPaymentSystems.Add(new CardPaymentSystem
                                    {
                                        PaymentSystemID = visaID
                                    });
                                }
                                if (paymentSystem.Contains("Mastercard"))
                                {
                                    card.CardPaymentSystems.Add(new CardPaymentSystem
                                    {
                                        PaymentSystemID = mastercardID
                                    });
                                }
                                if (paymentSystem.Contains("Maestro"))
                                {
                                    card.CardPaymentSystems.Add(new CardPaymentSystem
                                    {
                                        PaymentSystemID = maestroID
                                    });
                                }
                                if (paymentSystem.Contains("Мир"))
                                {
                                    card.CardPaymentSystems.Add(new CardPaymentSystem
                                    {
                                        PaymentSystemID = mirID
                                    });
                                }
                                if (paymentSystem.Contains("American Express"))
                                {
                                    card.CardPaymentSystems.Add(new CardPaymentSystem
                                    {
                                        PaymentSystemID = americanExpressID
                                    });
                                }
                                card.paymentSystems += paymentSystem + ",";
                            }
                            #endregion

                            if (!string.IsNullOrEmpty(cardInfo.serviceCost))
                            {
                                cardInfo.serviceCost = cardInfo.serviceCost.Replace(" ", "").Replace("от", "");

                                var serviceCost = cardInfo.serviceCost.Split("-");
                                if (serviceCost.Length == 1)
                                {
                                    card.ServiceCostFrom = 0;
                                    card.ServiceCostTo = decimal.Parse(serviceCost[0].Replace("до", ""));

                                }
                                else
                                {
                                    card.ServiceCostFrom = decimal.Parse(serviceCost[0]);
                                    card.ServiceCostTo = decimal.Parse(serviceCost[1]);
                                }
                            }

                            if (!string.IsNullOrEmpty(cardInfo.rate))
                            {
                                if (cardInfo.rate.Contains("от"))
                                {
                                    card.IsRateTo = true;
                                }
                                else if (cardInfo.rate.Contains("до"))
                                {
                                    card.IsRateTo = false;
                                }
                                else if (!cardInfo.rate.Contains("от") && !cardInfo.rate.Contains("до"))
                                {
                                    card.IsRateTo = null;
                                }
                                card.Rate = decimal.Parse(cardInfo.rate.Replace("до", "").Replace("от", "").Replace("%", "").Replace(",", "."));
                            }

                            if (!string.IsNullOrEmpty(cardInfo.cashback))
                            {
                                if (cardInfo.cashback == "нет")
                                {
                                }
                                else if (cardInfo.cashback == "есть")
                                {
                                    card.IsCashback = true;
                                    card.Cashback = 1;
                                }
                                else if (cardInfo.cashback.Contains("до"))
                                {
                                    card.IsCashback = true;
                                    card.Cashback = decimal.Parse(cardInfo.cashback.Replace("до ", "").Replace("%", "").Replace(",", "."));
                                }
                            }

                            if (cardInfo.creditLimit.Contains("до"))
                            {
                                card.CreditLimit = decimal.Parse(cardInfo.creditLimit.Replace("до", "").Replace(" ", ""));
                            }
                            else if (cardInfo.creditLimit.Contains("см. условия"))
                            {
                                card.CreditLimit = 0;
                            }

                        }
                        catch (Exception ex)
                        {
                            continue;
                        }

                        if (cardInfo.src.Contains("i-individual-design-previ"))
                        {
                            card.IsCustomDesign = true;
                        }
                        else if (!string.IsNullOrEmpty(cardInfo.src) && !cardInfo.src.Contains("i-no-design-card"))
                        {
                            try
                            {
                                Random random = new Random();
                                var n = random.Next(10000, 99999).ToString();

                                string url = @"C:\Users\t3l3f\source\repos\MyProject\MyProfile\wwwroot\resources\tmp\" + n + "_small.png";
                                client.DownloadFile(new Uri(cardInfo.src), url);
                                card.SmallLogo = @"/resources/cards/" + n + "_small.png";
                            }
                            catch (Exception ex1)
                            {

                            }
                        }
                        newCards.Add(card);
                    }

                    repository.CreateRange(newCards, true);
                }

            }
        }

        private void CreditCardsLoading()
        {
            if (repository.GetAll<Card>().Any(x => x.AccountTypeID == (int)AccountTypes.Credit) == false)
            {
                using (StreamReader reader = new StreamReader(hostingEnvironment.WebRootPath + @"\\json\\credit-cards.json"))
                using (WebClient client = new WebClient())
                {
                    List<CreditCard> cards = (List<CreditCard>)JsonConvert.DeserializeObject<List<CreditCard>>(reader.ReadToEnd());
                    List<Card> newCards = new List<Card>();

                    var banks = repository.GetAll<Bank>().ToList();
                    int visaID = repository.GetAll<PaymentSystem>(x => x.CodeName == "Visa").FirstOrDefault().ID;
                    int mastercardID = repository.GetAll<PaymentSystem>(x => x.CodeName == "Mastercard").FirstOrDefault().ID;
                    int maestroID = repository.GetAll<PaymentSystem>(x => x.CodeName == "Maestro").FirstOrDefault().ID;
                    int mirID = repository.GetAll<PaymentSystem>(x => x.CodeName == "Mir").FirstOrDefault().ID;
                    int payPalID = repository.GetAll<PaymentSystem>(x => x.CodeName == "PayPal").FirstOrDefault().ID;
                    int americanExpressID = repository.GetAll<PaymentSystem>(x => x.CodeName == "AmericanExpress").FirstOrDefault().ID;

                    foreach (var cardInfo in cards)
                    {
                        Card card = new Card();
                        try
                        {
                            card.AccountTypeID = (int)AccountTypes.Credit;
                            card.Raiting = cardInfo.raiting;
                            card.Name = cardInfo.cardName;
                            card.BankID = banks.FirstOrDefault(x => x.Name == cardInfo.bankName)?.ID ?? null;

                            if (!cardInfo.bankiCardID.Contains("specials"))
                            {
                                card.bankiruCardID = int.Parse(cardInfo.bankiCardID.Replace("/", ""));
                            }

                            #region payment system
                            card.CardPaymentSystems = new List<CardPaymentSystem>();

                            foreach (var paymentSystem in cardInfo.paymentSystems)
                            {
                                if (paymentSystem.Contains("Visa"))
                                {
                                    card.CardPaymentSystems.Add(new CardPaymentSystem
                                    {
                                        PaymentSystemID = visaID
                                    });
                                }
                                if (paymentSystem.Contains("Mastercard"))
                                {
                                    card.CardPaymentSystems.Add(new CardPaymentSystem
                                    {
                                        PaymentSystemID = mastercardID
                                    });
                                }
                                if (paymentSystem.Contains("Maestro"))
                                {
                                    card.CardPaymentSystems.Add(new CardPaymentSystem
                                    {
                                        PaymentSystemID = maestroID
                                    });
                                }
                                if (paymentSystem.Contains("Мир"))
                                {
                                    card.CardPaymentSystems.Add(new CardPaymentSystem
                                    {
                                        PaymentSystemID = mirID
                                    });
                                }
                                if (paymentSystem.Contains("American Express"))
                                {
                                    card.CardPaymentSystems.Add(new CardPaymentSystem
                                    {
                                        PaymentSystemID = americanExpressID
                                    });
                                }
                                card.paymentSystems += paymentSystem + ",";
                            }
                            #endregion

                            if (!string.IsNullOrEmpty(cardInfo.serviceCost))
                            {
                                cardInfo.serviceCost = cardInfo.serviceCost.Replace(" ", "").Replace("от", "");

                                var serviceCost = cardInfo.serviceCost.Split("-");
                                if (serviceCost.Length == 1)
                                {
                                    card.ServiceCostFrom = 0;
                                    card.ServiceCostTo = decimal.Parse(serviceCost[0].Replace("до", ""));

                                }
                                else
                                {
                                    card.ServiceCostFrom = decimal.Parse(serviceCost[0]);
                                    card.ServiceCostTo = decimal.Parse(serviceCost[1]);
                                }
                            }

                            if (!string.IsNullOrEmpty(cardInfo.rate))
                            {
                                if (cardInfo.rate.Contains("от"))
                                {
                                    card.IsRateTo = true;
                                }
                                else if (cardInfo.rate.Contains("до"))
                                {
                                    card.IsRateTo = false;
                                }
                                else if (!cardInfo.rate.Contains("от") && !cardInfo.rate.Contains("до"))
                                {
                                    card.IsRateTo = null;
                                }
                                card.Rate = decimal.Parse(cardInfo.rate.Replace("до", "").Replace("от", "").Replace("%", "").Replace(",", "."));
                            }

                            if (!string.IsNullOrEmpty(cardInfo.cashback))
                            {
                                if (cardInfo.cashback == "нет")
                                {
                                }
                                else if (cardInfo.cashback == "есть")
                                {
                                    card.IsCashback = true;
                                    card.Cashback = 1;
                                }
                                else if (cardInfo.cashback.Contains("до"))
                                {
                                    card.IsCashback = true;
                                    card.Cashback = decimal.Parse(cardInfo.cashback.Replace("до ", "").Replace("%", "").Replace(",", "."));
                                }
                            }

                            if (cardInfo.creditLimit.Contains("до"))
                            {
                                card.CreditLimit = decimal.Parse(cardInfo.creditLimit.Replace("до", "").Replace(" ", ""));
                            }
                            else if (cardInfo.creditLimit.Contains("см. условия"))
                            {
                                card.CreditLimit = 0;
                            }

                        }
                        catch (Exception ex)
                        {
                            continue;
                        }

                        if (cardInfo.src.Contains("i-individual-design-previ"))
                        {
                            card.IsCustomDesign = true;
                        }
                        else if (!string.IsNullOrEmpty(cardInfo.src) && !cardInfo.src.Contains("i-no-design-card"))
                        {
                            try
                            {
                                Random random = new Random();
                                var n = random.Next(10000, 99999).ToString();

                                string url = @"C:\Users\t3l3f\source\repos\MyProject\MyProfile\wwwroot\resources\tmp\" + n + "_small.png";
                                client.DownloadFile(new Uri(cardInfo.src), url);
                                card.SmallLogo = @"/resources/cards/" + n + "_small.png";
                            }
                            catch (Exception ex1)
                            {

                            }
                        }
                        newCards.Add(card);
                    }

                    repository.CreateRange(newCards, true);
                }

            }
        }

        private void DebitCardsLoading()
        {
            if (repository.GetAll<Card>().Any() == false)
            {
                using (StreamReader reader = new StreamReader(hostingEnvironment.WebRootPath + @"\\json\\debitCards.json"))
                using (WebClient client = new WebClient())
                {
                    List<DebitCard> cards = (List<DebitCard>)JsonConvert.DeserializeObject<List<DebitCard>>(reader.ReadToEnd());
                    List<Card> newCards = new List<Card>();

                    var banks = repository.GetAll<Bank>().ToList();
                    int visaID = repository.GetAll<PaymentSystem>(x => x.CodeName == "Visa").FirstOrDefault().ID;
                    int mastercardID = repository.GetAll<PaymentSystem>(x => x.CodeName == "Mastercard").FirstOrDefault().ID;
                    int maestroID = repository.GetAll<PaymentSystem>(x => x.CodeName == "Maestro").FirstOrDefault().ID;
                    int mirID = repository.GetAll<PaymentSystem>(x => x.CodeName == "Mir").FirstOrDefault().ID;
                    int payPalID = repository.GetAll<PaymentSystem>(x => x.CodeName == "PayPal").FirstOrDefault().ID;

                    foreach (var cardInfo in cards)
                    {
                        Card card = new Card();
                        try
                        {
                            card.AccountTypeID = (int)AccountTypes.Debed;
                            card.Raiting = cardInfo.raiting;
                            card.Name = cardInfo.cardName;
                            card.BankID = banks.FirstOrDefault(x => x.Name == cardInfo.bankName)?.ID ?? null;

                            if (!cardInfo.bankiCardID.Contains("specials"))
                            {
                                card.bankiruCardID = int.Parse(cardInfo.bankiCardID.Replace("/", ""));
                            }

                            #region payment system
                            card.CardPaymentSystems = new List<CardPaymentSystem>();

                            foreach (var paymentSystem in cardInfo.paymentSystems)
                            {
                                if (paymentSystem.Contains("Visa"))
                                {
                                    card.CardPaymentSystems.Add(new CardPaymentSystem
                                    {
                                        PaymentSystemID = visaID
                                    });
                                }
                                if (paymentSystem.Contains("Mastercard"))
                                {
                                    card.CardPaymentSystems.Add(new CardPaymentSystem
                                    {
                                        PaymentSystemID = mastercardID
                                    });
                                }
                                if (paymentSystem.Contains("Maestro"))
                                {
                                    card.CardPaymentSystems.Add(new CardPaymentSystem
                                    {
                                        PaymentSystemID = maestroID
                                    });
                                }
                                if (paymentSystem.Contains("Мир"))
                                {
                                    card.CardPaymentSystems.Add(new CardPaymentSystem
                                    {
                                        PaymentSystemID = mirID
                                    });
                                }
                                card.paymentSystems += paymentSystem + ",";
                            }
                            #endregion

                            if (!string.IsNullOrEmpty(cardInfo.serviceCost))
                            {
                                cardInfo.serviceCost = cardInfo.serviceCost.Replace(" ", "").Replace("от", "");

                                var serviceCost = cardInfo.serviceCost.Split("-");
                                if (serviceCost.Length == 1)
                                {
                                    card.ServiceCostFrom = 0;
                                    card.ServiceCostTo = decimal.Parse(serviceCost[0].Replace("до", ""));

                                }
                                else
                                {
                                    card.ServiceCostFrom = decimal.Parse(serviceCost[0]);
                                    card.ServiceCostTo = decimal.Parse(serviceCost[1]);
                                }
                            }

                            if (!string.IsNullOrEmpty(cardInfo.interest))
                            {
                                if (cardInfo.interest == "нет")
                                {
                                }
                                else if (cardInfo.interest == "есть")
                                {
                                    card.IsInterest = true;
                                    card.Interest = 1;
                                }
                                else if (cardInfo.interest.Contains("до"))
                                {
                                    card.IsInterest = true;
                                    card.Interest = decimal.Parse(cardInfo.interest.Replace("до ", "").Replace("%", "").Replace(",", "."));
                                }
                            }

                            if (!string.IsNullOrEmpty(cardInfo.cashback))
                            {
                                if (cardInfo.cashback == "нет")
                                {
                                }
                                else if (cardInfo.cashback == "есть")
                                {
                                    card.IsCashback = true;
                                    card.Cashback = 1;
                                }
                                else if (cardInfo.cashback.Contains("до"))
                                {
                                    card.IsCashback = true;
                                    card.Cashback = decimal.Parse(cardInfo.cashback.Replace("до ", "").Replace("%", "").Replace(",", "."));
                                }
                            }

                            foreach (var bonus in cardInfo.bonuses)
                            {
                                card.bonuses += bonus + ",";
                            }
                        }
                        catch (Exception ex)
                        {
                            continue;
                        }

                        if (!string.IsNullOrEmpty(cardInfo.src) && cardInfo.src != "//cdn.banki.ru/static/common/dist/media/cards/i-no-design-card.png?3c9582f023" && !cardInfo.src.Contains("i-individual-design-previ"))
                        {
                            try
                            {
                                Random random = new Random();
                                var n = random.Next(1000, 9999).ToString();

                                string url = @"C:\Users\t3l3f\source\repos\MyProject\MyProfile\wwwroot\resources\cards\" + n + "_small.png";
                                client.DownloadFile(new Uri(cardInfo.src), url);
                                card.SmallLogo = @"/resources/cards/" + n + "_small.png";
                            }
                            catch (Exception ex1)
                            {

                            }
                        }
                        newCards.Add(card);
                    }

                    repository.CreateRange(newCards, true);
                }

            }
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
                    PaymentTariffID = (int)PaymentTariffTypes.Freedom,
                    PaymentHistories = new List<PaymentHistory> {
                        new PaymentHistory {
                            DateFrom = now,
                            DateTo = now.AddYears(1),
                            //DateTo = now.AddMonths(2),
                            PaymentTariffID = (int)PaymentTariffTypes.Freedom,
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

            //if (!db_summuries.Any(x => x.ID == (int)SummaryType.AllSubScriptionPrice))
            //{
            //    summaries.Add(new Summary
            //    {
            //        Name = "Общая цена всех подписок",
            //        CodeName = "AllSubScriptionPrice",
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

        public class DebitCard
        {
            public string src { get; set; }
            public string cardName { get; set; }
            public string bankiCardID { get; set; }
            public string bankName { get; set; }
            public List<string> paymentSystems { get; set; }
            public string interest { get; set; }
            public string serviceCost { get; set; }
            public List<string> bonuses { get; set; }
            public string cashback { get; set; }
            public int raiting { get; set; }
        }

        public class CreditCard
        {
            public string src { get; set; }
            public string cardName { get; set; }
            public string bankiCardID { get; set; }
            public string bankName { get; set; }
            public List<string> paymentSystems { get; set; }
            public string rate { get; set; }
            public string serviceCost { get; set; }
            public string creditLimit { get; set; }
            public string cashback { get; set; }
            public int raiting { get; set; }
        }

        public class SubScriptionJson
        {
            public string src { get; set; }
            public string raiting { get; set; }
            public string price { get; set; }
            public string title { get; set; }
            public string category { get; set; }
            public string site { get; set; }
        }

        public class BaseAreaAndSection
        {
            public string AreaEng { get; set; }
            public string AreaRus { get; set; }
            public string CategoryEng { get; set; }
            public string CategoryRus { get; set; }
            public string KeyWords { get; set; }
            public string Color { get; set; }
            public string Icon { get; set; }
            public string Background { get; set; }
            public string SectionType { get; set; }
        }
    }
}
