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

Vue.config.devtools = true;
