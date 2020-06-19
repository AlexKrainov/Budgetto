var UserVue = new Vue({
	el: ".page-settings",
	data: {
		page: null,
		isCallActions: true,
		actions: [
			"BudgetVue.load",
			"BudgetVue.loadTotalCharts",
			"BudgetVue.loadLimitCharts",
			"BudgetVue.loadGoalCharts",
			"BudgetVue.loadBigCharts",
			"GoalListVue.load"
		],
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

			return $.ajax({
				type: "POST",
				url: "/UserSettings/SaveSettings",
				data: JSON.stringify(settings),
				contentType: "application/json",
				dataType: 'json',
				context: this,
				success: function (response) {
					if (this.isCallActions) {
						for (var i = 0; i < this.actions.length; i++) {
							var fn = window.getFunctionFromString(this.actions[i]);
							if (typeof fn === 'function') {
								fn();
							}
						}
					}
				}
			});
		},
		change: function (isCallActions) {
			this.isCallActions = isCallActions;
			this.saveBudgetSettings();
		},
		toggleBudgetTotal: function (sectionType) {
			if (sectionType == 1) {//EarningChart
				BudgetVue.earningData.isShow = document.querySelector("[data-prop=BudgetPages_EarningChart]").checked;
			} else if (sectionType == 2) { //SpendingChart
				BudgetVue.spendingData.isShow = document.querySelector("[data-prop=BudgetPages_SpendingChart]").checked;
			} else if (sectionType == 3) { //Invest chart
				BudgetVue.investingData.isShow = document.querySelector("[data-prop=BudgetPages_InvestingChart]").checked;
			}
			//setTimeout(BudgetVue.resizeCharts(), 1000);
		},
		toggleLimits: function () {
			let checked = document.querySelector("[data-prop=BudgetPages_IsShow_Limits]").checked;
			for (var i = 0; i < BudgetVue.limitsChartsData.length; i++) {
				BudgetVue.limitsChartsData[i].isShow = checked;
			}
		},
		toggleGoals: function () {
			let checked = document.querySelector("[data-prop=BudgetPages_IsShow_Goals]").checked;
			for (var i = 0; i < BudgetVue.goalChartsData.length; i++) {
				BudgetVue.goalChartsData[i].isShow = checked;
			}
		},
		toggleBigCharts: function () {
			let checked = document.querySelector("[data-prop=BudgetPages_IsShow_BigCharts]").checked;
			for (var i = 0; i < BudgetVue.bigChartsData.length; i++) {
				BudgetVue.bigChartsData[i].isShow = checked;
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
