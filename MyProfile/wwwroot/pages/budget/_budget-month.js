var BudgetMethods = {
    periodType: PeriodTypeEnum.Month,
    mounted: function () {
        this.getPageSettings();

        this.templateID = document.getElementById("templateID_hidden").value;
        this.budgetDate = GetDateByFormat(Date.parse(document.getElementById("budgetDate_hidden").value), "YYYY/MM/DD");

        let dateConfig = GetFlatpickrRuConfig_Month(this.budgetDate);
        this.flatpickrStart = flatpickr('#budget-date', dateConfig);

        //table
        $('#modalTimeLine').on('hide.bs.modal', function () {
            //  BudgetVue.closeTimeline();
        })

        this.refresh();

        window.layoutHelpers.on('resize', this.resizeAll);

        this.earningData.isShow = UserInfo.UserSettings.Dashboard_Month_IsShow_EarningChart;
        this.spendingData.isShow = UserInfo.UserSettings.Dashboard_Month_IsShow_SpendingChart;
        this.investingData.isShow = UserInfo.UserSettings.Dashboard_Month_IsShow_InvestingChart;

        RecordVue.callback = this.refreshAfterChangeRecords;
    },
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
            url: "/BudgetTotal/LoadByMonth?to=" + this.budgetDate,
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
        if (!UserInfo.UserSettings.Dashboard_Month_IsShow_LimitCharts) {
            return false;
        }
        return $.ajax({
            type: "GET",
            url: "/Limit/LoadCharts?date=" + this.budgetDate + "&periodTypesEnum=1",
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
        if (!UserInfo.UserSettings.Dashboard_Month_IsShow_GoalCharts) {
            return false;
        }

        return $.ajax({
            type: "GET",
            url: `/Goal/LoadCharts?date=${this.budgetDate}&periodTypesEnum=1`,
            contentType: "application/json",
            dataType: 'json',
            context: this,
            success: function (response) {
                if (!UserInfo.UserSettings.Dashboard_Month_IsShow_GoalCharts) {
                    return false;
                }

                this.goalChartsData = response.goalChartsData;

                setTimeout(this.initGoalCharts, 10);
            }
        });
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
            url: "/Chart/LoadCharts?date=" + this.budgetDate + "&periodType=1",
            contentType: "application/json",
            dataType: 'json',
            context: this,
            success: function (response) {

                if (!UserInfo.UserSettings.Dashboard_Month_IsShow_GoalCharts) {
                    return false;
                }

                this.bigChartsData = response.bigChartsData;

                for (var i = 0; i < this.bigChartsData.length; i++) {
                    HideLoading('#bigChart_' + this.bigChartsData[i].chartID);
                }

                setTimeout(this.initBigChartCharts, 10);
            }
        });
    },
};