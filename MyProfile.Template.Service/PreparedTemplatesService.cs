using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView;
using MyProfile.Entity.ModelView.AreaAndSection;
using MyProfile.Entity.ModelView.BudgetView;
using MyProfile.Entity.ModelView.Template;
using MyProfile.Identity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace MyProfile.Template.Service
{
    public class PreparedTemplateService : IPreparedService
    {
        private UserInfoModel currentUser;
        private List<PeriodType> periodTypes;
        private IEnumerable<AreaLightModelView> groupedAreaAndSection;
        private CultureInfo cultureInfo;
        private int counterID = 0;

        public PreparedTemplateService(List<PeriodType> periodTypes,
            IEnumerable<AreaLightModelView> groupedAreaAndSection)
        {
            currentUser = UserInfo.Current;
            this.periodTypes = periodTypes;
            this.groupedAreaAndSection = groupedAreaAndSection;
            cultureInfo = new CultureInfo(currentUser.Currency.SpecificCulture, false);
        }


        public IPreparedModel GetPreparedItems()
        {
            PreparedTemplatesViewModel model = new PreparedTemplatesViewModel();

            #region PeriodType.Month
            model.Periods.Add(new PreparedTemplateViewModel
            {
                PeriodTypeID = (int)PeriodTypesEnum.Month,
                PeriodTypeName = "Финансы на месяцам",
                Templates = new List<TemplateViewModel>
                {
                    GetTemplateByAreas(PeriodTypesEnum.Month),
                },
                IsSelected = true,
            });

            if (groupedAreaAndSection.Any(x => x.CodeName == "Travel" && x.BudgetSections.Count() > 2))
            {
                model.Periods[0].Templates.Add(GetTemplateByTravel(PeriodTypesEnum.Month));
            }

            if (groupedAreaAndSection.Any(x => x.CodeName == "Auto" && x.BudgetSections.Count() > 2))
            {
                model.Periods[0].Templates.Add(GetTemplateByAuto(PeriodTypesEnum.Month));
            }

            if (groupedAreaAndSection.Any(x => x.CodeName == "Health & Fitness" && x.BudgetSections.Count() > 2))
            {
                model.Periods[0].Templates.Add(GetTemplateByHealth(PeriodTypesEnum.Month));
            }

            if (groupedAreaAndSection.Any(x => x.CodeName == "Kids" && x.BudgetSections.Count() > 2))
            {
                model.Periods[0].Templates.Add(GetTemplateByKids(PeriodTypesEnum.Month));
            }

            if (groupedAreaAndSection.Any(x => (x.CodeName == "Income" || x.CodeName == "Investment") && x.BudgetSections.Count() > 0))
            {
                model.Periods[0].Templates.Add(GetTemplateByIncomeAndInvest(PeriodTypesEnum.Month));
            }

            #endregion

            #region PeriodType.Year
            model.Periods.Add(new PreparedTemplateViewModel
            {
                PeriodTypeID = (int)PeriodTypesEnum.Year,
                PeriodTypeName = "Финансы на год",
                Templates = new List<TemplateViewModel>
                {
                    GetTemplateByAreas(PeriodTypesEnum.Year),
                }
            });

            if (groupedAreaAndSection.Any(x => x.CodeName == "Travel" && x.BudgetSections.Count() > 2))
            {
                model.Periods[1].Templates.Add(GetTemplateByTravel(PeriodTypesEnum.Year));
            }

            if (groupedAreaAndSection.Any(x => x.CodeName == "Auto" && x.BudgetSections.Count() > 2))
            {
                model.Periods[1].Templates.Add(GetTemplateByAuto(PeriodTypesEnum.Year));
            }

            if (groupedAreaAndSection.Any(x => x.CodeName == "Health & Fitness" && x.BudgetSections.Count() > 2))
            {
                model.Periods[1].Templates.Add(GetTemplateByHealth(PeriodTypesEnum.Year));
            }

            if (groupedAreaAndSection.Any(x => x.CodeName == "Kids" && x.BudgetSections.Count() > 2))
            {
                model.Periods[1].Templates.Add(GetTemplateByKids(PeriodTypesEnum.Year));
            }

            if (groupedAreaAndSection.Any(x => (x.CodeName == "Income" || x.CodeName == "Investment") && x.BudgetSections.Count() > 0))
            {
                model.Periods[1].Templates.Add(GetTemplateByIncomeAndInvest(PeriodTypesEnum.Year));
            }
            #endregion

            #region fill the table
            foreach (var periods in model.Periods)
            {
                foreach (var template in periods.Templates)
                {
                    FillTable(template);
                }
            }
            #endregion

            return model;
        }

        private void FillTable(TemplateViewModel template)
        {
            var now = DateTime.Now;
            int totalCounter,
                dateCounter = 1;
            NumberFormatInfo numberFormatInfo = new CultureInfo(currentUser.Currency.SpecificCulture, false).NumberFormat;
            List<FooterCell> footerCells = new List<FooterCell>();
            var ziroValue = 0.ToString("C", numberFormatInfo);

            switch (template.PeriodTypeID)
            {
                case (int)PeriodTypesEnum.Year:
                    totalCounter = 12;
                    break;
                case (int)PeriodTypesEnum.Years10:
                    totalCounter = 10;
                    break;
                case (int)PeriodTypesEnum.Month:
                default:
                    totalCounter = DateTime.DaysInMonth(now.Year, now.Month);
                    break;
            }

            while (dateCounter <= totalCounter)
            {
                List<Cell> cells = new List<Cell>();

                if (template.PeriodTypeID == (int)PeriodTypesEnum.Month && dateCounter == 7)
                {
                    dateCounter = 25;
                }

                for (int i = 0; i < template.Columns.Count; i++)
                {
                    var column = template.Columns[i];
                    string v = ziroValue;
                    Cell cell = new Cell();

                    if (dateCounter == 6 && template.PeriodTypeID == (int)PeriodTypesEnum.Month)
                    {
                        cell.Value = "...";
                    }
                    else
                    {
                        numberFormatInfo.CurrencyDecimalDigits = column.PlaceAfterCommon;

                        if (column.TemplateColumnType == TemplateColumnType.BudgetSection)// && currentDate.Date > dateTimeNow.Date)
                        {
                            v = "0 ₽";
                            cell.NaturalValue = -1;//For order in table
                        }
                        else if (column.TemplateColumnType == TemplateColumnType.DaysForMonth)
                        {
                            v = SetFormatForDate(new DateTime(now.Year, now.Month, dateCounter), column.Format, column.TemplateColumnType);

                        }
                        else if (column.TemplateColumnType == TemplateColumnType.MonthsForYear)
                        {
                            v = SetFormatForDate(new DateTime(now.Year, dateCounter, 1), column.Format, column.TemplateColumnType);
                            cell.NaturalValue = dateCounter;
                        }
                        else if (column.TemplateColumnType == TemplateColumnType.BudgetSection && column.Formula.Count() == 0)
                        {
                            v = "";
                            cell.NaturalValue = -1;
                        }
                        cell.Value = v;
                    }

                    cells.Add(cell.CloneObject());
                }
                template.Rows.Add(cells);

                dateCounter += 1;
            }
        }
        private string SetFormatForDate(DateTime dateTime, string format, TemplateColumnType templateColumnType)
        {
            string v;

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

        private TemplateViewModel GetTemplateByAreas(PeriodTypesEnum periodTypesEnum)
        {
            int order = 0;
            var firstColumnType = GetDaysTemplateColumnTypeByPeriodType(periodTypesEnum);
            string defaultFormatColumn = GetDefaultFormatColumn(periodTypesEnum);
            var template = new TemplateViewModel
            {
                Name = "Шаблон по группам категорий",
                Description = "",
                PeriodTypeID = (int)periodTypesEnum,
                PeriodName = periodTypes.FirstOrDefault(x => x.ID == (int)periodTypesEnum).Name,
                IsCreatedAsPrepared = true
            };

            template.Columns = new List<Column>();
            template.Columns.Add(new Column
            {
                Order = order++,
                TemplateColumnType = firstColumnType,
                Name = GetNameColumnByTemplateColumnType(firstColumnType),
                IsShow = true,
                Format = defaultFormatColumn,
                Formula = new List<FormulaItem>(),
                PlaceAfterCommon = 0,
                TotalAction = FooterActionType.Undefined
            });

            foreach (var area in groupedAreaAndSection)
            {
                if (area.BudgetSections == null | area.BudgetSections.Count() == 0)
                {
                    continue;
                }
                var column = new Column
                {
                    Name = area.Name,
                    Order = order++,
                    TemplateColumnType = TemplateColumnType.BudgetSection,
                    ColumnSectionType = area.BudgetSections.FirstOrDefault().SectionTypeCodeName,
                    Format = "",
                    PlaceAfterCommon = 2,
                    IsShow = true,
                    TotalAction = FooterActionType.Sum,
                    TemplateBudgetSections = area.BudgetSections.Select(x => new Entity.ModelView.TemplateBudgetSection
                    {
                        BudgetAreaID = area.ID,
                        BudgetAreaName = area.Name,
                        SectionID = x.ID,
                        SectionName = x.Name,
                        SectionTypeID = x.SectionTypeID,
                    })
                    .ToList(),
                };

                for (int i = 0; i < column.TemplateBudgetSections.Count; i++)
                {
                    if (i != 0)
                    {
                        if (column.TemplateBudgetSections[i - 1].SectionTypeID == (int)SectionTypeEnum.Spendings
                           && column.TemplateBudgetSections[i].SectionTypeID == (int)SectionTypeEnum.Earnings)
                        {
                            column.Formula.Add(new FormulaItem
                            {
                                ID = null,
                                Type = FormulaFieldType.Mark,
                                Value = "-",
                            });
                        }
                        else
                        {
                            column.Formula.Add(new FormulaItem
                            {
                                ID = null,
                                Type = FormulaFieldType.Mark,
                                Value = "+",
                            });
                        }
                    }
                    column.Formula.Add(new FormulaItem
                    {
                        ID = column.TemplateBudgetSections[i].SectionID,
                        Value = "[ " + column.TemplateBudgetSections[i].SectionName + " ]",
                        Type = FormulaFieldType.Section
                    });
                }

                template.Columns.Add(column);
            }

            return template;
        }
        private TemplateViewModel GetTemplateByTravel(PeriodTypesEnum periodTypesEnum)
        {
            int order = 0;
            var firstColumnType = GetDaysTemplateColumnTypeByPeriodType(periodTypesEnum);
            string defaultFormatColumn = GetDefaultFormatColumn(periodTypesEnum);
            var template = new TemplateViewModel
            {
                Name = "Шаблон по путешествиям",
                Description = "",
                PeriodTypeID = (int)periodTypesEnum,
                PeriodName = periodTypes.FirstOrDefault(x => x.ID == (int)periodTypesEnum).Name,
                IsCreatedAsPrepared = true
            };

            template.Columns = new List<Column>();
            template.Columns.Add(new Column
            {
                Order = order++,
                TemplateColumnType = firstColumnType,
                Name = GetNameColumnByTemplateColumnType(firstColumnType),
                IsShow = true,
                Format = defaultFormatColumn,
                Formula = new List<FormulaItem>(),
                PlaceAfterCommon = 0,
                TotalAction = FooterActionType.Undefined
            });

            var area = groupedAreaAndSection.FirstOrDefault(x => x.CodeName == "Travel");
            var sections = area.BudgetSections.ToList();

            foreach (var section in sections)
            {
                var column = new Column
                {
                    Name = section.Name,
                    Order = order++,
                    TemplateColumnType = TemplateColumnType.BudgetSection,
                    ColumnSectionType = section.SectionTypeCodeName,
                    Format = "",
                    PlaceAfterCommon = 2,
                    IsShow = true,
                    TotalAction = FooterActionType.Sum,
                    TemplateBudgetSections = new List<Entity.ModelView.TemplateBudgetSection> {
                        new Entity.ModelView.TemplateBudgetSection
                        {
                            BudgetAreaID = area.ID,
                            BudgetAreaName = area.Name,
                            SectionID = section.ID,
                            SectionName = section.Name,
                            SectionTypeID = section.SectionTypeID,
                        }
                    }
                };

                for (int i = 0; i < column.TemplateBudgetSections.Count; i++)
                {
                    if (i != 0)
                    {
                        if (column.TemplateBudgetSections[i - 1].SectionTypeID == (int)SectionTypeEnum.Spendings
                             && column.TemplateBudgetSections[i].SectionTypeID == (int)SectionTypeEnum.Earnings)
                        {
                            column.Formula.Add(new FormulaItem
                            {
                                ID = null,
                                Type = FormulaFieldType.Mark,
                                Value = "-",
                            });
                        }
                        else
                        {
                            column.Formula.Add(new FormulaItem
                            {
                                ID = null,
                                Type = FormulaFieldType.Mark,
                                Value = "+",
                            });
                        }
                    }
                    column.Formula.Add(new FormulaItem
                    {
                        ID = column.TemplateBudgetSections[i].SectionID,
                        Value = "[ " + column.TemplateBudgetSections[i].SectionName + " ]",
                        Type = FormulaFieldType.Section
                    });
                }

                template.Columns.Add(column);
            }

            return template;
        }
        private TemplateViewModel GetTemplateByAuto(PeriodTypesEnum periodTypesEnum)
        {
            int order = 0;
            var firstColumnType = GetDaysTemplateColumnTypeByPeriodType(periodTypesEnum);
            string defaultFormatColumn = GetDefaultFormatColumn(periodTypesEnum);
            var template = new TemplateViewModel
            {
                Name = "Шаблон по авто",
                Description = "",
                PeriodTypeID = (int)periodTypesEnum,
                PeriodName = periodTypes.FirstOrDefault(x => x.ID == (int)periodTypesEnum).Name,
                IsCreatedAsPrepared = true
            };

            template.Columns = new List<Column>();
            template.Columns.Add(new Column
            {
                Order = order++,
                TemplateColumnType = firstColumnType,
                Name = GetNameColumnByTemplateColumnType(firstColumnType),
                IsShow = true,
                Format = defaultFormatColumn,
                Formula = new List<FormulaItem>(),
                PlaceAfterCommon = 0,
                TotalAction = FooterActionType.Undefined
            });

            var area = groupedAreaAndSection.FirstOrDefault(x => x.CodeName == "Auto");
            var sections = area.BudgetSections.ToList();

            foreach (var section in sections)
            {
                var column = new Column
                {
                    Name = section.Name,
                    Order = order++,
                    TemplateColumnType = TemplateColumnType.BudgetSection,
                    ColumnSectionType = section.SectionTypeCodeName,
                    Format = "",
                    PlaceAfterCommon = 2,
                    IsShow = true,
                    TotalAction = FooterActionType.Sum,
                    TemplateBudgetSections = new List<Entity.ModelView.TemplateBudgetSection> {
                        new Entity.ModelView.TemplateBudgetSection
                        {
                            BudgetAreaID = area.ID,
                            BudgetAreaName = area.Name,
                            SectionID = section.ID,
                            SectionName = section.Name,
                            SectionTypeID = section.SectionTypeID,
                        }
                    }
                };

                for (int i = 0; i < column.TemplateBudgetSections.Count; i++)
                {
                    if (i != 0)
                    {
                        if (column.TemplateBudgetSections[i - 1].SectionTypeID == (int)SectionTypeEnum.Spendings
                           && column.TemplateBudgetSections[i].SectionTypeID == (int)SectionTypeEnum.Earnings)
                        {
                            column.Formula.Add(new FormulaItem
                            {
                                ID = null,
                                Type = FormulaFieldType.Mark,
                                Value = "-",
                            });
                        }
                        else
                        {
                            column.Formula.Add(new FormulaItem
                            {
                                ID = null,
                                Type = FormulaFieldType.Mark,
                                Value = "+",
                            });
                        }
                    }
                    column.Formula.Add(new FormulaItem
                    {
                        ID = column.TemplateBudgetSections[i].SectionID,
                        Value = "[ " + column.TemplateBudgetSections[i].SectionName + " ]",
                        Type = FormulaFieldType.Section
                    });
                }

                template.Columns.Add(column);
            }

            return template;
        }
        private TemplateViewModel GetTemplateByHealth(PeriodTypesEnum periodTypesEnum)
        {
            int order = 0;
            var firstColumnType = GetDaysTemplateColumnTypeByPeriodType(periodTypesEnum);
            string defaultFormatColumn = GetDefaultFormatColumn(periodTypesEnum);
            var template = new TemplateViewModel
            {
                Name = "Шаблон здоровья",
                Description = "",
                PeriodTypeID = (int)periodTypesEnum,
                PeriodName = periodTypes.FirstOrDefault(x => x.ID == (int)periodTypesEnum).Name,
                IsCreatedAsPrepared = true
            };

            template.Columns = new List<Column>();
            template.Columns.Add(new Column
            {
                Order = order++,
                TemplateColumnType = firstColumnType,
                Name = GetNameColumnByTemplateColumnType(firstColumnType),
                IsShow = true,
                Format = defaultFormatColumn,
                Formula = new List<FormulaItem>(),
                PlaceAfterCommon = 0,
                TotalAction = FooterActionType.Undefined
            });

            var area = groupedAreaAndSection.FirstOrDefault(x => x.CodeName == "Health & Fitness");
            var sections = area.BudgetSections.ToList();

            foreach (var section in sections)
            {
                var column = new Column
                {
                    Name = section.Name,
                    Order = order++,
                    TemplateColumnType = TemplateColumnType.BudgetSection,
                    ColumnSectionType = section.SectionTypeCodeName,
                    Format = "",
                    PlaceAfterCommon = 2,
                    IsShow = true,
                    TotalAction = FooterActionType.Sum,
                    TemplateBudgetSections = new List<Entity.ModelView.TemplateBudgetSection> {
                        new Entity.ModelView.TemplateBudgetSection
                        {
                            BudgetAreaID = area.ID,
                            BudgetAreaName = area.Name,
                            SectionID = section.ID,
                            SectionName = section.Name,
                            SectionTypeID = section.SectionTypeID,
                        }
                    }
                };

                for (int i = 0; i < column.TemplateBudgetSections.Count; i++)
                {
                    if (i != 0)
                    {
                        if (column.TemplateBudgetSections[i - 1].SectionTypeID == (int)SectionTypeEnum.Spendings
                          && column.TemplateBudgetSections[i].SectionTypeID == (int)SectionTypeEnum.Earnings)
                        {
                            column.Formula.Add(new FormulaItem
                            {
                                ID = null,
                                Type = FormulaFieldType.Mark,
                                Value = "-",
                            });
                        }
                        else
                        {
                            column.Formula.Add(new FormulaItem
                            {
                                ID = null,
                                Type = FormulaFieldType.Mark,
                                Value = "+",
                            });
                        }
                    }
                    column.Formula.Add(new FormulaItem
                    {
                        ID = column.TemplateBudgetSections[i].SectionID,
                        Value = "[ " + column.TemplateBudgetSections[i].SectionName + " ]",
                        Type = FormulaFieldType.Section
                    });
                }

                template.Columns.Add(column);
            }

            return template;
        }
        private TemplateViewModel GetTemplateByKids(PeriodTypesEnum periodTypesEnum)
        {
            int order = 0;
            var firstColumnType = GetDaysTemplateColumnTypeByPeriodType(periodTypesEnum);
            string defaultFormatColumn = GetDefaultFormatColumn(periodTypesEnum);
            var template = new TemplateViewModel
            {
                Name = "Шаблон дети",
                Description = "",
                PeriodTypeID = (int)periodTypesEnum,
                PeriodName = periodTypes.FirstOrDefault(x => x.ID == (int)periodTypesEnum).Name,
                IsCreatedAsPrepared = true
            };

            template.Columns = new List<Column>();
            template.Columns.Add(new Column
            {
                Order = order++,
                TemplateColumnType = firstColumnType,
                Name = GetNameColumnByTemplateColumnType(firstColumnType),
                IsShow = true,
                Format = defaultFormatColumn,
                Formula = new List<FormulaItem>(),
                PlaceAfterCommon = 0,
                TotalAction = FooterActionType.Undefined
            });

            var area = groupedAreaAndSection.FirstOrDefault(x => x.CodeName == "Kids");
            var sections = area.BudgetSections.ToList();

            foreach (var section in sections)
            {
                var column = new Column
                {
                    Name = section.Name,
                    Order = order++,
                    TemplateColumnType = TemplateColumnType.BudgetSection,
                    ColumnSectionType = section.SectionTypeCodeName,
                    Format = "",
                    PlaceAfterCommon = 2,
                    IsShow = true,
                    TotalAction = FooterActionType.Sum,
                    TemplateBudgetSections = new List<Entity.ModelView.TemplateBudgetSection> {
                        new Entity.ModelView.TemplateBudgetSection
                        {
                            BudgetAreaID = area.ID,
                            BudgetAreaName = area.Name,
                            SectionID = section.ID,
                            SectionName = section.Name,
                            SectionTypeID = section.SectionTypeID,
                        }
                    }
                };

                for (int i = 0; i < column.TemplateBudgetSections.Count; i++)
                {
                    if (i != 0)
                    {
                        if (column.TemplateBudgetSections[i - 1].SectionTypeID == (int)SectionTypeEnum.Spendings
                           && column.TemplateBudgetSections[i].SectionTypeID == (int)SectionTypeEnum.Earnings)
                        {
                            column.Formula.Add(new FormulaItem
                            {
                                ID = null,
                                Type = FormulaFieldType.Mark,
                                Value = "-",
                            });
                        }
                        else
                        {
                            column.Formula.Add(new FormulaItem
                            {
                                ID = null,
                                Type = FormulaFieldType.Mark,
                                Value = "+",
                            });
                        }
                    }
                    column.Formula.Add(new FormulaItem
                    {
                        ID = column.TemplateBudgetSections[i].SectionID,
                        Value = "[ " + column.TemplateBudgetSections[i].SectionName + " ]",
                        Type = FormulaFieldType.Section
                    });
                }

                template.Columns.Add(column);
            }

            return template;
        }

        private TemplateViewModel GetTemplateByIncomeAndInvest(PeriodTypesEnum periodTypesEnum)
        {
            int order = 0;
            var firstColumnType = GetDaysTemplateColumnTypeByPeriodType(periodTypesEnum);
            string defaultFormatColumn = GetDefaultFormatColumn(periodTypesEnum);
            var template = new TemplateViewModel
            {
                Name = "Шаблон доходов и инвестиций",
                Description = "",
                PeriodTypeID = (int)periodTypesEnum,
                PeriodName = periodTypes.FirstOrDefault(x => x.ID == (int)periodTypesEnum).Name,
                IsCreatedAsPrepared = true
            };

            template.Columns = new List<Column>();
            template.Columns.Add(new Column
            {
                Order = order++,
                TemplateColumnType = firstColumnType,
                Name = GetNameColumnByTemplateColumnType(firstColumnType),
                IsShow = true,
                Format = defaultFormatColumn,
                Formula = new List<FormulaItem>(),
                PlaceAfterCommon = 0,
                TotalAction = FooterActionType.Undefined
            });

            var areaIncome = groupedAreaAndSection.FirstOrDefault(x => x.CodeName == "Income");

            #region Paycheck
            var paycheck = areaIncome.BudgetSections.FirstOrDefault(x => x.CodeName == "Paycheck");
            if (paycheck != null)
            {
                var column = new Column
                {
                    Name = paycheck.Name,
                    Order = order++,
                    TemplateColumnType = TemplateColumnType.BudgetSection,
                    ColumnSectionType = paycheck.SectionTypeCodeName,
                    Format = "",
                    PlaceAfterCommon = 2,
                    IsShow = true,
                    TotalAction = FooterActionType.Sum,
                    TemplateBudgetSections = new List<Entity.ModelView.TemplateBudgetSection> {
                        new Entity.ModelView.TemplateBudgetSection
                        {
                            BudgetAreaID = areaIncome.ID,
                            BudgetAreaName = areaIncome.Name,
                            SectionID = paycheck.ID,
                            SectionName = paycheck.Name,
                            SectionTypeID = paycheck.SectionTypeID,
                        }
                    }
                };


                column.Formula.Add(new FormulaItem
                {
                    ID = paycheck.ID,
                    Value = "[ " + paycheck.Name + " ]",
                    Type = FormulaFieldType.Section
                });

                template.Columns.Add(column);
            }
            #endregion

            #region "Part-time job", "Passive income", "Alimony"
            var sections = areaIncome.BudgetSections.Where(x => new string[] { "Part-time job", "Passive income", "Alimony" }.Contains(x.CodeName)).ToList();

            if (sections != null && sections.Count > 0)
            {
                var column = new Column
                {
                    Name = "Дополнительный заработок",
                    Order = order++,
                    TemplateColumnType = TemplateColumnType.BudgetSection,
                    ColumnSectionType = Enum.GetName(typeof(SectionTypeEnum), SectionTypeEnum.Earnings),
                    Format = "",
                    PlaceAfterCommon = 2,
                    IsShow = true,
                    TotalAction = FooterActionType.Sum,
                    TemplateBudgetSections = sections.Select(x => new Entity.ModelView.TemplateBudgetSection
                    {
                        BudgetAreaID = areaIncome.ID,
                        BudgetAreaName = areaIncome.Name,
                        SectionID = x.ID,
                        SectionName = x.Name,
                        SectionTypeID = x.SectionTypeID,
                    }
                    ).ToList()
                };


                for (int i = 0; i < column.TemplateBudgetSections.Count; i++)
                {
                    if (i != 0)
                    {
                        if (column.TemplateBudgetSections[i - 1].SectionTypeID == (int)SectionTypeEnum.Spendings
                            && column.TemplateBudgetSections[i].SectionTypeID == (int)SectionTypeEnum.Earnings)
                        {
                            column.Formula.Add(new FormulaItem
                            {
                                ID = null,
                                Type = FormulaFieldType.Mark,
                                Value = "-",
                            });
                        }
                        else
                        {
                            column.Formula.Add(new FormulaItem
                            {
                                ID = null,
                                Type = FormulaFieldType.Mark,
                                Value = "+",
                            });
                        }
                    }
                    column.Formula.Add(new FormulaItem
                    {
                        ID = column.TemplateBudgetSections[i].SectionID,
                        Value = "[ " + column.TemplateBudgetSections[i].SectionName + " ]",
                        Type = FormulaFieldType.Section
                    });
                }

                template.Columns.Add(column);
            }
            #endregion

            #region "Cashback", "Freelance", "Gift money"

            sections = areaIncome.BudgetSections.Where(x => new string[] { "Cashback", "Freelance", "Gift money" }.Contains(x.CodeName))
                .Union(areaIncome.BudgetSections.Where(x => string.IsNullOrEmpty(x.CodeName))).ToList();

            if (sections != null && sections.Count > 0)
            {
                foreach (var section in sections)
                {
                    var column = new Column
                    {
                        Name = section.Name,
                        Order = order++,
                        TemplateColumnType = TemplateColumnType.BudgetSection,
                        ColumnSectionType = section.SectionTypeCodeName,
                        Format = "",
                        PlaceAfterCommon = 2,
                        IsShow = true,
                        TotalAction = FooterActionType.Sum,
                        TemplateBudgetSections = new List<Entity.ModelView.TemplateBudgetSection> {
                        new Entity.ModelView.TemplateBudgetSection
                        {
                            BudgetAreaID = areaIncome.ID,
                            BudgetAreaName = areaIncome.Name,
                            SectionID = section.ID,
                            SectionName = section.Name,
                            SectionTypeID = section.SectionTypeID,
                        }
                    }
                    };


                    for (int i = 0; i < column.TemplateBudgetSections.Count; i++)
                    {
                        if (i != 0)
                        {
                            if (column.TemplateBudgetSections[i - 1].SectionTypeID == (int)SectionTypeEnum.Spendings
                                && column.TemplateBudgetSections[i].SectionTypeID == (int)SectionTypeEnum.Earnings)
                            {
                                column.Formula.Add(new FormulaItem
                                {
                                    ID = null,
                                    Type = FormulaFieldType.Mark,
                                    Value = "-",
                                });
                            }
                            else
                            {
                                column.Formula.Add(new FormulaItem
                                {
                                    ID = null,
                                    Type = FormulaFieldType.Mark,
                                    Value = "+",
                                });
                            }
                        }
                        column.Formula.Add(new FormulaItem
                        {
                            ID = column.TemplateBudgetSections[i].SectionID,
                            Value = "[ " + column.TemplateBudgetSections[i].SectionName + " ]",
                            Type = FormulaFieldType.Section
                        });
                    }

                    template.Columns.Add(column);
                }
            }

            #endregion

            var areaInvestment = groupedAreaAndSection.FirstOrDefault(x => x.CodeName == "Investment");

            #region Interest Income", "Dividend", "Rental Income"

            sections = areaInvestment.BudgetSections.Where(x => new string[] { "Interest Income", "Dividend", "Rental Income" }.Contains(x.CodeName)).ToList();

            if (sections != null && sections.Count > 0)
            {
                foreach (var section in sections)
                {
                    var column = new Column
                    {
                        Name = section.Name,
                        Order = order++,
                        TemplateColumnType = TemplateColumnType.BudgetSection,
                        ColumnSectionType = section.SectionTypeCodeName,
                        Format = "",
                        PlaceAfterCommon = 2,
                        IsShow = true,
                        TotalAction = FooterActionType.Sum,
                        TemplateBudgetSections = new List<Entity.ModelView.TemplateBudgetSection> {
                            new Entity.ModelView.TemplateBudgetSection
                            {
                                BudgetAreaID = areaInvestment.ID,
                                BudgetAreaName = areaInvestment.Name,
                                SectionID = section.ID,
                                SectionName = section.Name,
                                SectionTypeID = section.SectionTypeID,
                            }
                    }
                    };


                    for (int i = 0; i < column.TemplateBudgetSections.Count; i++)
                    {
                        if (i != 0)
                        {
                            if (column.TemplateBudgetSections[i - 1].SectionTypeID == (int)SectionTypeEnum.Spendings
                                && column.TemplateBudgetSections[i].SectionTypeID == (int)SectionTypeEnum.Earnings)
                            {
                                column.Formula.Add(new FormulaItem
                                {
                                    ID = null,
                                    Type = FormulaFieldType.Mark,
                                    Value = "-",
                                });
                            }
                            else
                            {
                                column.Formula.Add(new FormulaItem
                                {
                                    ID = null,
                                    Type = FormulaFieldType.Mark,
                                    Value = "+",
                                });
                            }
                        }
                        column.Formula.Add(new FormulaItem
                        {
                            ID = column.TemplateBudgetSections[i].SectionID,
                            Value = "[ " + column.TemplateBudgetSections[i].SectionName + " ]",
                            Type = FormulaFieldType.Section
                        });
                    }

                    template.Columns.Add(column);
                }
            }
            #endregion

            #region "Commission", "Investment Tax"
            sections = areaInvestment.BudgetSections.Where(x => new string[] { "Commission", "Investment Tax" }.Contains(x.CodeName)).ToList();

            if (sections != null && sections.Count > 0)
            {
                var column = new Column
                {
                    Name = "Комиссии и налоги",
                    Order = order++,
                    TemplateColumnType = TemplateColumnType.BudgetSection,
                    ColumnSectionType = Enum.GetName(typeof(SectionTypeEnum), SectionTypeEnum.Spendings),
                    Format = "",
                    PlaceAfterCommon = 2,
                    IsShow = true,
                    TotalAction = FooterActionType.Sum,
                    TemplateBudgetSections = sections.Select(x => new Entity.ModelView.TemplateBudgetSection
                    {
                        BudgetAreaID = areaInvestment.ID,
                        BudgetAreaName = areaInvestment.Name,
                        SectionID = x.ID,
                        SectionName = x.Name,
                        SectionTypeID = x.SectionTypeID,
                    }
                    ).ToList()
                };


                for (int i = 0; i < column.TemplateBudgetSections.Count; i++)
                {
                    if (i != 0)
                    {
                        if (column.TemplateBudgetSections[i - 1].SectionTypeID == (int)SectionTypeEnum.Spendings
                            && column.TemplateBudgetSections[i].SectionTypeID == (int)SectionTypeEnum.Earnings)
                        {
                            column.Formula.Add(new FormulaItem
                            {
                                ID = null,
                                Type = FormulaFieldType.Mark,
                                Value = "-",
                            });
                        }
                        else
                        {
                            column.Formula.Add(new FormulaItem
                            {
                                ID = null,
                                Type = FormulaFieldType.Mark,
                                Value = "+",
                            });
                        }
                    }
                    column.Formula.Add(new FormulaItem
                    {
                        ID = column.TemplateBudgetSections[i].SectionID,
                        Value = "[ " + column.TemplateBudgetSections[i].SectionName + " ]",
                        Type = FormulaFieldType.Section
                    });
                }

                template.Columns.Add(column);
            }
            #endregion

            return template;
        }

        private string GetDefaultFormatColumn(PeriodTypesEnum periodTypesEnum)
        {
            switch (periodTypesEnum)
            {
                case PeriodTypesEnum.Month:
                    return "dd";
                case PeriodTypesEnum.Weeks:
                    break;
                case PeriodTypesEnum.Year:
                    return "MMMM";
                case PeriodTypesEnum.Years10:
                    break;
                default:
                    break;
            }
            return "";
        }

        private string GetNameColumnByTemplateColumnType(TemplateColumnType firstColumn)
        {
            switch (firstColumn)
            {
                case TemplateColumnType.BudgetSection:
                    break;
                case TemplateColumnType.DaysForMonth:
                    return "Дни";
                case TemplateColumnType.MonthsForYear:
                    return "Месяцы";
                case TemplateColumnType.YearsFor10Year:
                    break;
                case TemplateColumnType.Percent:
                    break;
                case TemplateColumnType.Comment:
                    break;
                case TemplateColumnType.WeeksForMonth:
                    break;
                case TemplateColumnType.Error:
                    break;
                default:
                    return "Дни";
            }
            return "Дни";
        }

        private TemplateColumnType GetDaysTemplateColumnTypeByPeriodType(PeriodTypesEnum periodTypesEnum)
        {
            switch (periodTypesEnum)
            {
                case PeriodTypesEnum.Month:
                    return TemplateColumnType.DaysForMonth;
                case PeriodTypesEnum.Weeks:
                    break;
                case PeriodTypesEnum.Year:
                    return TemplateColumnType.MonthsForYear;
                case PeriodTypesEnum.Years10:
                    break;
                default:
                case PeriodTypesEnum.Undefined:
                    return TemplateColumnType.DaysForMonth;
            }
            return TemplateColumnType.DaysForMonth;
        }
    }
}
