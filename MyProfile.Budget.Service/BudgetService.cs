using DynamicExpresso;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
using System;
using System.Collections.Generic;
using System.Globalization;
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
				case (int)PeriodTypesEnum.Months:
					budgetRecords = GetBudgetRecords(from, to, x => x.DateTimeOfPayment.Month);
					totalCounter = 12;
					break;
				case (int)PeriodTypesEnum.Years10:
					budgetRecords = GetBudgetRecords(from, to, x => x.DateTimeOfPayment.Year);
					totalCounter = 10;
					break;
				case (int)PeriodTypesEnum.Days:
				default:
					budgetRecords = GetBudgetRecords(from, to, x => x.DateTimeOfPayment.Day);
					totalCounter = DateTime.DaysInMonth(from.Year, from.Month);
					break;
			}

			for (int dateTimeCounter = 1; dateTimeCounter <= totalCounter; dateTimeCounter++)
			{
				List<Cell> cells = new List<Cell>();
				List<FooterCell> footerCells = new List<FooterCell>();

				if (budgetRecords.Count != indexBudgetRecords && budgetRecords.FirstOrDefault(x => x.Key == dateTimeCounter) != null)
				{
					var budgetRecordsDay = budgetRecords.FirstOrDefault(x => x.Key == dateTimeCounter);
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

							cells.Add(new Cell { Value = total.ToString("C", CultureInfo.CreateSpecificCulture("ru_RU")), NaturalValue = total, IsShow = column.IsShow });
							footerCells.Add(new FooterCell { Value = total });
						}
						else if (column.TemplateColumnType == TemplateColumnType.DaysForMonth)
						{
							string v = SetFormatForDate(new DateTime(from.Year, from.Month, dateTimeCounter), column.Format, column.TemplateColumnType);

							cells.Add(new Cell { Value = v, IsShow = column.IsShow });
							footerCells.Add(new FooterCell { Value = 0 });
						}
						else if (column.TemplateColumnType == TemplateColumnType.MonthsForYear)
						{
							string v = SetFormatForDate(new DateTime(from.Year, dateTimeCounter, 1), column.Format, column.TemplateColumnType);
							cells.Add(new Cell { Value = (new DateTime(from.Year, dateTimeCounter, 1)).ToString("MM.yyyy"), IsShow = column.IsShow });
							footerCells.Add(new FooterCell { Value = 0 });
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
							v = SetFormatForDate(new DateTime(from.Year, from.Month, dateTimeCounter), column.Format, column.TemplateColumnType);
						}
						else if (column.TemplateColumnType == TemplateColumnType.MonthsForYear)
						{
							v = SetFormatForDate(new DateTime(from.Year, dateTimeCounter, 1), column.Format, column.TemplateColumnType);
						}

						cells.Add(new Cell { Value = v, IsShow = column.IsShow });
						footerCells.Add(new FooterCell { Value = 0 });
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
					total.Add(footerRow[i].Value);
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

		public IList<IGrouping<int, TmpBudgetRecord>> GetBudgetRecords(DateTime from, DateTime to, Func<TmpBudgetRecord, int> expresion)
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
			  .GroupBy(expresion)
			  .ToList();
		}

		private Func<TmpBudgetRecord, object> get(Func<object, object> p)
		{
			throw new NotImplementedException();
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
		public decimal NaturalValue { get; set; }
		public bool IsShow { get; set; } = true;
	}
	public class FooterCell
	{
		public decimal Value { get; set; }
		public bool IsShow { get; internal set; } = true;
	}

}
