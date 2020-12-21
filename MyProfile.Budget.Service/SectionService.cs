﻿using LinqKit;
using Microsoft.EntityFrameworkCore;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView;
using MyProfile.Entity.ModelView.CommonViewModels;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using MyProfile.Entity.ModelEntitySave;
using MyProfile.User.Service;
using MyProfile.Entity.ModelView.AreaAndSection;
using Common.Service;
using MyProfile.Entity.ModelView.TemplateModelView;
using MyProfile.UserLog.Service;
using Microsoft.Extensions.Caching.Memory;

namespace MyProfile.Budget.Service
{
    public class SectionService
    {
        private IBaseRepository repository;
        private CollectionUserService collectionUserService;
        private UserLogService userLogService;
        private CommonService commonService;
        private IMemoryCache cache;

        public SectionService(IBaseRepository repository, IMemoryCache cache)
        {
            this.repository = repository;
            this.collectionUserService = new CollectionUserService(repository);
            this.userLogService = new UserLogService(repository);
            this.commonService = new CommonService(repository);
            this.cache = cache;
        }


        public IList<BudgetAreaModelView> GetFullModelByUserID()
        {
            var currentUserID = UserInfo.Current.ID;
            List<BudgetAreaModelView> sections;

            if (cache.TryGetValue(typeof(BudgetAreaModelView).Name + "_" + currentUserID, out sections) == false)
            {
                sections = repository.GetAll<BudgetArea>(x => x.UserID == currentUserID)
                .Select(x => new BudgetAreaModelView
                {
                    ID = x.ID,
                    Name = x.Name,
                    Owner = x.User.Name,
                    Description = x.Description,
                    IsShowOnSite = x.IsShowOnSite,
                    IsShowInCollective = x.IsShowInCollective,
                    Sections = x.BudgetSectinos
                        .OrderBy(p => p.ID)
                        .Select(y => new BudgetSectionModelView
                        {
                            ID = y.ID,
                            SectionTypeID = y.SectionTypeID,
                            SectionTypeName = y.SectionTypeID != null ? y.SectionType.Name : null,
                            Name = y.Name,
                            Description = y.Description,
                            CssColor = y.CssColor,
                            CssBackground = y.CssBackground,
                            CssIcon = y.CssIcon,
                            IsShowOnSite = y.IsShowOnSite,
                            IsShowInCollective = y.IsShowInCollective,
                            AreaID = y.BudgetAreaID,
                            AreaName = y.BudgetArea.Name,
                            Owner = x.User.Name,
                            CanEdit = x.UserID == currentUserID,
                            HasRecords = y.BudgetRecords.Any(q => q.IsDeleted != true),
                            IsShow_Filtered = true,
                            IsShow = true,
                            //CollectiveSections = y.CollectiveSections
                            //.Select(z => new BudgetSectionModelView
                            //{
                            //    ID = z.ChildSectionID ?? 0,
                            //    Name = z.ChildSection.Name,
                            //    AreaName = z.ChildSection.BudgetArea.Name
                            //}),
                        })
                })
                .ToList();

                cache.Set(typeof(BudgetAreaModelView).Name + "_" + currentUserID, sections, DateTime.Now.AddDays(1));
            }
            return sections;
        }

        public async Task<IEnumerable<SectionLightModelView>> GetAllSectionByUser()
        {
            var currentUserID = UserInfo.Current.ID;
            List<SectionLightModelView> sections;

            if (cache.TryGetValue(typeof(SectionLightModelView).Name + "_" + currentUserID, out sections) == false)
            {
                sections = await repository.GetAll<BudgetSection>(x => x.BudgetArea.UserID == UserInfo.Current.ID && x.IsShowOnSite)
                 .Select(x => new SectionLightModelView
                 {
                     ID = x.ID,
                     Name = x.Name,
                     BudgetAreaID = x.BudgetArea.ID,
                     BudgetAreaname = x.BudgetArea.Name,
                     CssBackground = x.CssBackground,
                     SectionTypeID = x.SectionTypeID,
                 })
                 .ToListAsync();

                cache.Set(typeof(SectionLightModelView).Name + "_" + currentUserID, sections, DateTime.Now.AddDays(1));
            }

            return sections;
        }

