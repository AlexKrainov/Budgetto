var RecordVue = new Vue({
	el: "#record-container",
	data: {
		sections: [],
		dateTimeOfPayment: null,
		money: null,
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
