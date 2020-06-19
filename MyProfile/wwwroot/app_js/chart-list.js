var ChartListVue = new Vue({
	el: "#chart-list-vue",
	data: {
		charts: [] ,
	},
	watch: {},
	mounted: function () {
		this.load()
			.then(function () {
			});
	},
	methods: {
		load: function () {
			return sendAjax("/Chart/GetCharts", null, "GET")
				.then(function (result) {
					if (result.isOk = true) {
						ChartListVue.charts = result.charts;
					}
				});
		},
		edit: function (chartID) {

		},
		getDateByFormat: function (date, format) {
			return GetDateByFormat(date, format);
		},
	}
});

Vue.config.devtools = true;
