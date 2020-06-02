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

namespace MyProfile.Budget.Service
{
	public class SectionService
	{
		private IBaseRepository repository;

		public SectionService(IBaseRepository repository)
		{
			this.repository = repository;
		}


		public async Task<List<BudgetAreaModelView>> GetFullModel()
		{
			var thisMonth = DateTime.Now.AddDays(-DateTime.Now.Day);
			//var predicate = PredicateBuilder.True<BudgetArea>().CommonFilter();

			var areas = await GetFullModelByUserID(UserInfo.Current.ID);

			if (UserInfo.Current.IsAllowCollectiveBudget && UserInfo.Current.CollectiveUserIDs.Count > 0)
			{
				List<BudgetAreaModelView> collectiveAreas = new List<BudgetAreaModelView>();

				foreach (var UserID in UserInfo.Current.CollectiveUserIDs)
				{
					collectiveAreas.AddRange(await GetFullModelByUserID(UserID));
				}

				var allIncludedArea = areas.SelectMany(x => x.CollectiveAreas).Select(y => y.ID).ToList();

				for (int i = 0; i < collectiveAreas.Count; i++)
				{
					if (allIncludedArea.Any(x => x == collectiveAreas[i].ID))
					{
						var mainArea = areas.FirstOrDefault(x => x.CollectiveAreas.Any(y => y.ID == collectiveAreas[i].ID));
						var includedArea = mainArea.CollectiveAreas.FirstOrDefault(y => y.ID == collectiveAreas[i].ID);
						includedArea.Name = collectiveAreas[i].Name;
						includedArea.Description = collectiveAreas[i].Description;
						includedArea.Owner = collectiveAreas[i].Owner;
						includedArea.CanEdit = false;

						//var removeSectionID = new List<int>();

						foreach (var collectiveAreaSection in collectiveAreas[i].Sections)
						{
							if (mainArea.Sections.SelectMany(y => y.CollectiveSections).Any(x => x.ID == collectiveAreaSection.ID))
							{
								var includedAreaWithSection = mainArea.Sections.FirstOrDefault(y => y.CollectiveSections.Any(x => x.ID == collectiveAreaSection.ID));
								var includedCollectiveSection = includedAreaWithSection.CollectiveSections.FirstOrDefault(y => y.ID == collectiveAreaSection.ID);

								includedCollectiveSection.Name = collectiveAreaSection.Name;
								includedCollectiveSection.Description = collectiveAreaSection.Description;
								includedCollectiveSection.Owner = collectiveAreaSection.Owner;
								includedCollectiveSection.CanEdit = false;
							}
							else
							{
								mainArea.Sections.Add(collectiveAreaSection);
							}
						}
					}
					else
					{
						areas.Add(collectiveAreas[i]);
					}
				}
			}

			return areas;
		}

		private async Task<List<BudgetAreaModelView>> GetFullModelByUserID(Guid UserID)
		{
			var areas = await repository.GetAll<BudgetArea>(x => x.UserID == UserID)
				.Select(x => new BudgetAreaModelView
				{
					UserID = x.UserID ?? Guid.Empty,
					ID = x.ID,
					Name = x.Name,
					CssIcon = x.CssIcon,
					Owner = x.User.Name,
					CanEdit = x.UserID == UserInfo.UserID,
					Description = x.Description,
					CollectiveAreas = x.CollectiveAreas.Select(y => new BudgetAreaModelView { ID = y.ChildAreaID ?? 0, Name = y.ChildArea.Name }).ToList(),
					Sections = x.BudgetSectinos
						.Select(y => new BudgetSectionModelView
						{
							UserID = y.UserID ?? Guid.Empty,
							ID = y.ID,
							SectionTypeID = y.SectionTypeID,
							SectionTypeName = y.SectionTypeID != null ? y.SectionType.Name : null,
							Name = y.Name,
							Description = y.Description,
							CssColor = y.CssColor,
							CssIcon = y.CssIcon,
							IsShow = y.IsShow,
							AreaID = y.BudgetAreaID,
							AreaName = y.BudgetArea.Name,
							Owner = x.User.Name,
							CanEdit = x.UserID == UserInfo.UserID,
							CollectiveSections = y.CollectiveSections.Select(z => new BudgetSectionModelView { ID = z.ChildSectionID ?? 0, Name = z.ChildSection.Name }).ToList(),
						}).ToList()
				})
				.ToListAsync();

			return areas;
		}

		public async Task<int> SaveIncludedArea(int areaID, List<int> includedAreas)
		{
			var area = await repository.GetAll<BudgetArea>(x => x.ID == areaID)
				.Include(x => x.CollectiveAreas)
				.FirstOrDefaultAsync();

			if (includedAreas != null && includedAreas.Count > 0)
			{
				if (area.CollectiveAreas.Count() > 0)
				{
					repository.DeleteRange(area.CollectiveAreas);
				}

				List<CollectiveArea> collectiveAreas = new List<CollectiveArea>();

				foreach (var includedArea in includedAreas)
				{
					collectiveAreas.Add(new CollectiveArea { AreaID = areaID, ChildAreaID = includedArea });
				}
				area.CollectiveAreas = collectiveAreas;
			}
			else
			{
				repository.DeleteRange(area.CollectiveAreas);
			}

			return await repository.SaveAsync();
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
				repository.DeleteRange(section.CollectiveSections);
			}

			return await repository.SaveAsync();
		}

		public async Task<List<Select2Item>> GetSectionsForSelect2()
		{
			return await repository.GetAll<BudgetArea>(x => x.UserID == UserInfo.UserID || x.UserID == null)
				.SelectMany(x => x.BudgetSectinos)
				.Select(x => new Select2Item
				{
					id = x.ID,
					text = x.Name,
				})
				.ToListAsync();
		}

		public async Task<List<int>> GetCollectionSectionBySectionID(List<int> sectionIDs)
		{
			return await repository.GetAll<CollectiveSection>(x => sectionIDs.Contains(x.SectionID ?? 0))
			 .Select(x => x.ChildSectionID ?? 0)
			 .ToListAsync();
		}
	}
}
