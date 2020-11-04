using LinqKit;
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

namespace MyProfile.Budget.Service
{
    public class SectionService
    {
        private IBaseRepository repository;
        private CollectionUserService collectionUserService;
        private UserLogService userLogService;
        private CommonService commonService;

        public SectionService(IBaseRepository repository)
        {
            this.repository = repository;
            this.collectionUserService = new CollectionUserService(repository);
            this.userLogService = new UserLogService(repository);
            this.commonService = new CommonService(repository);
        }


        public IQueryable<BudgetAreaModelView> GetFullModelByUserID()
        {
            Guid userID = UserInfo.Current.ID;

            return repository.GetAll<BudgetArea>(x => x.UserID == userID)
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
                            CanEdit = x.UserID == userID,
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
                });
        }

        public async Task<IEnumerable<SectionLightModelView>> GetAllSectionByUser()
        {
            return await repository.GetAll<BudgetSection>(x => x.BudgetArea.UserID == UserInfo.Current.ID && x.IsShowOnSite)
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
        }

        public async Task<IEnumerable<BudgetSectionModelView>> GetAllSectionForRecords()
        {
            return await repository.GetAll<BudgetSection>(x => x.BudgetArea.UserID == UserInfo.Current.ID && x.IsShowOnSite)
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
                    CollectiveSections = x.CollectiveSections.Select(y => new BudgetSectionModelView
                    {
                        ID = y.ChildSection.ID,
                        CanEdit = false,
                        Description = y.ChildSection.Description,
                        Name = y.ChildSection.Name,
                        Owner = y.ChildSection.BudgetArea.User.Name,
                        IsShowOnSite = true,

                    })
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<AreaLightModelView>> GetAllAreaAndSectionByPerson()
        {
            var currentUserID = UserInfo.Current.ID;

            return await repository.GetAll<BudgetArea>(x => x.UserID == currentUserID && x.IsShowOnSite)
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
                .ToListAsync();
        }

        public async Task<BudgetAreaModelView> CreateOrUpdateArea(BudgetAreaModelView area)
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
                await repository.UpdateAsync(budgetArea, true);
                await userLogService.CreateUserLog(currentUser.UserSessionID, UserLogActionType.Area_Edit);
            }
            else
            {
                await repository.CreateAsync(budgetArea, true);
                await userLogService.CreateUserLog(currentUser.UserSessionID, UserLogActionType.Area_Create);
            }

            area.ID = budgetArea.ID;
            area.IsUpdated = true;
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
                await repository.UpdateAsync(budgetSection, true);
                await userLogService.CreateUserLog(currentUser.UserSessionID, UserLogActionType.Section_Edit);
            }
            else
            {
                await repository.CreateAsync(budgetSection, true);
                await userLogService.CreateUserLog(currentUser.UserSessionID, UserLogActionType.Section_Create);
            }

            await SaveIncludedSection(section.ID, section.CollectiveSections.Select(x => x.ID).ToList());

            section.ID = budgetSection.ID;
            section.AreaID = budgetSection.BudgetAreaID;

            section.IsUpdated = true;
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
                    await userLogService.CreateUserLog(currentUser.UserSessionID, UserLogActionType.Area_Delete);
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
                    await userLogService.CreateUserLog(currentUser.UserSessionID, UserLogActionType.Section_Delete);
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
