var UserVue = new Vue({
	el: ".page-settings",
	data: {

	},
	watch: {
	},
	mounted: function () {
	},
	methods: {
		openSettings: function () {
			document.querySelector(".page-settings").classList.add("theme-settings-open");
		},
		closeSettings: function () {
			document.querySelector(".page-settings").classList.remove("theme-settings-open");
		}
	}
});


Vue.config.devtools = true;
