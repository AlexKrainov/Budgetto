var TemplateVue = new Vue({
	el: "#template-columns",
	data: {
		template: [],
		sections: [],

		counterColumn: -100,
		counterTemplateBudgetSections: -100,

		column: {},


	},
	watch: {
		"template.name": function (newValue, oldValue) {

		},
	},
	mounted: function () {
		this.init()
			.then(function () {
				TemplateVue.loadBAranAndRType();
			});
	},
	methods: {
		init: function () {
			return sendAjax("/Template/GetData/3", null, "GET")
				.then(function (result) {
					if (result.isOk = true) {
						TemplateVue.template = result.template;
						$("#templateName").val(result.template.name);
					}
				});
		},
		loadBAranAndRType: function () {
			return sendAjax("/Section/GetAllSectionByPerson", null, "GET")
				.then(function (result) {
					if (result.isOk = true) {
						TemplateVue.sections = result.sections;

					}
				});
		},
		removeColumn: function (columnID) {
			let columnIndex = this.template.columns.findIndex(x => x.id == columnID);
			if (columnIndex) {
				this.template.columns.splice(columnIndex, 1);
			}
		},
		addColumn: function () {

			$("#modalDataTypeColumn").modal("show");

			this.column = {
				"id": this.counterColumn++,
				"name": "New column",
				"order": this.template.columns.length,
				"isShow": true,
				"totalAction": 1,
				"formula": "",
				templateBudgetSections: []
			};
		},
		addColumn_Complete: function () {
			$("#modalDataTypeColumn").modal("hide");
			this.template.columns.push(this.column);
			//$("input[name=data-column-type]:selected").removeProp("selected");
			this.column = {};
		},
		addColumnOption_step1: function (column) {
			this.column = column;
			$('.selectpicker').selectpicker("destroy").selectpicker('refresh');
			$("#modals-slide").modal("show");
		},
		addColumnOption_step2: function () {

			let sectionName = $("#budgetArea option:selected").text();
			let sectionID = $("#budgetArea").val();

			if (this.column.templateBudgetSections
				.find(x => x.sectionName == sectionName || x.sectionID == sectionID) != undefined) {
				$("#budgetArea")
					.next()
					.addClass("form-control is-invalid")
					.after('<div class="invalid-tooltip">Please provide a valid state.</div>');

				return;
			} else {
				let $divInvalidTooltip = $("#budgetArea")
					.next()
					.removeClass("form-control is-invalid")
					.next();

				if ($divInvalidTooltip.prop("class").indexOf("is-invalid") > 0) {
					$divInvalidTooltip.remove();
				}
			}

			if (this.column.formula.length > 0) {
				this.column.formula += " + <span class='badge badge-secondary'>" + sectionName + "</span>";
			} else {
				this.column.formula += "<span class='badge badge-secondary'>" + sectionName + "</span>";
			}

			this.column.templateBudgetSections.push({
				id: this.counterTemplateBudgetSections++,
				sectionID: sectionID,
				sectionName: sectionName
			});
			$("#modals-slide").modal("hide");
		},
		saveTemplate: function () {
			return sendAjax("/Template/Save", this.template, "POST")
				.then(function (result) {
					if (result.isOk = true) {
						TemplateVue.template = result.template;
					}
				});
		},
		openFormula: function (columnID) {
			let columnIndex = this.template.columns.findIndex(x => x.id == columnID);
			if (columnIndex >= 0) {
				for (var i = 0; i < this.template.columns[columnIndex].templateBudgetSections.length; i++) {
					FormulaVue.fields.push({
						sectionID: this.template.columns[columnIndex].templateBudgetSections[i].sectionID,
						codeName: this.template.columns[columnIndex].templateBudgetSections[i].sectionCodeName,
						name: this.template.columns[columnIndex].templateBudgetSections[i].sectionName
					});
				}

				//FormulaVue.formula = this.template.columns[columnIndex].formula;
				$("#modal-formula").modal("show");
			}
		}
	}
});


var FormulaVue = new Vue({
	el: "#modal-formula",
	data: {
		fields: [],
		formula: [],

		number: [],
		mark: [],
		parentheses: ["(", ")"],

		isValid: null,
	},
	computed: {
	},
	mounted: function () {
	},
	methods: {
		addField: function (field) {

		},
		refreshFormula: function () {
			var $el = $('#formula');

			$el.tagsinput({
				tagClass: function (item) {
					switch (item.continent) {
						case 'number': return 'badge badge-primary';
						case 'mark': return 'badge badge-danger';
						case 'parentheses': return 'badge badge-success';
						//case 'Africa': return 'badge badge-default';
						//case 'Asia': return 'badge badge-warning';
					}
				},

				itemValue: 'value',
				itemText: 'text',
			});

			$el.tagsinput('add', { value: 1, text: 'Amsterdam', continent: 'number' });
			$el.tagsinput('add', { value: 4, text: 'Washington', continent: 'mark' });
			$el.tagsinput('add', { value: 7, text: 'Sydney', continent: 'parentheses' });
			$el.tagsinput('add', { value: 10, text: 'Beijing', continent: 'number' });
			$el.tagsinput('add', { value: 13, text: 'Cairo', continent: 'mark' });
			$el.tagsinput('add', { value: 11, text: 'Amsterdam', continent: 'number' });
			$el.tagsinput('add', { value: 41, text: 'Washington', continent: 'mark' });
			$el.tagsinput('add', { value: 71, text: 'Sydney', continent: 'parentheses' });
			$el.tagsinput('add', { value: 110, text: 'Beijing', continent: 'number' });
			$el.tagsinput('add', { value: 131, text: 'Cairo', continent: 'mark' });
		}
	}
});

Vue.config.devtools = true;
