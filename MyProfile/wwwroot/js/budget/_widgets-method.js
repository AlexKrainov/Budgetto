var WidgetsMethods = {
   
    
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
        this.refrehViewTable();
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
        setTimeout(this.resizeGoalCharts, 10);
    },
    resizeGoalCharts: function () {
        for (var i = 0; i < this.goalCharts.length; i++) {
            if (this.goalCharts[i]) {
                this.goalCharts[i].resize();
            }
        }
        this.refrehViewTable();
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
            let options = null;

            if (this.bigCharts[i]) {
                this.bigCharts[i].destroy();
            }

            switch (bigChartData.chartTypeCodeName) {
                case "line":
                case "bar":
                    options = {
                        title: {
                            display: true,
                            text: bigChartData.name,
                        },
                        scales: {
                            yAxes: [{
                                ticks: {
                                    // Include a dollar sign in the ticks
                                    callback: function (value, index, values) {
                                        return new Intl.NumberFormat(UserInfo.Currency.SpecificCulture, { style: 'currency', currency: UserInfo.Currency.CodeName }).format(value)
                                    }
                                }
                            }]
                        },
                        tooltips: {
                            callbacks: {
                                label: function (tooltipItem, data) {
                                    console.log(arguments);
                                    return new Intl.NumberFormat(UserInfo.Currency.SpecificCulture, { style: 'currency', currency: UserInfo.Currency.CodeName }).format(tooltipItem.value);
                                }
                            }
                        },
                    };
                    break;
                case "doughnut":
                case "pie":
                    options = {
                        title: {
                            display: true,
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
                    datasets: bigChartData.dataSets,
                    labels: bigChartData.labels
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
};