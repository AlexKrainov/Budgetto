using Microsoft.Extensions.Caching.Memory;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView;
using MyProfile.Entity.ModelView.TemplateModelView;
using MyProfile.Entity.Repository;
using Newtonsoft.Json;
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
    }
}
