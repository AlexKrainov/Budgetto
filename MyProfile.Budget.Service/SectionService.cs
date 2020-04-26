using Microsoft.EntityFrameworkCore;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView;
using MyProfile.Entity.ModelView.CommonViewModels;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProfile.Budget.Service
{
	public class SectionService
	{
		private IBaseRepository repository;

		public SectionService(IBaseRepository repository)
		{
			this.repository = repository;
		}

		public async Task<List<BudgetAreaModelView>> GetSectionForEdit()
		{
			var thisMonth = DateTime.Now.AddDays(-DateTime.Now.Day);

			var sections = await repository.GetAll<BudgetArea>(x => x.PersonID == UserInfo.PersonID || x.PersonID == null)
				.Select(x => new BudgetAreaModelView
				{
					ID = x.ID,
					Name = x.Name,
					CssIcon = x.CssIcon,
					IsGlobal = x.PersonID == null,
					Description = x.Description,
					Sections = x.BudgetSectinos
						.Where(y => y.PersonID == UserInfo.PersonID || y.PersonID == null)
						.Select(y => new BudgetSecionModelView
						{
							ID = y.ID,
							Name = y.Name,
							Description = y.Description,
							CssColor = y.CssColor,
							CssIcon = y.CssIcon,
							IsGlobal = y.IsByDefault,
							AreaID = y.BudgetAreaID,
							AreaName = y.BudgetArea.Name,
							MoneyThisMonth = y.BudgetRecords.Where(z => z.DateTimeOfPayment >= thisMonth).Sum(q => q.Total),
							MoneyThisYear = y.BudgetRecords.Where(z => z.DateTimeOfPayment.Year == thisMonth.Year).Sum(q => q.Total),
							Money = y.BudgetRecords.Sum(q => q.Total),
						})
				})
				.ToListAsync();

			return sections;
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
