using Common.Service.Curency;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
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
namespace Common.Service
{
    public class CurrencyService
    {
        private IBaseRepository repository;
        private IMemoryCache cache;

        public CurrencyService(IBaseRepository repository,
            IMemoryCache cache)
        {
            this.repository = repository;
            this.cache = cache;
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
                 })
                 .ToList();

                cache.Set(typeof(CurrencyClientModelView).Name, currencies, DateTime.Now.AddDays(15));
            }
            return currencies;
        }

        public int HistoryLoad()
        {
            GetCurrency getCurrency = new GetCurrency(repository, cache, GetCurrencyInfo());

            return getCurrency.HisotoryLoad();
        }

        public async Task<CurrencyRateHistory> GetRateByCodeAsync(DateTime date, string charCode, Guid? userSessionID = null)
        {
            GetCurrencyAsync getCurrencyAsync = new GetCurrencyAsync(repository, cache, GetCurrencyInfo());

            return await getCurrencyAsync.GetRateByCode(date, charCode, userSessionID);
        }

        public CurrencyRateHistory GetRateByCode(DateTime date, string charCode, Guid? userSessionID = null)
        {
            GetCurrency getCurrency = new GetCurrency(repository, cache, GetCurrencyInfo());

            return getCurrency.GetRateByCode(date, charCode, userSessionID);
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
}
