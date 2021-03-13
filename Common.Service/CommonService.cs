using Microsoft.Extensions.Caching.Memory;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView;
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
    }
}
