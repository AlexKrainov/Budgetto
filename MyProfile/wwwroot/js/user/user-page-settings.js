var UserVue = new Vue({
    el: ".page-settings",
    data: {
        page: null,
        isCallActions: true,
        actions: [
            "BudgetVue.refresh",
            "GoalListVue.load",
            "LimitListVue.load"
        ],
        partActions: [],

        ajaxData: null,
    },
    watch: {
    },
    mounted: function () {
        this.page = document.querySelector(".page-settings").attributes.getNamedItem("data-page").value;
    },
    methods: {
        // CONTROL SETTINGS
        openSettings: function () {
            document.querySelector(".page-settings").classList.add("theme-settings-open");
        },
        closeSettings: function () {
            document.querySelector(".page-settings").classList.remove("theme-settings-open");
        },
        // PAGES
        // Budget/Month
        saveBudgetSettings: function () {
            let allControls = document.querySelectorAll("[name=user_settings]");
            let settings = { pageName: this.page };

            for (var i = 0; i < allControls.length; i++) {
                if (allControls[i].type == "checkbox") {
                    settings[allControls[i].attributes.getNamedItem("data-prop").value] = allControls[i].checked;
                }
            }

            if (this.ajaxData && (this.ajaxData.readyState == 1 || this.ajaxData.readyState == 3)) { // OPENED & LOADING
                this.ajaxData.abort();
            }

            this.ajaxData = $.ajax({
                type: "POST",
                url: "/UserSettings/SaveSettings",
                data: JSON.stringify(settings),
                contentType: "application/json",
                dataType: 'json',
                context: this,
                success: function (response) {
                    let actions = this.isCallActions ? this.acthions : this.partActions;
                    for (var i = 0; i < actions.length; i++) {
                        UserVue.callMethod(actions[i]);
                    }
                }
            });
            return this.ajaxData;
        },
        change: function (isCallActions, partActions) {
            this.isCallActions = isCallActions;
            this.partActions = partActions;
            this.saveBudgetSettings();
        },
        toggleBudgetTotal: function (sectionType, method) {
            if (sectionType == 1) {//EarningChart
                UserInfo.UserSettings.Dashboard_Month_IsShow_EarningChart = BudgetVue.earningData.isShow = document.querySelector("[data-prop=BudgetPages_EarningChart]").checked;
            } else if (sectionType == 2) { //SpendingChart
                UserInfo.UserSettings.Dashboard_Month_IsShow_SpendingChart = BudgetVue.spendingData.isShow = document.querySelector("[data-prop=BudgetPages_SpendingChart]").checked;
            } else if (sectionType == 3) { //Invest chart
                UserInfo.UserSettings.Dashboard_Month_IsShow_InvestingChart = BudgetVue.investingData.isShow = document.querySelector("[data-prop=BudgetPages_InvestingChart]").checked;
            }

            if (typeof (BudgetVue.refrehViewTable) == "function") {
                setTimeout(BudgetVue.refrehViewTable, 50);
            }
        },
        toggleLimits: function (method) {
            let checked = document.querySelector("[data-prop=BudgetPages_IsShow_Limits]").checked;
            for (var i = 0; i < BudgetVue.limitsChartsData.length; i++) {
                BudgetVue.limitsChartsData[i].isShow = checked;
            }
            UserInfo.UserSettings.Dashboard_Month_IsShow_LimitCharts = checked;

            if (checked == false && typeof (BudgetVue.refrehViewTable) == "function") {
                setTimeout(BudgetVue.refrehViewTable, 50);
            }
        },
        toggleGoals: function (method) {
            let checked = document.querySelector("[data-prop=BudgetPages_IsShow_Goals]").checked;
            for (var i = 0; i < BudgetVue.goalChartsData.length; i++) {
                BudgetVue.goalChartsData[i].isShow = checked;
            }
            UserInfo.UserSettings.Dashboard_Month_IsShow_GoalCharts = checked;

            if (checked == false && typeof (BudgetVue.refrehViewTable) == "function") {
                setTimeout(BudgetVue.refrehViewTable, 50);
            }
        },
        toggleBigCharts: function (method) {
            let checked = document.querySelector("[data-prop=BudgetPages_IsShow_BigCharts]").checked;
            for (var i = 0; i < BudgetVue.bigChartsData.length; i++) {
                BudgetVue.bigChartsData[i].isShow = checked;
            }
            UserInfo.UserSettings.Dashboard_Month_IsShow_BigCharts = checked;

            if (checked == false && typeof (BudgetVue.refrehViewTable) == "function") {
                setTimeout(BudgetVue.refrehViewTable, 50);
            }
        },

        callMethod: function (method) {
            var fn = window.getFunctionFromString(method);
            if (typeof (fn) === "function") {
                fn();
            }
        }
    }
});

window.getFunctionFromString = function (string) {
    var scope = window;
    var scopeSplit = string.split('.');
    for (i = 0; i < scopeSplit.length - 1; i++) {
        scope = scope[scopeSplit[i]];

        if (scope == undefined) return;
    }

    return scope[scopeSplit[scopeSplit.length - 1]];
}

Vue.config.devtools = true;
