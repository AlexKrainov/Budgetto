using Microsoft.Extensions.Caching.Memory;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView;
using MyProfile.Entity.ModelView.Currency;
using MyProfile.Entity.ModelView.TemplateModelView;
using MyProfile.Entity.Repository;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Common.Service
{
    public class CommonService
    {
        private IBaseRepository repository;
        private IMemoryCache cache;

        public CommonService(IBaseRepository repository, IMemoryCache cache)
        {
            this.repository = repository;
            this.cache = cache;
        }
        public List<PeriodType> GetPeriodTypes()
        {
            return repository.GetAll<PeriodType>()
                .Where(x => x.ID == (int)PeriodTypesEnum.Month || x.ID == (int)PeriodTypesEnum.Year)
                .ToList();
        }

        public string GenerateFormulaBySections(List<TemplateBudgetSectionPlusViewModel> sections)
        {
            List<FormulaItem> items = new List<FormulaItem>();

            for (int i = 0; i < sections.Count; i++)
            {
                if (i != 0) //i % 2 != 0)
                {
                    items.Add(new FormulaItem
                    {
                        Type = FormulaFieldType.Mark,
                        Value = "+"
                    });
                }

                items.Add(new FormulaItem
                {
                    ID = sections[i].BudgetSectionID,
                    Value = "[ " + sections[i].Name + " ]",
                    Type = FormulaFieldType.Section
                });
            }

            return JsonConvert.SerializeObject(items);
        }


        public List<DictionariesModelView> GetTotalActions()
        {
            List<DictionariesModelView> z = new List<DictionariesModelView>();

            z.Add(new DictionariesModelView
            {
                TemplateColumnType = TemplateColumnType.BudgetSection,
                Variants = new List<DictionaryItem>
                {
                    new DictionaryItem
                    {
                        FooterActionTypeID = FooterActionType.Undefined, Name = "Not selected",
                    },
                    new DictionaryItem
                    {
                        FooterActionTypeID = FooterActionType.Sum, Name = "SUM",
                    },new DictionaryItem
                    {
                        FooterActionTypeID = FooterActionType.Avr, Name = "AVR",
                    },
                    new DictionaryItem
                    {
                        FooterActionTypeID = FooterActionType.Max, Name = "MAX",
                    },
                    new DictionaryItem
                    {
                        FooterActionTypeID = FooterActionType.Min, Name = "MIN",
                    }
                }
            });
            return z;
        }

        public List<CurrencyClientModelView> GetCurrencyInfo()
        {
            List<CurrencyClientModelView> currencies;

            if (cache.TryGetValue(typeof(CurrencyClientModelView).Name, out currencies) == false)
            {
                currencies = repository.GetAll<Currency>()
                 .Select(x => new CurrencyClientModelView
                 {
                     codeName = x.CodeName,
                     codeName_CBR = x.CodeName_CBR,
                     codeNumber_CBR = x.CodeNumber_CBR,
                     icon = x.Icon,
                     id = x.ID,
                     name = x.Name,
                     specificCulture = x.SpecificCulture,
                     cbr_link = "http://www.cbr.ru/scripts/XML_daily.asp?date_req="
                 })
                 .ToList();

                cache.Set(typeof(CurrencyClientModelView).Name, currencies, DateTime.Now.AddDays(15));
            }
            return currencies;
        }

        public async Task<BankCurrencyData> GetRatesFromBank(DateTime date, string codeName_CBR)
        {
            string text = string.Empty;
            var _link = "http://www.cbr.ru/scripts/XML_daily.asp?date_req=" + date.ToString("dd/MM/yyyy");
            WebRequest wr = WebRequest.Create(_link);
            wr.Timeout = 3500;
            decimal rate = -1;
            int nominal = -1;
            Dictionary<DateTime, List<BankCurrencyData>> currencies = new Dictionary<DateTime, List<BankCurrencyData>>();
            BankCurrencyData bankCurrencyData = null;

            if (cache.TryGetValue("List_BankCurrencyData", out currencies))
            {
                if (currencies.ContainsKey(date.Date))
                {
                    bankCurrencyData = currencies[date.Date].FirstOrDefault(x => x.CodeName_CBR  == codeName_CBR);
                }
            }else
            {
                currencies = new Dictionary<DateTime, List<BankCurrencyData>>();
            }

            if (bankCurrencyData == null)
            {
                try
                {
                    var currenciesDB = GetCurrencyInfo();
                    List<BankCurrencyData> bankCurrencyDatas = new List<BankCurrencyData>();

                    var response = await wr.GetResponseAsync();
                    var readStream = new StreamReader(((HttpWebResponse)response).GetResponseStream(), Encoding.GetEncoding("windows-1251"));
                    var doc = XElement.Load(readStream);

                    foreach (XElement element in doc.Elements("Valute"))
                    {
                        var _codeName_CBR = element.Attribute("ID").Value;
                        var numCode = element.Element("NumCode");
                        var charCode = element.Element("CharCode");
                        var name = element.Element("Name");
                        var elementPrice = element.Element("Value");
                        var elementNominal = element.Element("Nominal");

                        if (!string.IsNullOrEmpty(numCode.Value)
                            && !string.IsNullOrEmpty(charCode.Value)
                            && !string.IsNullOrEmpty(name.Value)
                            && !string.IsNullOrEmpty(elementPrice.Value)
                            && !string.IsNullOrEmpty(elementNominal.Value))
                        {
                            int _currencyID = -1;

                            if (currenciesDB.Any(x => x.codeName_CBR == _codeName_CBR))
                            {
                                _currencyID = currenciesDB.FirstOrDefault(x => x.codeName_CBR == _codeName_CBR).id;
                            }

                           var _bankCurrencyData = new BankCurrencyData
                            {
                                CurrencyID = _currencyID,
                                CharCode = charCode.Value,
                                Date = date.Date,
                                Name = name.Value,
                                Nominal = int.Parse(elementNominal.Value),
                                Rate = decimal.Parse(elementPrice.Value.Replace(",", ".")),
                                NumCode = numCode.Value,
                                CodeName_CBR = _codeName_CBR
                            };

                            if (_codeName_CBR == codeName_CBR)
                            {
                                bankCurrencyData = _bankCurrencyData;
                            }
                            bankCurrencyDatas.Add(_bankCurrencyData);
                        }
                    }
                    currencies.Add(date.Date, bankCurrencyDatas);

                    cache.Set("List_BankCurrencyData", currencies, DateTime.Now.AddDays(30));
                }
                catch (Exception ex)
                {
                    
                }
            }
            return bankCurrencyData;
        }

        public async Task<string> GetRateFromBank(string link, DateTime date)
        {
            string text = string.Empty;
            var _link = link + date.ToString("dd/MM/yyyy");
            WebRequest wr = WebRequest.Create(_link);
            wr.Timeout = 3500;

            try
            {
                var response = await wr.GetResponseAsync();
                var readStream = new StreamReader(((HttpWebResponse)response).GetResponseStream(), Encoding.GetEncoding("windows-1251"));
                text = readStream.ReadToEnd();
            }
            catch (Exception ex)
            {
                //await userLogService.CreateErrorLogAsync(UserInfo.Current.UserSessionID, where: "Budget.GetRateFromBank", ex, comment: _link);
            }
            return text;
        }
    }
    public class DictionariesModelView
    {
        public TemplateColumnType TemplateColumnType { get; set; }
        public List<DictionaryItem> Variants { get; set; }
    }
    public class DictionaryItem
    {
        public FooterActionType FooterActionTypeID { get; set; }
        public string Name { get; set; }

    }

    [XmlRoot(ElementName = "Valute")]
    public class Valute
    {
        [XmlElement(ElementName = "NumCode")]
        public string NumCode { get; set; }
        [XmlElement(ElementName = "CharCode")]
        public string CharCode { get; set; }
        [XmlElement(ElementName = "Nominal")]
        public string Nominal { get; set; }
        [XmlElement(ElementName = "Name")]
        public string Name { get; set; }
        [XmlElement(ElementName = "Value")]
        public string Value { get; set; }
        [XmlAttribute(AttributeName = "ID")]
        public string ID { get; set; }
    }

    [XmlRoot(ElementName = "ValCurs")]
    public class ValCurs
    {
        [XmlElement(ElementName = "Valute")]
        public List<Valute> Valute { get; set; }
        [XmlAttribute(AttributeName = "Date")]
        public string Date { get; set; }
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }
    }

}
