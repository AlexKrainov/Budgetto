using LinqKit;
using Microsoft.EntityFrameworkCore;
using MyProfile.Budget.Service;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView.BudgetView;
using MyProfile.Entity.ModelView.Chart;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
            Stopwatch stopwatch = new Stopwatch();
            List<string> stopwatchs = new List<string>();
            bool isShow = true;


            stopwatch.Start();//1

            var currentUser = UserInfo.Current;

            stopwatch.Stop();
            stopwatchs.Add((stopwatch.ElapsedMilliseconds).ToString());
            stopwatch.Reset();

            List<ChartEditModel> charts = new List<ChartEditModel>();
            Func<TmpBudgetRecord, int> groupBy = x => x.DateTimeOfPayment.Day;


            stopwatch.Start();//2

            if (periodTypesEnum == PeriodTypesEnum.Month)
            {
                isShow = currentUser.UserSettings.Month_BigCharts;
                charts = await GetChartListView(x => x.VisibleElement.IsShow_BudgetMonth);
            }
            else if (periodTypesEnum == PeriodTypesEnum.Year)
            {
                isShow = currentUser.UserSettings.Year_BigCharts;
                charts = await GetChartListView(x => x.VisibleElement.IsShow_BudgetYear);
                groupBy = x => x.DateTimeOfPayment.Month;
            }

            stopwatch.Stop();
            stopwatchs.Add((stopwatch.ElapsedMilliseconds).ToString());
            stopwatch.Reset();

            stopwatch.Start();//3

            var chartIDs = charts.Select(x => x.ID);
            var allSections = repository.GetAll<Chart>(x => chartIDs.Contains(x.ID))
                .SelectMany(x => x.ChartFields)
                .SelectMany(x => x.SectionGroupCharts)
                .Select(x => x.BudgetSectionID);

            stopwatch.Stop();
            stopwatchs.Add((stopwatch.ElapsedMilliseconds).ToString());
            stopwatch.Reset();

            stopwatch.Start();//4

            var dataGroupByDay = budgetRecordService.GetBudgetRecordsGroup(start, finish,
                groupBy,
                y => allSections.Contains(y.BudgetSectionID)).ToList();

            stopwatch.Stop();
            stopwatchs.Add((stopwatch.ElapsedMilliseconds).ToString());
            stopwatch.Reset();

            List<ChartViewModel> chartData = new List<ChartViewModel>();

            var labels = new List<string>();
            int totalDaysOrMonth = 12;

            if (charts.Count > 0 && periodTypesEnum == PeriodTypesEnum.Month)
            {
                totalDaysOrMonth = (int)((finish - start).TotalDays + 1);

                for (int day = 1; day < totalDaysOrMonth; day++)
                {
                    labels.Add(day.ToString());
                }
            }
            else if (charts.Count > 0 && periodTypesEnum == PeriodTypesEnum.Year)
            {
                labels = new List<string> { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12" };
            }

            foreach (var chart in charts)
            {
                stopwatch.Start();//5

                ChartViewModel chartViewModel = new ChartViewModel
                {
                    ChartID = "bigChart_" + chart.ID,
                    Name = chart.Name,
                    Decription = chart.Description,
                    ChartTypesEnum = chart.ChartTypeID,
                    ChartTypeCodeName = chart.ChartTypeCodeName,
                    DataSets = new List<IChartDataSet>(),
                    Labels = new List<string>(),
                    IsShow = isShow
                };

                switch (chart.ChartTypeID)
                {
                    case ChartTypesEnum.Line:
                    case ChartTypesEnum.GroupedBar:

                        if (periodTypesEnum == PeriodTypesEnum.Month)
                        {
                            foreach (var fieldItem in chart.Fields)
                            {
                                IChartData chartLine;

                                if (chart.ChartTypeID == ChartTypesEnum.Line)
                                {
                                    chartLine = new ChartLineViewModel
                                    {
                                        Label = fieldItem.Name,
                                        BorderColor = fieldItem.CssColor,
                                        Fill = true,
                                        Data = new decimal[totalDaysOrMonth + 1]
                                    };
                                }
                                else
                                {
                                    chartLine = new ChartGroupedBarViewModel
                                    {
                                        Label = fieldItem.Name,
                                        BackgroundColor = fieldItem.CssColor,
                                        Data = new decimal[totalDaysOrMonth + 1]
                                    };
                                }

                                foreach (var _data in dataGroupByDay)
                                {
                                    chartLine.Data[_data.Key] = _data.Where(x => fieldItem.Sections.Contains(x.SectionID)).Sum(x => x.Total);
                                }

                                chartLine.Data = chartLine.Data.Skip(1).ToArray();

                                chartViewModel.DataSets.Add(chartLine);
                            }
                        }
                        else if (periodTypesEnum == PeriodTypesEnum.Year)
                        {
                            foreach (var fieldItem in chart.Fields)
                            {
                                IChartData chartLine;

                                if (chart.ChartTypeID == ChartTypesEnum.Line)
                                {
                                    chartLine = new ChartLineViewModel
                                    {
                                        Label = fieldItem.Name,
                                        BorderColor = fieldItem.CssColor,
                                        Fill = true,
                                        Data = new decimal[totalDaysOrMonth + 1]
                                    };
                                }
                                else
                                {
                                    chartLine = new ChartGroupedBarViewModel
                                    {
                                        Label = fieldItem.Name,
                                        BackgroundColor = fieldItem.CssColor,
                                        Data = new decimal[totalDaysOrMonth + 1]
                                    };
                                }

                                foreach (var _data in dataGroupByDay)
                                {
                                    chartLine.Data[_data.Key] = await budgetRecordService.GetTotalSpendsForLimitByFilter(new Entity.ModelView.CalendarFilterModels
                                    {
                                        StartDate = new DateTime(start.Year, _data.Key, 01, 00, 00, 00),
                                        EndDate = new DateTime(start.Year, _data.Key, DateTime.DaysInMonth(start.Year, _data.Key), 23, 59, 59),
                                        Sections = fieldItem.Sections.ToList()
                                    });
                                }

                                chartLine.Data = chartLine.Data.Skip(1).ToArray();

                                chartViewModel.DataSets.Add(chartLine);
                            }
                        }
                        chartViewModel.Labels = labels;
                        break;
                    case ChartTypesEnum.Bar:
                    case ChartTypesEnum.Pie:
                    case ChartTypesEnum.Doughnut:
                        //if (periodTypesEnum == PeriodTypesEnum.Month)
                        //{
                        var chartPie = new ChartPieViewModel
                        {
                            BackgroundColor = new List<string>(),
                            Label = string.Empty,
                            Data = new List<decimal>()
                        };

                        foreach (var fieldItem in chart.Fields)
                        {
                            chartPie.BackgroundColor.Add(fieldItem.CssColor);

                            chartPie.Data.Add(await budgetRecordService.GetTotalSpendsForLimitByFilter(new Entity.ModelView.CalendarFilterModels
                            {
                                StartDate = start,
                                EndDate = finish,
                                Sections = fieldItem.Sections.ToList()
                            })); //<-- 1100 ElapsedMilliseconds

                            //chartPie.Data.Add(dataGroupBySection.Where(x => fieldItem.Sections.Contains(x.Key)).Sum(y => y.Sum(p => p.Total))); //<-- 3600 ElapsedMilliseconds
                            //chartPie.Data.Add(data2.Where(x => fieldItem.Sections.Contains(x.Key)).Sum(x => x.Key));//<-- 7100 ElapsedMilliseconds

                            chartViewModel.Labels.Add(fieldItem.Name);
                        }
                        chartViewModel.DataSets.Add(chartPie);
                        //}
                        //else if (periodTypesEnum == PeriodTypesEnum.Year)
                        //{

                        //}
                        break;
                    case ChartTypesEnum.Bubble:
                        break;
                    default:
                    case ChartTypesEnum.Undefined:
                        break;
                }

                chartData.Add(chartViewModel);

                stopwatch.Stop();
                stopwatchs.Add(chart.ChartTypeCodeName + " : " + (stopwatch.ElapsedMilliseconds).ToString());
                stopwatch.Reset();
            }

            return chartData;
        }

        public async Task<List<ChartEditModel>> GetChartListView(Expression<Func<Chart, bool>> expression = null)
        {
            var currentUser = UserInfo.Current;
            var predicate = PredicateBuilder.True<Chart>();

            predicate = predicate.And(x => x.UserID == currentUser.ID
                //&& currentUser.UserSettings.Month_BigCharts
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
                        Sections = y.SectionGroupCharts
                            .Select(z => z.BudgetSectionID)
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