        public IEnumerable<BudgetSectionModelView> GetAllSectionForRecords()
        {
            var currentUserID = UserInfo.Current.ID;
            List<BudgetSectionModelView> sections;

            if (cache.TryGetValue(typeof(BudgetSection).Name + "_" + currentUserID, out sections) == false)
            {
                sections = repository.GetAll<BudgetSection>(x => x.BudgetArea.UserID == currentUserID && x.IsShowOnSite)
                    .OrderByDescending(x => x.BudgetRecords.Count())
                    .Select(x => new BudgetSectionModelView
                    {
                        ID = x.ID,
                        Name = x.Name,
                        Description = x.Description,
                        CssIcon = x.CssIcon,
                        CssColor = x.CssColor,
                        CssBackground = x.CssBackground,
                        AreaName = x.BudgetArea.Name,
                        AreaID = x.BudgetAreaID,
                        RecordCount = x.BudgetRecords.Count(),
                        IsShow_Filtered = true,
                        IsShow = true,
                        SectionTypeID = x.SectionTypeID,
                        Tags = x.BudgetRecords.SelectMany(y => y.Tags).GroupBy(y => y.UserTag).Select(y => new TagSectionModelView { ID = y.Key.ID, Title= y.Key.Title, Count = y.Count() }).OrderByDescending(y => y.Count).ToList()
                        //CollectiveSections = x.CollectiveSections.Select(y => new BudgetSectionModelView
                        //{
                        //    ID = y.ChildSection.ID,
                        //    CanEdit = false,
                        //    Description = y.ChildSection.Description,
                        //    Name = y.ChildSection.Name,
                        //    Owner = y.ChildSection.BudgetArea.User.Name,
                        //    IsShowOnSite = true,
                        //})
                    })
                    .ToList();

                cache.Set(typeof(BudgetSection).Name + "_" + currentUserID, sections, DateTime.Now.AddDays(1));
            }
            return sections;
        }

        public IEnumerable<AreaLightModelView> GetAllAreaAndSectionByPerson()
        {
            var currentUserID = UserInfo.Current.ID;
            List<AreaLightModelView> sections;

            if (cache.TryGetValue(typeof(AreaLightModelView).Name + "_" + currentUserID, out sections) == false)
            {
                sections = repository.GetAll<BudgetArea>(x => x.UserID == currentUserID && x.IsShowOnSite)
                    .Select(x => new AreaLightModelView
                    {
                        ID = x.ID,
                        Name = x.Name,
                        BudgetSections = x.BudgetSectinos
                            .Where(y => y.BudgetArea.UserID == currentUserID)
                            .Select(y => new SectionLightModelView
                            {
                                ID = y.ID,
                                Name = y.Name,
                            })
                    })
                    .ToList();

                cache.Set(typeof(AreaLightModelView).Name + "_" + currentUserID, sections, DateTime.Now.AddDays(1));
            }
            return sections;
        }

        public BudgetAreaModelView CreateOrUpdateArea(BudgetAreaModelView area)
        {
            var currentUser = UserInfo.Current;
            var budgetArea = new BudgetArea
            {
                ID = area.ID,
                Description = area.Description,
                Name = area.Name,
                UserID = currentUser.ID,
                IsShowInCollective = area.IsShowInCollective,
                IsShowOnSite = area.IsShowOnSite,
            };
            if (budgetArea.ID > 0)
            {
                area.IsUpdated = true;
                repository.Update(budgetArea, true);
                userLogService.CreateUserLog(currentUser.UserSessionID, UserLogActionType.Area_Edit);
            }
            else
            {
                repository.Create(budgetArea, true);
                userLogService.CreateUserLog(currentUser.UserSessionID, UserLogActionType.Area_Create);
            }

            area.ID = budgetArea.ID;

            cache.Remove(typeof(AreaLightModelView).Name + "_" + currentUser.ID);
            cache.Remove(typeof(BudgetSection).Name + "_" + currentUser.ID);
            cache.Remove(typeof(SectionLightModelView).Name + "_" + currentUser.ID);
            cache.Remove(typeof(BudgetAreaModelView).Name + "_" + currentUser.ID);

            return area;
        }

