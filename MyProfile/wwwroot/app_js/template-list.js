
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
		}
	}
});

Vue.config.devtools = true;
