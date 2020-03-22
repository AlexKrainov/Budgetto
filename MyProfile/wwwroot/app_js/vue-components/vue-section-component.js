Vue.component("vue-section-component", {
	template: `<div><input type='search' :id='dataSearchId' :name='dataSearchName' :class='dataSearchClass' :style='dataSearchStyle' v-model='dataSearchText' v-on:click='onchange'/></div>`,
	props: {
		dataSearchId: String,
		dataSearchName: String,
		dataSearchClass: {
			type: String,
			default: 'form-control'	
		},
		dataSearchStyle: String,
		dataSectionItems: [Array],
		dataSearchText: String,
		 
	},
	//computed: {

	//},
	data: function () {
		return {
			count: 0
		}
	},
	mounted: function () {
		console.log("hello");
		sendAjax("/Section/GetAllSectionByPerson", null, "GET")
			.then(function (result) {

			});
	},
	methods: {
		onchange: function () {

		}
	}
});