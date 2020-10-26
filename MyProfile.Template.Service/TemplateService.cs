using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelEntitySave;
using MyProfile.Entity.ModelView;
using MyProfile.Entity.ModelView.TemplateModelView;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
using MyProfile.User.Service;
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
        private UserLogService userLogService;

        public TemplateService(IBaseRepository repository, UserLogService userLogService)
        {
            this.repository = repository;
            this.userLogService = userLogService;
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
                            IsDefault = x.IsDefault,

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

                if (false)//addCollectiveBudget)
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

        public async Task<List<TemplateViewModel>> GetTemplates()
        {
            return await repository.GetAll<Template>(x => x.UserID == UserInfo.Current.ID && x.IsDeleted == false)
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
                            IsDefault = x.IsDefault,
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="template"></param>
        /// <param name="isRemove">== true - remove, == false - recovery</param>
        /// <returns></returns>
        public async Task<bool> RemoveOrRecovery(int templateID, bool isRemove)
        {
            var currentUser = UserInfo.Current;
            var db_item = await repository.GetAll<Template>(x => x.ID == templateID && x.UserID == currentUser.ID).FirstOrDefaultAsync();

            if (db_item != null)
            {
                db_item.IsDeleted = isRemove;
                if (isRemove)
                {
                    db_item.DateDelete = DateTime.Now.ToUniversalTime();
                }
                else
                {
                    db_item.DateDelete = null;
                }
                await repository.UpdateAsync(db_item, true);
                await userLogService.CreateUserLog(currentUser.UserSessionID, db_item.DateDelete != null ? UserLogActionType.TemplateDelete : UserLogActionType.TemplateRecovery);
                return true;
            }
            return false;
        }

        public async Task<List<TemplateViewModel_Short>> GetNameTemplates(Expression<Func<Template, bool>> predicate)
        {
            return await repository.GetAll<Template>(predicate)
                        .OrderBy(x => x.Name)
                        .Select(x => new TemplateViewModel_Short
                        {
                            ID = x.ID,
                            Name = x.Name,
                            PeriodName = x.PeriodType.Name,
                            PeriodTypeID = x.PeriodTypeID,
                            IsDefault = x.IsDefault,
                        })
                        .ToListAsync();
        }

        public async Task<TemplateErrorModelView> SaveTemplate(TemplateViewModel template)
        {
            TemplateErrorModelView modelView = new TemplateErrorModelView { Template = template };
            var currentUser = UserInfo.Current;
            #region Check name

            if (await repository.AnyAsync<Template>(x => x.UserID == currentUser.ID && x.Name == template.Name && x.ID != template.ID))
            {
                template.Name = template.Name + "_copy";
                //modelView.IsOk = false;
                //modelView.NameAlreadyExist = true;
                //modelView.ErrorMessage = "Шаблон с таким именем уже существует";

                //return modelView;
            }

            #endregion
            try
            {
                if (template.ID == 0)//create
                {
                    Template templateDB = new Template();
                    templateDB.UserID = currentUser.ID;
                    templateDB.PeriodTypeID = template.PeriodTypeID;
                    templateDB.DateCreate = templateDB.DateEdit = DateTime.MinValue == template.DateCreate ? DateTime.Now : template.DateCreate;
                    templateDB.IsCountCollectiveBudget = template.IsCountCollectiveBudget == true;
                    templateDB.MaxRowInAPage = 30;
                    templateDB.Name = template.Name ?? "Шаблон";
                    templateDB.Description = template.Description;
                    templateDB.IsDefault = template.IsDefault;

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
                        await repository.CreateAsync(templateColumnDB, true);

                        foreach (var templateAreaType in column.TemplateBudgetSections)
                        {
                            await repository.CreateAsync(new Entity.Model.TemplateBudgetSection
                            {
                                BudgetSectionID = templateAreaType.SectionID,
                                TemplateColumnID = templateColumnDB.ID
                            }, true);
                        }
                    }
                    template.ID = templateDB.ID;

                    await userLogService.CreateUserLog(currentUser.UserSessionID, UserLogActionType.TemplateCreate);
                }
                else
                {
                    var templateBudgetSections = repository.GetAll<Entity.Model.TemplateBudgetSection>(x => x.TemplateColumn.TemplateID == template.ID);
                    repository.DeleteRange(templateBudgetSections, true);

                    var columns = repository.GetAll<TemplateColumn>(x => x.TemplateID == template.ID);
                    repository.DeleteRange(columns, true);

                    Template templateDB = repository.GetByID<Template>(template.ID);

                    templateDB.UserID = UserInfo.Current.ID;
                    templateDB.PeriodTypeID = template.PeriodTypeID;
                    templateDB.DateEdit = DateTime.Now;
                    templateDB.IsCountCollectiveBudget = template.IsCountCollectiveBudget == true;
                    templateDB.MaxRowInAPage = 30;
                    templateDB.Name = template.Name ?? "Шаблон";
                    templateDB.Description = template.Description;
                    templateDB.IsDefault = template.IsDefault;

                    await repository.UpdateAsync(templateDB, true);

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
                            Format = column.Format,
                        };
                        await repository.CreateAsync(templateColumnDB, true);

                        foreach (var templateAreaType in column.TemplateBudgetSections)
                        {
                            await repository.CreateAsync(new Entity.Model.TemplateBudgetSection
                            {
                                BudgetSectionID = templateAreaType.SectionID,
                                TemplateColumnID = templateColumnDB.ID
                            }, true);
                        }
                    }
                    await userLogService.CreateUserLog(currentUser.UserSessionID, UserLogActionType.TemplateEdit);
                }
            }
            catch (Exception ex)
            {
                modelView.IsOk = false;
                modelView.ErrorMessage = "Произошла ошибка во время сохранения.";
            }

            await setDefaultTemplates(template);

            return modelView;
        }

        private async Task<int> setDefaultTemplates(TemplateViewModel template)
        {
            if (template.IsDefault)
            {
                var templatesWithIsDefault = await repository.GetAll<Template>
                    (x => x.UserID == UserInfo.Current.ID
                    && x.PeriodTypeID == template.PeriodTypeID
                    && x.IsDefault == true
                    && x.ID != template.ID)
                    .ToListAsync();

                for (int i = 0; i < templatesWithIsDefault.Count(); i++)
                {
                    templatesWithIsDefault[i].IsDefault = false;
                    repository.Update(templatesWithIsDefault[i]);
                }
                return await repository.SaveAsync();
            }
            return 1;
        }


    }
}
