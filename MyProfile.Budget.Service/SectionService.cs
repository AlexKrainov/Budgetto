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

			var areas = await GetFullModelByPersonID(UserInfo.Current.ID);

			if (UserInfo.Current.IsAllowCollectiveBudget && UserInfo.Current.CollectivePersonIDs.Count > 0)
			{
				List<BudgetAreaModelView> collectiveAreas = new List<BudgetAreaModelView>();

				foreach (var personID in UserInfo.Current.CollectivePersonIDs)
				{
					collectiveAreas.AddRange(await GetFullModelByPersonID(personID));
				}


				foreach (var area in areas)
				{
					if (!string.IsNullOrEmpty(area.IncludedCollectiveAreas_Raw))
					{
						var includedCollectiveAreas_Raws = JsonConvert.DeserializeObject<List<IncludedCollectiveItem>>(area.IncludedCollectiveAreas_Raw);
						area.IncludedCollectiveAreas = includedCollectiveAreas_Raws.Select(x => new BudgetAreaModelView { ID = x.id, PersonID = x.personID }).ToList();

						foreach (var section in area.Sections)
						{
							var includedCollectiveSection_Raws = JsonConvert.DeserializeObject<List<IncludedCollectiveItem>>(section.IncludedCollectiveSection_Raw);
							section.IncludedCollectiveSections = includedCollectiveSection_Raws.Select(x => new BudgetSectionModelView { ID = x.id, PersonID = x.personID }).ToList();
						}
					}
				}

				var allIncludedArea = areas.SelectMany(x => x.IncludedCollectiveAreas).Select(y => y.ID).ToList();
				//var allIncludedSection = areas.SelectMany(x => x.IncludedCollectiveAreas).SelectMany(y => y.Sections).Select(h => h.ID);

				for (int i = 0; i < collectiveAreas.Count; i++)
				{
					if (allIncludedArea.Any(x => x == collectiveAreas[i].ID))
					{
						var includeArea = areas.FirstOrDefault(x => x.IncludedCollectiveAreas.Any(y => y.ID == collectiveAreas[i].ID));
						includeArea = collectiveAreas[i];

						//var removeSectionID = new List<int>();

						foreach (var collectiveAreaSection in collectiveAreas[i].Sections)
						{
							if (includeArea.Sections.SelectMany(y => y.IncludedCollectiveSections).Any(x => x.ID == collectiveAreaSection.ID))
							{
								var includedCollectiveSection = includeArea.Sections.SelectMany(y => y.IncludedCollectiveSections).FirstOrDefault(x => x.ID == collectiveAreaSection.ID);
								includedCollectiveSection = collectiveAreaSection;
							}
							else
							{
								includeArea.Sections.Add(collectiveAreaSection);
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

		private async Task<List<BudgetAreaModelView>> GetFullModelByPersonID(Guid personID)
		{
			var thisMonth = DateTime.Now.AddDays(-DateTime.Now.Day);
			//var predicate = PredicateBuilder.True<BudgetArea>().CommonFilter();

			var areas = await repository.GetAll<BudgetArea>(x => x.PersonID == personID)
				.Select(x => new BudgetAreaModelView
				{
					PersonID = x.PersonID ?? Guid.Empty,
					ID = x.ID,
					Name = x.Name,
					CssIcon = x.CssIcon,
					Owner = x.Person.Name,
					CanEdit = x.PersonID == UserInfo.PersonID,
					Description = x.Description,
					IncludedCollectiveAreas_Raw = x.IncludedCollectiveAreas,
					Sections = x.BudgetSectinos
						.Where(y => y.PersonID == personID)
						.Select(y => new BudgetSectionModelView
						{
							PersonID = y.PersonID ?? Guid.Empty,
							ID = y.ID,
							Name = y.Name,
							Description = y.Description,
							CssColor = y.CssColor,
							CssIcon = y.CssIcon,
							IsShow = y.IsShow,
							AreaID = y.BudgetAreaID,
							AreaName = y.BudgetArea.Name,
							MoneyThisMonth = y.BudgetRecords.Where(z => z.DateTimeOfPayment >= thisMonth).Sum(q => q.Total),
							MoneyThisYear = y.BudgetRecords.Where(z => z.DateTimeOfPayment.Year == thisMonth.Year).Sum(q => q.Total),
							Money = y.BudgetRecords.Sum(q => q.Total),

							IncludedCollectiveSection_Raw = y.IncludedCollectiveSections,
							Owner = x.Person.Name,
							CanEdit = x.PersonID == UserInfo.PersonID,
						}).ToList()
				})
				.ToListAsync();

			return areas;
		}

		public async Task<int> SaveIncludedArea(int areaID, List<int> includedAreas)
		{
			var area = await repository.GetByIDAsync<BudgetArea>(areaID);

			if (includedAreas != null && includedAreas.Count > 0)
			{
				area.IncludedCollectiveAreas = JsonConvert.SerializeObject(
					repository
					.GetAll<BudgetArea>(x => includedAreas.Contains(x.ID))
					.Select(y => new IncludedCollectiveItem { id = y.ID, personID = y.PersonID ?? Guid.Empty })
					.ToList());
			}
			else
			{
				area.IncludedCollectiveAreas = null;
			}


			return await repository.SaveAsync();
		}

		public async Task<List<Select2Item>> GetSectionsForSelect2()
		{
			return await repository.GetAll<BudgetArea>(x => x.PersonID == UserInfo.PersonID || x.PersonID == null)
				.SelectMany(x => x.BudgetSectinos)
				.Select(x => new Select2Item
				{
					id = x.ID,
					text = x.Name,
				})
				.ToListAsync();
		}

	}
}
