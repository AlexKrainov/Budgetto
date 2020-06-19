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

			Chart newChart = new Chart
			{
				ChartTypeID = (int)chart.ChartTypeID,
				//DateCreate = now,
				ID = chart.ID,
				LastDateEdit = now,
				Name = chart.Name ?? "testChart",
				Description = chart.Description,
				PeriodTypeID = 1,
				UserID = UserInfo.Current.ID,
				ChartFields = chart.Fields.Select(x => new Entity.Model.ChartField
				{
					ChartID = chart.ID,
					Name = x.Name,
					CssColor = x.CssColor,
					SectionGroupCharts = x.Sections.Select(y => new Entity.Model.SectionGroupChart
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

			if (chart.ID > 0)//update
			{
				repository.DeleteRange(await repository.GetAll<Chart>(x => x.ID == chart.ID).SelectMany(y => y.ChartFields).Select(z => z.SectionGroupCharts).ToListAsync());
				repository.DeleteRange(await repository.GetAll<Chart>(x => x.ID == chart.ID).Select(y => y.ChartFields).ToListAsync());
				await repository.SaveAsync();
				repository.Update(newChart);
			}
			else
			{
				newChart.DateCreate = now;
				await repository.CreateAsync(newChart);
			}
			await repository.SaveAsync();
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

			var charts = await repository.GetAll<Chart>(predicate)
				.Select(x => new ChartEditModel
				{
					ID = x.ID,
					Name = x.Name,
					Description = x.Description,
					DateCreate = x.DateCreate,
					LastDateEdit = x.LastDateEdit,
					ChartTypeID = (ChartTypesEnum)x.ChartTypeID,
					ChartTypeCodeName = x.ChartType.CodeName,
					IsShowBudgetMonth = x.VisibleElement.IsShow_BudgetMonth,
					IsShowBudgetYear = x.VisibleElement.IsShow_BudgetYear,
					Fields = x.ChartFields.Select(y => new ChartFieldItem
					{
						CssColor = y.CssColor,
						Name = y.Name,
						ID = y.ID,
						Sections = y.SectionGroupCharts.Select(z => z.BudgetSectionID).ToList()
					}).ToList()
				})
				.ToListAsync();

			return charts;
		}

	}
}
