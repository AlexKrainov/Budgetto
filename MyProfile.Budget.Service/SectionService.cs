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

namespace MyProfile.Budget.Service
{
    public class SectionService
    {
        private IBaseRepository repository;
        private CollectionUserService collectionUserService;

        public SectionService(IBaseRepository repository)
        {
            this.repository = repository;
            this.collectionUserService = new CollectionUserService(repository);
        }


        public IQueryable<BudgetAreaModelView> GetFullModelByUserID()
        {
            Guid userID = UserInfo.Current.ID;

            return repository.GetAll<BudgetArea>(x => x.UserID == userID)
                .Select(x => new BudgetAreaModelView
                {
                    ID = x.ID,
                    Name = x.Name,
                    CssIcon = x.CssIcon,
                    Owner = x.User.Name,
                    Description = x.Description,
                    IsShowOnSite = x.IsShowOnSite,
                    IsShowInCollective = x.IsShowInCollective,
                    Sections = x.BudgetSectinos
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
                            HasRecords = y.BudgetRecords.Any(),
                            //CollectiveSections = y.CollectiveSections
                            //.Select(z => new BudgetSectionModelView
                            //{
                            //    ID = z.ChildSectionID ?? 0,
                            //    Name = z.ChildSection.Name,
                            //    AreaName = z.ChildSection.BudgetArea.Name
                            //}),
                        })
                        .OrderByDescending(p => p.Name.Length)
                });
        }

        public async Task<IEnumerable<SectionLightModelView>> GetAllSectionByPerson()
        {
            return await repository.GetAll<BudgetSection>(x => x.UserID == UserInfo.Current.ID && x.IsShowOnSite)
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
                    IsShow = true,
                    CollectiveSections = x.CollectiveSections.Select(y => new BudgetSectionModelView
                    {
                        ID = y.ChildSection.ID,
                        CanEdit = false,
                        Description = y.ChildSection.Description,
                        Name = y.ChildSection.Name,
                        Owner = y.ChildSection.User.Name,
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
            var budgetArea = new BudgetArea
            {
                ID = area.ID,
                CssIcon = area.CssIcon,
                Description = area.Description,
                Name = area.Name,
                UserID = UserInfo.Current.ID,
                IsShowInCollective = area.IsShowInCollective,
                IsShowOnSite = area.IsShowOnSite,
            };
            if (budgetArea.ID > 0)
            {
                await repository.UpdateAsync(budgetArea, true);
            }
            else
            {
                await repository.CreateAsync(budgetArea, true);
            }

            area.ID = budgetArea.ID;
            area.IsUpdated = true;
            return area;
        }

        public async Task<BudgetSectionModelView> CreateOrUpdateSection(BudgetSectionModelView section)
        {
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
                UserID = UserInfo.Current.ID,
                IsShowInCollective = section.IsShowInCollective,
                IsShowOnSite = section.IsShowOnSite,
            };
            if (budgetSection.ID > 0)
            {
                await repository.UpdateAsync(budgetSection, true);
            }
            else
            {
                await repository.CreateAsync(budgetSection, true);
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

        public async Task<List<int>> GetCollectionSectionBySectionID(List<int> sectionIDs)
        {
            return await repository.GetAll<CollectiveSection>(x => sectionIDs.Contains(x.SectionID ?? 0))
             .Select(x => x.ChildSectionID ?? 0)
             .ToListAsync();
        }
    }
}
