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
        showFilter: false,

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
        //Limit charts
        loadLimitCharts: function () {
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

                let backgroundColor = ['#4CAF50', '#f9f9f9']; //green
                let hoverBackgroundColor = ['#4CAF50', '#f9f9f9'];//green

                if (limitChartData.percent1 > 85) {
                    backgroundColor = ['#d9534f', '#f9f9f9'];//red
                    hoverBackgroundColor = ['#d9534f', '#f9f9f9'];//red
                } else if (limitChartData.percent1 > 65) {
                    backgroundColor = ['#FFD950', '#f9f9f9'];//yellow
                    hoverBackgroundColor = ['#FFD950', '#f9f9f9'];//yellow
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
            return $.ajax({
                type: "GET",
                url: "/Goal/LoadCharts?date=" + this.budgetDate,
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

                let backgroundColor = ['#4CAF50', '#f9f9f9']; //green
                let hoverBackgroundColor = ['#4CAF50', '#f9f9f9'];//green

                if (goalChartData.percent < 0) {
                    backgroundColor = ['#d9534f', '#f9f9f9'];//red
                    hoverBackgroundColor = ['#d9534f', '#f9f9f9'];//red
                }
                //else if (goalChartData.percent > 65) {
                //	backgroundColor = ['#FFD950', '#f9f9f9'];//yellow
                //	hoverBackgroundColor = ['#FFD950', '#f9f9f9'];//yellow
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
            return $.ajax({
                type: "GET",
                url: "/Chart/LoadCharts?date=" + this.budgetDate + "&periodType=" + 1,
                contentType: "application/json",
                dataType: 'json',
                context: this,
                success: function (response) {

                    this.bigChartsData = response.bigChartsData;

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
        refresh: function () {
            this.load()
                .then(function () {
                    BudgetVue.initTable();
                });
            this.loadTotalCharts();
            this.loadLimitCharts();
            this.loadGoalCharts();
            this.loadBigCharts();
        },
        initTable: function () {
            $("#table").DataTable();
        },

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
        clickCell: function (rowIndex, cellIndex, event) {
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

            this.stylingClickedCells(event, "td_s", cellIndex);

            return this.loadTimeLine(filter)
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
        closeTimeline() {
            this.clearAllStyle();
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
