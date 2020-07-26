using LinqKit;
using Microsoft.EntityFrameworkCore;
using MyProfile.Budget.Service;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView.Chart;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MyProfile.Chart.Service
{
    using Chart = MyProfile.Entity.Model.Chart;

    public class ChartService
    {
        private IBaseRepository repository;
        private BudgetRecordService budgetRecordService;

        public ChartService(IBaseRepository repository)
        {
            this.repository = repository;
            this.budgetRecordService = new BudgetRecordService(repository);
        }

        public async Task<int> CreateOrUpdate(ChartEditModel chart)
        {
            var now = DateTime.Now.ToUniversalTime();

            if (chart.ID > 0)//update
            {
                var oldChart = await repository
                    .GetAll<Chart>(x => x.ID == chart.ID)
                    .Select(x => new
                    {
                        x.DateCreate,
                        x.VisibleElementID,
                        ChartFields = x.ChartFields.ToList(),
                    })
                    .FirstOrDefaultAsync();

                Chart newChart = new Chart
                {
                    ID = chart.ID,
                    DateCreate = oldChart.DateCreate,
                    LastDateEdit = now,
                    Name = chart.Name ?? "График",
                    Description = chart.Description,
                    UserID = UserInfo.Current.ID,
                    ChartTypeID = (int)chart.ChartTypeID,
                    ChartFields = chart.Fields.Select(x => new ChartField
                    {
                        ChartID = chart.ID,
                        Name = x.Name,
                        CssColor = x.CssColor,
                        SectionGroupCharts = x.Sections
                            .Select(y => new SectionGroupChart
                            {
                                BudgetSectionID = y,
                            })
                            .ToList()
                    }).ToList(),
                    VisibleElementID = oldChart.VisibleElementID,
                    VisibleElement = new VisibleElement
                    {
                        ID = oldChart.VisibleElementID,
                        IsShow_BudgetMonth = chart.IsShowBudgetMonth,
                        IsShow_BudgetYear = chart.IsShowBudgetYear
                    }
                };

                repository.DeleteRange(oldChart.ChartFields);
                await repository.SaveAsync();
                await repository.UpdateAsync(newChart, true);
            }
            else
            {
                Chart newChart = new Chart
                {
                    ChartTypeID = (int)chart.ChartTypeID,
                    ID = chart.ID,
                    DateCreate = now,
                    LastDateEdit = now,
                    Name = chart.Name ?? "График",
                    Description = chart.Description,
                    UserID = UserInfo.Current.ID,
                    ChartFields = chart.Fields.Select(x => new ChartField
                    {
                        Name = x.Name,
                        CssColor = x.CssColor,
                        SectionGroupCharts = x.Sections.Select(y => new SectionGroupChart
                        {
                            BudgetSectionID = y,
                        }).ToList()
                    }).ToList(),
                    VisibleElement = new VisibleElement
                    {
                        IsShow_BudgetMonth = chart.IsShowBudgetMonth,
                        IsShow_BudgetYear = chart.IsShowBudgetYear
                    }
                };

                await repository.CreateAsync(newChart, true);
                await repository.SaveAsync();
            }
            return 1;
        }

        public async Task<List<ChartViewModel>> GetChartData(DateTime start, DateTime finish, PeriodTypesEnum periodTypesEnum)
        {
            var currentUser = UserInfo.Current;
            List<ChartEditModel> charts = new List<ChartEditModel>();

            if (periodTypesEnum == PeriodTypesEnum.Month)
            {
                charts = await GetChartListView(x => x.VisibleElement.IsShow_BudgetMonth);
            }
            else if (periodTypesEnum == PeriodTypesEnum.Year)
            {
                charts = await GetChartListView(x => x.VisibleElement.IsShow_BudgetYear);
            }
            var allSections = charts.SelectMany(x => x.Fields).SelectMany(x => x.Sections).ToList();

            var data = budgetRecordService.GetBudgetRecordsGroup(start, finish,
                x => x.DateTimeOfPayment.Day,
                y => allSections.Contains(y.BudgetSectionID));

            List<ChartViewModel> chartData = new List<ChartViewModel>();

            foreach (var chart in charts)
            {
                ChartViewModel chartViewModel = new ChartViewModel
                {
                    ChartID = "bigChart_" + chart.ID,
                    Name = chart.Name,
                    Decription = chart.Description,
                    ChartTypesEnum = chart.ChartTypeID,
                    ChartTypeCodeName = chart.ChartTypeCodeName,
                    DataSets = new List<IChartDataSet>(),
                    Labels = new List<string>(),
                    IsShow = currentUser.UserSettings.BudgetPages_IsShow_BigCharts
                };

                switch (chart.ChartTypeID)
                {
                    case ChartTypesEnum.Line:

                        if (periodTypesEnum == PeriodTypesEnum.Month)
                        {
                            for (int day = 1; day < (finish - start).TotalDays; day++)
                            {
                                chartViewModel.Labels.Add(day.ToString());
                            }

                            foreach (var fieldItem in chart.Fields)
                            {
                                var chartLine = new ChartLineViewModel
                                {
                                    Label = fieldItem.Name,
                                    BorderColor = fieldItem.CssColor,
                                    Fill = true,
                                    Data = new List<decimal>()
                                };

                                for (int day = 1; day < (finish - start).TotalDays; day++)
                                {
                                    var _data = data.FirstOrDefault(x => x.Key == day);

                                    if (_data != null)
                                    {
                                        chartLine.Data.Add(_data.Where(x => fieldItem.Sections.Contains(x.SectionID)).Sum(x => x.Total));
                                    }
                                    else
                                    {
                                        chartLine.Data.Add(0);
                                    }
                                }
                                chartViewModel.DataSets.Add(chartLine);
                            }
                        }
                        else if (periodTypesEnum == PeriodTypesEnum.Year)
                        {

                        }
                        break;
                    case ChartTypesEnum.Bar:
                    case ChartTypesEnum.Pie:
                    case ChartTypesEnum.Doughnut:
                        if (periodTypesEnum == PeriodTypesEnum.Month)
                        {
                            var chartPie = new ChartPieViewModel
                            {
                                BackgroundColor = new List<string>(),
                                Label = string.Empty,
                                Data = new List<decimal>()
                            };

                            foreach (var fieldItem in chart.Fields)
                            {
                                chartPie.BackgroundColor.Add(fieldItem.CssColor);
                                chartPie.Data.Add(data.SelectMany(x => x).Where(x => fieldItem.Sections.Contains(x.SectionID)).Sum(x => x.Total));

                                chartViewModel.Labels.Add(fieldItem.Name);
                            }
                            chartViewModel.DataSets.Add(chartPie);
                        }
                        else if (periodTypesEnum == PeriodTypesEnum.Year)
                        {

                        }
                        break;
                    case ChartTypesEnum.Bubble:
                        break;
                    case ChartTypesEnum.GroupedBar:
                        if (periodTypesEnum == PeriodTypesEnum.Month)
                        {
                            for (int day = 1; day < (finish - start).TotalDays; day++)
                            {
                                chartViewModel.Labels.Add(day.ToString());
                            }

                            foreach (var fieldItem in chart.Fields)
                            {
                                var chartLine = new ChartGroupedBarViewModel
                                {
                                    Label = fieldItem.Name,
                                    BackgroundColor = fieldItem.CssColor,
                                    Data = new List<decimal>()
                                };

                                for (int day = 1; day < (finish - start).TotalDays; day++)
                                {
                                    var _data = data.FirstOrDefault(x => x.Key == day);

                                    if (_data != null)
                                    {
                                        chartLine.Data.Add(_data.Where(x => fieldItem.Sections.Contains(x.SectionID)).Sum(x => x.Total));
                                    }
                                    else
                                    {
                                        chartLine.Data.Add(0);
                                    }
                                }
                                chartViewModel.DataSets.Add(chartLine);
                            }
                        }
                        else if (periodTypesEnum == PeriodTypesEnum.Year)
                        {

                        }
                        break;
                    default:
                    case ChartTypesEnum.Undefined:
                        break;
                }

                chartData.Add(chartViewModel);
            }

            return chartData;
        }

        public async Task<List<ChartEditModel>> GetChartListView(Expression<Func<Chart, bool>> expression = null)
        {
            var currentUser = UserInfo.Current;
            var predicate = PredicateBuilder.True<Chart>();

            predicate = predicate.And(x => x.UserID == currentUser.ID
                //&& currentUser.UserSettings.BudgetPages_IsShow_BigCharts
                && x.IsDeleted == false);

            if (expression != null) { predicate = predicate.And(expression); }

            return await repository.GetAll<Chart>(predicate)
                .Select(x => new ChartEditModel
                {
                    ID = x.ID,
                    Name = x.Name,
                    Description = x.Description,
                    DateCreate = x.DateCreate,
                    LastDateEdit = x.LastDateEdit,
                    ChartTypeID = (ChartTypesEnum)x.ChartTypeID,
                    ChartTypeCodeName = x.ChartType.CodeName,
                    ChartTypeName = x.ChartType.Name,
                    IsShowBudgetMonth = x.VisibleElement.IsShow_BudgetMonth,
                    IsShowBudgetYear = x.VisibleElement.IsShow_BudgetYear,
                    Fields = x.ChartFields.Select(y => new ChartFieldItem
                    {
                        CssColor = y.CssColor,
                        Name = y.Name,
                        ID = y.ID,
                        Sections = y.SectionGroupCharts.Select(z => z.BudgetSectionID).ToList()
                    })
                })
                .ToListAsync();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="isRemove">== true - remove, == false - recovery</param>
        /// <returns></returns>
        public async Task<bool> RemoveOrRecovery(ChartEditModel chart, bool isRemove)
        {
            var db_chart = await repository.GetAll<Chart>(x => x.ID == chart.ID && x.UserID == UserInfo.Current.ID).FirstOrDefaultAsync();

            if (db_chart != null)
            {
                db_chart.IsDeleted = isRemove;
                db_chart.LastDateEdit = DateTime.Now.ToUniversalTime();
                await repository.UpdateAsync(db_chart, true);
                return true;
            }
            return false;
        }

    }
}
