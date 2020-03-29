var BudgetVue = new Vue({
	el: "#budget-vue",
	data: {
		template: {},
		sections: [],
		rows: [],
		footerRow: [],

		counterColumn: -100,
		counterTemplateBudgetSections: -100,

		column: {},
	},
	mounted: function () {
		//this.init()
		//	.then(function () {
		this.loadRows();
		//});
	},
	methods: {
		//init: function () {
		//	return sendAjax("/Template/GetData/3", null, "GET")
		//		.then(function (result) {
		//			if (result.isOk = true) {
		//				BudgetVue.template = result.template;
		//			}
		//		});
		//},

		loadRows: function () {
			let templateID = document.getElementById("templateID").value;
			return sendAjax("/Budget/GetBudget?templateID=" + templateID, null, "GET")
				.then(function (result) {
					if (result.isOk = true) {
						BudgetVue.rows = result.rows;
						BudgetVue.footerRow = result.footerRow;
						BudgetVue.template = result.template;
					}
				});
		},
		onChooseSection: function (value, event) {
			console.log(value);
			console.log(event);
		},
		refresh: function () {
			console.log("Refresh");
		}
	}
});


Vue.config.devtools = true;
