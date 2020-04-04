var AreaVue = new Vue({
	el: "#area-vue",
	data: {
		areas: [],
		area: {},

		searchText: null,
	},
	watch: {

	},
	mounted: function () {
		this.init();
	},
	methods: {
		init: function () {
			sendAjax("/Section/GetAllSectionForEdit", null, "GET")
				.then(function (result) {
					if (result.isOk) {
						AreaVue.areas = result.sections;
					}
				});
		},
		selectArea: function (area, event) {
			document.querySelectorAll("#area-vue li.list-group-item").forEach(function (el, index) {
				el.classList.remove("active-section");
			});
			$(event.target).closest("li").addClass("active-section");

			SectionVue.sections = area.sections;
			SectionVue.areaID = area.id;
		},
		create: function () {
			this.area = {
				name: this.searchText,
			};
			$("#modal-area").modal("show");
		},
		edit: function (area) {
			this.area = area;
			$("#modal-area").modal("show");
		},
		save: function () {
			if (this.area.name.length > 0) {
				sendAjax("/Section/SaveArea", this.area, "POST")
					.then(function (result) {
						if (result.isOk) {
							AreaVue.area = result.area;
							if (AreaVue.areas.findIndex(x => x.id == result.area.id) == -1) {
								AreaVue.areas.push(result.area);
							}
							$("#modal-area").modal("hide");

						}
					});
			}
		},
		remove: function (area) {
			if (!this.area.isGlobal) {
				sendAjax("/Section/RemoveArea?id=" + this.area.id, null, "POST")
			}
		}
	}
});

var SectionVue = new Vue({
	el: "#section-vue",
	data: {
		sections: [],
		section: {},

		areaID: null,

		icons: [],
		searchIcon: '',
	},
	watch: {
		searchIcon: function (newValue, oldValue) {
			if (newValue) {
				newValue = newValue.toLocaleLowerCase();
			}

			for (var i = 0; i < this.icons.length; i++) {
				this.icons[i].isShow = this.icons[i].nameClass.toLocaleLowerCase().indexOf(newValue) >= 0 || this.icons[i].name.toLocaleLowerCase().indexOf(newValue) >= 0;
			}
		}
	},
	mounted: function () {
		$.getJSON("/json/font-awesome.json", function () { })
			.done(function (json) {
				SectionVue.icons = json;
			});
	},
	methods: {
		create: function () {
			this.section = {
				name: "",
			}
			$("#modal-section").modal("show");
		},
		edit: function (section) {
			this.section = section;

			$("#cssColor").colorPick({
				'initialColor': this.section.cssColor ? this.section.cssColor : "#E5E9EB",
				//'palette': ['#5BADFF', "#F06568", "#3C9E71","#C6C6C6"],
				//'localizationColor': [],
				'onColorSelected': function () {
					//let title = '';
					//let localColor = this.localizationColor.find(x => x.ColorCode.toUpperCase() == this.color.toUpperCase());
					//if (localColor && localColor.ColorName) {
					//title = localColor.ColorName;
					SectionVue.section.cssColor = this.color;
					//$("#carColorLocal").text(title);
					//this.element.attr("title", title);
					this.element.css({ 'backgroundColor': this.color, 'color': this.color });
					//}
				}
			});

			$("#modal-section").modal("show");
		},
		remove: function (section) {

		},
		selectIcon: function (item) {
			this.section.cssIcon = item.nameClass;
			this.searchIcon = '';
		},
		save: function () {
			if (this.section.name.length > 0) {
				this.section.areaID = this.areaID;

				sendAjax("/Section/SaveSection", this.section, "POST")
					.then(function (result) {
						if (result.isOk) {
							SectionVue.section = result.section;
							if (SectionVue.sections.findIndex(x => x.id == result.section.id) == -1) {
								SectionVue.sections.push(result.section);
							}
							$("#modal-section").modal("hide");

						}
					});
			}
		}

	}
});