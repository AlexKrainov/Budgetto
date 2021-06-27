var BudgetVue = new Vue({
    el: "#budget-vue",
    data: {
        periodType: BudgetMethods.periodType,

        budgetDate: null,
        budgetYear: null,
        templateID: null,

        template: {},
        queueNumber: 0, //save last reorder column
        rows: [],
        footerRow: [],

        column: {},
        flatpickr: {},

        records: [],

        //table
        tableAjax: null,
        dataTable: null,
        excelDataTable: null,
        pageViewSettings: {
            version: 1.0,
            isTableViewCompact: false
        },
        isGenerateExcel: false,

        //total charts
        totalChartsAjax: null,
        earningData: {},
        earningChart: undefined,
        spendingData: {},
        spendingChart: undefined,
        investingData: {},
        investingChart: undefined,

        //limit charts
        limitsAjax: null,
        limitsChartsData: [],
        limitsCharts: [],

        //Goal charts
        goalsAjax: null,
        goalChartsData: [],
        goalCharts: [],

        //Big charts
        bigChartsAjax: null,
        bigChartsData: [],
        bigCharts: [],
        bigChartHeight: 310,

        //Main chart
        statisticsAjax: null,
        statisticsData: {},
        statisticsChart: undefined,
        statisticsChartHeight: 310,
        totalCharts: [],
        statisticsChartViewType: 1, //1-by sections, 2 - by tags

        //Summary
        summary: {
            isShow: BudgetMethods.isShowSummary,
            earningsPerHour: {},
            expensesPerDay: {},
            cashFlow: {},
            allAccountsMoney: {},
            allSubScriptionPrice: {},
        },
        chashFlowChart: undefined,
        summaryAjax: null,
        //Accounts
        accounts: [],
        accountsAjax: null,
        listReRender: true,

        //ToDoLists
        lists: [],
        toDoListAjax: null,
        refreshEditItem: true,

        //Progress
        progresses: [],
        progressAjax: null,

        isLoading: false,
        counter: -4999,
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
                        enabled: false
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
            this.refrehHeaderTable();
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
            this.resizeLimitCharts();
        },
        resizeLimitCharts: function () {
            for (var i = 0; i < this.limitsCharts.length; i++) {
                if (this.limitsCharts[i]) {
                    this.limitsCharts[i].resize();
                }
            }
            this.refrehHeaderTable();
        },
        hideLimit: function (limitID) {
            ShowLoading('#limit_' + limitID);
            return $.ajax({
                type: "GET",
                url: "/Limit/ToggleLimit?id=" + limitID + "&periodType=" + this.periodType,
                contentType: "application/json",
                dataType: 'json',
                context: limitID,
                success: function (response) {
                    HideLoading('#limit_' + this);
                    if (response.isOk = true) {
                        BudgetVue.limitsChartsData.splice(BudgetVue.limitsChartsData.findIndex(x => x.id == this), 1);
                    }
                },
                error: function () {
                    HideLoading('#limit_' + this);
                }
            });
        },
        getSectionsTitleForLimit: function (sections) {
            if (sections.length > 0) {// sections
                let li_s = "";
                for (var i = 0; i < sections.length; i++) {
                    li_s += `<li>${sections[i].name}</li>`;
                }
                return "<ul class='my-1 pl-3'>" + li_s + "</ul>";
            }
            return "";
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
            this.refrehHeaderTable();
        },
        addGoalMoney: function (goal) {
            GoalAddMoneyVue.addMoney(goal);
        },
        hideGoal: function (goalID) {
            ShowLoading('#goal_' + goalID);
            return $.ajax({
                type: "GET",
                url: "/Goal/ToggleGoal?id=" + goalID + "&periodType=" + this.periodType,
                contentType: "application/json",
                dataType: 'json',
                context: goalID,
                success: function (response) {
                    HideLoading('#goal_' + this);
                    if (response.isOk = true) {
                        BudgetVue.goalChartsData.splice(BudgetVue.goalChartsData.findIndex(x => x.id == this), 1);
                    }
                },
                error: function () {
                    HideLoading('#goal_' + this);
                }
            });
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
                                //text: bigChartData.name,
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
                                            return new Intl.NumberFormat(UserInfo.Currency.SpecificCulture, { style: 'currency', currency: UserInfo.Currency.CodeName }).format(value).split(",")[0] + " ₽"//.replace(/\D00(?=\D*$)/, '')
                                        },
                                        fontColor: fontColor,
                                        beginAtZero: true
                                    },
                                }]
                            },
                            tooltips: {
                                callbacks: {
                                    label: function (tooltipItem, data) {
                                        return new Intl.NumberFormat(UserInfo.Currency.SpecificCulture, { style: 'currency', currency: UserInfo.Currency.CodeName }).format(tooltipItem.value).split(",")[0] + " ₽"; //.replace(/\D00(?=\D*$)/, '');
                                    }
                                }
                            },
                            legend: {
                                display: bigChartData.chartTypesEnum == 1 || bigChartData.chartTypesEnum == 6,
                                fontColor: fontColor
                            },
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

                                        return `${label}: ${new Intl.NumberFormat(UserInfo.Currency.SpecificCulture, { style: 'currency', currency: UserInfo.Currency.CodeName }).format(money).split(",")[0]} ₽`;
                                    }
                                }
                            },
                            //legend: {},
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
                this.bigChartHeight = 370;
            } else if (chartSize > 450) {
                this.bigChartHeight = 600;
            }
            this.refrehHeaderTable();
        },
        toggleBigChart: function (chartID) {
            ShowLoading('#chart_' + chartID);
            return $.ajax({
                type: "GET",
                url: "/Chart/ToggleChart?id=" + chartID + "&periodType=" + this.periodType + "&isBudgetPage=true",
                contentType: "application/json",
                dataType: 'json',
                context: chartID,
                success: function (response) {
                    HideLoading('#chart_' + this);
                    if (response.isOk = true) {
                        BudgetVue.bigChartsData.splice(BudgetVue.bigChartsData.findIndex(x => x.id == this), 1);
                    }
                },
                error: function () {
                    HideLoading('#chart_' + this);
                }
            });
        },

        //STATISTICS
        loadStatistics: BudgetMethods.loadStatistics,
        initStatisticsChart: function (isRefreshTotalCharts) {
            if (isRefreshTotalCharts) {
                this.resizeStatisticsChartsTotal();
            }

            let options = null;
            let fontColor = themeSettings.isDarkStyle() ? '#fff' : '#aaa';

            if (this.statisticsChart) {
                this.statisticsChart.destroy();
            }

            let datasets = [];
            let maxValue = 0;

            for (var i = 0; i < this.statisticsData.totalView.length; i++) {
                let index = this.statisticsData.dataSets.findIndex(x => x.label == this.statisticsData.totalView[i].name);
                if (this.statisticsData.totalView[i].isSelected) {
                    datasets.push(this.statisticsData.dataSets[index]);
                    if (this.statisticsData.dataSets[index].maxValue > maxValue) {
                        maxValue = this.statisticsData.dataSets[index].maxValue;
                    }
                }
            }

            if (maxValue != 0) {
                maxValue = maxValue + (maxValue * 0.2);
            }

            console.log(maxValue);

            options = {
                legend: {
                    display: false,
                },
                title: {
                    display: false,// bigChartData.chartTypesEnum == 2,
                    //text: bigChartData.name,
                },
                scales: {
                    xAxes: [{
                        ticks: {
                            fontColor: fontColor
                        }
                    }],
                    yAxes: [{
                        //stacked: true,
                        ticks: {
                            // Include a dollar sign in the ticks
                            callback: function (value, index, values) {
                                return new Intl.NumberFormat(UserInfo.Currency.SpecificCulture, { style: 'currency', currency: UserInfo.Currency.CodeName }).format(value).split(",")[0] + " ₽"//.replace(/\D00(?=\D*$)/, '')
                            },
                            fontColor: fontColor,
                            // minimum value
                            // max: maxValue,// this.statisticsData.maxTotal, // maximum value
                            beginAtZero: true,
                        },
                    }]
                },
                tooltips: {
                    callbacks: {
                        label: function (tooltipItem, data) {
                            let values = data.datasets[tooltipItem.datasetIndex].data;
                            var total = values[tooltipItem.index];
                            if (tooltipItem.index != 0) {
                                total = total - values[tooltipItem.index - 1];
                            }

                            return " + "
                                + new Intl.NumberFormat(UserInfo.Currency.SpecificCulture, { style: 'currency', currency: UserInfo.Currency.CodeName }).format(total).split(",")[0] + " ₽"
                                + " = " + new Intl.NumberFormat(UserInfo.Currency.SpecificCulture, { style: 'currency', currency: UserInfo.Currency.CodeName }).format(tooltipItem.value).split(",")[0] + " ₽"; //.replace(/\D00(?=\D*$)/, '');
                        }
                    }
                },
                //legend: {
                //    display: true,
                //    fontColor: fontColor
                //},
                responsive: false,
                maintainAspectRatio: false,
            };

            this.statisticsChart = new Chart(document.getElementById("main-chart"), {
                type: "line",
                data: {
                    labels: this.statisticsData.labels,
                    datasets: datasets,
                },
                options: options
            });

            //setTimeout(this.resizeBigCharts, 10);
            this.resizeStatisticsChart();

        },
        resizeStatisticsChart: function () {
            let chartSize = 0;

            this.statisticsChart.resize();
            chartSize = this.statisticsChart.height;

            if (chartSize < 300) {
                this.statisticsChartHeight = 310;
            } else if (chartSize >= 300 && chartSize <= 450) {
                this.statisticsChartHeight = 370;
            } else if (chartSize > 450) {
                this.statisticsChartHeight = 600;
            }
            this.refrehHeaderTable();
        },
        resizeStatisticsChartsTotal: function () {
            for (var i = 0; i < this.statisticsData.totalView.length; i++) {
                let chartData = this.statisticsData.totalView[i];
                let index = this.totalCharts.findIndex(x => x.canvas && x.canvas.id == chartData.chartID);
                let chart = {};

                if (index >= 0) {
                    chart = this.totalCharts[index];
                    chart.destroy();
                }

                //  if (this.earningData.data) {
                chart = new Chart(document.getElementById(chartData.chartID).getContext("2d"), {
                    type: 'line',
                    data: {
                        datasets: [{
                            data: chartData.data,
                            borderWidth: 1,
                            backgroundColor: chartData.backgroundColor,
                            borderColor: chartData.borderColor,
                            pointBorderColor: 'rgba(0,0,0,0)',
                            pointRadius: 1,
                            lineTension: 0
                        }],
                        labels: chartData.labels
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
                if (index == -1) {
                    this.totalCharts.push(chart);
                }
            }
            setTimeout(this.resizeMainTotalCharts, 50);
        },
        resizeMainTotalCharts: function () {
            for (var i = 0; i < this.totalCharts.length; i++) {

                if (this.totalCharts[i]) {
                    this.totalCharts[i].resize();
                }
            }

            this.refrehHeaderTable();
        },
        selectTotalView: function (totalView) {
            if (totalView.sectionTypeEnum == 0) {
                return false;
            }

            totalView.isSelected = !totalView.isSelected;
            this.initStatisticsChart();

            for (var i = 0; i < this.statisticsData.sections.length; i++) {
                if (this.statisticsData.sections[i].sectionTypeID == totalView.sectionTypeEnum) {
                    this.statisticsData.sections[i].isShow = totalView.isSelected;
                }
            }
        },
        //Summary
        loadSummaries: BudgetMethods.loadSummaries,
        initSummaryCharts: function () {
            if (this.summary.cashFlow.isShow && this.summary.cashFlow.isChart) {
                if (this.chashFlowChart) {
                    this.chashFlowChart.destroy();
                }
                this.chashFlowChart = new Chart(document.getElementById('chashFlowChart').getContext("2d"), {
                    type: 'line',
                    data: {
                        datasets: [{
                            data: this.summary.cashFlow.data,
                            borderWidth: 1,
                            backgroundColor: 'rgba(38, 180, 255, .2)',
                            borderColor: 'rgba(38, 180, 255, 1)',
                            pointBorderColor: 'rgba(0,0,0,0)',
                            pointRadius: 1,
                            lineTension: 0
                        }],
                        labels: this.summary.cashFlow.labels
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
                            enabled: false,
                            //mode: 'index',
                            //position: 'nearest',
                            //custom: customTooltips
                        },
                        responsive: false,
                        maintainAspectRatio: false
                    }
                });
            }

            setTimeout(this.resizeSummaryCharts, 50);
        },
        resizeSummaryCharts: function () {
            if (this.chashFlowChart) {
                this.chashFlowChart.resize();
            }

            this.refrehHeaderTable();
        },
        //ACCOUNTS
        loadAccounts: BudgetMethods.loadAccounts,
        editMainAccount: function (account, isCash) {
            if (account == undefined) {
                MainAccountVue.edit(undefined, isCash);
            } else {
                MainAccountVue.edit(JSCopyObject(account), isCash);
            }
        },
        editAccount: function (account, mainAccount) {
            if (account == undefined) {
                AccountVue.edit(undefined, mainAccount);
            } else {
                AccountVue.edit(JSCopyObject(account), mainAccount);
            }
        },
        removeOrRecoveryAccount: function (account) {
            AccountVue.removeOrRecovery(account);
        },
        removeOrRecoveryMainAccount: function (account) {
            MainAccountVue.removeOrRecovery(account);
        },
        showHideAccount: function (account, isHide) {
            return AccountVue.showHide(account, isHide);
        },
        transferMoney: function (accountID) {
            let accounts = [];
            for (var i = 0; i < this.accounts.length; i++) {
                accounts = accounts.concat(this.accounts[i].accounts);
            }
            AccountTransferVue.transferMoney(accounts, accountID);
        },
        showHideAccounts: function (mainAccount, isHide) {
            for (var i = 0; i < mainAccount.accounts.length; i++) {
                if (mainAccount.accounts[i].isHideCurrentAccount != isHide) {
                    mainAccount.accounts[i].isHideCurrentAccount = isHide;
                    AccountVue.showHide(mainAccount.accounts[i], isHide);
                }
            }
        },
        //ToDoList
        loadToDoLists: BudgetMethods.loadToDoLists,
        afterLoadLists: function () {

            this.$nextTick(() => {
                let els = document.querySelectorAll(".perfect-scroll");
                for (var i = 0; i < els.length; i++) {
                    new PerfectScrollbar(els[i]);
                }
            });
        },
        addListItem: function (list) {
            if (!list.items) {
                list.items = [];
            }

            if (!list.editItem.text || list.editItem.text.length == 0) {
                $("#edititem_list_" + list.id + " input[name=input-text]").addClass("is-invalid");
                return false;
            } else {
                $("#edititem_list_" + list.id + " input[name=input-text]").removeClass("is-invalid");
            }

            if (list.editItem.id) {
                let index = list.items.findIndex(x => x.id == list.editItem.id);
                if (index != -1) {
                    list.items[index].text = list.editItem.text;
                }
            } else {
                list.items.push({
                    id: this.counter,
                    text: list.editItem.text,
                    isDone: false,
                    isDeleted: false,
                    order: list.items.length + 1
                });
                this.counter++;
            }
            list.editItem = { id: 0, text: "" };

            this.saveList(list);

            this.$nextTick(() => {
                //scroll to
                $("#list_" + list.id + " .perfect-scroll").scrollTop($("#list_" + list.id + " .perfect-scroll").prop("scrollHeight"));
            });
        },
        editListItem: function (list, item) {
            //this.refreshEditItem = false;
            //this.$nextTick(() => {
            //this.refreshEditItem = true;
            list.editItem = item;
            $("#edititem_list_" + list.id + " input[name=input-text]").focus();
            //Not good
            //this.$forceUpdate();
            //});
        },
        keyItemPress: function (e, list) {
            if (e.keyCode == 13) {
                this.addListItem(list);
            }
        },
        saveList: function (list) {

            var items = $("#list_" + list.id + " div.todo-item")
            for (var i = 0; i < items.length; i++) {
                let id = $(items[i]).attr("item-id") * 1;
                let _index = list.items.findIndex(x => x.id == id);
                list.items[_index].order = i;
            }

            //ShowLoading("#list_" + list.id);

            BudgetVue.queueNumber += 1;

            if (Lists == undefined) {
                var Lists = [list];
            } else {
                let index = Lists.findIndex(x => x.id == list.id);
                if (index == -1) {
                    Lists.push(list);
                } else {
                    Lists[index] = list;
                }
            }

            setTimeout(function () {
                if (BudgetVue.queueNumber == 1) {
                    BudgetVue.queueNumber = 0;

                    for (var i = 0; i < Lists.length; i++) {
                        $.ajax({
                            type: "POST",
                            url: "/ToDoList/EditList",
                            data: JSON.stringify(Lists[i]),
                            context: BudgetVue,
                            contentType: "application/json",
                            dataType: 'json',
                            success: function (response) {

                                if (response.isOk) {
                                    let index = this.lists.findIndex(x => x.id == response.list.id);
                                    this.lists[index] = response.list;

                                } else {
                                    //ToDo error
                                }
                                //  HideLoading("#list_" + this.list.id);
                                Lists = undefined;

                                BudgetVue.$forceUpdate();
                            },
                            error: function () {
                                // HideLoading("#list_" + this.list.id);
                            }
                        });
                    }

                } else {
                    BudgetVue.queueNumber -= 1;
                }
            }, 3000);
        },
        remvoeList: function (list) {

            let listsDelete = [list];
            ShowLoading("#list_" + list.id);

            return $.ajax({
                type: "POST",
                url: "/ToDoList/RemoveList",
                data: JSON.stringify(listsDelete),
                context: {
                    $this: this,
                    list: list
                },
                contentType: "application/json",
                dataType: 'json',
                success: function (response) {

                    if (response.isOk) {
                        this.list.isDeleted = true;
                    }
                    HideLoading("#list_" + this.list.id);
                },
                error: function () {
                    HideLoading("#list_" + this.list.id);
                }
            });
        },
        recoveryList: function (list) {
            ShowLoading('#list_' + list.id);

            return $.ajax({
                type: "GET",
                url: "/ToDoList/Recovery?listID=" + list.id,
                context: list,
                contentType: "application/json",
                dataType: 'json',
                success: function (response) {
                    this.isDeleted = !response.isOk;
                    HideLoading('#list_' + this.id);
                }
            });
        },
        hideList: function (list) {
            ShowLoading('#list_' + list.id);

            return $.ajax({
                type: "GET",
                url: "/ToDoList/HideList?listID=" + list.id + "&periodType=" + this.periodType,
                context: {
                    $this: this,
                    list: list
                },
                contentType: "application/json",
                dataType: 'json',
                success: function (response) {
                    HideLoading('#list_' + this.list.id);
                    let index = this.$this.lists.findIndex(x => x.id == this.list.id);
                    this.$this.lists.splice(index, 1);
                }
            });
        },

        //progress
        loadProgress: BudgetMethods.loadProgress,
        getProgressCompleteStyle: function (index) {
            //if (index <= this.progresses.length
            //    && this.progresses[index].isComplete && this.progresses[index + 1].isComplete) {
            //    return "progress-complete";
            //}
            //return "";
            return index <= (this.progresses.length - 2) && this.progresses[index].isComplete && this.progresses[index + 1].isComplete ? "progress-complete" : "";
        },

        //resize and refresh
        refresh: function (typeRefresh) {
            //let typeRefresh = [
            //    "onlyTable",
            //    "runtimeData",
            //];
            if (typeRefresh == undefined || typeRefresh == 'onlyTable' || typeRefresh == "runtimeData" || typeRefresh == "all" || typeRefresh == "only-progress") {

                if (this.templateID != "-1") {
                    this.isLoading = true;
                    ShowLoading(".table-container");

                    if (this.dataTable) {//sometimes bug with : Cannot read property 'mData' of undefined
                        this.dataTable = undefined;
                        $("#table").DataTable().destroy();
                    }
                    this.load()
                        .then(function () {
                            HideLoading(".table-container");
                            try {
                                BudgetVue.initTable();
                            } catch (e) {
                                console.log(e);
                                BudgetVue.template = {};
                                BudgetVue.footerRow = [];
                                BudgetVue.row = [];
                                BudgetVue.refresh('onlyTable');
                            }
                            this.isLoading = false;
                        });
                }
            }

            if (typeRefresh == 'onlyTable') {
                return false;
            }
            if (typeRefresh == 'only-progress') {
                this.loadProgress();
                return false;
            }

            if (typeRefresh == "runtimeData") {
                this.loadBigCharts();
                this.loadStatistics();
                this.loadTotalCharts();
                this.loadLimitCharts();
                this.loadAccounts();
                this.loadSummaries();
                this.loadProgress();
                return false;
            }
            if (typeRefresh == "onlyGoal") {
                ShowLoading("#goal-contrainer");
                this.loadGoalCharts()
                    .then(function () {
                        HideLoading("#goal-contrainer");
                    });
                return;
            }

            if (typeRefresh == "onlySummery") {
                this.loadAccounts();
                this.loadSummaries();
                this.loadProgress();
                return;
            }

            this.loadBigCharts();
            this.loadStatistics();
            this.loadTotalCharts();
            this.loadLimitCharts();
            this.loadGoalCharts();
            this.loadAccounts();
            this.loadSummaries();
            this.loadToDoLists();
            this.loadProgress();
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
            this.resizeSummaryCharts();
        },
        refrehHeaderTable: function () {
            if (this.dataTable && this.dataTable.fixedHeader) {
                this.dataTable.fixedHeader.adjust()
            }
            $('[data-toggle="tooltip"]').tooltip();
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
            this.dataTable.on('column-reorder', this.moveColumn);

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
        moveColumn: function (e, settings, details) {
            BudgetVue.queueNumber += 1;

            setTimeout(function () {
                if (BudgetVue.queueNumber == 1) {
                    BudgetVue.queueNumber = 0;

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
                            toastr.success("Изменения в шаблоне сохранены");
                        }
                    });
                } else {
                    BudgetVue.queueNumber -= 1;
                }
            }, 1500);

        },
        toExcel: function () {
            this.isGenerateExcel = true;

            BudgetVue.excelDataTable = $("#excel-table").DataTable({
                dom: 'Bfrtip',
                buttons: ['excelHtml5'],
                retrieve: true
            });

            setTimeout(function () {
                $(".buttons-excel").click();

                BudgetVue.isGenerateExcel = false;

                setTimeout(function () {
                    BudgetVue.excelDataTable.destroy();
                }, 2000);
            }, 100);

        },
        //View cell
        getCellContent: function (cell, cellIndex, rowIndex) {
            let isDoneReminder_class = cell.reminders && cell.reminders.some(x => x.isDone) ? 'reminder-is-done' : '';

            if (this.periodType == PeriodTypeEnum.Month) {
                return `<div class="cell-head">${this.getCellActions(cell, cellIndex, rowIndex) + this.getCellValue(cell, cellIndex, rowIndex)}</div>
                    <div class="cell-footer mt-1">
                        <span class="cell-reminder-icons ${isDoneReminder_class}"
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
                        <i class="remind-cell-action">+<i class="ion ion-ios-alarm text-big" onclick="ReminderVue.addReminders('${cell.currentDate}')" title="Добавить напоминание"></i></i>
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
                    <i class="fas fa-history show-history-cell-action pl-1" onclick="BudgetVue.clickFooterCell(${cellIndex})" title="Посмотреть историю"></i>
                </span>`;
        },
        getCellValue: function (cell, cellIndex, rowIndex) {
            if (cell.value.indexOf(",")) {
                let values = cell.value.split(",");
                let generalValue = ` data-type="${cell.templateColumnType}" data-value='${cell.naturalValue}'`;

                if (rowIndex >= 0) {
                    if (values.length == 2) {
                        return `<span ${generalValue} onclick="BudgetVue.showHistory(${rowIndex}, ${cellIndex},'${cell.currentDate}', event)" title="Посмотреть историю"> 
                                ${values[0]}
                                <span class="money-muted">,${values[1]}</span>
                            </span> `;
                    } else {
                        return `<span ${generalValue} onclick="BudgetVue.showHistory(${rowIndex}, ${cellIndex},'${cell.currentDate}', event)" title="Посмотреть историю"> ${cell.value}</span>`;
                    }
                } else {//footer
                    if (values.length == 2) {
                        return `<span ${generalValue} onclick="BudgetVue.clickFooterCell(${cellIndex})" class="font-weight-semibold"> 
                                ${values[0]}
                                <span class="money-muted">,${values[1]}</span>
                            </span> `;
                    } else {
                        return `<span ${generalValue} onclick="BudgetVue.clickFooterCell(${cellIndex})" class="font-weight-semibold"> ${cell.value}</span>`;
                    }
                }
            } else {
                return `<span ${generalValue} onclick="BudgetVue.showHistory(${rowIndex}, ${cellIndex},'${cell.currentDate}', event)" title="Посмотреть историю"> ${cell.value}</span>`;
            }
        },
        getHeaderTitle: function (column) {

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
                    let beforeRepeatIcon = '';
                    let afterRepeatIcon = '';

                    if (reminder.isRepeat) {
                        beforeRepeatIcon = '<i class="fas fa-sync" style="font-size: 8px; color: gray; padding-top: -3px; position: absolute;"></i>';
                    }

                    if (reminder.count > 1) {
                        count = reminder.count;

                        if (reminder.isRepeat) {
                            beforeRepeatIcon = '';
                            afterRepeatIcon = '<i class="fas fa-sync" style="font-size: 8px; color: gray; padding-top: -3px;"></i>';
                        }
                    }

                    icons += `<span class="mr-1"><i class="${reminder.cssIcon}" title="${reminder.titles}"></i>${beforeRepeatIcon}<span class="reminder-count">${count}</span>${afterRepeatIcon}</span>`;
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

            let filter = { sections: sections, isSection: true };
            if (this.periodType == PeriodTypeEnum.Month) {

                filter.startDate = moment(this.budgetDate, "YYYY/MM/DD").add(rowIndex, "days").format();
                filter.endDate = moment(this.budgetDate, "YYYY/MM/DD").add((rowIndex + 1), "days").add(-1, "minutes").format();

            } else if (this.periodType == PeriodTypeEnum.Year) {

                filter.startDate = moment(currentDate, "YYYY/MM/DD").format();
                filter.endDate = moment(currentDate, "YYYY/MM/DD").endOf("month").format();
            }

            return HistoryVue.showHistory(filter, currentDate);
        },
        showAllHistory: function () {
            let filter = {
                sections: RecordVue.recordComponent.sectionComponent.sections.map(x => x.id),
                isSection: true
            };
            if (this.periodType == PeriodTypeEnum.Month) {

                filter.startDate = moment(this.budgetDate).format();
                filter.endDate = moment(this.budgetDate).endOf("month").format();

            } else if (this.periodType == PeriodTypeEnum.Year) {

                filter.startDate = `${this.budgetYear}-01-01T00:00:01+00:00`;
                filter.endDate = `${this.budgetYear}-12-31T23:59:59+00:00`;
            }

            return HistoryVue.showHistory(filter);
        },
        showLimitHistory: function (limitID) {
            let index = this.limitsChartsData.findIndex(x => x.id == limitID);

            let filter = {
                sections: this.limitsChartsData[index].sections.map(x => x.id),
                isSection: true
            };
            if (this.periodType == PeriodTypeEnum.Month) {

                filter.startDate = moment(this.budgetDate).format();
                filter.endDate = moment(this.budgetDate).endOf("month").format();

            } else if (this.periodType == PeriodTypeEnum.Year) {

                filter.startDate = `${this.budgetYear}-01-01T00:00:01+00:00`;
                filter.endDate = `${this.budgetYear}-12-31T23:59:59+00:00"`;
            }

            return HistoryVue.showHistory(filter);
        },
        clickFooterCell: function (cellIndex) {

            let filter = { sections: [] };

            if (this.template.columns[cellIndex].templateColumnType == TemplateColumnTypeEnum.BudgetSection) {

                filter.sections = this.template.columns[cellIndex].templateBudgetSections.map(x => x.sectionID);
                this.stylingClickedCells(event, "td_s", cellIndex);

            } else if (this.template.columns[cellIndex].templateColumnType == TemplateColumnTypeEnum.DaysForMonth
                || this.template.columns[cellIndex].templateColumnType == TemplateColumnTypeEnum.MonthsForYear) {

                filter.sections = this.template.columns.map(x => x.templateBudgetSections.map(x => x.sectionID)).flat();
                this.stylingClickedCells(event, "table", cellIndex);

            } else {
                return null;
            }

            if (this.periodType == PeriodTypeEnum.Month) {

                filter.startDate = moment(this.budgetDate).format();
                filter.endDate = moment(this.budgetDate).endOf("month").format();

            } else if (this.periodType == PeriodTypeEnum.Year) {

                filter.startDate = `${this.budgetYear}-01-01T00:00:01+00:00`;
                filter.endDate = `${this.budgetYear}-12-31T23:59:59+00:00"`;
            }

            return HistoryVue.showHistory(filter)
        },
        stylingClickedCells: function (event, type, cellIndex) {
            this.clearAllStyle();

            if (type == "td") {
                $(event.target).closest("td").addClass("table-primary");
            } else if (type == "tr") {
                $(event.target).closest("tr").addClass("table-primary");
            } else if (type == "td_s") {
                $("#table td:nth-child(" + (++cellIndex) + ")").addClass("table-primary");
            } else if (type == "table") {
                $("#table td").addClass("table-primary");
            }
        },
        //after clicked a row, a cell or a footer cell
        clearAllStyle: function () {
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
        getSectionsTitle: function (sections, sectionTypeEnum) {
            let title = ""
            if (sectionTypeEnum == SectionTypeEnum.Earnings) {
                title += "Доходы за";
            } else if (sectionTypeEnum == SectionTypeEnum.Investments) {
                title += "Пополнения за ";
            } else if (sectionTypeEnum == SectionTypeEnum.Spendings) {
                title += "Расходы за ";
            }

            if (this.periodType == PeriodTypeEnum.Month) {
                title += " месяц ";
            } else if (this.periodType == PeriodTypeEnum.Year) {
                title += " год ";
            }

            if (sectionTypeEnum == SectionTypeEnum.Investments) {
                title += "по счетам: ";
            } else {
                title += "по категориям: ";
            }

            if (sections && sections.length > 0) {// sections
                let li_s = "";
                for (var i = 0; i < sections.length; i++) {
                    li_s += `<li>${sections[i].name}</li>`;
                }
                return title + "<ul class='my-1 pl-3'>" + li_s + "</ul>";
            }
            return title;
        },

        //helpers
        getCurrencyValue: function (value, currency) {
            if (currency == undefined || currency == "RUB" || currency.codeName == "RUB") {
                return new Intl.NumberFormat(UserInfo.Currency.SpecificCulture, { style: 'currency', currency: UserInfo.Currency.CodeName })
                    .format(value)
                    .split(",")[0] + " ₽";
            }
            else {
                return new Intl.NumberFormat(currency.specificCulture, { style: 'currency', currency: currency.codeName })
                    .format(value);
            }
            //    if (currency.codeName == "USD") {
            //    return new Intl.NumberFormat(currency.specificCulture, { style: 'currency', currency: currency.codeName })
            //        .format(value)
            //        .split(".")[0];
            //} else if (currency.codeName == "EUD") {
            //    return new Intl.NumberFormat(currency.specificCulture, { style: 'currency', currency: currency.codeName })
            //        .format(value)
            //        .split(",")[0] + " €";
            //}
            //.replace(/\D00(?=\D*$)/, '')
            //.split(",")[0]
        },

        getDateByFormat: function (date, format) {
            return GetDateByFormat(date, format);
        },
    }
});
