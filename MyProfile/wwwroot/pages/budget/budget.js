var BudgetVue = new Vue({
    el: "#budget-vue",
    data: {
        periodType: BudgetMethods.periodType,

        budgetDate: null,
        budgetYear: null,
        templateID: null,

        template: {},
        columnOrderQueue: 0, //save last reorder column
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
        isGenerateExcel: false,

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
        changeView: BudgetMethods.changeView,
        //Total charts
        loadTotalCharts: BudgetMethods.loadTotalCharts,
        initTotalCharts: function () {
            if (this.earningChart) {
                this.earningChart.destroy();
            }
            //  if (this.earningData.data) {
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
            // }

            if (this.spendingChart) {
                this.spendingChart.destroy()
            }
            // if (this.spendingData.data) {
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
            // }

            //if (this.investingData.data) {
            this.investingChart = new Chart(document.getElementById('investmentsChart').getContext("2d"), {
                type: 'line',
                data: {
                    datasets: [{
                        data: this.investingData.data,
                        borderWidth: 1,
                        backgroundColor: 'rgba(136, 151, 170, .2)',
                        borderColor: 'rgba(136, 151, 170, 1)',
                        pointBorderColor: 'rgba(0,0,0,0)',
                        pointRadius: 1,
                        lineTension: 0
                    }],
                    labels: this.investingData.labels
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
            //}

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
        loadLimitCharts: BudgetMethods.loadLimitCharts,
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
            this.refrehViewTable();
        },
        //Goal charts
        loadGoalCharts: BudgetMethods.loadGoalCharts,
        OLD_initGoalCharts: function () {
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
            setTimeout(this.resizeGoalCharts, 10);
        },
        OLD_resizeGoalCharts: function () {
            for (var i = 0; i < this.goalCharts.length; i++) {
                if (this.goalCharts[i]) {
                    this.goalCharts[i].resize();
                }
            }
            this.refrehViewTable();
        },
        addGoalMoney: function (goal) {
            GoalAddMoneyVue.addMoney(goal);
        },
        //big charts
        loadBigCharts: BudgetMethods.loadBigCharts,
        initBigChartCharts: function () {
            for (var i = 0; i < this.bigChartsData.length; i++) {
                let bigChartData = this.bigChartsData[i];
                let options = null;
                let fontColor = themeSettings.isDarkStyle() ? '#fff' : '#aaa';

                if (this.bigCharts[i]) {
                    this.bigCharts[i].destroy();
                }

                switch (bigChartData.chartTypeCodeName) {
                    case "line":
                    case "bar":
                        options = {
                            title: {
                                display: false,// bigChartData.chartTypesEnum == 2,
                                text: bigChartData.name,
                            },
                            scales: {
                                xAxes: [{
                                    ticks: {
                                        fontColor: fontColor
                                    }
                                }],
                                yAxes: [{
                                    ticks: {
                                        // Include a dollar sign in the ticks
                                        callback: function (value, index, values) {
                                            return new Intl.NumberFormat(UserInfo.Currency.SpecificCulture, { style: 'currency', currency: UserInfo.Currency.CodeName }).format(value)
                                        },
                                        fontColor: fontColor,
                                        beginAtZero: true
                                    },
                                }]
                            },
                            tooltips: {
                                callbacks: {
                                    label: function (tooltipItem, data) {
                                        return new Intl.NumberFormat(UserInfo.Currency.SpecificCulture, { style: 'currency', currency: UserInfo.Currency.CodeName }).format(tooltipItem.value);
                                    }
                                }
                            },
                            //legend: themeSettings.isDarkStyle() ? {
                            //    labels: {  fontColor: '#fff' }
                            //} : {},
                            responsive: false,
                            maintainAspectRatio: false
                        };
                        break;
                    case "doughnut":
                    case "pie":
                        options = {
                            title: {
                                display: false,// true,
                                text: bigChartData.name,
                            },
                            tooltips: {
                                callbacks: {
                                    label: function (tooltipItem, data) {
                                        let label = "";
                                        let money = "";
                                        try {
                                            label = data.labels[tooltipItem.index];
                                            money = data.datasets[0].data[tooltipItem.index];
                                        } catch (e) {
                                            console.log(e);
                                        }

                                        return `${label}: ${new Intl.NumberFormat(UserInfo.Currency.SpecificCulture, { style: 'currency', currency: UserInfo.Currency.CodeName }).format(money)}`;
                                    }
                                }
                            },

                        };
                        break;
                    default:
                }

                this.bigCharts[i] = new Chart(document.getElementById(bigChartData.chartID), {
                    type: bigChartData.chartTypeCodeName,
                    data: {
                        labels: bigChartData.labels,
                        datasets: bigChartData.dataSets,
                    },
                    options: options
                });

            }
            //setTimeout(this.resizeBigCharts, 10);
            this.resizeBigCharts();
        },
        resizeBigCharts: function () {
            let chartSize = 0;
            for (var i = 0; i < this.bigCharts.length; i++) {
                let chart = this.bigCharts[i];
                if (chart) {
                    chart.resize();
                    chartSize = chart.height;
                }
            }
            if (chartSize < 300) {
                this.bigChartHeight = 310;
            } else if (chartSize >= 300 && chartSize <= 450) {
                this.bigChartHeight = 410;
            } else if (chartSize > 450) {
                this.bigChartHeight = 600;
            }
            this.refrehViewTable();
        },
        //resize and refresh
        refresh: function (typeRefresh) {
            //let typeRefresh = [
            //    "onlyTable",
            //    "runtimeData",
            //];
            if (typeRefresh == undefined || typeRefresh == 'onlyTable' || typeRefresh == "runtimeData" || typeRefresh == "all") {
                ShowLoading(".table-container");

                if (this.dataTable) {
                    this.dataTable.destroy();
                    $('[data-toggle="tooltip"]').tooltip('dispose');
                }

                this.load()
                    .then(function () {
                        HideLoading(".table-container");
                        BudgetVue.initTable();
                    });
            }

            if (typeRefresh == 'onlyTable') {
                return false;
            }

            if (typeRefresh == "runtimeData") {
                this.loadBigCharts();
                this.loadTotalCharts();
                this.loadLimitCharts();
                return false;
            }
            if (typeRefresh == "onlyGoal") {
                ShowLoading("#goal-contrainer");
                this.loadGoalCharts().then(function () {
                    HideLoading("#goal-contrainer");
                });
                return;
            }

            this.loadBigCharts();
            this.loadTotalCharts();
            this.loadLimitCharts();
            this.loadGoalCharts();

        },
        refreshAfterChangeRecords: function (dateTimeOfPayment) {
            let dateOfPayment = moment(dateTimeOfPayment);

            if (this.periodType == PeriodTypeEnum.Month) {

                let currentBudgetDate = moment(this.budgetDate, "YYYY/MM/DD");

                if (dateOfPayment.get("month") == currentBudgetDate.get("month") && dateOfPayment.get("year") == currentBudgetDate.get("year")) {
                    return this.refresh("runtimeData");
                }

            } else if (this.periodType == PeriodTypeEnum.Year) {

                if (dateOfPayment.get("year") == this.budgetYear) {
                    return this.refresh("runtimeData");
                }
            }


            return false;
        },
        resizeAll: function () {
            this.resizeTotalCharts();
            this.resizeLimitCharts();
            //this.resizeGoalCharts();
            this.resizeBigCharts();
        },
        refrehViewTable: function () {
            if (this.dataTable && this.dataTable.fixedHeader) {
                this.dataTable.fixedHeader.adjust()
            }
        },
        initTable: function () {
            this.dataTable = $("#table").DataTable({
                columnDefs: [
                    {
                        targets: '_all',
                        className: "column-min-width",
                        "orderDataType": "dom-text-numeric",
                        type: "num"
                    },
                ],
                colReorder: {
                    realtime: false
                }
            });
            this.dataTable.on('column-reorder', function (e, settings, details) {
                BudgetVue.columnOrderQueue += 1;

                setTimeout(function () {
                    if (BudgetVue.columnOrderQueue == 1) {
                        console.log(details.mapping);
                        console.log(BudgetVue.dataTable.colReorder.order());
                        BudgetVue.columnOrderQueue = 0;

                        let order = [];

                        $("#table th").each(function (index) {
                            let el = $(this);
                            order.push({
                                id: el.data("column-id"),
                                newOrder: el.data("column-index"),
                                oldOrder: el.data("column-order"),
                            });
                        });

                        $.ajax({
                            type: "POST",
                            url: "/Budget/TemplateChangeColumns",
                            contentType: "application/json",
                            data: JSON.stringify({
                                listOrder: order,
                                templateID: BudgetVue.template.id
                            }),
                            dataType: 'json',
                            success: function (response) {


                            }
                        });
                    } else {
                        BudgetVue.columnOrderQueue -= 1;
                    }
                }, 1500);

            });

            $.fn.dataTable.ext.order["dom-text-numeric"] = function (settings, col) {
                return this.api().column(col, { order: 'index' }).nodes().map(function (td, i) {
                    return $(td).find("span[data-value]").data("value");
                });
            };

            //cannot get in time because of vue.js
            setTimeout(function () {
                $('[data-toggle="tooltip"]').tooltip();
            }, 1000);
        },
        toExcel: function () {
            this.isGenerateExcel = true;
            $("#excel-table").DataTable({
                dom: 'Bfrtip',
                buttons: ['excelHtml5']
            });

            setTimeout(function () {
                $(".buttons-excel").click();

                BudgetVue.isGenerateExcel = false;
            }, 100);
        },
        //View cell
        getCellContent: function (cell, cellIndex, rowIndex) {
            if (this.periodType == PeriodTypeEnum.Month) {
                return `<div class="cell-head">${this.getCellActions(cell, cellIndex, rowIndex) + this.getCellValue(cell, cellIndex, rowIndex)}</div>
                    <div class="cell-footer mt-1">
                        <span class="cell-reminder-icons"
                                onclick="ReminderVue.showReminders('${cell.currentDate}')">${this.getRemindersIcons(cell)}</span>
                        <span class="cell-section-icons"></span>
                    </div>`;
            } else {
                return `<div class="cell-head">${this.getCellActions(cell, cellIndex, rowIndex) + this.getCellValue(cell, cellIndex, rowIndex)}</div>
                    <div class="cell-footer mt-1"><span class="cell-reminder-icons"></span><span class="cell-section-icons"></span></div>`;
            }
        },

        getCellFooterContent: function (cell, cellIndex) {
            return this.getCellFooterActions(cellIndex) + this.getCellValue(cell, cellIndex);
        },
        getCellActions: function (cell, cellIndex, rowIndex) {
            if (this.periodType == PeriodTypeEnum.Month) {
                if (cell.templateColumnType == 2) {//Days
                    return `
                    <span class="float-left cell-actions">
                        <i class="ion ion-md-add add-cell-action" onclick="RecordVue.showModal('${cell.currentDate}', 'BudgetVue.refreshAfterChangeRecords')" title="Добавить запись" ></i>
                        <i class="fas fa-history show-history-cell-action pl-1" onclick="BudgetVue.showHistory(${rowIndex}, ${cellIndex},'${cell.currentDate}', event)" title="Посмотреть историю"></i>
                        <i class="remind-cell-action">+<i class="fas fa-bell" onclick="ReminderVue.addReminders('${cell.currentDate}')" title="Добавить напоминание"></i></i>
                    </span>`;
                } else {
                    return `
                    <span class="float-left cell-actions">
                        <i class="fas fa-history show-history-cell-action pl-1" onclick="BudgetVue.showHistory(${rowIndex}, ${cellIndex},'${cell.currentDate}', event)" title="Посмотреть историю"></i>
                    </span>`;
                }
            } else {
                return `
                    <span class="float-left cell-actions">
                        <i class="fas fa-history show-history-cell-action pl-1" onclick="BudgetVue.showHistory(${rowIndex}, ${cellIndex},'${cell.currentDate}', event)"></i>
                    </span>`;
            }
        },
        getCellFooterActions: function (cellIndex) {
            return `
                <span class="float-left cell-actions">
                    <i class="fas fa-history show-history-cell-action pl-1" onclick="BudgetVue.clickFooterCell(${cellIndex})"></i>
                </span>`;
        },
        getCellValue: function (cell, cellIndex, rowIndex) {
            if (cell.value.indexOf(",")) {
                let values = cell.value.split(",");
                let generalValue = ` data-type="${cell.templateColumnType}" data-value='${cell.naturalValue}'`;

                if (rowIndex >= 0) {
                    if (values.length == 2) {
                        return `<span ${generalValue} onclick="BudgetVue.showHistory(${rowIndex}, ${cellIndex},'${cell.currentDate}', event)"> 
                                ${values[0]}
                                <span class="money-muted">,${values[1]}</span>
                            </span> `;
                    } else {
                        return `<span ${generalValue} onclick="BudgetVue.showHistory(${rowIndex}, ${cellIndex},'${cell.currentDate}', event)"> ${cell.value}</span>`;
                    }
                } else {//footer
                    if (values.length == 2) {
                        return `<span ${generalValue} onclick="BudgetVue.clickFooterCell(${cellIndex})"> 
                                ${values[0]}
                                <span class="money-muted">,${values[1]}</span>
                            </span> `;
                    } else {
                        return `<span ${generalValue} onclick="BudgetVue.clickFooterCell(${cellIndex})"> ${cell.value}</span>`;
                    }
                }
            } else {
                return `<span ${generalValue} onclick="BudgetVue.showHistory(${rowIndex}, ${cellIndex},'${cell.currentDate}', event)"> ${cell.value}</span>`;
            }
        },
        getTitle: function (column) {

            if (column.templateColumnType == 1) {// sections
                let li_s = "";
                for (var i = 0; i < column.templateBudgetSections.length; i++) {
                    li_s += `<li>${column.templateBudgetSections[i].sectionName}</li>`;
                }
                //     console.log(this.template.id + ")" + li_s);
                return "<ul class='my-1 pl-3'>" + li_s + "</ul>";
            }
            return "";
        },
        getRemindersIcons: function (cell) {
            if (cell.templateColumnType == 2) {//Type days
                let icons = ``;
                for (var i = 0; i < cell.reminders.length; i++) {

                    let reminder = cell.reminders[i];
                    let count = "";

                    if (reminder.count > 1) {
                        count = reminder.count;
                    }

                    icons += `<span class="mr-3"><i class="${reminder.cssIcon}" title="${reminder.titles}"></i><span class="reminder-count">${count}</span></span>`;
                }
                return icons;
            }
            return "";
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
        showHistory: function (rowIndex, cellIndex, currentDate, event) {
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

            let filter = { sections: sections };
            if (this.periodType == PeriodTypeEnum.Month) {

                filter.startDate = moment(this.budgetDate, "YYYY/MM/DD").add(rowIndex, "days").format();
                filter.endDate = moment(this.budgetDate, "YYYY/MM/DD").add((rowIndex + 1), "days").add(-1, "minutes").format();

            } else if (this.periodType == PeriodTypeEnum.Year) {

                filter.startDate = moment(currentDate, "YYYY/MM/DD").format();
                filter.endDate = moment(currentDate, "YYYY/MM/DD").endOf("month").format();
            }

            return HistoryVue.showHistory(filter, currentDate);
        },
        clickFooterCell: function (cellIndex) {

            let filter = { sections: this.template.columns[cellIndex].templateBudgetSections.map(x => x.sectionID) };
            if (this.periodType == PeriodTypeEnum.Month) {

                filter.startDate = moment(this.budgetDate).format();
                filter.endDate = moment(this.budgetDate).endOf("month").format();

            } else if (this.periodType == PeriodTypeEnum.Year) {

                filter.startDate = `${this.budgetYear}-01-01T00:00:01+00:00`;
                filter.endDate = `${this.budgetYear}-12-31T23:59:59+00:00"`;
            }

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
            for (var i = 0; i < el_s.length; i++) {
                el_s[i].classList.remove("table-primary");
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

        //helpers
        getCurrencyValue: function (value) {
            return new Intl.NumberFormat(UserInfo.Currency.SpecificCulture, { style: 'currency', currency: UserInfo.Currency.CodeName }).format(value);
        },
    }
});