        public async Task<BudgetSectionModelView> CreateOrUpdateSection(BudgetSectionModelView section)
        {
            var currentUser = UserInfo.Current;
            var budgetSection = new BudgetSection
            {
                ID = section.ID,
                CssIcon = section.CssIcon,
                CssColor = section.CssColor,
                CssBackground = section.CssBackground,
                Description = section.Description,
                Name = section.Name,
                BudgetAreaID = section.AreaID,
                SectionTypeID = section.SectionTypeID,
                IsShowInCollective = section.IsShowInCollective,
                IsShowOnSite = section.IsShowOnSite,
            };
            if (budgetSection.ID > 0)
            {
                List<int> errorLogEditIDs = new List<int>();
                try
                {
                    await repository.UpdateAsync(budgetSection, true);
                    section.IsSaved = true;
                }
                catch (Exception ex)
                {
                    errorLogEditIDs.Add(await userLogService.CreateErrorLogAsync(currentUser.UserSessionID, "BudgetRecord_Edit", ex));
                }
                await userLogService.CreateUserLogAsync(currentUser.UserSessionID, UserLogActionType.Section_Edit, errorLogIDs: errorLogEditIDs);

                section.IsUpdated = true;
            }
            else
            {
                List<int> errorLogCreateIDs = new List<int>();
                try
                {
                    await repository.CreateAsync(budgetSection, true);
                    section.IsSaved = true;
                }
                catch (Exception ex)
                {
                    errorLogCreateIDs.Add(await userLogService.CreateErrorLogAsync(currentUser.UserSessionID, "BudgetRecord_Create", ex));
                }
                await userLogService.CreateUserLogAsync(currentUser.UserSessionID, UserLogActionType.Section_Create, errorLogIDs: errorLogCreateIDs);

                section.IsUpdated = false;
            }

            await SaveIncludedSection(section.ID, section.CollectiveSections.Select(x => x.ID).ToList());

            section.ID = budgetSection.ID;
            section.AreaID = budgetSection.BudgetAreaID;

            cache.Remove(typeof(AreaLightModelView).Name + "_" + currentUser.ID);
            cache.Remove(typeof(BudgetSection).Name + "_" + currentUser.ID);
            cache.Remove(typeof(SectionLightModelView).Name + "_" + currentUser.ID);
            cache.Remove(typeof(BudgetAreaModelView).Name + "_" + currentUser.ID);

            return section;
        }

        public async Task<int> SaveIncludedSection(int sectionID, List<int> includedSections)
        {
            var section = await repository.GetAll<BudgetSection>(x => x.ID == sectionID)
                .Include(x => x.CollectiveSections)
                .FirstOrDefaultAsync();

            if (includedSections != null && includedSections.Count > 0)
            {
                if (section.CollectiveSections.Count() > 0)
                {
                    repository.DeleteRange(section.CollectiveSections);
                }

                List<CollectiveSection> collectiveSections = new List<CollectiveSection>();

                foreach (var includedArea in includedSections)
                {
                    collectiveSections.Add(new CollectiveSection { SectionID = sectionID, ChildSectionID = includedArea });
                }
                section.CollectiveSections = collectiveSections;
            }
            else
            {
                if (section != null)
                {
                    repository.DeleteRange(section.CollectiveSections);
                }
            }
            //ToDo remove cache

            //cache.Remove(typeof(AreaLightModelView).Name + "_" + currentUser.ID);
            //cache.Remove(typeof(BudgetSection).Name + "_" + currentUser.ID);
            //cache.Remove(typeof(SectionLightModelView).Name + "_" + currentUser.ID);
            //cache.Remove(typeof(BudgetAreaModelView).Name + "_" + currentUser.ID);

            return await repository.SaveAsync();
        }

        public async Task<List<int>> GetCollectionSectionIDsBySectionID(List<int> sectionIDs)
        {
            return await repository.GetAll<CollectiveSection>(x => sectionIDs.Contains(x.SectionID ?? 0))
             .Select(x => x.ChildSectionID ?? 0)
             .ToListAsync();
        }


