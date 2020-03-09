using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyProfile.Budget.Service
{
	public class BudgetService
	{

		private IBaseRepository repository;

		public BudgetService(IBaseRepository repository)
		{
			this.repository = repository;
		}

		public List<List<Cell>> GetBudgetDataByMonth(DateTime from, DateTime to, TemplateViewModel template)
		{
			var budgetRecords = GetDataByMonth(from, to);

			List<List<Cell>> rows = new List<List<Cell>>();

			int allColumnsCount = template.Columns.Count;
			int indexBudgetRecords = 0;
			int totalDays = DateTime.DaysInMonth(from.Year, from.Month);

			for (int numberOfDay = 1; numberOfDay <= totalDays; numberOfDay++)
			{
				List<Cell> cells = new List<Cell>();

				if (budgetRecords.Count != indexBudgetRecords && numberOfDay == budgetRecords[indexBudgetRecords].Key)
				{
					var budgetRecordsDay = budgetRecords[indexBudgetRecords];
					indexBudgetRecords++;

					for (int i = 0; i < allColumnsCount; i++)
					{
						var column = template.Columns[i];

						if (column.TemplateColumnType == TemplateColumnType.DaysForMonth)
						{
							cells.Add(new Cell { Value = (new DateTime(from.Year, from.Month, numberOfDay)).ToShortDateString() });
						}
						else if (column.TemplateColumnType == TemplateColumnType.BudgetSection)
						{
							decimal total = 0;

							foreach (var templateBudgetSection in column.TemplateBudgetSections)
							{
								total += budgetRecordsDay
									.Where(x => x.SectionID == templateBudgetSection.BudgetSectionID)
									.Sum(x => x.Total);
							}
							cells.Add(new Cell { Value = total.ToString() });
						}

					}
				}
				else
				{
					for (int i = 0; i < allColumnsCount; i++)
					{
						var column = template.Columns[i];

						if (column.TemplateColumnType == TemplateColumnType.DaysForMonth)
						{
							cells.Add(new Cell { Value = (new DateTime(from.Year, from.Month, numberOfDay)).ToShortDateString() });
						}
						else
						{
							cells.Add(new Cell { Value = "0" });
						}

					}
				}
				rows.Add(cells);
			}


			return rows;
		}

		public IList<IGrouping<int, TmpBudgetRecord>> GetDataByMonth(DateTime from, DateTime to)
		{
			return repository
			  .GetAll<BudgetRecord>(x => x.PersonID == UserInfo.PersonID && from <= x.DateTimeOfPayment && to >= x.DateTimeOfPayment)
			  .Select(x => new TmpBudgetRecord
			  {
				  Total = x.Total,
				  DateTimeOfPayment = x.DateTimeOfPayment,
				  SectionID = x.BudgetSectionID,
				  SectionName = x.BudgetSection.Name,
				  AreaID = x.BudgetSection.BudgetArea.ID,
				  AreaName = x.BudgetSection.BudgetArea.Name
			  })
			  .GroupBy(x => x.DateTimeOfPayment.Day)
			  .ToList();
		}
	}
	public class TmpBudgetRecord
	{
		public decimal Total { get; set; }
		public DateTime DateTimeOfPayment { get; set; }
		public int SectionID { get; set; }
		public string SectionName { get; set; }
		public int AreaID { get; set; }
		public string AreaName { get; set; }
	}
	public class Cell
	{
		public string Value { get; set; }
	}
}
