var BudgetMethods = {
    periodType: PeriodTypeEnum.Year,
    mounted: function () {
        this.getPageSettings();
        this.templateID = document.getElementById("templateID_hidden").value;
        this.budgetYear = document.getElementById("budgetMonth_hidden").value;

        this.refresh();
        window.layoutHelpers.on('resize', this.resizeAll);

        //this.earningData.isShow = UserInfo.UserSettings.Dashboard_Month_IsShow_EarningChart;
        //this.spendingData.isShow = UserInfo.UserSettings.Dashboard_Month_IsShow_SpendingChart;
        //this.investingData.isShow = UserInfo.UserSettings.Dashboard_Month_IsShow_InvestingChart;

        RecordVue.callback = this.refreshAfterChangeRecords;
    },
    load: function () {
        return sendAjax("/Budget/GetYearBudget?year=" + this.budgetYear + "&templateID=" + this.templateID, null, "POST")
            .then(function (result) {
                if (result.isOk == true) {
                    BudgetVue.rows = result.rows;
                    BudgetVue.footerRow = result.footerRow;
                    BudgetVue.template = result.template;

                }
            });

        $.fn.dataTable.SearchPanes.defaults = false;
    },
    changeView: function (year) {
        let select = $("#budget-year")[0];
        //let selectedIndex = select.selectedIndex;
        //let allOptions = select.options.length;

        //if (year == -1 && selectedIndex != 0) {
        //    this.budgetYear = this.budgetYear * 1 + year;
        //    return this.refresh("runtimeData");
        //}
        //if (year == 1 && ++selectedIndex < allOptions) {
        //    this.budgetYear = this.budgetYear * 1 + year;
        //    return this.refresh("runtimeData");
        //}
        if (year == -1) {
            let val = $("#budget-year option:selected").prev().prop("selected", true).text();
            if (val) {
                this.budgetYear = val;
                return this.refresh("runtimeData");
            }
        }
        if (year == 1) {
            let val = $("#budget-year option:selected").next().prop("selected", true).text();
            if (val) {
                this.budgetYear = val;
                return this.refresh("runtimeData");
            }
        }
    },
    //Total charts
    loadTotalCharts: function () {
        if (!(UserInfo.UserSettings.Dashboard_Year_IsShow_InvestingChart
            || UserInfo.UserSettings.Dashboard_Year_IsShow_SpendingChart
            || UserInfo.UserSettings.Dashboard_Year_IsShow_EarningChart)) {
            return false;
        }
        return $.ajax({
            type: "GET",
            url: "/BudgetTotal/LoadByYear?year=" + this.budgetYear,
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
    //Limit charts
    loadLimitCharts: function () {
        if (!UserInfo.UserSettings.Dashboard_Year_IsShow_LimitCharts) {
            return false;
        }
        return $.ajax({
            type: "GET",
            url: "/Limit/LoadCharts?year=" + this.budgetYear + "&periodTypesEnum=3",
            contentType: "application/json",
            dataType: 'json',
            context: this,
            success: function (response) {

                this.limitsChartsData = response.limitsChartsData;

                setTimeout(this.initLimitCharts, 10);
            }
        });
    },
    //Goal charts
    loadGoalCharts: function () {
        if (!UserInfo.UserSettings.Dashboard_Year_IsShow_GoalCharts) {
            return false;
        }

        return $.ajax({
            type: "GET",
            url: `/Goal/LoadCharts?year=${this.budgetYear}&periodTypesEnum=3`,
            contentType: "application/json",
            dataType: 'json',
            context: this,
            success: function (response) {

                this.goalChartsData = response.goalChartsData;

                setTimeout(this.initGoalCharts, 10);
            }
        });
    },
    //big charts
    loadBigCharts: function () {
        if (!UserInfo.UserSettings.Dashboard_Year_IsShow_BigCharts) {
            return false;
        }

        for (var i = 0; i < this.bigChartsData.length; i++) {
            ShowLoading('#bigChart_' + this.bigChartsData[i].chartID);
        }

        return $.ajax({
            type: "GET",
            url: "/Chart/LoadCharts?year=" + this.budgetYear + "&periodType=3",
            contentType: "application/json",
            dataType: 'json',
            context: this,
            success: function (response) {

                this.bigChartsData = response.bigChartsData;

                for (var i = 0; i < this.bigChartsData.length; i++) {
                    HideLoading('#bigChart_' + this.bigChartsData[i].chartID);
                }

                setTimeout(this.initBigChartCharts, 10);
            }
        });
    },
};

//var BudgetVue1 = new Vue({
//	el: "#budget-vue",
//	data: {
//		budgetYear: null,
//		templateID: null,

//		template: {},
//		rows: [],
//		footerRow: [],

//		column: {},
//		flatpickr: {},

//		records: [],
//	},
//	watch: {
//	},
//	mounted: function () {

//		this.templateID = document.getElementById("templateID_hidden").value;
//		this.budgetYear = document.getElementById("budgetMonth_hidden").value;

//		this.refresh();
//	},
//	methods: {
//		load: function () {
//			return sendAjax("/Budget/GetYearBudget?year=" + this.budgetYear + "&templateID=" + this.templateID, null, "POST")
//				.then(function (result) {
//					if (result.isOk == true) {
//						BudgetVue.rows = result.rows;
//						BudgetVue.footerRow = result.footerRow;
//						BudgetVue.template = result.template;

//					}
//				});

//			$.fn.dataTable.SearchPanes.defaults = false;
//		},
//		refresh: function () {
//			this.load().then(function () {
//				BudgetVue.initTable();
//			});
//		},
//		initTable: function () {
//			$("#table").DataTable();
//		},
//		getCellValue: function (cell) {
//			if (cell.value.indexOf(",")) {
//				let values = cell.value.split(",");

//				if (values.length == 2) {
//					return `<span>${values[0]}<span class="money-muted">,${values[1]}</span></span>`;
//				} else {
//					return `<span>${cell.value}</span>`
//				}
//			} else {
//				return `<span>${cell.value}</span>`
//			}
//		},
//		clickCell: function (rowIndex, cellIndex) {
//			let templateColumnTypes = [2, 3, 4, 7]; // DaysForMonth = 2,MonthsForYear = 3,YearsFor10Year = 4,WeeksForMonth = 7

//			let sections = [];

//			if (this.template.columns[cellIndex].templateColumnType == 1) {//BudgetSection/Money
//				sections = this.template.columns[cellIndex].templateBudgetSections.map(x => x.sectionID);
//			} else if (templateColumnTypes.indexOf(this.template.columns[cellIndex].templateColumnType) >= 0) {
//				for (var i = 0; i < this.template.columns.length; i++) {
//					sections = sections.concat(this.template.columns[i].templateBudgetSections.map(x => x.sectionID));
//				}
//			} else {
//				return;
//			}

//			let filter = {
//				sections: sections,
//				startDate: moment(new Date(this.budgetYear, rowIndex, 1)).format(),
//				endDate: moment(new Date(this.budgetYear, rowIndex, 1)).endOf("month").format()
//			};

//			return this.loadTimeLine(filter)
//		},
//		clickFooterCell: function (cellIndex) {
//			let filter = {
//				sections: this.template.columns[cellIndex].templateBudgetSections.map(x => x.sectionID),
//				startDate: moment(new Date(this.budgetYear, 0, 1)).format(),
//				endDate: moment(new Date(this.budgetYear, 11, 31)).format(),
//			};

//			return this.loadTimeLine(filter)
//		},
//		loadTimeLine: function (filter) {
//			return $.ajax({
//				type: "POST",
//				url: "/Budget/LoadingRecordsForTableView",
//				data: JSON.stringify(filter),
//				contentType: "application/json",
//				dataType: 'json',
//				context: this,
//				success: function (response) {
//					this.records = response.data;
//					$("#modalTimeLine").modal("show");
//				}
//			});
//		},
//		GetDateByFormat: function (date, format) {
//			return GetDateByFormat(date, format);
//		},
//		edit: function (record) {

//			RecordVue.recordComponent.editByElement({
//				id: record.id,
//				isCorrect: true,
//				isSaved: true,
//				money: record.money,
//				sectionID: record.sectionID,
//				sectionName: record.sectionName,
//				tag: record.rawData,
//				dateTimeOfPayment: record.dateTimeOfPayment,
//			},
//				BudgetVue.load
//			);
//			$("#modalTimeLine").modal("hide");
//		}
//	}
//});


Vue.config.devtools = true;
