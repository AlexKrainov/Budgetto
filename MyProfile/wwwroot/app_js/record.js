var RecordVue2 = new Vue({
	el: "#record-container2",
	data: {
		sections: [],
		dateTimeOfPayment: null,
		money: null,
		description: null,
		searchSection: null,
	},
	mounted: function () {
		this.loadBAranAndRType();

		$('#record-date2').flatpickr({
			altInput: true
		});
	},
	methods: {
		loadBAranAndRType: function () {
			return sendAjax("/Section/GetAllSectionByPerson", null, "GET")
				.then(function (result) {
					if (result.isOk = true) {
						RecordVue2.sections = result.sections;
						$("#budgetArea2").select2();
					}
				});
		},
		save: function () {
			let obj = {
				money: this.money,
				dateTimeOfPayment: this.dateTimeOfPayment,
				sectionID: $("#budgetArea2").val()
			}
			return sendAjax("/Record/Save", obj, "POST")
				.then(function (result) {
					if (result.isOk = true) {
						BudgetVue2.loadRows();
						$('#modal-record2').modal('hide');
					}
				});
		},
		onChooseSection: function (section) {
			console.log(section);
		}
	}
});

var RecordVue = new Vue({
	el: "#record-container",
	data: {
		sections: [],
		dateTimeOfPayment: null,
		money: null,
		description: null,
	},
	mounted: function () {
		this.loadBAranAndRType();

		$('#record-date').flatpickr({
			altInput: true
		});
	},
	methods: {
		loadBAranAndRType: function () {
			return sendAjax("/Section/GetAllSectionByPerson", null, "GET")
				.then(function (result) {
					if (result.isOk = true) {
						RecordVue.sections = result.sections;
						$("#budgetArea").select2();
					}
				});
		},
		save: function () {
			let obj = {
				money: this.money,
				dateTimeOfPayment: this.dateTimeOfPayment,
				sectionID: $("#budgetArea").val()
			}
			return sendAjax("/Record/Save", obj, "POST")
				.then(function (result) {
					if (result.isOk = true) {
						BudgetVue.loadRows();
						$('#modal-record').modal('hide');
					}
				});
		}
	}
});

Vue.config.devtools = true;
