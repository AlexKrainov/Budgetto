using DynamicExpresso;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
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

		/// <summary>
		/// Dynamic calculation
		/// https://stackoverflow.com/questions/12431286/calculate-result-from-a-string-expression-dynamically
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <param name="template"></param>
		/// <returns></returns>
		public Tuple<List<List<Cell>>, List<Cell>> GetBudgetDataByMonth(DateTime from, DateTime to, TemplateViewModel template)
		{
			var budgetRecords = GetDataByMonth(from, to);

			List<List<Cell>> rows = new List<List<Cell>>();
			List<List<FooterCell>> footers = new List<List<FooterCell>>();

			List<string> columnsFormula = GetColumnsFormula(template);

			int allColumnsCount = template.Columns.Count;
			int indexBudgetRecords = 0;
			int totalDays = DateTime.DaysInMonth(from.Year, from.Month);

			for (int numberOfDay = 1; numberOfDay <= totalDays; numberOfDay++)
			{
				List<Cell> cells = new List<Cell>();
				List<FooterCell> footerCells = new List<FooterCell>();

				if (budgetRecords.Count != indexBudgetRecords && numberOfDay == budgetRecords[indexBudgetRecords].Key)
				{
					var budgetRecordsDay = budgetRecords[indexBudgetRecords];
					indexBudgetRecords++;

					for (int i = 0; i < allColumnsCount; i++)
					{
						var column = template.Columns[i];
						string expression = columnsFormula[i];

						if (column.TemplateColumnType == TemplateColumnType.DaysForMonth)
						{
							cells.Add(new Cell { Value = (new DateTime(from.Year, from.Month, numberOfDay)).ToShortDateString(), IsShow = column.IsShow });
							footerCells.Add(new FooterCell { Value = numberOfDay });
						}
						else if (column.TemplateColumnType == TemplateColumnType.BudgetSection)
						{
							decimal total = 0;

							foreach (var formulaItem in column.Formula)
							{
								if (formulaItem.Type == FormulaFieldType.Section)
								{
									total = budgetRecordsDay
									.Where(x => x.SectionID == formulaItem.ID)
									.Sum(x => x.Total);

									expression = expression.Replace($"[{formulaItem.ID}]", total.ToString());
								}
							}
							total = Math.Round(total, 2);

							var interpreter = new Interpreter();
							total = interpreter.Eval<decimal>(expression.Replace(",", "."));//becase the Interpreter doesn't understand ,
																							//total = CSharpScript.EvaluateAsync<decimal>(expression).Result;
							cells.Add(new Cell { Value = total.ToString(), IsShow = column.IsShow });
							footerCells.Add(new FooterCell { Value = total });
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
							cells.Add(new Cell { Value = (new DateTime(from.Year, from.Month, numberOfDay)).ToShortDateString(), IsShow = column.IsShow });
							footerCells.Add(new FooterCell { Value = numberOfDay });
						}
						else
						{
							cells.Add(new Cell { Value = "0", IsShow = column.IsShow });
							footerCells.Add(new FooterCell { Value = 0 });
						}
					}
				}
				rows.Add(cells);
				footers.Add(footerCells);
			}

			return new Tuple<List<List<Cell>>, List<Cell>>(
				rows,
				CalculateFooter(footers, template));
		}

		private List<Cell> CalculateFooter(List<List<FooterCell>> footersData, TemplateViewModel template)
		{
			List<Cell> footer = new List<Cell>();

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
						cell.Value = total.Sum().ToString();
						break;
					case FooterActionType.Avr:
						cell.Value = total.Average().ToString();
						break;
					case FooterActionType.Min:
						cell.Value = total.Min().ToString();
						break;
					case FooterActionType.Max:
						cell.Value = total.Max().ToString();
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
		public bool IsShow { get; set; } = true;
	}
	public class FooterCell
	{
		public decimal Value { get; set; }
	}

}
