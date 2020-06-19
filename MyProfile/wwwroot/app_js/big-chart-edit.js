var ChartEditVue = new Vue({
	el: "#big-chart-edit-vue",
	data: {
		chart: {
			chartTypeName: '',
			fields: []
		},
		//metadata
		sections: [],
		fieldsCount: -789,
		isSaving: false,
	},
	watch: {},
	mounted: function () {
		let elWizard = $('#big-chart-edit-vue').smartWizard({
			autoAdjustHeight: false,
			backButtonSupport: false,
			useURLhash: false,
			showStepURLhash: false,
			lang: { // Language variables for button
				next: 'Вперед',
				previous: 'Назад'
			},
		});
		elWizard.on("leaveStep", function (e, anchorObject, stepIndex, stepDirection) {
			let canGo = true;

			if (stepIndex == 1) { //check for empty fields
				if (chart.fields.length == 0) {

				}
			}

			return canGo;
		});

		console.log(elWizard);

		this.loadSections();
	},
	methods: {
		loadSections: function () {
			return sendAjax("/Section/GetAllSectionByPerson", null, "GET")
				.then(function (result) {
					if (result.isOk = true) {
						ChartEditVue.sections = result.sections;
					}
				});
		},
		selectChart: function (chartTypeID, chartTypeName, event) {
			let charts = document.querySelectorAll(".chart-list-item");
			for (var i = 0; i < charts.length; i++) {
				charts[i].className = 'chart-list-item';// .classList.remove("active");
			}

			event.currentTarget.classList.add("acitve");
			this.chart.chartTypeID = chartTypeID;
			this.chart.chartTypeName = chartTypeName;

			setTimeout(function () {
				$('#big-chart-edit-vue').smartWizard("next");
			}, 1000);
		},
		addField: function () {
			this.fieldsCount += 1;
			this.chart.fields.push({ id: this.fieldsCount, name: "", cssColor: "" });

			//init control
			setTimeout(function () {

				let el_name = "#chart_section_" + ChartEditVue.fieldsCount;
				console.log(el_name);
				$(el_name).select2({ placeholder: 'Выбор категорий' });

				el_name = "#chart_color_" + ChartEditVue.fieldsCount;
				$(el_name).colorPick({
					'initialColor': GetRandomColor(),
					'palette': PaletteColorPicker,
					//'localizationColor': [],
					'onColorSelected': function () {
						let id = this.element[0].id.replace("chart_color_", "");
						var index = ChartEditVue.chart.fields.findIndex(x => x.id == id); // .cssColor = this.color;
						if (index >= 0) {
							ChartEditVue.chart.fields[index].cssColor = this.color;
						}
						this.element.css({ 'backgroundColor': this.color, 'color': this.color });
					}
				});
			}, 10);
		},
		save: function () {
			this.isSaving = true;

			try {
				for (var i = 0; i < this.chart.fields.length; i++) {
					let el_name = "#chart_section_" + this.chart.fields[i].id;
					this.chart.fields[i].sections = $(el_name).select2("val");
				}

				return sendAjax("/Chart/Save", this.chart, "POST", this)
					.then(function (result) {
						if (result.isOk == true) {
							this.chart = result.chart;
						} else {
							console.log(result.message);
						}
						this.isSaving = false;
					});

			} catch (e) {
				this.isSaving = false;
			}
		},
		removeField: function (field) {
			let indexField = this.chart.fields.findIndex(x => x.id == field.id);
			this.chart.fields.splice(indexField, 1);
		}
	}
});

Vue.config.devtools = true;
