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

            CardJsonToCard();
            //CardToJson();
            //CreditRCardsLoading();
            //DebitCardsLoading();
            //BanksLoading();
            //CreateParentAccount();
            //CreateTelegramBotAccount();
            //CreateTelegramAccountStatus();
            // LoadTimeZone();
        }

        private void CardJsonToCard()
        {
            //if (repository.GetAll<Card>().Any() == false)
            {
                using (StreamReader reader = new StreamReader(hostingEnvironment.WebRootPath + @"\\json\\CardToProd.json"))
                {
                    List<Card> cards = (List<Card>)JsonConvert.DeserializeObject<List<Card>>(reader.ReadToEnd());

                    //repository.Create(cards, true);
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
    }
}
