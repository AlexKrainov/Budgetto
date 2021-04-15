var BudgetMethods = {
    periodType: PeriodTypeEnum.Month,
    isShowSummary: UserInfo.UserSettings.Dashboard_Month_IsShow_Summary,
    mounted: function () {
        this.getPageSettings();

        this.templateID = document.getElementById("templateID_hidden").value;
        this.budgetDate = document.getElementById("budgetDate_hidden").value;

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
        if (this.tableAjax && (this.tableAjax.readyState == 1 || this.tableAjax.readyState == 3)) { // OPENED & LOADING
            this.tableAjax.abort();
        } else {
            if (this.dataTable) {
                this.dataTable.destroy();
                this.template.columns = [];//fixed bugs with change title for columns and export excel after change template
                $('[data-toggle="tooltip"]').tooltip('dispose');
            }
        }
        //this.rows = [];
        //this.footerRow = [];
        //this.template = {};

        this.tableAjax = $.ajax({
            type: "POST",
            url: "/Budget/GetMonthBudget?month=" + this.budgetDate + "&templateID=" + this.templateID,
            context: this,
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (result) {
                if (result.isOk == true) {
                    this.rows = result.rows;
                    this.footerRow = result.footerRow;
                    this.template = result.template;
                }
                return true;
            },
            error: function (xhr, status, error) {
                this.isSaving = false;
                console.log(error);
            }
        });

        return this.tableAjax;
    },
    changeView: function (months) {
        var result = new Date(this.flatpickrStart.latestSelectedDateObj);
        result.setMonth(result.getMonth() + months);
        console.log(result);

        this.flatpickrStart.setDate(result, true);
    },
    //Total charts
    loadTotalCharts: function () {
        if (!(UserInfo.UserSettings.Dashboard_Month_IsShow_InvestingChart
            || UserInfo.UserSettings.Dashboard_Month_IsShow_SpendingChart
            || UserInfo.UserSettings.Dashboard_Month_IsShow_EarningChart)) {
            return false;
        }

        if (this.totalChartsAjax && (this.totalChartsAjax.readyState == 1 || this.totalChartsAjax.readyState == 3)) { // OPENED & LOADING
            this.totalChartsAjax.abort();
        }

        this.totalChartsAjax =  $.ajax({
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
        return this.totalChartsAjax;
    },
    //Limit charts
    loadLimitCharts: function () {
        if (!UserInfo.UserSettings.Dashboard_Month_IsShow_LimitCharts) {
            return false;
        }

        if (this.limitsAjax && (this.limitsAjax.readyState == 1 || this.limitsAjax.readyState == 3)) { // OPENED & LOADING
            this.limitsAjax.abort();
        }

        this.limitsAjax =  $.ajax({
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

        return this.limitsAjax;
    },
    //Goal charts
    loadGoalCharts: function () {
        if (!UserInfo.UserSettings.Dashboard_Month_IsShow_GoalCharts) {
            return false;
        }

        if (this.goalsAjax && (this.goalsAjax.readyState == 1 || this.goalsAjax.readyState == 3)) { // OPENED & LOADING
            this.goalsAjax.abort();
        }

        this.goalsAjax =  $.ajax({
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

                //setTimeout(this.initGoalCharts, 10);
            }
        });
        return this.goalsAjax;
    },
    //big charts
    loadBigCharts: function () {
        if (!UserInfo.UserSettings.Dashboard_Month_IsShow_BigCharts) {
            return false;
        }

        if (this.bigChartsAjax && (this.bigChartsAjax.readyState == 1 || this.bigChartsAjax.readyState == 3)) { // OPENED & LOADING
            this.bigChartsAjax.abort();
        }

        for (var i = 0; i < this.bigChartsData.length; i++) {
            ShowLoading('#bigChart_' + this.bigChartsData[i].chartID);
        }

        this.bigChartsAjax = $.ajax({
            type: "GET",
            url: "/Chart/LoadCharts?date=" + this.budgetDate + "&periodType=1",
            contentType: "application/json",
            dataType: 'json',
            context: this,
            success: function (response) {

                //if (!UserInfo.UserSettings.Dashboard_Month_IsShow_GoalCharts) {
                //    return false;
                //}

                this.bigChartsData = response.bigChartsData;

                for (var i = 0; i < this.bigChartsData.length; i++) {
                    HideLoading('#bigChart_' + this.bigChartsData[i].chartID);
                }

                setTimeout(this.initBigChartCharts, 100);
            }
        });

        return this.bigChartsAjax;
    },
    //Accounts 
    loadAccounts: function () {
        if (!(UserInfo.UserSettings.Dashboard_Month_IsShow_Accounts)) {
            return false;
        }

        if (this.accountsAjax && (this.accountsAjax.readyState == 1 || this.accountsAjax.readyState == 3)) { // OPENED & LOADING
            this.accountsAjax.abort();
        }

        ShowLoading('#accounts-view');

        this.accountsAjax = $.ajax({
            type: "GET",
            url: "/Account/GetAccounts?date=" + this.budgetDate + "&periodType=1",
            contentType: "application/json",
            dataType: 'json',
            context: this,
            success: function (response) {
                this.accounts = response.accounts;

                HideLoading('#accounts-view');
            }
        });
        return this.accountsAjax;
    },
    //Summary
    loadSummaries: function () {
        if (!(UserInfo.UserSettings.Dashboard_Month_IsShow_Summary)) {
            return false;
        }

        if (this.summaryAjax && (this.summaryAjax.readyState == 1 || this.summaryAjax.readyState == 3)) { // OPENED & LOADING
            this.summaryAjax.abort();
        }

        ShowLoading('#summary-view');

        this.summaryAjax = $.ajax({
            type: "GET",
            url: "/Summary/GetSummaries?date=" + this.budgetDate + "&periodType=1",
            contentType: "application/json",
            dataType: 'json',
            context: this,
            success: function (response) {
                this.summary = response.summaries;

                this.initSummaryCharts();
                HideLoading('#summary-view');
            }
        });
        return this.summaryAjax;
    },
    //ToDoList 
    loadToDoLists: function () {
        if (!(UserInfo.UserSettings.Dashboard_Month_IsShow_ToDoLists)) {
            return false;
        }

        if (this.toDoListAjax && (this.toDoListAjax.readyState == 1 || this.toDoListAjax.readyState == 3)) { // OPENED & LOADING
            this.toDoListAjax.abort();
        }

        ShowLoading('#todoList-view');

        this.accountsAjax = $.ajax({
            type: "GET",
            url: "/ToDoList/GetListsByPeriodType?periodType=1",
            contentType: "application/json",
            dataType: 'json',
            context: this,
            success: function (response) {
                this.lists = response.lists;

                HideLoading('#todoList-view');
            }
        });
        return this.accountsAjax;
    },
};