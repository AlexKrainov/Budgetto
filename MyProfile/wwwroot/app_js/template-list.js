﻿
var TemplateListVue = new Vue({
	el: "#template-list-vue",
	data: {
		templates: [],
		activeTemplatePeriodTypeID: -1,
		search: null,
	},
	watch: {
		search: function (newValue, oldValue) {
			if (!newValue) {
				this.templates.forEach(function (el, index) { el.isShow = true; });
			}

			for (var i = 0; i < this.templates.length; i++) {
				this.templates[i].isShow = this.templates[i].name.indexOf(newValue) >= 0;
			}
		}
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
				return `/Budget/DaysBudget?templateID=${template.id}`;
			} else if (template.periodTypeID == 3) { //PeriodTypesEnum.Months
				return `/Budget/MonthsBudget?templateID=${template.id}`;
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
