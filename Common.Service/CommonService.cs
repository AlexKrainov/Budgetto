using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView;
using MyProfile.Entity.ModelView.TemplateModelView;
using MyProfile.Entity.Repository;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Common.Service
{
    public class CommonService
    {
        private IBaseRepository repository;

        public CommonService(IBaseRepository repository)
        {
            this.repository = repository;
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
    }
}
