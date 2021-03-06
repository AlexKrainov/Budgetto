using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MyProfile.Common;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView.Currency;
using MyProfile.Entity.Repository;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Common.Service.Curency
{
    public class GetCurrency
    {
        private IBaseRepository repository;
        private IMemoryCache cache;
        private List<CurrencyClientModelView> currenciesDB;

        public GetCurrency(IBaseRepository repository,
            IMemoryCache cache,
            List<CurrencyClientModelView> currencies)
        {
            this.repository = repository;
            this.cache = cache;
            this.currenciesDB = currencies;
        }

        public List<CurrencyRateHistory> GetRatesByDate(DateTime date, Guid? userSessionID = null)
        {
            Dictionary<DateTime, List<CurrencyRateHistory>> currencies = new Dictionary<DateTime, List<CurrencyRateHistory>>();
            List<CurrencyRateHistory> bankCurrencyData = null;

            bankCurrencyData = CheckCache(currencies, date);

            try
            {
                if (bankCurrencyData == null)
                {
                    bankCurrencyData = CheckDataBase(currencies, date);
                }
            }
            catch (Exception ex)
            {
                ErrorLog log = new ErrorLog
                {
                    CurrentDate = DateTime.Now.ToUniversalTime(),
                    Comment = "CheckDataBase.bankCurrencyData = " + bankCurrencyData == null ? "null" : bankCurrencyData.ToString(),
                    ErrorText = ExceptionWorker.GetExceptionMessages(ex),
                    Where = "CurrancyService.CheckCBRxmlDailySite",
                    UserSessionID = userSessionID,
                };
                repository.Create(log, true);
            }

            try
            {
                if (bankCurrencyData == null)
                {
                    bankCurrencyData = CheckCBRxmlDailySite(currencies, date, userSessionID);
                }
            }
            catch (Exception ex)
            {
                ErrorLog log = new ErrorLog
                {
                    CurrentDate = DateTime.Now.ToUniversalTime(),
                    Comment = "GetRatesFromBank",
                    ErrorText = ExceptionWorker.GetExceptionMessages(ex),
                    Where = "CurrancyService.CheckCBRxmlDailySite",
                    UserSessionID = userSessionID,
                };
                repository.Create(log, true);
            }

            try
            {
                if (bankCurrencyData == null)
                {
                    bankCurrencyData = CheckCBR_Site(currencies, date, userSessionID);
                }
            }
            catch (Exception ex)
            {
                ErrorLog log = new ErrorLog
                {
                    CurrentDate = DateTime.Now.ToUniversalTime(),
                    Comment = "CheckCBR_Site",
                    ErrorText = ExceptionWorker.GetExceptionMessages(ex),
                    Where = "CurrancyService.CheckCBRxmlDailySite",
                    UserSessionID = userSessionID,
                };
                repository.Create(log, true);
            }

            return bankCurrencyData;
        }

        public CurrencyRateHistory GetRateByCode(DateTime date, string charCode, Guid? userSessionID = null)
        {
            Dictionary<DateTime, List<CurrencyRateHistory>> currencies = new Dictionary<DateTime, List<CurrencyRateHistory>>();
            CurrencyRateHistory bankCurrencyData = null;

            bankCurrencyData = CheckCache(currencies, date, charCode);

            try
            {
                if (bankCurrencyData == null)
                {
                    bankCurrencyData = CheckDataBase(currencies, date, charCode);
                }
            }
            catch (Exception ex)
            {
                ErrorLog log = new ErrorLog
                {
                    CurrentDate = DateTime.Now.ToUniversalTime(),
                    Comment = "CheckDataBase.bankCurrencyData = " + bankCurrencyData == null ? "null" : bankCurrencyData.ToString(),
                    ErrorText = ExceptionWorker.GetExceptionMessages(ex),
                    Where = "CurrancyService.CheckCBRxmlDailySite",
                    UserSessionID = userSessionID,
                };
                repository.Create(log, true);
            }

            try
            {
                if (bankCurrencyData == null)
                {
                    bankCurrencyData = CheckCBRxmlDailySite(currencies, date, charCode, userSessionID);
                }
            }
            catch (Exception ex)
            {
                ErrorLog log = new ErrorLog
                {
                    CurrentDate = DateTime.Now.ToUniversalTime(),
                    Comment = "GetRatesFromBank",
                    ErrorText = ExceptionWorker.GetExceptionMessages(ex),
                    Where = "CurrancyService.CheckCBRxmlDailySite",
                    UserSessionID = userSessionID,
                };
                repository.Create(log, true);
            }

            try
            {
                if (bankCurrencyData == null)
                {
                    bankCurrencyData = CheckCBR_Site(currencies, date, charCode, userSessionID);
                }
            }
            catch (Exception ex)
            {
                ErrorLog log = new ErrorLog
                {
                    CurrentDate = DateTime.Now.ToUniversalTime(),
                    Comment = "CheckCBR_Site",
                    ErrorText = ExceptionWorker.GetExceptionMessages(ex),
                    Where = "CurrancyService.CheckCBRxmlDailySite",
                    UserSessionID = userSessionID,
                };
                repository.Create(log, true);
            }

            return bankCurrencyData;
        }

        public int HisotoryLoad()
        {
            Dictionary<DateTime, List<CurrencyRateHistory>> currencies = new Dictionary<DateTime, List<CurrencyRateHistory>>();
            CurrencyRateHistory bankCurrencyData = null;
            var dateSearch = DateTime.Now.AddDays(-2).Date;
            DateTime minDate = new DateTime(2010, 01, 01);

            //refresh -2 date rows
            repository.DeleteRange(repository.GetAll<CurrencyRateHistory>(x => x.Date == dateSearch).ToList(), true);

            var historyDates = repository.GetAll<CurrencyRateHistory>()
                .OrderBy(x => x.Date)
                .Select(x => x.Date)
                .GroupBy(x => x)
                .Select(x => x.Key)
                .ToList();

            while (historyDates.Any(x => x == dateSearch))
            {
                dateSearch = dateSearch.AddDays(-1);

                if (dateSearch < minDate)
                {
                    return 0;
                }
            }

            try
            {
                bankCurrencyData = CheckCBRxmlDailySite(currencies, dateSearch, "USD");
            }
            catch (Exception ex)
            {
                ErrorLog log = new ErrorLog
                {
                    CurrentDate = DateTime.Now.ToUniversalTime(),
                    Comment = "GetRatesFromBank",
                    ErrorText = ExceptionWorker.GetExceptionMessages(ex),
                    Where = "CurrancyService.HisotoryLoad",
                };
                repository.Create(log, true);
            }

            try
            {
                if (bankCurrencyData == null)
                {
                    bankCurrencyData = CheckCBR_Site(currencies, dateSearch, "USD");
                }
            }
            catch (Exception ex)
            {
                ErrorLog log = new ErrorLog
                {
                    CurrentDate = DateTime.Now.ToUniversalTime(),
                    Comment = "CheckCBR_Site",
                    ErrorText = ExceptionWorker.GetExceptionMessages(ex),
                    Where = "CurrancyService.HisotoryLoad",
                };
                repository.Create(log, true);
            }


            return 1;
        }

        private CurrencyRateHistory CheckCache(Dictionary<DateTime, List<CurrencyRateHistory>> currencies, DateTime date, string charCode)
        {
            if (cache.TryGetValue("List_BankCurrencyData", out currencies))
            {
                if (currencies.ContainsKey(date.Date))
                {
                    return currencies[date.Date].FirstOrDefault(x => x.CharCode == charCode);
                }
            }
            return null;
        }
        private List<CurrencyRateHistory> CheckCache(Dictionary<DateTime, List<CurrencyRateHistory>> currencies, DateTime date)
        {
            if (cache.TryGetValue("List_BankCurrencyData", out currencies))
            {
                if (currencies.ContainsKey(date.Date))
                {
                    return currencies[date.Date];
                }
            }
            return null;
        }

        private CurrencyRateHistory CheckDataBase(Dictionary<DateTime, List<CurrencyRateHistory>> currencies, DateTime date, string charCode)
        {
            if (repository.Any<CurrencyRateHistory>(x => x.Date.Date == date.Date && x.CharCode == charCode))
            {
                var currencyRateHistories = repository.GetAll<CurrencyRateHistory>(x => x.Date.Date == date.Date)
                           .Select(x => new CurrencyRateHistory
                           {
                               ID = x.ID,
                               CharCode = x.CharCode,
                               CodeName_CBR = x.CodeName_CBR,
                               CurrencyID = x.CurrencyID,
                               Date = x.Date,
                               Name = x.Name,
                               Nominal = x.Nominal,
                               NumCode = x.NumCode,
                               Rate = x.Rate
                           })
                           .ToList();

                if (currencyRateHistories != null && currencyRateHistories.Count != 0)
                {
                    if (currencies == null) { currencies = new Dictionary<DateTime, List<CurrencyRateHistory>>(); }

                    currencies.Add(date.Date, currencyRateHistories);
                    cache.Set("List_BankCurrencyData", currencies, DateTime.Now.AddDays(5));


                    return currencyRateHistories.FirstOrDefault(x => x.CharCode == charCode);
                }
            }
            return null;
        }

        private List<CurrencyRateHistory> CheckDataBase(Dictionary<DateTime, List<CurrencyRateHistory>> currencies, DateTime date)
        {
            if (repository.Any<CurrencyRateHistory>(x => x.Date.Date == date.Date ))
            {
                var currencyRateHistories = repository.GetAll<CurrencyRateHistory>(x => x.Date.Date == date.Date)
                           .Select(x => new CurrencyRateHistory
                           {
                               ID = x.ID,
                               CharCode = x.CharCode,
                               CodeName_CBR = x.CodeName_CBR,
                               CurrencyID = x.CurrencyID,
                               Date = x.Date,
                               Name = x.Name,
                               Nominal = x.Nominal,
                               NumCode = x.NumCode,
                               Rate = x.Rate
                           })
                           .ToList();

                if (currencyRateHistories != null && currencyRateHistories.Count != 0)
                {
                    if (currencies == null) { currencies = new Dictionary<DateTime, List<CurrencyRateHistory>>(); }

                    currencies.Add(date.Date, currencyRateHistories);
                    cache.Set("List_BankCurrencyData", currencies, DateTime.Now.AddDays(5));


                    return currencyRateHistories;
                }
            }
            return null;
        }

        private CurrencyRateHistory CheckCBRxmlDailySite(Dictionary<DateTime, List<CurrencyRateHistory>> currencies, DateTime date, string charCode, Guid? userSessionID = null)
        {
            string text = string.Empty;
            CurrencyRateHistory bankCurrencyData = null;
            List<CurrencyRateHistory> bankCurrencyDatas = new List<CurrencyRateHistory>();

            string _link = "https://www.cbr-xml-daily.ru/daily_json.js"; //get currencies today

            if (DateTime.Now.Date != date.Date)
            {
                _link = "https://www.cbr-xml-daily.ru/archive/" + date.Date.ToString("yyyy/MM/dd").Replace(".","/") + "/daily_json.js";
            }
            string s = string.Empty;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response1 = client.GetAsync(_link).Result;
                    //nesekmes atveju error..
                    response1.EnsureSuccessStatusCode();
                    s = response1.Content.ReadAsStringAsync().Result;
                }
            }
            catch (Exception ex)
            {
                ErrorLog log = new ErrorLog
                {
                    CurrentDate = DateTime.Now.ToUniversalTime(),
                    Comment = _link,
                    ErrorText = ExceptionWorker.GetExceptionMessages(ex),
                    Where = "CurrancyService.CheckCBRxmlDailySite",
                    UserSessionID = userSessionID,
                };
                repository.Create(log, true);
            }

            try
            {
                var doc = XElement.Load(new XmlNodeReader(JsonConvert.DeserializeXmlNode(s, "currancies")));

                foreach (XElement _element in doc.Element("Valute").Elements())
                {
                    var elements = _element.Elements();
                    string _codeName_CBR = elements.FirstOrDefault(x => x.Name == "ID").Value;
                    string numCode = elements.FirstOrDefault(x => x.Name == "NumCode").Value;
                    string _charCode = elements.FirstOrDefault(x => x.Name == "CharCode").Value;
                    string name = elements.FirstOrDefault(x => x.Name == "Name").Value;
                    string elementPrice = elements.FirstOrDefault(x => x.Name == "Value").Value;
                    string elementNominal = elements.FirstOrDefault(x => x.Name == "Nominal").Value;

                    if (!string.IsNullOrEmpty(numCode)
                        && !string.IsNullOrEmpty(_charCode)
                        && !string.IsNullOrEmpty(name)
                        && !string.IsNullOrEmpty(elementPrice)
                        && !string.IsNullOrEmpty(elementNominal))
                    {
                        int _currencyID = -1;

                        if (currenciesDB.Any(x => x.codeName_CBR == _codeName_CBR))
                        {
                            _currencyID = currenciesDB.FirstOrDefault(x => x.codeName_CBR == _codeName_CBR).id;
                        }

                        var _bankCurrencyData = new CurrencyRateHistory
                        {
                            CurrencyID = _currencyID,
                            CharCode = _charCode,
                            Date = date.Date,
                            Name = name,
                            Nominal = int.Parse(elementNominal),
                            Rate = decimal.Parse(elementPrice.Replace(",", "."), CultureInfo.InvariantCulture),
                            NumCode = numCode,
                            CodeName_CBR = _codeName_CBR
                        };

                        if (_charCode == charCode)
                        {
                            bankCurrencyData = _bankCurrencyData;
                        }
                        bankCurrencyDatas.Add(_bankCurrencyData);
                    }
                }

                var dbHistories = repository.GetAll<CurrencyRateHistory>(x => x.Date.Date == date.Date)
                    .ToList();

                if (dbHistories.Count == 0)
                {
                    repository.CreateRange(bankCurrencyDatas);
                }
                else
                {
                    foreach (var currencyRate in bankCurrencyDatas)
                    {
                        var rateHistory = dbHistories.FirstOrDefault(x => x.CharCode == currencyRate.CharCode);
                        if (rateHistory != null)
                        {
                            rateHistory.Rate = currencyRate.Rate;
                        }
                        else
                        {
                            repository.Create(currencyRate);
                        }
                    }
                }
                repository.Save();

                if (currencies == null) { currencies = new Dictionary<DateTime, List<CurrencyRateHistory>>(); }

                if (currencies.ContainsKey(date.Date))
                {
                    currencies[date.Date] = bankCurrencyDatas;
                }
                else
                {
                    currencies.Add(date.Date, bankCurrencyDatas);
                }

                cache.Set("List_BankCurrencyData", currencies, DateTime.Now.AddDays(5));

            }
            catch (Exception ex)
            {
                ErrorLog log = new ErrorLog
                {
                    CurrentDate = DateTime.Now.ToUniversalTime(),
                    Comment = "Parse error",
                    ErrorText = ExceptionWorker.GetExceptionMessages(ex),
                    Where = "CurrancyService.CheckCBRxmlDailySite",
                    UserSessionID = userSessionID,
                };

                repository.Create(log, true);
            }
            return bankCurrencyData;
        }

        private List<CurrencyRateHistory> CheckCBRxmlDailySite(Dictionary<DateTime, List<CurrencyRateHistory>> currencies, DateTime date, Guid? userSessionID = null)
        {
            string text = string.Empty;
            List<CurrencyRateHistory> bankCurrencyDatas = new List<CurrencyRateHistory>();

            string _link = "https://www.cbr-xml-daily.ru/daily_json.js"; //get currencies today

            if (DateTime.Now.Date != date.Date)
            {
                _link = "https://www.cbr-xml-daily.ru/archive/" + date.Date.ToString("yyyy/MM/dd").Replace(".", "/") + "/daily_json.js";
            }
            string s = string.Empty;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response1 = client.GetAsync(_link).Result;
                    //nesekmes atveju error..
                    response1.EnsureSuccessStatusCode();
                    s = response1.Content.ReadAsStringAsync().Result;
                }
            }
            catch (Exception ex)
            {
                ErrorLog log = new ErrorLog
                {
                    CurrentDate = DateTime.Now.ToUniversalTime(),
                    Comment = _link,
                    ErrorText = ExceptionWorker.GetExceptionMessages(ex),
                    Where = "CurrancyService.CheckCBRxmlDailySite",
                    UserSessionID = userSessionID,
                };
                repository.Create(log, true);
            }

            try
            {
                var doc = XElement.Load(new XmlNodeReader(JsonConvert.DeserializeXmlNode(s, "currancies")));

                foreach (XElement _element in doc.Element("Valute").Elements())
                {
                    var elements = _element.Elements();
                    string _codeName_CBR = elements.FirstOrDefault(x => x.Name == "ID").Value;
                    string numCode = elements.FirstOrDefault(x => x.Name == "NumCode").Value;
                    string _charCode = elements.FirstOrDefault(x => x.Name == "CharCode").Value;
                    string name = elements.FirstOrDefault(x => x.Name == "Name").Value;
                    string elementPrice = elements.FirstOrDefault(x => x.Name == "Value").Value;
                    string elementNominal = elements.FirstOrDefault(x => x.Name == "Nominal").Value;

                    if (!string.IsNullOrEmpty(numCode)
                        && !string.IsNullOrEmpty(_charCode)
                        && !string.IsNullOrEmpty(name)
                        && !string.IsNullOrEmpty(elementPrice)
                        && !string.IsNullOrEmpty(elementNominal))
                    {
                        int _currencyID = -1;

                        if (currenciesDB.Any(x => x.codeName_CBR == _codeName_CBR))
                        {
                            _currencyID = currenciesDB.FirstOrDefault(x => x.codeName_CBR == _codeName_CBR).id;
                        }

                        var _bankCurrencyData = new CurrencyRateHistory
                        {
                            CurrencyID = _currencyID,
                            CharCode = _charCode,
                            Date = date.Date,
                            Name = name,
                            Nominal = int.Parse(elementNominal),
                            Rate = decimal.Parse(elementPrice.Replace(",", "."), CultureInfo.InvariantCulture),
                            NumCode = numCode,
                            CodeName_CBR = _codeName_CBR
                        };

                        bankCurrencyDatas.Add(_bankCurrencyData);
                    }
                }

                var dbHistories = repository.GetAll<CurrencyRateHistory>(x => x.Date.Date == date.Date)
                    .ToList();

                if (dbHistories.Count == 0)
                {
                    repository.CreateRange(bankCurrencyDatas);
                }
                else
                {
                    foreach (var currencyRate in bankCurrencyDatas)
                    {
                        var rateHistory = dbHistories.FirstOrDefault(x => x.CharCode == currencyRate.CharCode);
                        if (rateHistory != null)
                        {
                            rateHistory.Rate = currencyRate.Rate;
                        }
                        else
                        {
                            repository.Create(currencyRate);
                        }
                    }
                }
                repository.Save();

                if (currencies == null) { currencies = new Dictionary<DateTime, List<CurrencyRateHistory>>(); }

                if (currencies.ContainsKey(date.Date))
                {
                    currencies[date.Date] = bankCurrencyDatas;
                }
                else
                {
                    currencies.Add(date.Date, bankCurrencyDatas);
                }

                cache.Set("List_BankCurrencyData", currencies, DateTime.Now.AddDays(5));

            }
            catch (Exception ex)
            {
                ErrorLog log = new ErrorLog
                {
                    CurrentDate = DateTime.Now.ToUniversalTime(),
                    Comment = "Parse error",
                    ErrorText = ExceptionWorker.GetExceptionMessages(ex),
                    Where = "CurrancyService.CheckCBRxmlDailySite",
                    UserSessionID = userSessionID,
                };

                repository.Create(log, true);
            }
            return bankCurrencyDatas;
        }


        private CurrencyRateHistory CheckCBR_Site(Dictionary<DateTime, List<CurrencyRateHistory>> currencies, DateTime date, string charCode, Guid? userSessionID = null)
        {
            string text = string.Empty;
            string codeName_CBR = string.Empty;
            CurrencyRateHistory bankCurrencyData = null;
            List<CurrencyRateHistory> bankCurrencyDatas = new List<CurrencyRateHistory>();

            var _link = "http://www.cbr.ru/scripts/XML_daily.asp?date_req=" + date.ToString("dd/MM/yyyy");

            try
            {
                Stream stream = null;
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        HttpResponseMessage response1 = client.GetAsync(_link).Result;
                        response1.EnsureSuccessStatusCode();
                        stream = response1.Content.ReadAsStreamAsync().Result;
                    }
                }
                catch (Exception ex)
                {
                    ErrorLog log = new ErrorLog
                    {
                        CurrentDate = DateTime.Now.ToUniversalTime(),
                        Comment = _link,
                        ErrorText = ExceptionWorker.GetExceptionMessages(ex),
                        Where = "CurrancyService.CheckCBR_Site",
                        UserSessionID = userSessionID,
                    };
                    repository.Create(log, true);
                }
                var doc = XElement.Load(stream);

                foreach (XElement element in doc.Elements("Valute"))
                {
                    var _codeName_CBR = element.Attribute("ID").Value;
                    var numCode = element.Element("NumCode");
                    var _charCode = element.Element("CharCode");
                    var name = element.Element("Name");
                    var elementPrice = element.Element("Value");
                    var elementNominal = element.Element("Nominal");

                    if (!string.IsNullOrEmpty(numCode.Value)
                        && !string.IsNullOrEmpty(_charCode.Value)
                        && !string.IsNullOrEmpty(name.Value)
                        && !string.IsNullOrEmpty(elementPrice.Value)
                        && !string.IsNullOrEmpty(elementNominal.Value))
                    {
                        int _currencyID = -1;

                        if (currenciesDB.Any(x => x.codeName == _charCode.Value))
                        {
                            _currencyID = currenciesDB.FirstOrDefault(x => x.codeName == _charCode.Value).id;
                        }

                        var _bankCurrencyData = new CurrencyRateHistory
                        {
                            CurrencyID = _currencyID,
                            CharCode = _charCode.Value,
                            Date = date.Date,
                            Name = name.Value,
                            Nominal = int.Parse(elementNominal.Value),
                            Rate = decimal.Parse(elementPrice.Value.Replace(",", "."), CultureInfo.InvariantCulture),
                            NumCode = numCode.Value,
                            CodeName_CBR = _codeName_CBR
                        };

                        if (_bankCurrencyData.CharCode == charCode)
                        {
                            bankCurrencyData = _bankCurrencyData;
                        }
                        bankCurrencyDatas.Add(_bankCurrencyData);
                    }
                }

                var dbHistories = repository.GetAll<CurrencyRateHistory>(x => x.Date.Date == date.Date)
                    .ToList();

                if (dbHistories.Count == 0)
                {
                    repository.CreateRange(bankCurrencyDatas);
                }
                else
                {
                    foreach (var currencyRate in bankCurrencyDatas)
                    {
                        var rateHistory = dbHistories.FirstOrDefault(x => x.CodeName_CBR == currencyRate.CodeName_CBR);
                        if (rateHistory != null)
                        {
                            rateHistory.Rate = currencyRate.Rate;
                        }
                        else
                        {
                            repository.Create(currencyRate);
                        }
                    }
                }

                repository.Save();

                if (currencies == null) { currencies = new Dictionary<DateTime, List<CurrencyRateHistory>>(); }

                if (currencies.ContainsKey(date.Date))
                {
                    currencies[date.Date] = bankCurrencyDatas;
                }
                else
                {
                    currencies.Add(date.Date, bankCurrencyDatas);
                }

                cache.Set("List_BankCurrencyData", currencies, DateTime.Now.AddDays(5));
            }
            catch (Exception ex)
            {
                repository.Create(new ErrorLog
                {
                    CurrentDate = DateTime.Now.ToUniversalTime(),
                    Comment = "Parse Error",
                    ErrorText = ExceptionWorker.GetExceptionMessages(ex),
                    Where = "CurrancyService.CheckCBR_Site",
                    UserSessionID = userSessionID,
                }, true);
            }

            return bankCurrencyData;
        }

        private List<CurrencyRateHistory> CheckCBR_Site(Dictionary<DateTime, List<CurrencyRateHistory>> currencies, DateTime date, Guid? userSessionID = null)
        {
            string text = string.Empty;
            string codeName_CBR = string.Empty;
            List<CurrencyRateHistory> bankCurrencyDatas = new List<CurrencyRateHistory>();

            var _link = "http://www.cbr.ru/scripts/XML_daily.asp?date_req=" + date.ToString("dd/MM/yyyy");

            try
            {
                Stream stream = null;
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        HttpResponseMessage response1 = client.GetAsync(_link).Result;
                        response1.EnsureSuccessStatusCode();
                        stream = response1.Content.ReadAsStreamAsync().Result;
                    }
                }
                catch (Exception ex)
                {
                    ErrorLog log = new ErrorLog
                    {
                        CurrentDate = DateTime.Now.ToUniversalTime(),
                        Comment = _link,
                        ErrorText = ExceptionWorker.GetExceptionMessages(ex),
                        Where = "CurrancyService.CheckCBR_Site",
                        UserSessionID = userSessionID,
                    };
                    repository.Create(log, true);
                }
                var doc = XElement.Load(stream);

                foreach (XElement element in doc.Elements("Valute"))
                {
                    var _codeName_CBR = element.Attribute("ID").Value;
                    var numCode = element.Element("NumCode");
                    var _charCode = element.Element("CharCode");
                    var name = element.Element("Name");
                    var elementPrice = element.Element("Value");
                    var elementNominal = element.Element("Nominal");

                    if (!string.IsNullOrEmpty(numCode.Value)
                        && !string.IsNullOrEmpty(_charCode.Value)
                        && !string.IsNullOrEmpty(name.Value)
                        && !string.IsNullOrEmpty(elementPrice.Value)
                        && !string.IsNullOrEmpty(elementNominal.Value))
                    {
                        int _currencyID = -1;

                        if (currenciesDB.Any(x => x.codeName == _charCode.Value))
                        {
                            _currencyID = currenciesDB.FirstOrDefault(x => x.codeName == _charCode.Value).id;
                        }

                        var _bankCurrencyData = new CurrencyRateHistory
                        {
                            CurrencyID = _currencyID,
                            CharCode = _charCode.Value,
                            Date = date.Date,
                            Name = name.Value,
                            Nominal = int.Parse(elementNominal.Value),
                            Rate = decimal.Parse(elementPrice.Value.Replace(",", "."), CultureInfo.InvariantCulture),
                            NumCode = numCode.Value,
                            CodeName_CBR = _codeName_CBR
                        };

                        bankCurrencyDatas.Add(_bankCurrencyData);
                    }
                }

                var dbHistories = repository.GetAll<CurrencyRateHistory>(x => x.Date.Date == date.Date)
                    .ToList();

                if (dbHistories.Count == 0)
                {
                    repository.CreateRange(bankCurrencyDatas);
                }
                else
                {
                    foreach (var currencyRate in bankCurrencyDatas)
                    {
                        var rateHistory = dbHistories.FirstOrDefault(x => x.CodeName_CBR == currencyRate.CodeName_CBR);
                        if (rateHistory != null)
                        {
                            rateHistory.Rate = currencyRate.Rate;
                        }
                        else
                        {
                            repository.Create(currencyRate);
                        }
                    }
                }

                repository.Save();

                if (currencies == null) { currencies = new Dictionary<DateTime, List<CurrencyRateHistory>>(); }

                if (currencies.ContainsKey(date.Date))
                {
                    currencies[date.Date] = bankCurrencyDatas;
                }
                else
                {
                    currencies.Add(date.Date, bankCurrencyDatas);
                }

                cache.Set("List_BankCurrencyData", currencies, DateTime.Now.AddDays(5));
            }
            catch (Exception ex)
            {
                repository.Create(new ErrorLog
                {
                    CurrentDate = DateTime.Now.ToUniversalTime(),
                    Comment = "Parse Error",
                    ErrorText = ExceptionWorker.GetExceptionMessages(ex),
                    Where = "CurrancyService.CheckCBR_Site",
                    UserSessionID = userSessionID,
                }, true);
            }

            return bankCurrencyDatas;
        }
    }
}
