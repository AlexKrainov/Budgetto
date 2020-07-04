using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelEntitySave;
using MyProfile.Entity.ModelView;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MyProfile.Template.Service
{
    using Template = MyProfile.Entity.Model.Template;
    public class TemplateService
    {
        private IBaseRepository repository;

        public TemplateService(IBaseRepository repository)
        {
            this.repository = repository;
        }

        public async Task<TemplateViewModel> GetTemplateByID(Expression<Func<Template, bool>> predicate, bool addCollectiveBudget = true)
        {
            TemplateViewModel templateViewModel = new TemplateViewModel();

            if (predicate != null)
            {
                templateViewModel = await repository.GetAll<Template>(predicate)
                        .Select(x => new TemplateViewModel
                        {
                            ID = x.ID,
                            DateCreate = x.DateCreate,
                            DateEdit = x.DateEdit,
                            IsCountCollectiveBudget = x.IsCountCollectiveBudget,
                            MaxRowInAPage = x.MaxRowInAPage,
                            Name = x.Name,
                            Description = x.Description,
                            PeriodName = x.PeriodType.Name,
                            PeriodTypeID = x.PeriodTypeID,

                            Columns = x.TemplateColumns
                                .Select(y => new Column
                                {
                                    ID = y.ID,
                                    Name = y.Name,
                                    IsShow = y.IsShow,
                                    Formula = JsonConvert.DeserializeObject<List<FormulaItem>>(y.Formula) ?? new List<FormulaItem>(),
                                    Order = y.Order,
                                    TemplateColumnType = (TemplateColumnType)y.ColumnTypeID,
                                    TotalAction = (FooterActionType)y.FooterActionTypeID,
                                    PlaceAfterCommon = y.PlaceAfterCommon ?? 0,
                                    Format = y.Format,
                                    ColumnSectionType = y.TemplateBudgetSections.Where(d => d.BudgetSection.SectionTypeID != null).Select(h => h.BudgetSection.SectionType.CodeName).FirstOrDefault(),
                                    TemplateBudgetSections = y.TemplateBudgetSections
                                        .Select(z => new Entity.ModelView.TemplateBudgetSection
                                        {
                                            ID = z.ID,
                                            SectionID = z.BudgetSection.ID,
                                            SectionName = z.BudgetSection.Name,
                                            CollectionSections = z.BudgetSection.CollectiveSections.Select(q => new BudgetSectionModelView { ID = q.ChildSection.ID }).ToList()
                                        })
                                        .ToList()
                                })
                                .ToList()
                        })
                        .FirstOrDefaultAsync();

                if (templateViewModel != null)
                {
                    var template = repository.GetByID<Template>(templateViewModel.ID);
                    template.LastSeenDate = DateTime.Now.ToUniversalTime();
                    await repository.SaveAsync();
                }

                if (addCollectiveBudget)
                {
                    //Check if all peaple has IsAllowCollectiveBudget
                    var allTemplateSections = templateViewModel.Columns
                        .SelectMany(x => x.TemplateBudgetSections)
                        .Select(x => x.ID)
                        .ToList();

                    //var includedSections = repository.GetAll<BudgetSection>(x => !string.IsNullOrEmpty(x.IncludedCollectiveSections) && allTemplateSections.Contains(x.ID))
                    //	.Select(x => new
                    //	{
                    //		sectionID = x.ID,
                    //		x.IncludedCollectiveSections
                    //	})
                    //	.ToList();

                    //foreach (var incudedSection in includedSections)
                    //{
                    //	var includedCollectiveSection_Raws = JsonConvert.DeserializeObject<List<IncludedCollectiveItem>>(incudedSection.IncludedCollectiveSections);
                    //	//section.IncludedCollectiveSections = includedCollectiveSection_Raws.Select(x => new BudgetSectionModelView { ID = x.id, PersonID = x.personID }).ToList();


                    //}
                    foreach (var column in templateViewModel.Columns)
                    {
                        foreach (var columnSection in column.TemplateBudgetSections)
                        {
                            //if (includedSections.Any(x => x.sectionID == columnSection.SectionID))
                            //{
                            //	var includedCollectiveSection_Raws = JsonConvert.DeserializeObject<List<IncludedCollectiveItem>>(includedSections.FirstOrDefault(x => x.sectionID == columnSection.SectionID).IncludedCollectiveSections);
                            //	foreach (var includedCollectiveSection_Raw in includedCollectiveSection_Raws)
                            //	{
                            //		if (UserInfo.Current.AllCollectivePersonIDs.Contains(includedCollectiveSection_Raw.personID))
                            //		{

                            //		}
                            //	}
                            //}
                        }
                    }
                }
            }

            return templateViewModel;
        }

        public async Task<List<TemplateViewModel>> GetTemplates(Expression<Func<Template, bool>> predicate)
        {
            return await repository.GetAll<Template>(predicate)
                        .Select(x => new TemplateViewModel
                        {
                            ID = x.ID,
                            DateCreate = x.DateCreate,
                            DateEdit = x.DateEdit,
                            IsCountCollectiveBudget = x.IsCountCollectiveBudget,
                            //LastSeenDateTime = x.CodeName,
                            MaxRowInAPage = x.MaxRowInAPage,
                            Name = x.Name,
                            Description = x.Description,
                            PeriodName = x.PeriodType.Name,
                            PeriodTypeID = x.PeriodTypeID,
                            IsShow = true,

                            Columns = x.TemplateColumns
                                .Select(y => new Column
                                {
                                    ID = y.ID,
                                    Name = y.Name,
                                    IsShow = y.IsShow,
                                    Formula = JsonConvert.DeserializeObject<List<FormulaItem>>(y.Formula) ?? new List<FormulaItem>(),
                                    Order = y.Order,
                                    TemplateColumnType = (TemplateColumnType)y.ColumnTypeID,
                                    TotalAction = (FooterActionType)y.FooterActionTypeID,
                                    PlaceAfterCommon = y.PlaceAfterCommon ?? 0,
                                    Format = y.Format,
                                    TemplateBudgetSections = y.TemplateBudgetSections
                                        .Select(z => new Entity.ModelView.TemplateBudgetSection
                                        {
                                            ID = z.ID,
                                            SectionID = z.BudgetSection.ID,
                                            SectionName = z.BudgetSection.Name
                                        })
                                        .ToList()
                                })
                                .ToList()
                        })
                        .ToListAsync();
        }
        public async Task<List<TemplateViewModel_Short>> GetNameTemplates(Expression<Func<Template, bool>> predicate)
        {
            return await repository.GetAll<Template>(predicate)
                        .OrderBy(x => x.LastSeenDate)
                        .Select(x => new TemplateViewModel_Short
                        {
                            ID = x.ID,
                            Name = x.Name,
                            PeriodName = x.PeriodType.Name,
                            PeriodTypeID = x.PeriodTypeID,
                        })
                        .ToListAsync();
        }



        public TemplateViewModel SaveTemplate(TemplateViewModel template)
        {
            try
            {
                if (template.ID == 0)//create
                {
                    Template templateDB = new Template();
                    templateDB.UserID = Guid.Parse("EA02C872-0C3C-4112-7231-08D7BDD8901D");
                    templateDB.PeriodTypeID = template.PeriodTypeID;
                    templateDB.DateCreate = templateDB.DateEdit = DateTime.MinValue == template.DateCreate ? DateTime.Now : template.DateCreate;
                    templateDB.IsCountCollectiveBudget = template.IsCountCollectiveBudget == true;
                    templateDB.MaxRowInAPage = 30;
                    templateDB.Name = template.Name ?? "Template";
                    templateDB.Description = template.Description;

                    repository.Create(templateDB, true);

                    foreach (var column in template.Columns.OrderBy(x => x.Order))
                    {
                        var templateColumnDB = new TemplateColumn
                        {
                            IsShow = column.IsShow,
                            Name = column.Name,
                            Order = column.Order,
                            TemplateID = templateDB.ID,
                            Formula = JsonConvert.SerializeObject(column.Formula),
                            ColumnTypeID = (int)column.TemplateColumnType,
                            FooterActionTypeID = (int)column.TotalAction,
                            PlaceAfterCommon = column.PlaceAfterCommon,
                            Format = column.Format
                        };
                        repository.Create(templateColumnDB, true);

                        foreach (var templateAreaType in column.TemplateBudgetSections)
                        {
                            repository.Create(new Entity.Model.TemplateBudgetSection
                            {
                                BudgetSectionID = templateAreaType.SectionID,
                                TemplateColumnID = templateColumnDB.ID
                            }, true);
                        }
                    }
                }
                else
                {
                    var templateBudgetSections = repository.GetAll<Entity.Model.TemplateBudgetSection>(x => x.TemplateColumn.TemplateID == template.ID);
                    repository.DeleteRange(templateBudgetSections, true);

                    var columns = repository.GetAll<TemplateColumn>(x => x.TemplateID == template.ID);
                    repository.DeleteRange(columns, true);

                    Template templateDB = repository.GetByID<Template>(template.ID);

                    templateDB.UserID = Guid.Parse("EA02C872-0C3C-4112-7231-08D7BDD8901D");
                    templateDB.PeriodTypeID = template.PeriodTypeID;
                    templateDB.DateEdit = DateTime.Now;
                    templateDB.IsCountCollectiveBudget = template.IsCountCollectiveBudget == true;
                    templateDB.MaxRowInAPage = 30;
                    templateDB.Name = template.Name ?? "NameTemplate";
                    templateDB.Description = template.Description;

                    repository.Update(templateDB, true);

                    foreach (var column in template.Columns.OrderBy(x => x.Order))
                    {
                        var templateColumnDB = new TemplateColumn
                        {
                            IsShow = column.IsShow,
                            Name = column.Name,
                            Order = column.Order,
                            TemplateID = templateDB.ID,
                            Formula = JsonConvert.SerializeObject(column.Formula),
                            ColumnTypeID = (int)column.TemplateColumnType,
                            FooterActionTypeID = (int)column.TotalAction,
                            PlaceAfterCommon = column.PlaceAfterCommon,
                            Format = column.Format
                        };
                        repository.Create(templateColumnDB, true);

                        foreach (var templateAreaType in column.TemplateBudgetSections)
                        {
                            repository.Create(new Entity.Model.TemplateBudgetSection
                            {
                                BudgetSectionID = templateAreaType.SectionID,
                                TemplateColumnID = templateColumnDB.ID
                            }, true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return template;
        }
    }
}
