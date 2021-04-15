var BudgetMethods = {
    periodType: PeriodTypeEnum.Year,
    isShowSummary: UserInfo.UserSettings.Dashboard_Year_IsShow_Summary,
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

        if (this.tableAjax && (this.tableAjax.readyState == 1 || this.tableAjax.readyState == 3)) { // OPENED & LOADING
            this.tableAjax.abort();
        } else {
            if (this.dataTable) {
                this.template.columns = [];//fixed bugs with change title for columns and export excel after change template
                this.dataTable.destroy();
                $('[data-toggle="tooltip"]').tooltip('dispose');
            }
        }

        this.tableAjax = $.ajax({
            type: "POST",
            url: "/Budget/GetYearBudget?year=" + this.budgetYear + "&templateID=" + this.templateID,
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
        //if (!(UserInfo.UserSettings.Dashboard_Year_IsShow_InvestingChart
        //    || UserInfo.UserSettings.Dashboard_Year_IsShow_SpendingChart
        //    || UserInfo.UserSettings.Dashboard_Year_IsShow_EarningChart)) {
        //    return false;
        //}

        if (this.totalChartsAjax && (this.totalChartsAjax.readyState == 1 || this.totalChartsAjax.readyState == 3)) { // OPENED & LOADING
            this.totalChartsAjax.abort();
        }

        this.totalChartsAjax = $.ajax({
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
        return this.totalChartsAjax;
    },
    //Limit charts
    loadLimitCharts: function () {
        //if (!UserInfo.UserSettings.Dashboard_Year_IsShow_LimitCharts) {
        //    return false;
        //}

        if (this.limitsAjax && (this.limitsAjax.readyState == 1 || this.limitsAjax.readyState == 3)) { // OPENED & LOADING
            this.limitsAjax.abort();
        }

        this.limitsAjax = $.ajax({
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
        return this.limitsAjax;
    },
    //Goal charts
    loadGoalCharts: function () {
        //if (!UserInfo.UserSettings.Dashboard_Year_IsShow_GoalCharts) {
        //    return false;
        //}

        if (this.goalsAjax && (this.goalsAjax.readyState == 1 || this.goalsAjax.readyState == 3)) { // OPENED & LOADING
            this.goalsAjax.abort();
        }

        this.goalsAjax =  $.ajax({
            type: "GET",
            url: `/Goal/LoadCharts?year=${this.budgetYear}&periodTypesEnum=3`,
            contentType: "application/json",
            dataType: 'json',
            context: this,
            success: function (response) {

                this.goalChartsData = response.goalChartsData;

                //  setTimeout(this.initGoalCharts, 10);
            }
        });
        return this.goalsAjax;
    },
    //big charts
    loadBigCharts: function () {
        //if (!UserInfo.UserSettings.Dashboard_Year_IsShow_BigCharts) {
        //    return false;
        //}

        if (this.bigChartsAjax && (this.bigChartsAjax.readyState == 1 || this.bigChartsAjax.readyState == 3)) { // OPENED & LOADING
            this.bigChartsAjax.abort();
        }

        for (var i = 0; i < this.bigChartsData.length; i++) {
            ShowLoading('#bigChart_' + this.bigChartsData[i].chartID);
        }

        this.bigChartsAjax =  $.ajax({
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
            url: "/Account/GetAccounts?year=" + this.budgetYear + "&periodType=3",
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
            url: "/Summary/GetSummaries?year=" + this.budgetYear + "&periodType=3",
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
        if (!(UserInfo.UserSettings.Dashboard_Year_IsShow_ToDoLists)) {
            return false;
        }

        if (this.toDoListAjax && (this.toDoListAjax.readyState == 1 || this.toDoListAjax.readyState == 3)) { // OPENED & LOADING
            this.toDoListAjax.abort();
        }

        ShowLoading('#todoList-view');

        this.accountsAjax = $.ajax({
            type: "GET",
            url: "/ToDoList/GetListsByPeriodType?periodType=3",
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


