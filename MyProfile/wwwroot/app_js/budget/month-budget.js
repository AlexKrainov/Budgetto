﻿var BudgetVue = new Vue({
	el: "#budget-vue",
	data: {
		budgetDate: null,
		templateID: null,

		template: {},
		rows: [],
		footerRow: [],

		column: {},
		flatpickr: {},

		records: [],

		//charts
		earningData: { isShow: true },
		earningChart: undefined,

		spendingData: { isShow: true },
		spendingChart: undefined,

		investingData: { isShow: true },
		investingChart: undefined,
	},
	watch: {

	},
	mounted: function () {
		this.templateID = document.getElementById("templateID_hidden").value;
		this.budgetDate = GetDateByFormat(Date.parse(document.getElementById("budgetDate_hidden").value), "YYYY/MM/DD");

		//if (this.templateID == -1) {
		//	let options = document.getElementById("templates").children;
		//	if (options && options.length > 0) {
		//		options[0]["selected"] = "selected";
		//	}
		//}

		this.flatpickr = flatpickr('#budget-date', {
			altInput: true,
			//dateFormat: 'd.m.Y',
			defaultDate: this.budgetDate,
			plugins: [
				new monthSelectPlugin({
					shorthand: true, //defaults to false
					dateFormat: "yy/m/d", //defaults to "F Y"
					altFormat: "F Y", //defaults to "F Y"
					theme: "dark" // defaults to "light"
				})
			]
		});

		this.refresh();

		window.layoutHelpers.on('resize', this.resizeTotalCharts);
	},
	methods: {
		load: function () {
			return sendAjax("/Budget/GetMonthBudget?month=" + this.budgetDate + "&templateID=" + this.templateID, null, "POST")
				.then(function (result) {
					if (result.isOk == true) {
						BudgetVue.rows = result.rows;
						BudgetVue.footerRow = result.footerRow;
						BudgetVue.template = result.template;

					}
				});
			//$.fn.dataTable.SearchPanes.defaults = false;
		},
		loadTotalCharts: function () {
			return $.ajax({
				type: "GET",
				url: "/BudgetTotal/Load?to=" + this.budgetDate,
				contentType: "application/json",
				dataType: 'json',
				context: this,
				success: function (response) {
					this.earningData = response.earningData;
					this.spendingData = response.spendingData;
					this.investingData = response.investingData;

					this.initTotalCharts();
				}
			});
		},
		initTotalCharts: function () {
			if (this.earningChart) {
				this.earningChart.destroy();
			}
			this.earningChart = new Chart(document.getElementById('earningChart').getContext("2d"), {
				type: 'line',
				data: {
					datasets: [{
						data: this.earningData.data,
						borderWidth: 1,
						backgroundColor: 'rgba(136, 151, 170, .2)',
						borderColor: 'rgba(136, 151, 170, 1)',
						pointBorderColor: 'rgba(0,0,0,0)',
						pointRadius: 1,
						lineTension: 0
					}],
					labels: this.earningData.labels
				},
				options: {
					scales: {
						xAxes: [{
							display: false,
						}],
						yAxes: [{
							display: false
						}]
					},
					legend: {
						display: false
					},
					tooltips: {
						enabled: true
					},
					responsive: false,
					maintainAspectRatio: false
				}
			});

			if (this.spendingChart) {
				this.spendingChart.destroy()
			}
			this.spendingChart = new Chart(document.getElementById('spendingChart').getContext("2d"), {
				type: 'line',
				data: {
					datasets: [{
						data: this.spendingData.data,
						borderWidth: 1,
						backgroundColor: 'rgba(206, 221, 54, .2)',
						borderColor: 'rgba(206, 221, 54, 1)',
						pointBorderColor: 'rgba(0,0,0,0)',
						pointRadius: 1,
						lineTension: 0
					}],
					labels: this.spendingData.labels
				},
				options: {
					scales: {
						xAxes: [{
							display: false,
						}],
						yAxes: [{
							display: false
						}]
					},
					legend: {
						display: false
					},
					tooltips: {
						enabled: false
					},
					responsive: false,
					maintainAspectRatio: false
				}
			});

			this.investingChart = new Chart(document.getElementById('statistics-chart-5').getContext("2d"), {
				type: 'line',
				data: {
					datasets: [{
						data: [24, 92, 77, 90, 91, 78, 28, 49, 23, 81, 15, 97, 94, 16, 99, 61,
							38, 34, 48, 3, 5, 21, 27, 4, 33, 40, 46, 47, 48, 60
						],
						borderWidth: 1,
						backgroundColor: 'rgba(136, 151, 170, .2)',
						borderColor: 'rgba(136, 151, 170, 1)',
						pointBorderColor: 'rgba(0,0,0,0)',
						pointRadius: 1,
						lineTension: 0
					}],
					labels: ['12', '465', '', '', '', '', '', '', '', '', '', '', '', '', '', '', '', '', '', '', '', '', '', '', '', '', '', '', '', '']
				},

				options: {
					scales: {
						xAxes: [{
							display: false,
						}],
						yAxes: [{
							display: false
						}]
					},
					legend: {
						display: false
					},
					tooltips: {
						enabled: false
					},
					responsive: false,
					maintainAspectRatio: false
				}
			});
			this.resizeTotalCharts();
		},
		resizeTotalCharts: function () {
			if (this.earningChart) {
				this.earningChart.resize();
			}

			if (this.spendingChart) {
				this.spendingChart.resize();
			}

			if (this.investingChart) {
				this.investingChart.resize();
			}
		},

		refresh: function () {
			this.load();
			this.loadTotalCharts();
			//	.then(function () {
			//	BudgetVue.initTable();
			//});
		},
		//initTable: function () {
		//	$("#table").DataTable();
		//},

		//View cell
		getCellValue: function (cell) {
			if (cell.value.indexOf(",")) {
				let values = cell.value.split(",");

				if (values.length == 2) {
					return `<span>${values[0]}<span class="money-muted">,${values[1]}</span></span>`;
				} else {
					return `<span>${cell.value}</span>`;
				}
			} else {
				return `<span>${cell.value}</span>`;
			}
		},
		clickCell: function (rowIndex, cellIndex) {
			let templateColumnTypes = [2, 3, 4, 7]; // DaysForMonth = 2,MonthsForYear = 3,YearsFor10Year = 4,WeeksForMonth = 7

			let sections = [];

			if (this.template.columns[cellIndex].templateColumnType == 1) {//BudgetSection/Money
				sections = this.template.columns[cellIndex].templateBudgetSections.map(x => x.sectionID);
			} else if (templateColumnTypes.indexOf(this.template.columns[cellIndex].templateColumnType) >= 0) {
				for (var i = 0; i < this.template.columns.length; i++) {
					sections = sections.concat(this.template.columns[i].templateBudgetSections.map(x => x.sectionID));
				}
			} else {
				return;
			}

			let filter = {
				sections: sections,
				startDate: moment(this.budgetDate).add(rowIndex, "days").format(),
				endDate: moment(this.budgetDate).add((rowIndex + 1), "days").add(-1, "minutes").format()
			};

			return this.loadTimeLine(filter)
		},
		clickFooterCell: function (cellIndex) {
			let filter = {
				sections: this.template.columns[cellIndex].templateBudgetSections.map(x => x.sectionID),
				startDate: moment(this.budgetDate).format(),
				endDate: moment(this.budgetDate).endOf("month").format()
			};

			return this.loadTimeLine(filter)
		},
		loadTimeLine: function (filter) {
			return $.ajax({
				type: "POST",
				url: "/Budget/LoadingRecordsForTableView",
				data: JSON.stringify(filter),
				contentType: "application/json",
				dataType: 'json',
				context: this,
				success: function (response) {
					this.records = response.data;
					$("#modalTimeLine").modal("show");
				}
			});
		},
		GetDateByFormat: function (date, format) {
			return GetDateByFormat(date, format);
		},
		edit: function (record) {

			RecordVue.recordComponent.editByElement({
				id: record.id,
				isCorrect: true,
				isSaved: true,
				money: record.money,
				sectionID: record.sectionID,
				sectionName: record.sectionName,
				tag: record.rawData,
				dateTimeOfPayment: record.dateTimeOfPayment,
			},
				BudgetVue.load
			);
			$("#modalTimeLine").modal("hide");
		}
	}
});


Vue.config.devtools = true;
