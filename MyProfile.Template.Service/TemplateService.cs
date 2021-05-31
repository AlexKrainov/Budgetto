using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelEntitySave;
using MyProfile.Entity.ModelView;
using MyProfile.Entity.ModelView.TemplateModelView;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
using MyProfile.Progress.Service;
using MyProfile.User.Service;
using MyProfile.UserLog.Service;
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
        private ProgressService progressService;
        private UserCounterService userCounterService;

        public TemplateService(IBaseRepository repository, UserLogService userLogService, ProgressService progressService, UserCounterService userCounterService)
        {
            this.repository = repository;
            this.userLogService = userLogService;
            this.progressService = progressService;
            this.userCounterService = userCounterService;
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
                            IsCreatedByConstructor = x.IsCreatedByConstructor,
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
                                .OrderBy(y => y.Order)
                                .ToList()
                        })
                        .FirstOrDefaultAsync();

                if (templateViewModel != null)
                {
                    var template = repository.GetAll<Template>(x => x.ID == templateViewModel.ID).FirstOrDefault();
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

        public async Task<bool> ToggleTemplate(int id)
        {
            var isDefault = false;
            var currentUser = UserInfo.Current;
            var db_chart = await repository.GetAll<Template>(x => x.ID == id && x.UserID == currentUser.ID)
                .FirstOrDefaultAsync();

            if (db_chart != null)
            {
                isDefault = db_chart.IsDefault = !db_chart.IsDefault;

                db_chart.DateEdit = DateTime.Now.ToUniversalTime();
                repository.Update(db_chart);

                await userLogService.CreateUserLogAsync(currentUser.UserSessionID, UserLogActionType.Template_ToggleIsDefault);

                if (isDefault)
                {
                    await setDefaultTemplates(new TemplateViewModel
                    {
                        ID = db_chart.ID,
                        IsDefault = isDefault,
                        PeriodTypeID = db_chart.PeriodTypeID,
                    });
                }

                return isDefault;
            }
            return isDefault;
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
                            IsCreatedByConstructor = x.IsCreatedByConstructor,
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
                                .OrderBy(p => p.Order)
                                .ToList()
                        })
                        .ToListAsync();
        }

        public async Task ChangeColumnOrder(TemplateColumnOrder templateColumnOrder)
        {
            var currentUser = UserInfo.Current;
            var templateColumns = await repository.GetAll<Template>(x => x.ID == templateColumnOrder.TemplateID && x.UserID == currentUser.ID)
                .SelectMany(x => x.TemplateColumns)
                .OrderBy(x => x.Order)
                .ToListAsync();

            if (templateColumns != null)
            {
                for (int i = 0; i < templateColumnOrder.ListOrder.Count; i++)
                {
                    var z = templateColumns.FirstOrDefault(x => x.ID == templateColumnOrder.ListOrder[i].ID);
                    templateColumns.FirstOrDefault(x => x.ID == templateColumnOrder.ListOrder[i].ID).Order = i;
                }
                //for (int i = 0; i < templateColumns.Count; i++)
                //{
                //    templateColumns[i].Order = templateColumnOrder.ColumnsOrder[i];
                //}

                await repository.SaveAsync();
                await userLogService.CreateUserLogAsync(currentUser.UserSessionID, UserLogActionType.Template_ColumnOrder);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="template"></param>
        /// <param name="isRemove">== true - remove, == false - recovery</param>
        /// <returns></returns>
        public async Task<bool> RemoveOrRecovery(long templateID, bool isRemove)
        {
            var currentUser = UserInfo.Current;
            var db_item = await repository.GetAll<Template>(x => x.ID == templateID && x.UserID == currentUser.ID).FirstOrDefaultAsync();

            if (isRemove == false)
            {
                if (await userCounterService.CanCreateEntityAsync(BudgettoEntityType.Templates) == false)
                {
                    await userLogService.CreateUserLogAsync(currentUser.UserSessionID, UserLogActionType.CounterTemplates_Recovery_Limit);
                    return false;
                }
            }

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
                await userLogService.CreateUserLogAsync(currentUser.UserSessionID, db_item.DateDelete != null ? UserLogActionType.Template_Delete : UserLogActionType.Template_Recovery);
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

        public async Task<TemplateErrorModelView> SaveTemplate(TemplateViewModel template, bool saveAs, bool isContstructorSave = false)
        {
            TemplateErrorModelView modelView = new TemplateErrorModelView { Template = template };
            var currentUser = UserInfo.Current;
            var now = DateTime.Now.ToUniversalTime();
            bool isEdit = false;
            List<long> errorLogCreateIDs = new List<long>();

            #region Check name

            if (await repository.AnyAsync<Template>(x => x.UserID == currentUser.ID && x.Name == template.Name && x.ID != template.ID && x.IsDeleted == false))
            {
                if (saveAs)
                {
                    template.Name = template.Name + "_copy";
                }
                else
                {
                    template.Name = template.Name + "_(2)";

                    if (await repository.AnyAsync<Template>(x => x.UserID == currentUser.ID && x.Name == template.Name && x.ID != template.ID && x.IsDeleted == false))
                    {
                        template.Name = template.Name.Replace("_(2)", "_(3)");
                        if (await repository.AnyAsync<Template>(x => x.UserID == currentUser.ID && x.Name == template.Name && x.ID != template.ID && x.IsDeleted == false))
                        {
                            template.Name = template.Name.Replace("_(3)", "_(4)");
                            if (await repository.AnyAsync<Template>(x => x.UserID == currentUser.ID && x.Name == template.Name && x.ID != template.ID && x.IsDeleted == false))
                            {
                                template.Name = template.Name.Replace("_(4)", "_(5)");
                            }
                        }
                    }
                }
            }

            #endregion
            try
            {
                if (template.ID == 0)//create
                {
                    if (await userCounterService.CanCreateEntityAsync(BudgettoEntityType.Templates) == false)
                    {
                        await userLogService.CreateUserLogAsync(currentUser.UserSessionID, UserLogActionType.CounterTemplates_Create_Limit);
                        modelView.IsOk = false;
                        modelView.ErrorMessage = "Ошибка при создании. Превышен лимит доступных шаблонов.";
                        return modelView;
                    }

                    Template templateDB = new Template();
                    templateDB.UserID = currentUser.ID;
                    templateDB.PeriodTypeID = template.PeriodTypeID;
                    templateDB.DateCreate = templateDB.DateEdit = DateTime.MinValue == template.DateCreate ? now : template.DateCreate;
                    templateDB.IsCountCollectiveBudget = template.IsCountCollectiveBudget == true;
                    templateDB.MaxRowInAPage = 30;
                    templateDB.Name = template.Name ?? "Шаблон_" + now.ToShortDateString();
                    templateDB.Description = template.Description;
                    templateDB.IsDefault = template.IsDefault;
                    templateDB.IsCreatedByConstructor = template.IsCreatedByConstructor;

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


                }
                else
                {
                    isEdit = true;

                    var templateBudgetSections = repository.GetAll<Entity.Model.TemplateBudgetSection>(x => x.TemplateColumn.TemplateID == template.ID);
                    repository.DeleteRange(templateBudgetSections, true);

                    var columns = repository.GetAll<TemplateColumn>(x => x.TemplateID == template.ID);
                    repository.DeleteRange(columns, true);

                    Template templateDB = repository.GetAll<Template>(x => x.ID == template.ID).FirstOrDefault();

                    templateDB.UserID = UserInfo.Current.ID;
                    templateDB.PeriodTypeID = template.PeriodTypeID;
                    templateDB.DateEdit = DateTime.Now;
                    templateDB.IsCountCollectiveBudget = template.IsCountCollectiveBudget == true;
                    templateDB.MaxRowInAPage = 30;
                    templateDB.Name = template.Name ?? "Шаблон_" + now.ToShortDateString();
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

                }
            }
            catch (Exception ex)
            {
                modelView.IsOk = false;
                modelView.ErrorMessage = "Произошла ошибка во время сохранения.";

                errorLogCreateIDs.Add(await userLogService.CreateErrorLogAsync(currentUser.UserSessionID, "TemplateService_SaveTemplate", ex));
            }

            if (isEdit)
            {
                await userLogService.CreateUserLogAsync(currentUser.UserSessionID, UserLogActionType.Template_Edit, errorLogIDs: errorLogCreateIDs);
            }
            else
            {
                await userLogService.CreateUserLogAsync(currentUser.UserSessionID, UserLogActionType.Template_Create, errorLogIDs: errorLogCreateIDs);
            }

            #region Progress

            if (currentUser.IsCompleteIntroductoryProgress == false && isContstructorSave == false)
            {
                await progressService.SetCompleteProgressItemTypeAsync(currentUser.ID, ProgressTypeEnum.Introductory, ProgressItemTypeEnum.CreateOrEditTemplate);
            }

            #endregion

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
