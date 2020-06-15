var GoalEditVue = new Vue({
	el: "#goal-edit-vue",
	data: {
		//edit
		goal: {},
		flatpickrFrom: {},
		flatpickrTo: {},

		record: {},
		flatpickrDateTime: {},
		isSaving: false,
	},
	watch: {},
	mounted: function () {
	},
	methods: {
		getDateByFormat: function (date, format) {
			return GetDateByFormat(date, format);
		},
		edit: function (goal) {
			if (goal) {
				this.goal = goal;
			} else {
				this.goal.dateStart = GetDateByFormat(Date(), "YYYY/MM/DD");
			}

			this.flatpickrFrom = flatpickr('#date-from', {
				altInput: true,
				dateFormat: 'Y/m/d',
				defaultDate: this.goal.dateStart,

			});

			this.flatpickrTo = flatpickr('#date-to', {
				altInput: true,
				dateFormat: 'Y/m/d',
				defaultDate: this.goal.dateEnd,
			});

			//find goal by id 
			$("#modal-goal").modal("show");
		},
		save: function () {
			this.isSaving = true;

			return sendAjax("/Goal/Save", this.goal, "POST")
				.then(function (result) {
					if (result.isOk == true) {

						GoalListVue.load();
						//let goalIndex = GoalEditVue.goals.findIndex(x => x.id == result.goal.id);
						//if (goalIndex > 0) {
						//	GoalEditVue.goals[goalIndex] = result.goal;
						//} else {
						//	GoalEditVue.goals.push(result.goal);
						//}
						$("#modal-goal").modal("hide");
					} else {
						console.log(result.message);
					}
					GoalEditVue.isSaving = false;
				});
		},

		addMoney: function (goalID) {
			this.record = {
				goalID: goalID,
				dateTimeOfPayment: GetDateByFormat(Date(), "YYYY/MM/DD"),
			};

			this.flatpickrDateTime = flatpickr('#dateTimeOfPayment', {
				altInput: true,
				dateFormat: 'Y/m/d',
				defaultDate:  this.record.dateTimeOfPayment,
			});
			$("#modal-goal-add-money").modal("show");
		},
		saveMoney: function () {
			this.isSaving = true;

			return sendAjax("/Goal/SaveRecord", this.record, "POST")
				.then(function (result) {
					if (result.isOk == true) {

						GoalListVue.load();
						//let goalIndex = GoalEditVue.goals.findIndex(x => x.id == result.goal.id);
						//if (goalIndex > 0) {
						//	GoalEditVue.goals[goalIndex] = result.goal;
						//} else {
						//	GoalEditVue.goals.push(result.goal);
						//}
						$("#modal-goal-add-money").modal("hide");
					} else {
						console.log(result.message);
					}
					GoalEditVue.isSaving = false;
				});
		}
	}
});

Vue.config.devtools = true;
