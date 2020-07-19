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

		records: [],
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
			return sendAjax("/Budget/GetYearBudget?year=" + this.budgetYear + "&templateID=" + this.templateID, null, "POST")
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
		},
		clickCell: function (rowIndex, cellIndex) {
			let templateColumnTypes = [2, 3, 4, 7]; // DaysForMonth = 2,MonthsForYear = 3,YearsFor10Year = 4,WeeksForMonth = 7

			let sections = [];

			if (this.template.columns[cellIndex].templateColumnType == 1) {//BudgetSection/Money
				sections = this.template.columns[cellIndex].templateBudgetSections.map(x => x.sectionID);
			} else if (templateColumnTypes.indexOf(this.template.columns[cellIndex].templateColumnType) >= 0) {
				for (var i = 0; i < this.template.columns.length; i++) {
					sections = sections.concat(this.template.columns[i].templateBudgetSections.map(x => x.sectionID));
				}
			} else {
				return;
			}

			let filter = {
				sections: sections,
				startDate: moment(new Date(this.budgetYear, rowIndex, 1)).format(),
				endDate: moment(new Date(this.budgetYear, rowIndex, 1)).endOf("month").format()
			};

			return this.loadTimeLine(filter)
		},
		clickFooterCell: function (cellIndex) {
			let filter = {
				sections: this.template.columns[cellIndex].templateBudgetSections.map(x => x.sectionID),
				startDate: moment(new Date(this.budgetYear, 0, 1)).format(),
				endDate: moment(new Date(this.budgetYear, 11, 31)).format(),
			};

			return this.loadTimeLine(filter)
		},
		loadTimeLine: function (filter) {
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
		edit: function (record) {

			RecordVue.recordComponent.editByElement({
				id: record.id,
				isCorrect: true,
				isSaved: true,
				money: record.money,
				sectionID: record.sectionID,
				sectionName: record.sectionName,
				tag: record.rawData,
				dateTimeOfPayment: record.dateTimeOfPayment,
			},
				BudgetVue.load
			);
			$("#modalTimeLine").modal("hide");
		}
	}
});


Vue.config.devtools = true;
