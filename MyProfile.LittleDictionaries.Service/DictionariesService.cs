using Microsoft.EntityFrameworkCore;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView.Currency;
using MyProfile.Entity.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyProfile.LittleDictionaries.Service
{
	public class DictionariesService
	{
		private IBaseRepository repository;

		public DictionariesService(IBaseRepository repository)
		{
			this.repository = repository;
		}

		public List<PeriodType> GetPeriodTypes()
		{
			return repository.GetAll<PeriodType>()
				.Where(x => x.ID == (int)PeriodTypesEnum.Month || x.ID == (int)PeriodTypesEnum.Year)
				.ToList();
		}

		public List<DictionariesModelView> GetTotalActions()
		{
			List<DictionariesModelView> z = new List<DictionariesModelView>();

			z.Add(new DictionariesModelView
			{
				TemplateColumnType = TemplateColumnType.BudgetSection,
				Variants = new List<DictionaryItem>
				{
					new DictionaryItem
					{
						FooterActionTypeID = FooterActionType.Undefined, Name = "Not selected",
					},
					new DictionaryItem
					{
						FooterActionTypeID = FooterActionType.Sum, Name = "SUM",
					},new DictionaryItem
					{
						FooterActionTypeID = FooterActionType.Avr, Name = "AVR",
					},
					new DictionaryItem
					{
						FooterActionTypeID = FooterActionType.Max, Name = "MAX",
					},
					new DictionaryItem
					{
						FooterActionTypeID = FooterActionType.Min, Name = "MIN",
					}
				}
			});
			return z;
		}

		[Obsolete("ToDo will take this list from cache")]
		public async Task<object> GetCurrencyInfoForClient()
		{
			var currencies = await repository.GetAll<Currency>()
				 .Select(x => new CurrencyClientModelView
				 {
					 CodeName = x.CodeName,
					 CodeName_CBR = x.CodeName_CBR,
					 CodeNumber_CBR = x.CodeNumber_CBR,
					 Icon = x.Icon,
					 ID = x.ID,
					 Name = x.Name,
					 SpecificCulture = x.SpecificCulture,
					 CBR_Link = "http://www.cbr.ru/scripts/XML_daily.asp?date_req="
				 })
				 .ToListAsync();

			return currencies;
		}
	}
	public class DictionariesModelView
	{
		public TemplateColumnType TemplateColumnType { get; set; }
		public List<DictionaryItem> Variants { get; set; }
	}
	public class DictionaryItem
	{
		public FooterActionType FooterActionTypeID { get; set; }
		public string Name { get; set; }

	}
}
