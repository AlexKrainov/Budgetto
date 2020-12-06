var ChartListVue = new Vue({
    el: "#chart-list-vue",
    data: {
        charts: [],
        activeChartPeriodTypeID: -1,

        msnry: {},
    },
    watch: {},
    mounted: function () {
        this.load()
            .then(function () {
            });
    },
    methods: {
        load: function () {
            return sendAjax("/Chart/GetCharts", null, "GET")
                .then(function (result) {
                    if (result.isOk == true) {
                        ChartListVue.charts = result.charts;

                        setTimeout(function () {
                            if (ChartListVue.msnry && ChartListVue.msnry.destroy != undefined) {
                                ChartListVue.msnry.destroy();
                            }
                            ChartListVue.msnry = new Masonry('#charts', {
                                itemSelector: '.masonry-item:not(.d-none)',
                                columnWidth: '.masonry-item-sizer',
                                originLeft: true,
                                horizontalOrder: true
                            });
                        }, 100);
                    }
                });
        },
        edit: function (chartID) {

        },
        getDateByFormat: function (date, format) {
            return GetDateByFormat(date, format);
        },
        reloadView: function () {
            setTimeout(function () {
                ChartListVue.msnry.layout();
            }, 15);
        },
        getDateByFormat: function (date, format) {
            return GetDateByFormat(date, format);
        },
        remove: function (chart) {
            ShowLoading('#chart_' + chart.id);

            return $.ajax({
                type: "POST",
                url: "/Chart/Remove",
                data: JSON.stringify(chart),
                context: chart,
                contentType: "application/json",
                dataType: 'json',
                success: function (response) {
                    chart.isDeleted = response.isOk;
                    HideLoading('#chart_' + chart.id);
                }
            });
        },
        recovery: function (chart) {
            ShowLoading('#chart_' + chart.id);
            return $.ajax({
                type: "POST",
                url: "/Chart/Recovery",
                data: JSON.stringify(chart),
                context: chart,
                contentType: "application/json",
                dataType: 'json',
                success: function (response) {
                    chart.isDeleted = !response.isOk;
                    HideLoading('#chart_' + chart.id);
                }
            });
        },
        toggleChart: function (chartID, periodType) {
            ShowLoading('#chart_' + chartID);
            return $.ajax({
                type: "GET",
                url: "/Chart/ToggleChart?id=" + chartID + "&periodType=" + periodType,
                contentType: "application/json",
                dataType: 'json',
                context: {
                    id: chartID, periodType: periodType
                },
                success: function (response) {
                    HideLoading('#chart_' + this.id);
                    if (response.isOk = true) {
                        if (this.periodType == PeriodTypeEnum.Month) {
                            ChartListVue.charts[ChartListVue.charts.findIndex(x => x.id == this.id)].isShowBudgetMonth = response.isShow;
                        } else if (this.periodType == PeriodTypeEnum.Year) {
                            ChartListVue.charts[ChartListVue.charts.findIndex(x => x.id == this.id)].isShowBudgetYear = response.isShow;
                        }
                    }
                },
                error: function () {
                    HideLoading('#chart_' + this.id);
                }
            });
        }
    }
});


