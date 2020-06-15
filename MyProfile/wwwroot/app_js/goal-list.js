var GoalListVue = new Vue({
	el: "#goal-list-vue",
	data: {
		goals: [],

		//edit
	},
	watch: {},
	mounted: function () {
		this.load()
			.then(function () {
			});
	},
	methods: {
		load: function () {
			return sendAjax("/Goal/GetGoals", null, "GET")
				.then(function (result) {
					if (result.isOk = true) {
						GoalListVue.goals = result.goals;
					}
				});
		},

		getDateByFormat: function (date, format) {
			return GetDateByFormat(date, format);
		},
		addMoney: function (goal) {
			GoalEditVue.addMoney(goal);
		},
		edit: function (goal) {
			GoalEditVue.edit(goal);
		}
	}
});

Vue.config.devtools = true;
