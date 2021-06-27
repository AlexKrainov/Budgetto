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
using static System.Net.WebRequestMethods;
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
            AddBudgettoEntityType();
            AddPaymentCounter();

            CheckAndAddUserEntityCounter();
            CreateProgressTypes();
            //CheckUserProgress();
            //LoadTinkoffOperations();
            //UpdateCompanies();
            //CompaniesToJson();

            //CompaniesToProd();
           // LinkUserTagsAndCompanies();
        }


        private void CompaniesToProd()
        {
            if (repository.GetAll<Company>().Any() == false)
            {

                using (StreamReader reader = new StreamReader(hostingEnvironment.WebRootPath + @"\\json\\companies2prod.json"))
                {
                   var companies = (List<Company>)JsonConvert.DeserializeObject<List<Company>>(reader.ReadToEnd());
                    foreach (var company in companies)
                    {
                        company.ID = 0;
                    }
                    repository.CreateRange(companies, true);
                }

                using (StreamReader reader = new StreamReader(hostingEnvironment.WebRootPath + @"\\json\\mcc2prod.json"))
                {
                    List<MccCategory> mccCategories = (List<MccCategory>)JsonConvert.DeserializeObject<List<MccCategory>>(reader.ReadToEnd());

                    foreach (var mccCategory in mccCategories)
                    {
                        mccCategory.ID = 0;
                        foreach (var mccCode in mccCategory.MccCodes)
                        {
                            mccCode.ID = 0;
                        }
                    }

                    repository.CreateRange(mccCategories, true);
                }
            }
        }

        private void CompaniesToJson()
        {
            using (StreamWriter writer = new StreamWriter(hostingEnvironment.WebRootPath + @"\\json\\companies2prod.json"))
            {
                var cards = repository.GetAll<Company>()
                     .ToList();
                var s = JsonConvert.SerializeObject(cards);
                writer.Write(s);
                writer.Close();
            }

            using (StreamWriter writer = new StreamWriter(hostingEnvironment.WebRootPath + @"\\json\\mcc2prod.json"))
            {
                var cards = repository.GetAll<MccCategory>()
                     .ToList();
                var s = JsonConvert.SerializeObject(cards);
                writer.Write(s);
                writer.Close();
            }
        }

        private void CreateProgressTypes()
        {
            List<ProgressType> progressTypes = new List<ProgressType>();

            foreach (var type in Enum.GetNames(typeof(ProgressTypeEnum)))
            {
                if (!repository.Any<ProgressType>(x => x.CodeName == type))
                {
                    progressTypes.Add(new ProgressType { CodeName = type });
                }
            }

            if (progressTypes.Count > 0)
            {
                repository.CreateRange(progressTypes, true);
            }

            List<ProgressItemType> progressItemTypes = new List<ProgressItemType>();

            foreach (var type in Enum.GetNames(typeof(ProgressItemTypeEnum)))
            {
                if (!repository.Any<ProgressItemType>(x => x.CodeName == type))
                {
                    progressItemTypes.Add(new ProgressItemType { CodeName = type });
                }
            }

            if (progressItemTypes.Count > 0)
            {
                repository.CreateRange(progressItemTypes, true);
            }
        }

        private void CheckUserProgress()
        {
            var userIDs = repository.GetAll<MyProfile.Entity.Model.User>(x => x.IsDeleted == false).Select(x => x.ID).ToList();
            var progresses = repository.GetAll<MyProfile.Entity.Model.Progress>()
                .ToList();

            foreach (var userID in userIDs)
            {
                var newProgress = progresses.FirstOrDefault(x => x.ParentProgressID == null && x.UserID == userID && x.ProgressTypeID == (int)ProgressTypeEnum.Introductory);
                var progressFinancialLiteracyMonth = progresses.FirstOrDefault(x => x.ProgressTypeID == (int)ProgressTypeEnum.FinancialLiteracyMonth);

                if (newProgress == null)
                {
                    newProgress = new MyProfile.Entity.Model.Progress
                    {
                        ProgressTypeID = (int)ProgressTypeEnum.Introductory,
                        UserID = userID,
                        Progresses = new List<MyProfile.Entity.Model.Progress>()
                    };

                    #region ProgressTypeEnum.Introductory
                    newProgress.Progresses.Add(new MyProfile.Entity.Model.Progress
                    {
                        ProgressTypeID = (int)ProgressTypeEnum.Introductory,
                        ProgressItemTypeID = (int)ProgressItemTypeEnum.CreateRecord,
                        UserID = userID
                    });
                    newProgress.Progresses.Add(new MyProfile.Entity.Model.Progress
                    {
                        UserID = userID,
                        ProgressTypeID = (int)ProgressTypeEnum.Introductory,
                        ProgressItemTypeID = (int)ProgressItemTypeEnum.CreateLimit
                    });
                    newProgress.Progresses.Add(new MyProfile.Entity.Model.Progress
                    {
                        UserID = userID,
                        ProgressTypeID = (int)ProgressTypeEnum.Introductory,
                        ProgressItemTypeID = (int)ProgressItemTypeEnum.CreateNotification
                    });
                    newProgress.Progresses.Add(new MyProfile.Entity.Model.Progress
                    {
                        UserID = userID,
                        ProgressTypeID = (int)ProgressTypeEnum.Introductory,
                        ProgressItemTypeID = (int)ProgressItemTypeEnum.CreateOrEditTemplate
                    });
                    newProgress.Progresses.Add(new MyProfile.Entity.Model.Progress
                    {
                        UserID = userID,
                        ProgressTypeID = (int)ProgressTypeEnum.Introductory,
                        ProgressItemTypeID = (int)ProgressItemTypeEnum.CreateSection
                    });
                    newProgress.Progresses.Add(new MyProfile.Entity.Model.Progress
                    {
                        UserID = userID,
                        ProgressTypeID = (int)ProgressTypeEnum.Introductory,
                        ProgressItemTypeID = (int)ProgressItemTypeEnum.CreateArea
                    });
                    newProgress.Progresses.Add(new MyProfile.Entity.Model.Progress
                    {
                        UserID = userID,
                        ProgressTypeID = (int)ProgressTypeEnum.Introductory,
                        ProgressItemTypeID = (int)ProgressItemTypeEnum.CreateAccount
                    });

                    newProgress.Progresses.Add(new MyProfile.Entity.Model.Progress
                    {
                        UserID = userID,
                        ProgressTypeID = (int)ProgressTypeEnum.Introductory,
                        ProgressItemTypeID = (int)ProgressItemTypeEnum.CreateReminder
                    });
                    #endregion

                    repository.Create(newProgress, true);
                }

                if (progressFinancialLiteracyMonth == null)
                {
                    progressFinancialLiteracyMonth = new MyProfile.Entity.Model.Progress
                    {
                        ProgressTypeID = (int)ProgressTypeEnum.FinancialLiteracyMonth,
                        UserID = userID,
                        Progresses = new List<MyProfile.Entity.Model.Progress>()
                    };

                    progressFinancialLiteracyMonth.Progresses.Add(new Entity.Model.Progress
                    {
                        UserID = userID,
                        ProgressTypeID = (int)ProgressTypeEnum.FinancialLiteracyMonth,
                        ProgressItemTypeID = (int)ProgressItemTypeEnum.Investing10Percent,
                        NeedToBeValue = 10.ToString()
                    });
                    progressFinancialLiteracyMonth.Progresses.Add(new Entity.Model.Progress
                    {
                        UserID = userID,
                        ProgressTypeID = (int)ProgressTypeEnum.FinancialLiteracyMonth,
                        ProgressItemTypeID = (int)ProgressItemTypeEnum.EarnMoreThanSpend
                    });
                    progressFinancialLiteracyMonth.Progresses.Add(new Entity.Model.Progress
                    {
                        UserID = userID,
                        ProgressTypeID = (int)ProgressTypeEnum.FinancialLiteracyMonth,
                        ProgressItemTypeID = (int)ProgressItemTypeEnum.CreateRecords70PercentAMonth,
                        NeedToBeValue = 21.ToString()
                    });
                    repository.Create(progressFinancialLiteracyMonth, true);
                }

                //if (!newProgress.Progresses.Any(x => x.ParentProgressID != null && x.ProgressTypeID == (int)ProgressTypeEnum.Introductory && x.ProgressItemTypeID == (int)ProgressItemTypeEnum.CreateRecord))
                //{

                //}
            }
        }

        /// <summary>
        /// [Company], [MccCode], [MccCategory]
        /// </summary>
        private void LoadTinkoffOperations()
        {
            var now = DateTime.Now.ToUniversalTime();
            string[] moscow_cities = new string[] { "MOSCOW", "MOSKVA", "GOROD MOSKVA", "G. MOSKVA", "77 - MOSCOW", "MOSKVA G", "WWW.YAKITORIY" };
            string[] countries = new string[] { "RU", "RUS" };
            int bankID = repository.GetAll<Bank>(x => x.NameEn == "Tinkoff Bank").FirstOrDefault().ID;

            List<Company> companies = repository.GetAll<Company>().ToList();
            List<MccCategory> categories = repository.GetAll<MccCategory>().ToList();
            List<MccCode> codes = repository.GetAll<MccCode>().ToList();

            using (StreamReader reader = new StreamReader(hostingEnvironment.WebRootPath + @"\\json\\tinkoff-operations-Anna.json"))
            //using (StreamReader reader = new StreamReader(hostingEnvironment.WebRootPath + @"\\json\\tinkoff-operations-2.json"))
            //using (StreamReader reader = new StreamReader(hostingEnvironment.WebRootPath + @"\\json\\tinkoff-operations.json"))
            using (WebClient client = new WebClient())
            {
                List<TinkoffOperation> operations = JsonConvert.DeserializeObject<List<TinkoffOperation>>(reader.ReadToEnd());

                foreach (var operation in operations)
                {
                    #region company
                    if (operation.description.Contains("Билайн"))
                    {
                        operation.description = "Билайн";
                    }
                    if (operation.description.Contains("МосЭнергоСбыт"))
                    {
                        operation.description = "МосЭнергоСбыт";
                    }
                    if (operation.description.Contains("МТС +"))
                    {
                        operation.description = "МТС";
                    }
                    if (operation.description.Contains("Мамин ростелеком"))
                    {
                        operation.description = "Ростелеком";
                    }
                    if (operation.description.Contains("Дмитрий К.") && operation.operationPaymentType != "TEMPLATE" && operation.brand.name == "Сбербанк")
                    {
                        operation.description = "Сбербанк";
                    }
                    if (operation.description.Contains("Дмитрий К.") && operation.operationPaymentType != "TEMPLATE" && operation.brand.name == "Тинькофф Банк")
                    {
                        operation.description = "Тинькофф Банк";
                    }
                    var company = companies.FirstOrDefault(x => operation.brand != null && x.t_objectID == operation.brand.id);

                    if (company == null)
                    {
                        company = new Company();
                        company.CreateDate = now;

                        if (operation.description != "Пополнение счета Тинькофф Брокер"
                            && operation.description != "В бюджетные организации"
                            && operation.description != "Департамент Финансов города Москвы (ГБОУ Школа № 1935 л/сч 2607542000900589)"
                             && operation.description != "Перевод между счетами"
                             && operation.description != "Рокетбанк"
                             && operation.description != "Пополнение Tinkoff Black"
                             && operation.description != "Проценты на остаток по счету"
                            && operation.description != "Пополнение накопительного счета"
                            && operation.description != "Оплата базового номера +79688092636"
                            && operation.description != "Мой телефон +79035130446"
                            && operation.description != "Билайн +79660857970"
                            && operation.description != "Перевод с карты"
                            && operation.description != "Бонусы")
                        {
                            if (operation.brand != null
                                && operation.brand.baseColor != null
                                && operation.brand.name != "Перевод с карты"
                                && operation.brand.name != "Перевод на вклад"
                                && operation.brand.name != "Бонусы")
                            {
                                company.Name = operation.brand.name;
                                company.BankKeyWords += operation.brand.name + " | ";
                                company.TagKeyWords += operation.brand.name + " | ";
                                company.BrandColor = operation.brand.baseColor.ToUpper();
                                company.TextColor = operation.brand.baseTextColor.ToUpper();
                                company.t_objectID = operation.brand.id;


                                if (!System.IO.File.Exists(@"C:\Users\t3l3f\source\repos\MyProject\MyProfile\wwwroot\resources\companies\" + operation.brand.logoFile))
                                {
                                    try
                                    {
                                        //Random random = new Random();
                                        //var n = random.Next(10000, 99999).ToString();

                                        string url = @"C:\Users\t3l3f\source\repos\MyProject\MyProfile\wwwroot\resources\companies\" + operation.brand.logoFile;
                                        client.DownloadFile(new Uri("https://brands-prod.cdn-tinkoff.ru/general_logo/" + operation.brand.logoFile), url);
                                        company.LogoSquare = @"/resources/companies/" + operation.brand.logoFile;
                                    }
                                    catch (Exception ex1)
                                    {

                                    }
                                }
                                else
                                {
                                    company.LogoSquare = @"/resources/companies/" + operation.brand.logoFile;
                                }
                            }
                            //else if (operation.merchant != null)
                            //{
                            //    company.Name = operation.merchant.name;
                            //} 
                        }

                        if (operation.merchant != null)
                        {
                            if (!string.IsNullOrEmpty(company.Name) && operation.merchant.region != null)
                            {
                                if (moscow_cities.Contains(operation.merchant.region.city))
                                {
                                    company.City = "Moscow";
                                }
                                else
                                {
                                    company.City = operation.merchant.region.city;
                                }

                                if (countries.Contains(operation.merchant.region.country))
                                {
                                    company.Country = "Russia";
                                }
                                else
                                {
                                    company.Country = operation.merchant.region.country;
                                }
                            }

                        }
                        if (!string.IsNullOrEmpty(company.Name))
                        {
                            repository.Create(company, true);
                            companies.Add(company);
                        }
                    }
                    #endregion


                    var mccCode = new MccCode();
                    var category = categories.FirstOrDefault(x => x.Name == operation.category.name);

                    if (category == null)
                    {
                        category = new MccCategory { MccCodes = new List<MccCode>() };
                        category.BankID = bankID;
                        category.Name = operation.category.name;
                        if (int.TryParse(operation.category.id, out int bankCategoryID))
                        {
                            category.bankCategoryID = bankCategoryID;
                        }

                        category.IsSystem = (new string[] { "Переводы/иб", "Перевод между счетами", "Бонусы", "Проценты", "Пополнение вклада" }).Contains(operation.category.name);

                        mccCode = new MccCode
                        {
                            //CompanyID = company.ID == 0 ? (int?)null : company.ID,
                            Mcc = operation.mcc
                        };

                        if (operation.spendingCategory != null
                            && operation.spendingCategory.parentId != null
                            && int.TryParse(operation.spendingCategory.parentId, out int bankParentCategoryID))
                        {
                            category.bankParentCategoryID = bankParentCategoryID;
                        }

                        category.MccCodes.Add(mccCode);

                        repository.Create(category, true);
                        categories.Add(category);
                    }

                    if (mccCode.ID == 0 && !category.MccCodes.Any(x => x.Mcc == operation.mcc))
                    {
                        category.MccCodes.Add(new MccCode
                        {
                            //CompanyID = company.ID == 0 ? (int?)null : company.ID,
                            Mcc = operation.mcc
                        });
                        repository.Update(category, true);

                    }

                }
            }
        }

        /// <summary>
        /// https://beautifytools.com/excel-to-json-converter.php
        /// </summary>
        private void UpdateCompanies()
        {
            var now = DateTime.Now.ToUniversalTime();

            List<Company> companies = repository.GetAll<Company>().ToList();

            using (StreamReader reader = new StreamReader(hostingEnvironment.WebRootPath + @"\\json\\companies.json"))
            {
                List<Company> file_companies = JsonConvert.DeserializeObject<List<Company>>(reader.ReadToEnd());

                foreach (var file_company in file_companies)
                {
                    var company = companies.FirstOrDefault(x => x.ID == file_company.ID);

                    if (company != null)
                    {
                        company.TagKeyWords = file_company.TagKeyWords;
                        company.BankKeyWords = file_company.BankKeyWords;
                        company.Site = file_company.Site;
                        company.IsChecked = file_company.IsChecked;
                        company.Name = file_company.Name;

                        repository.Update(company, true);
                    }
                }
            }
        }

        private void LinkUserTagsAndCompanies()
        {
            var companies = repository.GetAll<Company>()
                .Select(x => new
                {
                    TagKeyWords = x.TagKeyWords.Replace(" ", ""),
                    CopmanyID = x.ID
                })
                .ToList();

            var userTags = repository.GetAll<UserTag>(x => x.IsDeleted != true && x.CompanyID == null).ToList();

            foreach (var tag in userTags)
            {
                var tagName = tag.Title.ToLower();

                foreach (var company in companies)
                {
                    var tagkeyWords = company.TagKeyWords.Split("|").ToList();
                    tagkeyWords = tagkeyWords.Select(x => x.ToLower()).ToList();

                    if (tagkeyWords.Contains(tagName))
                    {
                        tag.CompanyID = company.CopmanyID;
                        goto Exit;
                    }
                }
            Exit:
                continue;

            }
            repository.Save();

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

            if (!db_EntityType.Any(x => x.ID == (int)BudgettoEntityType.Templates))
            {
                entityTypes.Add(new EntityType
                {
                    CodeName = Enum.GetName(typeof(BudgettoEntityType), BudgettoEntityType.Templates)
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


            if (!db_PaymentCounter.Any(x => x.EntityTypeID == (int)BudgettoEntityType.Limits && x.PaymentTariffID == (int)PaymentTariffTypes.Free))
            {
                paymentCounters.Add(new PaymentCounter
                {
                    PaymentTariffID = (int)PaymentTariffTypes.Free,
                    EntityTypeID = (int)BudgettoEntityType.Limits,
                    CanBeCount = 1,
                    LastChanges = now,
                });
            }

            if (!db_PaymentCounter.Any(x => x.EntityTypeID == (int)BudgettoEntityType.Reminders && x.PaymentTariffID == (int)PaymentTariffTypes.Free))
            {
                paymentCounters.Add(new PaymentCounter
                {
                    PaymentTariffID = (int)PaymentTariffTypes.Free,
                    EntityTypeID = (int)BudgettoEntityType.Reminders,
                    CanBeCount = 1,
                    LastChanges = now,
                });
            }

            if (!db_PaymentCounter.Any(x => x.EntityTypeID == (int)BudgettoEntityType.ToDoLists && x.PaymentTariffID == (int)PaymentTariffTypes.Free))
            {
                paymentCounters.Add(new PaymentCounter
                {
                    PaymentTariffID = (int)PaymentTariffTypes.Free,
                    EntityTypeID = (int)BudgettoEntityType.ToDoLists,
                    CanBeCount = 1,
                    LastChanges = now,
                });
            }

            if (!db_PaymentCounter.Any(x => x.EntityTypeID == (int)BudgettoEntityType.Templates && x.PaymentTariffID == (int)PaymentTariffTypes.Free))
            {
                paymentCounters.Add(new PaymentCounter
                {
                    PaymentTariffID = (int)PaymentTariffTypes.Free,
                    EntityTypeID = (int)BudgettoEntityType.Templates,
                    CanBeCount = 2,
                    LastChanges = now,
                });
            }

            if (!db_PaymentCounter.Any(x => x.EntityTypeID == (int)BudgettoEntityType.Limits && x.PaymentTariffID == (int)PaymentTariffTypes.Standard))
            {
                paymentCounters.Add(new PaymentCounter
                {
                    PaymentTariffID = (int)PaymentTariffTypes.Standard,
                    EntityTypeID = (int)BudgettoEntityType.Limits,
                    CanBeCount = 3,
                    LastChanges = now,
                });
            }

            if (!db_PaymentCounter.Any(x => x.EntityTypeID == (int)BudgettoEntityType.Reminders && x.PaymentTariffID == (int)PaymentTariffTypes.Standard))
            {
                paymentCounters.Add(new PaymentCounter
                {
                    PaymentTariffID = (int)PaymentTariffTypes.Standard,
                    EntityTypeID = (int)BudgettoEntityType.Reminders,
                    CanBeCount = 3,
                    LastChanges = now,
                });
            }

            if (!db_PaymentCounter.Any(x => x.EntityTypeID == (int)BudgettoEntityType.ToDoLists && x.PaymentTariffID == (int)PaymentTariffTypes.Standard))
            {
                paymentCounters.Add(new PaymentCounter
                {
                    PaymentTariffID = (int)PaymentTariffTypes.Standard,
                    EntityTypeID = (int)BudgettoEntityType.ToDoLists,
                    CanBeCount = 3,
                    LastChanges = now,
                });
            }

            if (!db_PaymentCounter.Any(x => x.EntityTypeID == (int)BudgettoEntityType.Templates && x.PaymentTariffID == (int)PaymentTariffTypes.Standard))
            {
                paymentCounters.Add(new PaymentCounter
                {
                    PaymentTariffID = (int)PaymentTariffTypes.Standard,
                    EntityTypeID = (int)BudgettoEntityType.Templates,
                    CanBeCount = 3,
                    LastChanges = now,
                });
            }

            if (!db_PaymentCounter.Any(x => x.EntityTypeID == (int)BudgettoEntityType.Limits && x.PaymentTariffID == (int)PaymentTariffTypes.Freedom))
            {
                paymentCounters.Add(new PaymentCounter
                {
                    PaymentTariffID = (int)PaymentTariffTypes.Freedom,
                    EntityTypeID = (int)BudgettoEntityType.Limits,
                    CanBeCount = 99,
                    LastChanges = now,
                });
            }

            if (!db_PaymentCounter.Any(x => x.EntityTypeID == (int)BudgettoEntityType.Reminders && x.PaymentTariffID == (int)PaymentTariffTypes.Freedom))
            {
                paymentCounters.Add(new PaymentCounter
                {
                    PaymentTariffID = (int)PaymentTariffTypes.Freedom,
                    EntityTypeID = (int)BudgettoEntityType.Reminders,
                    CanBeCount = 99,
                    LastChanges = now,
                });
            }

            if (!db_PaymentCounter.Any(x => x.EntityTypeID == (int)BudgettoEntityType.ToDoLists && x.PaymentTariffID == (int)PaymentTariffTypes.Freedom))
            {
                paymentCounters.Add(new PaymentCounter
                {
                    PaymentTariffID = (int)PaymentTariffTypes.Freedom,
                    EntityTypeID = (int)BudgettoEntityType.ToDoLists,
                    CanBeCount = 99,
                    LastChanges = now,
                });
            }

            if (!db_PaymentCounter.Any(x => x.EntityTypeID == (int)BudgettoEntityType.Templates && x.PaymentTariffID == (int)PaymentTariffTypes.Freedom))
            {
                paymentCounters.Add(new PaymentCounter
                {
                    PaymentTariffID = (int)PaymentTariffTypes.Freedom,
                    EntityTypeID = (int)BudgettoEntityType.Templates,
                    CanBeCount = 99,
                    LastChanges = now,
                });
            }

            if (!db_PaymentCounter.Any(x => x.EntityTypeID == (int)BudgettoEntityType.Limits && x.PaymentTariffID == (int)PaymentTariffTypes.Premium))
            {
                paymentCounters.Add(new PaymentCounter
                {
                    PaymentTariffID = (int)PaymentTariffTypes.Premium,
                    EntityTypeID = (int)BudgettoEntityType.Limits,
                    CanBeCount = 5,
                    LastChanges = now,
                });
            }

            if (!db_PaymentCounter.Any(x => x.EntityTypeID == (int)BudgettoEntityType.Reminders && x.PaymentTariffID == (int)PaymentTariffTypes.Premium))
            {
                paymentCounters.Add(new PaymentCounter
                {
                    PaymentTariffID = (int)PaymentTariffTypes.Premium,
                    EntityTypeID = (int)BudgettoEntityType.Reminders,
                    CanBeCount = 5,
                    LastChanges = now,
                });
            }

            if (!db_PaymentCounter.Any(x => x.EntityTypeID == (int)BudgettoEntityType.ToDoLists && x.PaymentTariffID == (int)PaymentTariffTypes.Premium))
            {
                paymentCounters.Add(new PaymentCounter
                {
                    PaymentTariffID = (int)PaymentTariffTypes.Premium,
                    EntityTypeID = (int)BudgettoEntityType.ToDoLists,
                    CanBeCount = 5,
                    LastChanges = now,
                });
            }

            if (!db_PaymentCounter.Any(x => x.EntityTypeID == (int)BudgettoEntityType.Templates && x.PaymentTariffID == (int)PaymentTariffTypes.Premium))
            {
                paymentCounters.Add(new PaymentCounter
                {
                    PaymentTariffID = (int)PaymentTariffTypes.Premium,
                    EntityTypeID = (int)BudgettoEntityType.Templates,
                    CanBeCount = 5,
                    LastChanges = now,
                });
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
                //Payment = new MyProfile.Entity.Model.Payment
                //{
                //    DateFrom = now,
                //    DateTo = now.AddYears(10),
                //    //DateTo = now.AddMonths(2),
                //   // IsPaid = false,
                //    PaymentTariffID = (int)PaymentTariffTypes.Freedom,
                //},
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


        public class Subgroup
        {
            public string id { get; set; }
            public string name { get; set; }
        }

        public class Location
        {
            public double latitude { get; set; }
            public double longitude { get; set; }
        }

        public class Amount
        {
            public string value { get; set; }
            public string loyaltyProgramId { get; set; }
            public string loyalty { get; set; }
            public string name { get; set; }
            public string loyaltySteps { get; set; }
            public string loyaltyPointsId { get; set; }
            public string loyaltyPointsName { get; set; }
            public bool loyaltyImagine { get; set; }
            public bool partialCompensation { get; set; }
            public Currency currency { get; set; }
        }

        public class LoyaltyBonu
        {
            public string loyaltyType { get; set; }
            public Amount amount { get; set; }
        }

        public class Currency
        {
            public int code { get; set; }
            public string name { get; set; }
            public string strCode { get; set; }
        }

        public class CashbackAmount
        {
            public Currency currency { get; set; }
            public int value { get; set; }
        }

        public class DebitingTime
        {
            public object milliseconds { get; set; }
        }

        public class OperationTime
        {
            public object milliseconds { get; set; }
        }

        public class SpendingCategory
        {
            public string id { get; set; }
            public string name { get; set; }
            public string icon { get; set; }
            public string parentId { get; set; }
        }

        public class Category
        {
            public string id { get; set; }
            public string name { get; set; }
        }

        public class Region
        {
            public string country { get; set; }
            public string city { get; set; }
            public string address { get; set; }
            public string zip { get; set; }
        }

        public class Merchant
        {
            public string name { get; set; }
            public Region region { get; set; }
        }

        public class AccountAmount
        {
            public Currency currency { get; set; }
            public double value { get; set; }
        }

        public class Brand
        {
            public string name { get; set; }
            public string baseTextColor { get; set; }
            public string logo { get; set; }
            public string id { get; set; }
            public bool roundedLogo { get; set; }
            public string baseColor { get; set; }
            public string logoFile { get; set; }
            public string link { get; set; }
        }

        public class FeeAmount
        {
            public Currency currency { get; set; }
            public string value { get; set; }
        }

        public class FieldsValues
        {
            public string date { get; set; }
            public string igBillId { get; set; }
            public string account { get; set; }
            public string period { get; set; }
            public string payerCode { get; set; }
            public string bankContract { get; set; }
        }

        public class Payment
        {
            public bool sourceIsQr { get; set; }
            public string bankAccountId { get; set; }
            public string paymentId { get; set; }
            public string providerGroupId { get; set; }
            public string paymentType { get; set; }
            public FeeAmount feeAmount { get; set; }
            public string providerId { get; set; }
            public bool hasPaymentOrder { get; set; }
            public string comment { get; set; }
            public FieldsValues fieldsValues { get; set; }
            public bool repeatable { get; set; }
            public string cardNumber { get; set; }
            public string templateId { get; set; }
            public bool templateIsFavorite { get; set; }
        }

        public class TinkoffOperation
        {
            public bool isDispute { get; set; }
            public bool isOffline { get; set; }
            public bool hasStatement { get; set; }
            public bool isSuspicious { get; set; }
            public string authorizationId { get; set; }
            public string id { get; set; }
            public string status { get; set; }
            public bool operationTransferred { get; set; }
            public string idSourceType { get; set; }
            public string type { get; set; }
            public bool trancheCreationAllowed { get; set; }
            public Subgroup subgroup { get; set; }
            public List<Location> locations { get; set; }
            public List<LoyaltyBonu> loyaltyBonus { get; set; }
            public CashbackAmount cashbackAmount { get; set; }
            public string description { get; set; }
            public DebitingTime debitingTime { get; set; }
            public int cashback { get; set; }
            public Amount amount { get; set; }
            public OperationTime operationTime { get; set; }
            public SpendingCategory spendingCategory { get; set; }
            public List<object> offers { get; set; }
            public bool isHce { get; set; }
            public int mcc { get; set; }
            public Category category { get; set; }
            public List<object> additionalInfo { get; set; }
            public int virtualPaymentType { get; set; }
            public string account { get; set; }
            public string ucid { get; set; }
            public Merchant merchant { get; set; }
            public string card { get; set; }
            public List<object> loyaltyPayment { get; set; }
            public string group { get; set; }
            public string mccString { get; set; }
            public bool cardPresent { get; set; }
            public bool isExternalCard { get; set; }
            public string cardNumber { get; set; }
            public AccountAmount accountAmount { get; set; }
            public Brand brand { get; set; }
            public Payment payment { get; set; }
            public string operationPaymentType { get; set; }
            public string subcategory { get; set; }
            public string installmentStatus { get; set; }
            public string senderAgreement { get; set; }
            public bool? hasShoppingReceipt { get; set; }
            public string compensation { get; set; }
        }
    }
}
