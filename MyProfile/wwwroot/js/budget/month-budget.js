var BudgetVue = new Vue({
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

        //table

        //total charts
        earningData: { isShow: true },
        earningChart: undefined,

        spendingData: { isShow: true },
        spendingChart: undefined,

        investingData: { isShow: true },
        investingChart: undefined,

        //limit charts
        limitsChartsData: [],
        limitsCharts: [],

        //Goal charts
        goalChartsData: [],
        goalCharts: [],

        //Big charts
        bigChartsData: [],
        bigCharts: []
    },
    watch: {},
    mounted: function () {
        this.templateID = document.getElementById("templateID_hidden").value;
        this.budgetDate = GetDateByFormat(Date.parse(document.getElementById("budgetDate_hidden").value), "YYYY/MM/DD");

        let dateConfig = GetFlatpickrRuConfig_Month(this.budgetDate);
        this.flatpickrStart = flatpickr('#budget-date', dateConfig);

        //table
        $('#modalTimeLine').on('hide.bs.modal', function () {
            //  BudgetVue.closeTimeline();
        })

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
            $.fn.dataTable.SearchPanes.defaults = false;
        },
        //Total charts
        loadTotalCharts: function () {
            if (!(UserInfo.UserSettings.Dashboard_Month_IsShow_InvestingChart
                || UserInfo.UserSettings.Dashboard_Month_IsShow_SpendingChart
                || UserInfo.UserSettings.Dashboard_Month_IsShow_EarningChart)) {
                return false;
            }
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
        //Limit charts
        loadLimitCharts: function () {
            if (!UserInfo.UserSettings.Dashboard_Month_IsShow_LimitCharts) {
                return false;
            }
            return $.ajax({
                type: "GET",
                url: "/Limit/LoadCharts?date=" + this.budgetDate + "&periodTypesEnum=" + 1,
                contentType: "application/json",
                dataType: 'json',
                context: this,
                success: function (response) {

                    this.limitsChartsData = response.limitsChartsData;

                    setTimeout(this.initLimitCharts, 10);
                }
            });
        },
        initLimitCharts: function () {
            for (var i = 0; i < this.limitsChartsData.length; i++) {
                let limitChartData = this.limitsChartsData[i];

                if (this.limitsCharts[i]) {
                    this.limitsCharts[i].destroy();
                }

                let backgroundColor = ['#4CAF50', '#ededed']; //green
                let hoverBackgroundColor = ['#4CAF50', '#ededed'];//green

                if (limitChartData.percent1 > 85) {
                    backgroundColor = ['#d9534f', '#ededed'];//red
                    hoverBackgroundColor = ['#d9534f', '#ededed'];//red
                } else if (limitChartData.percent1 > 65) {
                    backgroundColor = ['#FFD950', '#ededed'];//yellow
                    hoverBackgroundColor = ['#FFD950', '#ededed'];//yellow
                }

                this.limitsCharts[i] = new Chart(document.getElementById(limitChartData.chartID).getContext("2d"), {
                    type: 'doughnut',
                    data: {
                        datasets: [{
                            data: [limitChartData.percent1, limitChartData.percent2],
                            backgroundColor: backgroundColor,
                            hoverBackgroundColor: hoverBackgroundColor,
                            borderWidth: 0
                        }]
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
                        cutoutPercentage: 94,
                        responsive: false,
                        maintainAspectRatio: false
                    }
                });

            }
            this.resizeLimitCharts();
        },
        resizeLimitCharts: function () {
            for (var i = 0; i < this.limitsCharts.length; i++) {
                if (this.limitsCharts[i]) {
                    this.limitsCharts[i].resize();
                }
            }
        },
        //Goal charts
        loadGoalCharts: function () {
            if (!UserInfo.UserSettings.Dashboard_Month_IsShow_GoalCharts) {
                return false;
            }

            return $.ajax({
                type: "GET",
                url: `/Goal/LoadCharts?date=${this.budgetDate}&periodTypesEnum=${1}`,
                contentType: "application/json",
                dataType: 'json',
                context: this,
                success: function (response) {

                    this.goalChartsData = response.goalChartsData;

                    setTimeout(this.initGoalCharts, 10);
                }
            });
        },
        initGoalCharts: function () {
            for (var i = 0; i < this.goalChartsData.length; i++) {
                let goalChartData = this.goalChartsData[i];

                if (this.goalCharts[i]) {
                    this.goalCharts[i].destroy();
                }

                let backgroundColor = ['#4CAF50', '#ededed']; //green
                let hoverBackgroundColor = ['#4CAF50', '#ededed'];//green

                if (goalChartData.percent < 0) {
                    backgroundColor = ['#d9534f', '#ededed'];//red
                    hoverBackgroundColor = ['#d9534f', '#ededed'];//red
                }
                //else if (goalChartData.percent > 65) {
                //	backgroundColor = ['#FFD950', '#ededed'];//yellow
                //	hoverBackgroundColor = ['#FFD950', '#ededed'];//yellow
                //}

                this.goalCharts[i] = new Chart(document.getElementById(goalChartData.chartID).getContext("2d"), {
                    type: 'doughnut',
                    data: {
                        datasets: [{
                            data: [goalChartData.percent, goalChartData.percent2],
                            backgroundColor: backgroundColor,
                            hoverBackgroundColor: hoverBackgroundColor,
                            borderWidth: 0
                        }]
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
                        cutoutPercentage: 94,
                        responsive: false,
                        maintainAspectRatio: false
                    }
                });

            }
            //setTimeout(this.resizeGoalCharts, 10);
            this.resizeGoalCharts();
        },
        resizeGoalCharts: function () {
            for (var i = 0; i < this.goalCharts.length; i++) {
                if (this.goalCharts[i]) {
                    this.goalCharts[i].resize();
                }
            }
        },
        //big charts
        loadBigCharts: function () {
            if (!UserInfo.UserSettings.Dashboard_Month_IsShow_BigCharts) {
                return false;
            }

            for (var i = 0; i < this.bigChartsData.length; i++) {
                ShowLoading('#bigChart_' + this.bigChartsData[i].chartID);
            }

            return $.ajax({
                type: "GET",
                url: "/Chart/LoadCharts?date=" + this.budgetDate + "&periodType=" + 1,
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
        initBigChartCharts: function () {
            for (var i = 0; i < this.bigChartsData.length; i++) {
                let bigChartData = this.bigChartsData[i];
                if (this.bigCharts[i]) {
                    this.bigCharts[i].destroy();
                }

                this.bigCharts[i] = new Chart(document.getElementById(bigChartData.chartID), {
                    type: bigChartData.chartTypeCodeName,
                    data: {
                        datasets: bigChartData.dataSets,
                        labels: bigChartData.labels
                    },
                    options: {
                        title: {
                            display: true,
                            text: bigChartData.name
                        }
                    }
                });

            }
            //setTimeout(this.resizeBigCharts, 10);
            this.resizeBigCharts();
        },
        resizeBigCharts: function () {
            for (var i = 0; i < this.bigCharts.length; i++) {
                if (this.bigCharts[i]) {
                    this.bigCharts[i].resize();
                }
            }
        },
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
                this.loadTotalCharts();
                this.loadLimitCharts();
                this.loadBigCharts();
                return false;
            }
            this.loadTotalCharts();
            this.loadLimitCharts();
            this.loadGoalCharts();
            this.loadBigCharts();
        },
        refreshAfterChangeRecords: function (dateTimeOfPayment) {
            let dateOfPayment = moment(dateTimeOfPayment);
            let currentBudgetDate = moment(this.budgetDate);
            if (dateOfPayment.get("month") == currentBudgetDate.get("month") && dateOfPayment.get("year") == currentBudgetDate.get("year")) {
                return this.refresh(undefined, true);
            }
            return false;
        },
        initTable: function () {
            $("#table").DataTable();
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
                <i class="ion ion-md-add add-cell-action" onclick='RecordVue.showModel("${cell.currentDate}", "BudgetVue.refreshAfterChangeRecords")'></i>
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
                    return `<span>${values[0]}<span class="money-muted">,${values[1]}</span></span>`;
                } else {
                    return `<span>${cell.value}</span>`;
                }
            } else {
                return `<span>${cell.value}</span>`;
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

    }
});


Vue.config.devtools = true;
