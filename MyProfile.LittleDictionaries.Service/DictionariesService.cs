using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView.Currency;
using MyProfile.Entity.Repository;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MyProfile.LittleDictionaries.Service
{
    public class DictionariesService
    {
        private IBaseRepository repository;
        private IMemoryCache cache;

        public DictionariesService(IBaseRepository repository,
            IMemoryCache cache)
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

        public List<CurrencyClientModelView> GetCurrencyInfoForClient(Guid currentUserID)
        {
            List<CurrencyClientModelView> currencies;

            if (cache.TryGetValue(typeof(CurrencyClientModelView).Name + "_" + currentUserID, out currencies) == false)
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

                cache.Set(typeof(CurrencyClientModelView).Name + "_" + currentUserID, currencies, DateTime.Now.AddDays(2));
            }
            return currencies;
        }

        public async void GetRatesFromBank(string link, DateTime date)
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
                var obj = JsonSerializer.Deserialize<ValCurs>(text, new JsonSerializerOptions());
            }
            catch (Exception ex)
            {
            }

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
