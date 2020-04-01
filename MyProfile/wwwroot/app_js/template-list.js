
var TemplateListVue = new Vue({
	el: "#template-list-vue",
	data: {
		templates: [],
	},
	watch: {
	},
	mounted: function () {
		this.init()
			.then(function () {

			});
	},
	methods: {
		init: function () {

			return sendAjax("/Template/GetTemplates", null, "GET")
				.then(function (result) {
					if (result.isOk = true) {
						TemplateListVue.templates = result.templates;
					}
				});
		},
		getLinkForView: function (template) {
			if (template.periodTypeID == 1) { //PeriodTypesEnum.Days
				return `/Budget/MonthBudget?month=01.03.2020&templateID=${template.id}&periodTypeID=${template.periodTypeID}`;

			} else {
				return '/Budget/Index/' + template.id;
			}
		},
		getDateByFormat: function (date, format) {
			return GetDateByFormat(date, format);
		}
	}
});

Vue.config.devtools = true;
