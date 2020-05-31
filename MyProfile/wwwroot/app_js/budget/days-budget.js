var BudgetVue = new Vue({
	el: "#budget-vue",
	data: {
		budgetDate: null,
		templateID: null,

		template: {},
		rows: [],
		footerRow: [],

		column: {},
		flatpickr: {},

		records: [],
	},
	watch: {
	},
	mounted: function () {
		this.templateID = document.getElementById("templateID_hidden").value;
		this.budgetDate = GetDateByFormat(Date.parse(document.getElementById("budgetDate_hidden").value), "YYYY/MM/DD");

		//if (this.templateID == -1) {
		//	let options = document.getElementById("templates").children;
		//	if (options && options.length > 0) {
		//		options[0]["selected"] = "selected";
		//	}
		//}

		this.flatpickr = flatpickr('#budget-date', {
			altInput: true,
			//dateFormat: 'd.m.Y',
			defaultDate: this.budgetDate,
			plugins: [
				new monthSelectPlugin({
					shorthand: true, //defaults to false
					dateFormat: "yy/m/d", //defaults to "F Y"
					altFormat: "F Y", //defaults to "F Y"
					theme: "dark" // defaults to "light"
				})
			]
		});

		this.refresh();
	},
	methods: {
		load: function () {
			return sendAjax("/Budget/GetDaysBudget?month=" + this.budgetDate + "&templateID=" + this.templateID, null, "POST")
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
					return `<span>${cell.value}</span>`;
				}
			} else {
				return `<span>${cell.value}</span>`;
			}
		},
		clickCell: function (rowIndex, cellIndex) {

			let sections = this.template.columns[cellIndex].templateBudgetSections.map(x => x.sectionID);
			let filter = {
				sections: sections,
				rowIndex: rowIndex,
				startDate: moment(this.budgetDate).add(rowIndex, "days").format(),
				endDate: moment(this.budgetDate).add((rowIndex + 1), "days").add(-1, "minutes").format()
			};

			return $.ajax({
				type: "POST",
				url: "/Budget/LoadingRecordsForTableView",
				data: JSON.stringify(filter),
				contentType: "application/json",
				dataType: 'json',
				context: this,
				success: function (response) {
					this.records = response.data;
					$("#modalTimeLine").modal("show");
				}
			});
		},
		GetDateByFormat: function (date, format) {
			return GetDateByFormat(date, format);
		},
	}
});


Vue.config.devtools = true;
