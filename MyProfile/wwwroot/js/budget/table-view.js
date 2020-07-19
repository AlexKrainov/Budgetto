var TableVue = new Vue({
	el: "#table-vue",
	data: {
		budgetDate: null,
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
		
	},
	methods: {
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
		}
	}
});


Vue.config.devtools = true;
