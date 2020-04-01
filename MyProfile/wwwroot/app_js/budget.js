var BudgetVue = new Vue({
	el: "#budget-vue",
	data: {
		budgetDate: null,
		templateID: null,

		template: {},
		sections: [],
		rows: [],
		footerRow: [],

		counterColumn: -100,
		counterTemplateBudgetSections: -100,

		column: {},
		flatpickr: {},
	},
	mounted: function () {

		this.templateID = document.getElementById("templateID_hidden").value;
		this.budgetDate = Date.parse(document.getElementById("budgetDate_hidden").value);


		this.loadRows().then(function () {
			BudgetVue.initTable();
		});

		this.flatpickr = flatpickr('#budget-date', {
			altInput: true,
			//dateFormat: 'd.m.Y',
			defaultDate: this.budgetDate,
			plugins: [
				new monthSelectPlugin({
					shorthand: true, //defaults to false
					dateFormat: "m.yy", //defaults to "F Y"
					altFormat: "F Y", //defaults to "F Y"
					theme: "dark" // defaults to "light"
				})
			]
		});

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

			$.fn.dataTable.SearchPanes.defaults = false;
		},

		initTable: function () {
			$("#table").DataTable();
		},
		onChooseSection: function (value, event) {
			console.log(value);
			console.log(event);
		},
		refresh: function () {
			console.log("Refresh");
		},
		getCellValue: function (cell) {
			if (cell.value.indexOf(",")) {
				let values = cell.value.split(",");

				if (values.length == 2) {
					return `<span>${values[0]}<span class="money-muted">,${values[1]}</span></span>`;
				} else {
					return `<span>${cell.value}</span>`
				}
			} else {
				return `<span>${cell.value}</span>`
			}
		}
	}
});


Vue.config.devtools = true;
