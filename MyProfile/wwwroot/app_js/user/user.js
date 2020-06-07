var UserVue = new Vue({
	el: ".page-settings",
	data: {
		page: null,
		actions: [
			"BudgetVue.load"
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
					for (var i = 0; i < this.actions.length; i++) {
						var fn = window.getFunctionFromString(this.actions[i]);
						if (typeof fn === 'function') {
							fn();
						}
					}

				}
			});
		},
		change: function () {
			this.saveBudgetSettings();
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
