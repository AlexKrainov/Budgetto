var BudgetVue = new Vue({
    el: "#budget-vue",
    data: {
        budgetDate: null,
        budgetYear: null,
        templateID: null,

        template: {},
        rows: [],
        footerRow: [],

        column: {},
        flatpickr: {},

        records: [],

        //table
        dataTable: null,
        pageViewSettings: {
            version: 1.0,
            isTableViewCompact: false
        },

        //total charts
        earningData: {},
        earningChart: undefined,

        spendingData: {},
        spendingChart: undefined,

        investingData: {},
        investingChart: undefined,

        //limit charts
        limitsChartsData: [],
        limitsCharts: [],

        //Goal charts
        goalChartsData: [],
        goalCharts: [],

        //Big charts
        bigChartsData: [],
        bigCharts: [],
        bigChartHeight: 310,
    },
    watch: {},
    mounted: BudgetMethods.mounted,
    methods: {
        load: BudgetMethods.load,
        //Total charts
        loadTotalCharts: BudgetMethods.loadTotalCharts,
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
                        backgroundColor: 'rgba(2, 188, 119, .2)',
                        borderColor: 'rgba(2, 188, 119, 1)',
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
                        backgroundColor: 'rgba(217, 83, 79, .2)',
                        borderColor: 'rgba(217, 83, 79, 1)',
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

            setTimeout(this.resizeTotalCharts, 50);
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
            this.refrehViewTable();
        },
        //Limit charts
        loadLimitCharts: WidgetsMethods.loadLimitCharts,
        initLimitCharts: WidgetsMethods.initLimitCharts,
        resizeLimitCharts: WidgetsMethods.resizeLimitCharts,
        //Goal charts
        loadGoalCharts: WidgetsMethods.loadGoalCharts,
        initGoalCharts: WidgetsMethods.initGoalCharts,
        resizeGoalCharts: WidgetsMethods.resizeGoalCharts,
        //big charts
        loadBigCharts: WidgetsMethods.loadBigCharts,
        initBigChartCharts: WidgetsMethods.initBigChartCharts,
        resizeBigCharts: WidgetsMethods.resizeBigCharts,
        //resize and refresh
        refresh: function (type, onlyRuntimeData) {
            if (type == undefined || type == 'onlyTable' || onlyRuntimeData) {
                this.load()
                    .then(function () {
                        BudgetVue.initTable();
                    });
            }

            if (type == 'onlyTable') {
                return false;
            }

            if (onlyRuntimeData) {
                this.loadBigCharts();
                this.loadTotalCharts();
                this.loadLimitCharts();
                return false;
            }

            this.loadBigCharts();
            this.loadTotalCharts();
            this.loadLimitCharts();
            this.loadGoalCharts();
        },
        refreshAfterChangeRecords: function (dateTimeOfPayment) {
            let dateOfPayment = moment(dateTimeOfPayment);
            let currentBudgetDate = moment(this.budgetDate, "YYYY/MM/DD");
            if (dateOfPayment.get("month") == currentBudgetDate.get("month") && dateOfPayment.get("year") == currentBudgetDate.get("year")) {
                return this.refresh(undefined, true);
            }
            return false;
        },
        resizeAll: function () {
            this.resizeTotalCharts();
            this.resizeLimitCharts();
            this.resizeGoalCharts();
            this.resizeBigCharts();
        },
        refrehViewTable: function () {
            if (this.dataTable && this.dataTable.fixedHeader) {
                this.dataTable.fixedHeader.adjust()
            }
        },
        initTable: function () {
            this.dataTable = $("#table").DataTable();
        },
        //View cell
        getCellContent: function (cell, cellIndex, rowIndex) {
            return this.getCellActions(cell, cellIndex, rowIndex) + this.getCellValue(cell);
        },
        getCellFooterContent: function (cell, cellIndex) {
            return this.getCellFooterActions(cellIndex) + this.getCellValue(cell);
        },
        getCellActions: function (cell, cellIndex, rowIndex) {
            return `
            <span class="float-left cell-actions">
                <i class="ion ion-md-add add-cell-action" onclick="RecordVue.showModel('${cell.currentDate}', 'BudgetVue.refreshAfterChangeRecords')"></i>
                <i class="fas fa-history show-history-cell-action pl-1" onclick="BudgetVue.clickCell(${rowIndex}, ${cellIndex},'${cell.currentDate}', event)"></i>
            </span>`;
        },
        getCellFooterActions: function (cellIndex) {
            return `
            <span class="float-left cell-actions">
                <i class="fas fa-history show-history-cell-action pl-1" onclick="BudgetVue.clickFooterCell(${cellIndex})"></i>
            </span>`;
        },
        getCellValue: function (cell) {
            if (cell.value.indexOf(",")) {
                let values = cell.value.split(",");

                if (values.length == 2) {
                    return `<span data-type="${cell.templateColumnType}" data-value='${cell.naturalValue}'>${values[0]}<span class="money-muted">,${values[1]}</span></span>`;
                } else {
                    return `<span data-type="${cell.templateColumnType}" data-value='${cell.naturalValue}'>${cell.value}</span>`;
                }
            } else {
                return `<span data-type="${cell.templateColumnType}" data-value='${cell.naturalValue}'>${cell.value}</span>`;
            }
        },
        mouseenterCell: function ($event) {
            if ($event.target.nodeName == "TD" && $event.target.classList.contains("show-actions") == false) {
                $event.target.classList.add("show-actions");
                return;
            }
        },
        mouseleaveCell: function ($event) {
            if ($event.target.nodeName == "TD" && $event.target.classList.contains("show-actions")) {
                $event.target.classList.remove("show-actions");
                return;
            }
        },
        clickCell: function (rowIndex, cellIndex, currentDate, event) {
            let templateColumnTypes = [2, 3, 4, 7]; // DaysForMonth = 2,MonthsForYear = 3,YearsFor10Year = 4,WeeksForMonth = 7

            let sections = [];

            if (this.template.columns[cellIndex].templateColumnType == 1) {//BudgetSection/Money
                sections = this.template.columns[cellIndex].templateBudgetSections.map(x => x.sectionID);
                this.stylingClickedCells(event, "td");
            } else if (templateColumnTypes.indexOf(this.template.columns[cellIndex].templateColumnType) >= 0) {
                for (var i = 0; i < this.template.columns.length; i++) {
                    sections = sections.concat(this.template.columns[i].templateBudgetSections.map(x => x.sectionID));
                }
                this.stylingClickedCells(event, "tr");
            } else {
                return;
            }

            let filter = {
                sections: sections,
                startDate: moment(this.budgetDate, "YYYY/MM/DD").add(rowIndex, "days").format(),
                endDate: moment(this.budgetDate, "YYYY/MM/DD").add((rowIndex + 1), "days").add(-1, "minutes").format()
            };

            return HistoryVue.showHistory(filter, currentDate);
        },
        clickFooterCell: function (cellIndex) {
            let filter = {
                sections: this.template.columns[cellIndex].templateBudgetSections.map(x => x.sectionID),
                startDate: moment(this.budgetDate).format(),
                endDate: moment(this.budgetDate).endOf("month").format()
            };

            this.stylingClickedCells(event, "td_s", cellIndex);

            return HistoryVue.showHistory(filter)
        },
        stylingClickedCells(event, type, cellIndex) {
            this.clearAllStyle();

            if (type == "td") {
                $(event.target).closest("td").addClass("table-primary");
            } else if (type == "tr") {
                $(event.target).closest("tr").addClass("table-primary");
            } else if (type == "td_s") {
                $("#table td:nth-child(" + ++cellIndex + ")").addClass("table-primary");
            }
        },
        //after clicked a row, a cell or a footer cell
        clearAllStyle() {
            let el_s = [...document.getElementsByClassName("table-primary")];
            console.log(el_s);
            for (var i = 0; i < el_s.length; i++) {
                el_s[i].classList.remove("table-primary");
                console.log(i);
            }
        },
        // page and table settings
        savePageSettings: function () {
            localStorage.setItem("pageViewSettings", JSON.stringify(this.pageViewSettings));
        },
        savePageSettings: function () {
            localStorage.setItem("pageViewSettings", JSON.stringify(this.pageViewSettings));
        },
        getPageSettings: function () {
            let pageViewSettings = localStorage.getItem("pageViewSettings");
            if (pageViewSettings != null) {
                try {
                    pageViewSettings = JSON.parse(pageViewSettings);

                    this.pageViewSettings = pageViewSettings;
                } catch (e) {

                }
            }
        },
    }
});

Vue.config.devtools = true;
