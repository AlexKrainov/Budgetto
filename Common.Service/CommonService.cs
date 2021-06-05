using Microsoft.Extensions.Caching.Memory;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView;
using MyProfile.Entity.ModelView.Company;
using MyProfile.Entity.ModelView.Reminder;
using MyProfile.Entity.ModelView.TemplateModelView;
using MyProfile.Entity.Repository;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

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
            List<PeriodType> periodTypes;

            if (cache.TryGetValue(typeof(PeriodType).Name, out periodTypes) == false)
            {
                return repository.GetAll<PeriodType>()
                   .Where(x => x.ID == (int)PeriodTypesEnum.Month || x.ID == (int)PeriodTypesEnum.Year)
                   .ToList();

                cache.Set(typeof(PeriodType).Name, periodTypes, DateTime.Now.AddDays(15));
            }
            return periodTypes;
           
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

        public List<TimeZoneClientModelView> GetTimeZones()
        {
            List<TimeZoneClientModelView> timezones;

            if (cache.TryGetValue(typeof(TimeZoneClientModelView).Name, out timezones) == false)
            {
                timezones = repository.GetAll<OlsonTZID>()
                 .Select(x => new TimeZoneClientModelView
                 {
                     OlzonTZID = x.ID,
                     TimeZoneID = x.TimeZoneID,
                     IsDST = x.TimeZone.IsDST,
                     OlzonTZName = x.Name,
                     WindowsTimezoneID = x.TimeZone.WindowsTimezoneID,
                     UtcOffset = x.TimeZone.UTCOffsetHours,
                     WindowsDisplayName = x.TimeZone.WindowsDisplayName,
                     Abreviature = x.TimeZone.Abreviatura,
                 })
                 .ToList();

                cache.Set(typeof(TimeZoneClientModelView).Name, timezones, DateTime.Now.AddDays(15));
            }
            return timezones;
        }

        public List<CompanyLightViewModel> GetCompanies()
        {
            List<CompanyLightViewModel> companies;

            if (cache.TryGetValue(typeof(CompanyLightViewModel).Name, out companies) == false)
            {
                companies = repository.GetAll<Company>()
                 .Select(x => new CompanyLightViewModel
                 {
                     ID = x.ID,
                     TagKeyWords = x.TagKeyWords,
                 })
                 .ToList();

                cache.Set(typeof(CompanyLightViewModel).Name, companies, DateTime.Now.AddDays(15));
            }
            return companies;
        }

        public string GetDays(int daysCount)
        {
            if ((new int[] { 1, 21, 31 }).Contains(daysCount))
            {
                return "день";
            }
            else if ((new int[] { 2, 3, 4, 22, 23, 24, 32, 33, 34}).Contains(daysCount))
            {
                return "дня";
            }
            return "дней";
        }
    }
}
