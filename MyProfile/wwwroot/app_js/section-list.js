var AreaVue = new Vue({
	el: "#area-vue",
	data: {
		areas: [],
		area: {},

		searchText: null
	},
	watch: {
		searchText: function (newValue, oldValue) {
			if (newValue) {
				newValue = newValue.toLocaleLowerCase();
			}

			for (var i = 0; i < this.areas.length; i++) {
				this.areas[i].isShow = this.areas[i].name.toLocaleLowerCase().indexOf(newValue) >= 0;
			}
		}
	},
	mounted: function () {
		this.init();
	},
	methods: {
		init: function () {
			sendAjax("/Section/GetAllSectionForEdit", null, "GET")
				.then(function (result) {
					if (result.isOk) {
						AreaVue.areas = result.areas;
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
			SectionVue.areaCanEdit = area.canEdit;

			SectionVue.areas = this.areas.map(function (x) { return { name: x.name, id: x.id } });
		},
		create: function () {
			this.area = {
				name: this.searchText,
			};
			$("#modal-area").modal("show");
		},
		edit: function (area) {
			this.area = area;
			$("#collectiveArea").select2();
			
			$("#modal-area").modal("show");
		},
		save: function () {
			if (this.area.name.length > 0) {
				this.area.collectiveAreas = $("#collectiveArea").select2("val").map(function (x) { return { id: x } });
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
			if (!area.isGlobal) {
				sendAjax("/Section/RemoveArea?id=" + area.id, null, "DELETE")
					.then(function (result) {
						if (result.isOk && result.wasDeleted) {
							toastr.success(result.text);
							let index = AreaVue.areas.findIndex(x => x.id == result.id);
							if (index && index > 0) {
								AreaVue.areas.splice(index, 1);
							}
						} else if (result.isOk && result.wasDeleted == false) {
							toastr.warning(result.text);
						} else {
							toastr.error(result.text);
						}
					});
			}
		},
	}
});

var SectionVue = new Vue({
	el: "#section-vue",
	data: {
		sections: [],
		section: {},

		areaID: null,
		areaCanEdit: true,
		areas: [], //for select

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
				areaID: this.areaID
			};

			$("#cssColor").colorPick({
				'initialColor': "#E5E9EB",
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
		edit: function (section) {
			this.section = section;
			$("#collectiveSections").select2();

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
			sendAjax("/Section/RemoveSection?id=" + section.id, null, "DELETE")
				.then(function (result) {
					if (result.isOk && result.wasDeleted) {
						toastr.success(result.text);
						let index = SectionVue.sections.findIndex(x => x.id == result.id);
						if (index && index > 0) {
							SectionVue.sections.splice(index, 1);
						}
					} else if (result.isOk && result.wasDeleted == false) {
						toastr.warning(result.text);
					} else {
						toastr.error(result.text);
					}
				});
		},
		selectIcon: function (item) {
			this.section.cssIcon = item.nameClass;
			this.searchIcon = '';
		},
		save: function () {
			if (this.section.name.length > 0) {
				this.section.collectiveSections = $("#collectiveSections").select2("val").map(function (x) { return { id: x } });
				sendAjax("/Section/SaveSection", this.section, "POST")
					.then(function (result) {
						if (result.isOk) {
							if (result.section.areaID == SectionVue.areaID) {
								SectionVue.section = result.section;
								if (SectionVue.sections.findIndex(x => x.id == result.section.id) == -1) {
									SectionVue.sections.push(result.section);
								}
							} else {
								let index = SectionVue.sections.findIndex(x => x.id == result.section.id)
								if (index >= 0) {
									SectionVue.sections.splice(index, 1);
									index = AreaVue.areas.findIndex(x => x.id == result.section.areaID);
									AreaVue.areas[index].sections.push(result.section);
								}
							}

							$("#modal-section").modal("hide");

						}
					});
			}
		},
		getMoney: function (money) {
			let result = numberOfThreeDigits(money);
			if (result == "") {
				return "0";
			}
			return result;
		},
		changeArea: function (event) {
			this.section.areaID = event.target.selectedOptions[0].value;
			this.section.areaName = event.target.selectedOptions[0].text;
		},
	}
});