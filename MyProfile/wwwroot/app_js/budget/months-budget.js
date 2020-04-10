var BudgetVue = new Vue({
	el: "#budget-vue",
	data: {
		budgetYear: null,
		templateID: null,

		template: {},
		rows: [],
		footerRow: [],

		column: {},
		flatpickr: {},
	},
	watch: {
	},
	mounted: function () {
		this.templateID = document.getElementById("templateID_hidden").value;
		this.budgetYear = document.getElementById("budgetYear_hidden").value;

		this.refresh();
	},
	methods: {
		load: function () {
			return sendAjax("/Budget/GetMonthsBudget?year=" + this.budgetYear + "&templateID=" + this.templateID, null, "POST")
				.then(function (result) {
					if (result.isOk == true) {
						BudgetVue.rows = result.rows;
						BudgetVue.footerRow = result.footerRow;
						BudgetVue.template = result.template;

					}
				});

			$.fn.dataTable.SearchPanes.defaults = false;
		},
		refresh: function () {
			this.load().then(function () {
				BudgetVue.initTable();
			});
		},
		initTable: function () {
			$("#table").DataTable();
		},

		//View cell
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
