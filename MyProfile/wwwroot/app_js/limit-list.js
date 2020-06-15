
var LimitListVue = new Vue({
	el: "#limit-list-vue",
	data: {
		limits: [],
		activeLimitPeriodTypeID: -1,

		//edit
		limit: { periodName: '', periodTypeID: -1 },
		flatpickrFrom: {},
		flatpickrTo: {},

		sections: [],
		periodTypes: [],
		isSaving: false,
	},
	watch: {},
	mounted: function () {
		this.load()
			.then(function () {
				LimitListVue.loadSections();
				LimitListVue.loadPeriodTypes();
			});
	},
	methods: {
		load: function () {
			return sendAjax("/Limit/GetLimits", null, "GET")
				.then(function (result) {
					if (result.isOk = true) {
						LimitListVue.limits = result.limits;
					}
				});
		},
		loadSections: function () {
			return sendAjax("/Section/GetAllSectionByPerson", null, "GET")
				.then(function (result) {
					if (result.isOk = true) {
						LimitListVue.sections = result.sections;
					}
				});
		},
		loadPeriodTypes: function () {
			return sendAjax("/Limit/GetPeriodTypes", null, "GET")
				.then(function (result) {
					if (result.isOk = true) {
						LimitListVue.periodTypes = result.periodTypes;
					}
				});
		},
		getDateByFormat: function (date, format) {
			return GetDateByFormat(date, format);
		},
		edit: function (limit) {

			if (limit) {
				this.limit = limit;
			} else {
				this.limit.periodTypeID = this.periodTypes[0].id;
				this.limit.periodName = this.periodTypes[0].name;

				this.limit.dateStart = GetDateByFormat(Date(), "YYYY/MM/DD");
			}

			this.flatpickrFrom = flatpickr('#date-from', {
				altInput: true,
				//dateFormat: 'd.m.Y',
				defaultDate: this.limit.dateStart,
				plugins: [
					new monthSelectPlugin({
						shorthand: true, //defaults to false
						dateFormat: "yy/m/d", //defaults to "F Y"
						altFormat: "F Y", //defaults to "F Y"
						theme: "dark" // defaults to "light"
					})
				]
			});

			this.flatpickrTo = flatpickr('#date-to', {
				altInput: true,
				//dateFormat: 'd.m.Y',
				defaultDate: this.limit.dateEnd,
				plugins: [
					new monthSelectPlugin({
						shorthand: true, //defaults to false
						dateFormat: "yy/m/d", //defaults to "F Y"
						altFormat: "F Y", //defaults to "F Y"
						theme: "dark" // defaults to "light"
					})
				]
			});

			$("#limitSections").val(this.limit.sections.map(x => x.id)).select2();

			//find limit by id 
			$("#modal-limit").modal("show");
		},
		save: function () {
			this.isSaving = true;
			this.limit.newSections = $("#limitSections").select2("val").map(function (x) { return { id: x } });

			return sendAjax("/Limit/Save", this.limit, "POST")
				.then(function (result) {
					if (result.isOk == true) {

						LimitListVue.load();
						//let limitIndex = LimitListVue.limits.findIndex(x => x.id == result.limit.id);
						//if (limitIndex > 0) {
						//	LimitListVue.limits[limitIndex] = result.limit;
						//} else {
						//	LimitListVue.limits.push(result.limit);
						//}
						$("#modal-limit").modal("hide");
					} else {
						console.log(result.message);
					}
					LimitListVue.isSaving = false;
				});
		},
		finishLimit: function () {
			this.limit.dateEnd = GetDateByFormat((new Date()).setMonth((new Date()).getMonth() - 1), "YYYY/MM/DD");
			this.flatpickrTo = flatpickr('#date-to', {
				altInput: true,
				//dateFormat: 'd.m.Y',
				defaultDate: this.limit.dateEnd,
				plugins: [
					new monthSelectPlugin({
						shorthand: true, //defaults to false
						dateFormat: "yy/m/d", //defaults to "F Y"
						altFormat: "F Y", //defaults to "F Y"
						theme: "dark" // defaults to "light"
					})
				]
			});
		}
	}
});

Vue.config.devtools = true;
