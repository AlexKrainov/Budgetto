using DynamicExpresso;
using LinqKit;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
using MyProfile.User.Service;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MyProfile.Budget.Service
{
	public class BudgetService
	{
		private IBaseRepository repository;
		private CollectionUserService collectionUserService;

		public BudgetService(IBaseRepository repository)
		{
			this.repository = repository;
			this.collectionUserService = new CollectionUserService(repository);
		}
		#region Budget table view

		/// <summary>
		/// Dynamic calculation
		/// https://stackoverflow.com/questions/12431286/calculate-result-from-a-string-expression-dynamically
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <param name="template"></param>
		/// <returns></returns>
		public Tuple<List<List<Cell>>, List<Cell>> GetBudgetData(DateTime from, DateTime to, TemplateViewModel template)
		{
			List<List<Cell>> rows = new List<List<Cell>>();
			List<List<FooterCell>> footers = new List<List<FooterCell>>();

			if (template == null || template.Columns.Count == 0)
			{
				return new Tuple<List<List<Cell>>, List<Cell>>(rows, new List<Cell>());
			}

			int allColumnsCount = template.Columns.Count;
			int indexBudgetRecords = 0;
			int totalCounter = 0;
			IList<IGrouping<int, TmpBudgetRecord>> budgetRecords;
			List<string> columnsFormula = GetColumnsFormula(template);

			switch (template.PeriodTypeID)
			{
				case (int)PeriodTypesEnum.Year:
					budgetRecords = GetBudgetRecords(from, to, x => x.DateTimeOfPayment.Month);
					totalCounter = 12;
					break;
				case (int)PeriodTypesEnum.Years10:
					budgetRecords = GetBudgetRecords(from, to, x => x.DateTimeOfPayment.Year);
					totalCounter = 10;
					break;
				case (int)PeriodTypesEnum.Month:
				default:
					budgetRecords = GetBudgetRecords(from, to, x => x.DateTimeOfPayment.Day);
					totalCounter = DateTime.DaysInMonth(from.Year, from.Month);
					break;
			}
			if (UserInfo.Current.IsAllowCollectiveBudget && UserInfo.Current.UserSettings.BudgetPages_WithCollective)
			{
				AddCollectionRecords(budgetRecords);
			}

			for (int dateCounter = 1; dateCounter <= totalCounter; dateCounter++)
			{
				List<Cell> cells = new List<Cell>();
				List<FooterCell> footerCells = new List<FooterCell>();

				if (budgetRecords.Count != indexBudgetRecords && budgetRecords.FirstOrDefault(x => x.Key == dateCounter) != null)
				{
					var budgetRecordsDay = budgetRecords.FirstOrDefault(x => x.Key == dateCounter);
					indexBudgetRecords++;

					for (int i = 0; i < allColumnsCount; i++)
					{
						var column = template.Columns[i];
						string expression = columnsFormula[i];

						if (column.TemplateColumnType == TemplateColumnType.BudgetSection)
						{
							decimal total = 0;

							foreach (var formulaItem in column.Formula)
							{
								total = 0;
								if (formulaItem.Type == FormulaFieldType.Section)
								{
									total = budgetRecordsDay
									.Where(x => x.SectionID == formulaItem.ID)
									.Sum(x => x.Total);

									expression = expression.Replace($"[{formulaItem.ID}]", total.ToString());
								}
							}

							var interpreter = new Interpreter();
							total = interpreter.Eval<decimal>(expression.Replace(",", "."));//becase the Interpreter doesn't understand , (comma)
							total = Math.Round(total, column.PlaceAfterCommon);
							//total = CSharpScript.EvaluateAsync<decimal>(expression).Result;

							cells.Add(new Cell { Value = total.ToString("C", CultureInfo.CreateSpecificCulture("ru_RU")), NaturalValue = total, IsShow = column.IsShow, TemplateColumnType = column.TemplateColumnType, dateCounter = dateCounter });
							footerCells.Add(new FooterCell { Value = total.ToString("C", CultureInfo.CreateSpecificCulture("ru_RU")), NaturalValue = total });
						}
						else if (column.TemplateColumnType == TemplateColumnType.DaysForMonth)
						{
							string v = SetFormatForDate(new DateTime(from.Year, from.Month, dateCounter), column.Format, column.TemplateColumnType);

							cells.Add(new Cell { Value = v, IsShow = column.IsShow, TemplateColumnType = column.TemplateColumnType, dateCounter = dateCounter });
							footerCells.Add(new FooterCell { Value = "0", NaturalValue = 0 });
						}
						else if (column.TemplateColumnType == TemplateColumnType.MonthsForYear)
						{
							string v = SetFormatForDate(new DateTime(from.Year, dateCounter, 1), column.Format, column.TemplateColumnType);
							cells.Add(new Cell { Value = (new DateTime(from.Year, dateCounter, 1)).ToString("MM.yyyy"), IsShow = column.IsShow, TemplateColumnType = column.TemplateColumnType, dateCounter = dateCounter });
							footerCells.Add(new FooterCell { Value = "0", NaturalValue = 0 });
						}
					}
				}
				else
				{
					for (int i = 0; i < allColumnsCount; i++)
					{
						var column = template.Columns[i];
						string v = "0";

						//if (column.TemplateColumnType == TemplateColumnType.BudgetSection)
						if (column.TemplateColumnType == TemplateColumnType.DaysForMonth)
						{
							v = SetFormatForDate(new DateTime(from.Year, from.Month, dateCounter), column.Format, column.TemplateColumnType);
						}
						else if (column.TemplateColumnType == TemplateColumnType.MonthsForYear)
						{
							v = SetFormatForDate(new DateTime(from.Year, dateCounter, 1), column.Format, column.TemplateColumnType);
						}

						cells.Add(new Cell { Value = v, IsShow = column.IsShow, TemplateColumnType = column.TemplateColumnType, dateCounter = dateCounter });
						footerCells.Add(new FooterCell { Value = "0", NaturalValue = 0 });
					}
				}
				rows.Add(cells);
				footers.Add(footerCells);
			}

			return new Tuple<List<List<Cell>>, List<Cell>>(
				rows,
				CalculateFooter(footers, template));
		}

		private string SetFormatForDate(DateTime dateTime, string format, TemplateColumnType templateColumnType)
		{
			string v = "";

			if (!string.IsNullOrEmpty(format))
			{
				v = dateTime.ToString(format);
			}
			else if (templateColumnType == TemplateColumnType.DaysForMonth)
			{
				v = dateTime.ToShortDateString();
			}
			else if (templateColumnType == TemplateColumnType.MonthsForYear)
			{
				v = dateTime.ToString("MM.yyyy");
			}
			else
			{
				v = dateTime.ToShortDateString();
			}

			return v;
		}

		private List<Cell> CalculateFooter(List<List<FooterCell>> footersData, TemplateViewModel template)
		{
			List<Cell> footer = new List<Cell>();
			decimal v;

			for (int i = 0; i < template.Columns.Count; i++)
			{
				List<decimal> total = new List<decimal>();
				Cell cell = new Cell() { IsShow = template.Columns[i].IsShow };

				foreach (var footerRow in footersData)
				{
					total.Add(footerRow[i].NaturalValue);
				}

				switch (template.Columns[i].TotalAction)
				{
					case FooterActionType.Sum:
						v = total.Sum();
						v = Math.Round(v, template.Columns[i].PlaceAfterCommon);
						cell.Value = v.ToString();
						break;
					case FooterActionType.Avr:
						v = total.Average();
						v = Math.Round(v, template.Columns[i].PlaceAfterCommon);
						cell.Value = v.ToString();
						break;
					case FooterActionType.Min:
						v = total.Min();
						v = Math.Round(v, template.Columns[i].PlaceAfterCommon);
						cell.Value = v.ToString();
						break;
					case FooterActionType.Max:
						v = total.Max();
						v = Math.Round(v, template.Columns[i].PlaceAfterCommon);
						cell.Value = v.ToString();
						break;
					case FooterActionType.Undefined:
					default:
						cell.Value = "";
						break;
				}
				footer.Add(cell);
			}
			return footer;
		}

		private List<string> GetColumnsFormula(TemplateViewModel template)
		{
			List<string> columnsFormula = new List<string>();

			foreach (var column in template.Columns)
			{

				if (column.TemplateColumnType == TemplateColumnType.BudgetSection)
				{
					string expression = "";
					foreach (var formulaItem in column.Formula)
					{
						if (formulaItem.Type == FormulaFieldType.Section)
						{
							expression += $"[{formulaItem.ID}]";
						}
						else
						{
							expression += formulaItem.Value;
						}


					}
					columnsFormula.Add(expression);
				}
				else
				{
					columnsFormula.Add(null);
				}
			}

			return columnsFormula;
		}

		private void AddCollectionRecords(IList<IGrouping<int, TmpBudgetRecord>> budgetRecords)
		{
			foreach (var record in budgetRecords)
			{
				foreach (var item in record)
				{
					if (item.CollectionSectionIDs.Count > 0)
					{
						item.Total += record.Where(x => item.CollectionSectionIDs.Contains(x.SectionID)).Sum(x => x.Total);
					}
				}
			}
		}

		#endregion

		public IList<IGrouping<int, TmpBudgetRecord>> GetBudgetRecords(
			DateTime from,
			DateTime to,
			Func<TmpBudgetRecord, int> groupBy,
			Expression<Func<BudgetRecord, bool>> expression = null)
		{
			return _getBudgetRecords(from, to, expression)
				.Select(x => new TmpBudgetRecord
				{
					Total = x.Total,
					DateTimeOfPayment = x.DateTimeOfPayment,
					SectionID = x.BudgetSectionID,
					SectionName = x.BudgetSection.Name,
					AreaID = x.BudgetSection.BudgetArea.ID,
					AreaName = x.BudgetSection.BudgetArea.Name,
					CollectionSectionIDs = x.BudgetSection.CollectiveSections.Select(u => u.ChildSectionID ?? 0).ToList(),
				})
			  .GroupBy(groupBy)
			  .ToList();
		}

		public IList<IGrouping<DateTime, TmpBudgetRecord>> GetBudgetRecordsByDate(
			DateTime from,
			DateTime to,
			Expression<Func<BudgetRecord, bool>> expression = null)
		{
			return _getBudgetRecords(from, to, expression)
				.Select(x => new TmpBudgetRecord
				{
					Total = x.Total,
					DateTimeOfPayment = x.DateTimeOfPayment,
					SectionID = x.BudgetSectionID,
					SectionName = x.BudgetSection.Name,
					AreaID = x.BudgetSection.BudgetArea.ID,
					AreaName = x.BudgetSection.BudgetArea.Name,
					CollectionSectionIDs = x.BudgetSection.CollectiveSections.Select(u => u.ChildSectionID ?? 0).ToList(),
				})
			  .GroupBy(x => x.DateTimeOfPayment.Date)
			  .ToList();
		}

		private IQueryable<BudgetRecord> _getBudgetRecords(
			DateTime from,
			DateTime to,
			Expression<Func<BudgetRecord, bool>> expression = null)
		{
			Guid currentUserID = UserInfo.Current.ID;
			List<Guid> allCollectiveUserIDs = collectionUserService.GetAllCollectiveUserIDs();
			var predicate = PredicateBuilder.True<BudgetRecord>();

			predicate = predicate.And(x => allCollectiveUserIDs.Contains(x.UserID)
				  && from <= x.DateTimeOfPayment && to >= x.DateTimeOfPayment
				  && x.IsDeleted == false
				  && (x.UserID != currentUserID ? x.IsHideForCollection == false : true));

			if (expression != null)
			{
				predicate = predicate.And(expression);
			}
			return repository.GetAll(predicate);
		}


		public async Task<IList<BudgetRecordModelView>> GetBudgetRecordsByCalendarFilter(CalendarFilterModels filter)
		{
			Guid currentUserID = UserInfo.Current.ID;
			var expression = PredicateBuilder.True<BudgetRecord>();

			if (filter.IsConsiderCollection)
			{
				List<Guid> allCollectiveUserIDs = await collectionUserService.GetAllCollectiveUserIDsAsync();
				expression = expression.And(x => allCollectiveUserIDs.Contains(x.UserID));
			}
			else
			{
				expression = expression.And(x => x.UserID == currentUserID);
			}

			expression = expression.And(x => filter.StartDate <= x.DateTimeOfPayment && filter.EndDate >= x.DateTimeOfPayment
				  && filter.Sections.Contains(x.BudgetSectionID)
				  && (x.UserID != currentUserID ? x.IsHideForCollection == false : true)
				  && x.IsDeleted == false);

			return await repository
			  .GetAll(expression)
			  .Select(x => new BudgetRecordModelView
			  {
				  ID = x.ID,
				  DateTimeCreate = x.DateTimeCreate,
				  DateTimeEdit = x.DateTimeEdit,
				  Description = x.Description,
				  IsConsider = x.IsHide,
				  RawData = x.RawData,
				  Money = x.Total,
				  DateTimeOfPayment = x.DateTimeOfPayment,
				  SectionID = x.BudgetSectionID,
				  SectionName = x.BudgetSection.Name,
				  AreaID = x.BudgetSection.BudgetArea.ID,
				  AreaName = x.BudgetSection.BudgetArea.Name
			  })
			  .OrderByDescending(x => x.DateTimeOfPayment.Date)
			  .ToListAsync();
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
		public List<int> CollectionSectionIDs { get; set; }
	}
	public class Cell
	{
		public string Value { get; set; }
		public decimal NaturalValue { get; set; }
		public bool IsShow { get; set; } = true;
		public TemplateColumnType TemplateColumnType { get; set; }
		public int dateCounter { get; set; }
	}
	public class FooterCell : Cell
	{
	}

}