        #region Deletes
        public async Task<Tuple<bool, bool, string>> DeleteArea(int areaID)
        {
            var currentUser = UserInfo.Current;
            var budgetArea = await repository.GetAll<BudgetArea>()
                .Where(x => x.ID == areaID && x.UserID == currentUser.ID)
                .FirstOrDefaultAsync();

            if (budgetArea != null && (budgetArea.BudgetSectinos == null || budgetArea.BudgetSectinos.Count() == 0))
            {
                try
                {
                    await repository.DeleteAsync(budgetArea, true);
                    await userLogService.CreateUserLogAsync(currentUser.UserSessionID, UserLogActionType.Area_Delete);

                    cache.Remove(typeof(AreaLightModelView).Name + "_" + currentUser.ID);
                    cache.Remove(typeof(BudgetSection).Name + "_" + currentUser.ID);
                    cache.Remove(typeof(SectionLightModelView).Name + "_" + currentUser.ID);
                    cache.Remove(typeof(BudgetAreaModelView).Name + "_" + currentUser.ID);
                }
                catch (Exception ex)
                {
                    return new Tuple<bool, bool, string>(false, false, "Не удалось удалить область");
                }
                return new Tuple<bool, bool, string>(true, true, "Удаление прошло успешно");
            }
            return new Tuple<bool, bool, string>(true, false, "Не удалось удалить область");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sectionID"></param>
        /// <returns>
        /// 1- isOk
        /// 2- wasDeleted
        /// 3- text 
        /// </returns>
        public async Task<Tuple<bool, bool, string>> DeleteSection(int sectionID)
        {
            string s = "";
            bool hasTemplates = false;
            var currentUser = UserInfo.Current;
            var budgetSection = await repository.GetAll<BudgetSection>()
                .Where(x => x.ID == sectionID && x.BudgetArea.UserID == currentUser.ID)
                .FirstOrDefaultAsync();

            if (budgetSection != null && budgetSection.TemplateBudgetSections != null || budgetSection.TemplateBudgetSections.Count() > 0)
            {
                hasTemplates = true;
                s += "Эта категория используется в шаблонах.";
            }
            else if (budgetSection != null && budgetSection.SectionGroupLimits != null || budgetSection.SectionGroupLimits.Count() > 0)
            {
                s += " Эта категоия используется в лимитах";
            }

            if (budgetSection != null &&
                (budgetSection.BudgetRecords == null || budgetSection.BudgetRecords.Count() == 0 || budgetSection.BudgetRecords.Count(x => x.IsDeleted == false) == 0))
            {
                try
                {
                    if (budgetSection.BudgetRecords.Count(x => x.IsDeleted == true) > 0)
                    {
                        await repository.DeleteRangeAsync(budgetSection.BudgetRecords, true);
                    }
                    await repository.DeleteAsync(budgetSection, true);
                    await userLogService.CreateUserLogAsync(currentUser.UserSessionID, UserLogActionType.Section_Delete);

                    cache.Remove(typeof(AreaLightModelView).Name + "_" + currentUser.ID);
                    cache.Remove(typeof(BudgetSection).Name + "_" + currentUser.ID);
                    cache.Remove(typeof(SectionLightModelView).Name + "_" + currentUser.ID);
                    cache.Remove(typeof(BudgetAreaModelView).Name + "_" + currentUser.ID);

                }
                catch (Exception ex)
                {
                    return new Tuple<bool, bool, string>(false, false, "Не удалось удалить категорию " + s);
                }

                //Remove section from Template.column formula
                var templateColumns = await repository.GetAll<TemplateColumn>(x => x.Template.UserID == currentUser.ID && x.Formula.Contains($":{sectionID},"))
                       .ToListAsync();
                foreach (var templateColumn in templateColumns)
                {
                    try
                    {
                        templateColumn.Formula = commonService.GenerateFormulaBySections(
                            templateColumn.TemplateBudgetSections.Select(x =>
                            new TemplateBudgetSectionPlusViewModel
                            {
                                BudgetSectionID = x.BudgetSectionID,
                                Name = x.BudgetSection.Name,
                            })
                            .ToList());
                        await repository.UpdateAsync(templateColumn, true);
                    }
                    catch (Exception ex)
                    {

                    }
                }
                return new Tuple<bool, bool, string>(true, true, "Удаление прошло успешно");
            }
            return new Tuple<bool, bool, string>(true, false, "Не удалось удалить категорию");
        }
        #endregion
    }
}
