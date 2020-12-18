using DynamicExpresso;
using LinqKit;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView;
using MyProfile.Entity.ModelView.BudgetView;
using MyProfile.Entity.ModelView.Reminder;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
using MyProfile.Reminder.Service;
using MyProfile.User.Service;
using Nager.Date;
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
        private BudgetRecordService budgetRecordService;
        private ReminderService reminderService;
        private CultureInfo cultureInfo;

        public BudgetService(IBaseRepository repository, BudgetRecordService budgetRecordService)
        {
            this.repository = repository;
            this.collectionUserService = new CollectionUserService(repository);
            this.budgetRecordService = budgetRecordService;
            this.reminderService = new ReminderService(repository);
            cultureInfo = new CultureInfo(UserInfo.Current.Currency.SpecificCulture, false);
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
            var dateTimeNow = DateTime.Now;

            IList<IGrouping<int, TmpBudgetRecord>> budgetRecords;
            List<ReminderShortModelView> reminders = null;
            List<string> columnsFormula = GetColumnsFormula(template);
            UserInfoModel currentUser = UserInfo.Current;
            NumberFormatInfo numberFormatInfo = new CultureInfo(currentUser.Currency.SpecificCulture, false).NumberFormat;

            switch (template.PeriodTypeID)
            {
                case (int)PeriodTypesEnum.Year:
                    budgetRecords = budgetRecordService.GetBudgetRecordsGroup(from, to, x => x.DateTimeOfPayment.Month).ToList();
                    totalCounter = 12;
                    break;
                case (int)PeriodTypesEnum.Years10:
                    budgetRecords = budgetRecordService.GetBudgetRecordsGroup(from, to, x => x.DateTimeOfPayment.Year).ToList();
                    totalCounter = 10;
                    break;
                case (int)PeriodTypesEnum.Month:
                default:
                    budgetRecords = budgetRecordService.GetBudgetRecordsGroup(from, to, x => x.DateTimeOfPayment.Day).ToList();
                    totalCounter = DateTime.DaysInMonth(from.Year, from.Month);

                    reminders = reminderService.GetRemindersByDateRange(from, to).ToList();
                    break;
            }
            if (currentUser.IsAllowCollectiveBudget && currentUser.UserSettings.BudgetPages_WithCollective)
            {
                AddCollectionRecords(budgetRecords);
            }

            for (int dateCounter = 1; dateCounter <= totalCounter; dateCounter++)
            {
                List<Cell> cells = new List<Cell>();
                List<FooterCell> footerCells = new List<FooterCell>();
                bool isWeekend = false,
                    isHoliday = false;
                DateTime currentDate = dateTimeNow;
                var interpreter = new Interpreter();

                #region understand is it weekend or not 
                if (template.PeriodTypeID == (int)PeriodTypesEnum.Month)
                {
                    currentDate = new DateTime(from.Year, from.Month, dateCounter);
                    isWeekend = currentDate.DayOfWeek == DayOfWeek.Sunday || currentDate.DayOfWeek == DayOfWeek.Saturday;
                    isHoliday = DateSystem.IsPublicHoliday(currentDate, CountryCode.RU);
                }
                else if (template.PeriodTypeID == (int)PeriodTypesEnum.Year)
                {
                    currentDate = new DateTime(from.Year, dateCounter, 1);
                }
                #endregion

                var cell = new Cell
                {
                    dateCounter = dateCounter,
                    IsWeekend = isWeekend,
                    IsHoliday = isHoliday,
                    CurrentDate = currentDate,
                };

                if (budgetRecords.Count != indexBudgetRecords && budgetRecords.FirstOrDefault(x => x.Key == dateCounter) != null)
                {
                    var budgetRecordsDay = budgetRecords.FirstOrDefault(x => x.Key == dateCounter);
                    indexBudgetRecords++;

                    for (int columnIndex = 0; columnIndex < allColumnsCount; columnIndex++)
                    {
                        var column = template.Columns[columnIndex];
                        string expression = columnsFormula[columnIndex];

                        cell.TemplateColumnType = column.TemplateColumnType;
                        cell.IsShow = column.IsShow;

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
                                else if (formulaItem.Type == FormulaFieldType.Days)
                                {
                                    expression = expression.Replace("[Days]", dateCounter.ToString());
                                }
                            }

                            try
                            {
                                if (!string.IsNullOrEmpty(expression))
                                {
                                    total = interpreter.Eval<decimal>(expression.Replace(",", "."));//becase the Interpreter doesn't understand , (comma)
                                    total = Math.Round(total, column.PlaceAfterCommon);
                                    //total = CSharpScript.EvaluateAsync<decimal>(expression).Result;


                                    numberFormatInfo.CurrencyDecimalDigits = column.PlaceAfterCommon;

                                    cell.Value = total.ToString("C", numberFormatInfo);
                                    cell.NaturalValue = total;

                                    cells.Add(cell.CloneObject());
                                    footerCells.Add(new FooterCell
                                    {
                                        Value = total.ToString("C", numberFormatInfo),
                                        NaturalValue = total
                                    });
                                }
                                else
                                {
                                    cells.Add(new Cell { Value = "", NaturalValue = -1 });
                                    footerCells.Add(new FooterCell());
                                }
                            }
                            catch (Exception ex)
                            {
                                cell.Value = "Ошибка формулы";
                                cell.NaturalValue = 0;
                                cell.TemplateColumnType = TemplateColumnType.Error;

                                cells.Add(cell.CloneObject());
                                footerCells.Add(new FooterCell
                                {
                                    Value = total.ToString("C", numberFormatInfo),
                                    NaturalValue = total
                                });
                            }
                        }
                        else if (column.TemplateColumnType == TemplateColumnType.DaysForMonth)
                        {
                            cell.Value = SetFormatForDate(new DateTime(from.Year, from.Month, dateCounter), column.Format, column.TemplateColumnType);
                            cell.NaturalValue = cell.dateCounter;

                            if (reminders != null && reminders.Count != 0)
                            {
                                var z = reminders
                                    .Where(x => currentDate.Date == x.DateReminder.Value.Date)
                                    .GroupBy(x => x.CssIcon)
                                    .Select(x => new ReminderCell
                                    {
                                        CssIcon = x.Key,
                                        Count = x.Count(),
                                        Titles = x.FirstOrDefault().Title,
                                        IsRepeat = x.Any(y => y.IsRepeat)
                                    })
                                    .ToList();


                                cell.Reminders.AddRange(z);
                            }

                            cells.Add(cell.CloneObject());
                            footerCells.Add(new FooterCell
                            {
                                Value = "0",
                                NaturalValue = 0
                            });
                        }
                        else if (column.TemplateColumnType == TemplateColumnType.MonthsForYear)
                        {
                            cell.Value = SetFormatForDate(new DateTime(from.Year, dateCounter, 1), column.Format, column.TemplateColumnType); ;
                            cell.NaturalValue = cell.dateCounter;

                            cells.Add(cell.CloneObject());
                            footerCells.Add(new FooterCell
                            {
                                Value = "0",
                                NaturalValue = 0
                            });
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < allColumnsCount; i++)
                    {
                        var column = template.Columns[i];
                        cell.TemplateColumnType = column.TemplateColumnType;
                        cell.IsShow = column.IsShow;

                        numberFormatInfo.CurrencyDecimalDigits = column.PlaceAfterCommon;

                        string v = 0.ToString("C", numberFormatInfo);
                        cell.NaturalValue = 0; //For order in table

                        if (column.TemplateColumnType == TemplateColumnType.BudgetSection && currentDate.Date > dateTimeNow.Date)
                        {
                            v = "";
                            cell.NaturalValue = -1;//For order in table
                        }
                        else if (column.TemplateColumnType == TemplateColumnType.DaysForMonth)
                        {
                            v = SetFormatForDate(new DateTime(from.Year, from.Month, dateCounter), column.Format, column.TemplateColumnType);

                            if (reminders != null && reminders.Count != 0)
                            {
                                var z = reminders
                                    .Where(x => currentDate.Date == x.DateReminder.Value.Date)
                                    .GroupBy(x => x.CssIcon)
                                    .Select(x => new ReminderCell
                                    {
                                        CssIcon = x.Key,
                                        Count = x.Count(),
                                        Titles = x.FirstOrDefault().Title,
                                        IsRepeat = x.Any(y => y.IsRepeat)
                                    })
                                    .ToList();


                                cell.Reminders.AddRange(z);
                            }
                            cell.NaturalValue = dateCounter;
                        }
                        else if (column.TemplateColumnType == TemplateColumnType.MonthsForYear)
                        {
                            v = SetFormatForDate(new DateTime(from.Year, dateCounter, 1), column.Format, column.TemplateColumnType);
                            cell.NaturalValue = dateCounter;
                        }
                        else if (column.TemplateColumnType == TemplateColumnType.BudgetSection && column.Formula.Count() == 0)
                        {
                            v = "";
                            cell.NaturalValue = -1;
                        }
                        cell.Value = v;

                        cells.Add(cell.CloneObject());
                        footerCells.Add(new FooterCell
                        {
                            Value = "0",
                            NaturalValue = 0
                        });
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
            string v ;

            if (!string.IsNullOrEmpty(format))
            {
                v = dateTime.ToString(format, cultureInfo);
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
            NumberFormatInfo numberFormatInfo = cultureInfo.NumberFormat;
            var specificCulture = UserInfo.Current.Currency.SpecificCulture;
            List<Cell> footer = new List<Cell>();
            decimal v;

            for (int i = 0; i < template.Columns.Count; i++)
            {
                List<decimal> totals = new List<decimal>();
                Cell cell = new Cell() { IsShow = template.Columns[i].IsShow };
                numberFormatInfo.CurrencyDecimalDigits = template.Columns[i].PlaceAfterCommon;

                foreach (var footerRow in footersData)
                {
                    totals.Add(footerRow[i].NaturalValue);
                }

                switch (template.Columns[i].TotalAction)
                {
                    case FooterActionType.Sum:
                        v = totals.Sum();
                        cell.Value = v.ToString("C", numberFormatInfo);
                        break;
                    case FooterActionType.Avr:
                        v = totals.Average();
                        cell.Value = v.ToString("C", numberFormatInfo);
                        break;
                    case FooterActionType.Min:
                        v = totals.Min();
                        cell.Value = v.ToString("C", numberFormatInfo);
                        break;
                    case FooterActionType.Max:
                        v = totals.Max();
                        cell.Value = v.ToString("C", numberFormatInfo);
                        break;
                    case FooterActionType.Undefined:
                    default:
                        cell.Value = "Итого";
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
                        else if (formulaItem.Type == FormulaFieldType.Days)
                        {
                            expression += "[Days]";
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
                    if (item.CollectionSectionIDs.Count() > 0)
                    {
                        item.Total += record.Where(x => item.CollectionSectionIDs.Contains(x.SectionID)).Sum(x => x.Total);
                    }
                }
            }
        }

        #endregion

    }


}
