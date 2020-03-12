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
				"formula": [],
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
				this.column.formula.push("+");
				this.column.formula.push("sectionID=" + sectionID);
			} else {
				this.column.formula.push("sectionID=" + sectionID);
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
				if (this.template.columns[columnIndex].formula) {
					FormulaVue.formula = this.template.columns[columnIndex].formula;
				}
				FormulaVue.init();
			}
		}
	}
});


var FormulaVue = new Vue({
	el: "#modal-formula",
	data: {
		fields: [],
		formula: [],
		items: [],

		number: ["0", "1", "2", "3", "4", "5", "6", "7", "8", "9"],
		mark: ["*", "/", "-", "+"],
		parentheses: ["(", ")"],

		isValid: null,
	},
	computed: {
	},
	mounted: function () {
	},
	methods: {
		init: function () {
			//for (var i = 0; i < this.fields.length; i++) {
			//	this.formula.push(this.fields[i].sectionID + "_" + this.fields[i].codeName);
			//}
			this.refreshFormula();

			$("#modal-formula").modal("show");
		},
		removeLast: function () {
			this.formula.pop();
			this.refreshFormula();
		},
		add: function (value) {
			this.formula.push(value.target.innerHTML);
			this.refreshFormula();
		},
		addField: function (field) {
			this.formula.push("sectinoID=" + field.sectinoID);
			this.refreshFormula();
		},
		refreshFormula: function () {
			var $el = $('#formula');

			$el.tagsinput({
				cancelConfirmKeysOnEmpty: true,
				tagClass: function (item) {
					switch (item.css) {
						case 'number': return 'badge badge-primary';
						case 'mark': return 'badge badge-danger';
						case 'parentheses': return 'badge badge-success';
						case 'section': return 'badge badge-default';
						//case 'Asia': return 'badge badge-warning';
					}
				},

				itemValue: 'value',
				itemText: 'text',
			});

			for (var i = 0; i < this.formula.length; i++) {
				this.items.push(this.understandValue(this.formula[i]));
				$el.tagsinput('add', this.items[i]);
			}
		},
		understandValue: function (value) {
			let el = { value: -1, text: "", css: "" };

			if (value.indexOf("sectionID=") >= 0) {
				let sectionID = value.replace("sectionID=", '');
				let text = this.fields.find(x => x.sectionID == sectionID).name;

				el.value = value;
				el.text = text;
				el.css = "section";

				return el;
			}

			if (this.mark.indexOf(value) >= 0) {
				el.value = value;
				el.text = value;
				el.css = "mark";

				return el;
			}

			if (this.number.indexOf(value) >= 0) {
				el.value = value;
				el.text = value;
				el.css = "number";

				return el;
			}

			if (this.parentheses.indexOf(value) >= 0) {
				el.value = value;
				el.text = value;
				el.css = "parentheses";

				return el;
			}


		}
	}
});

Vue.config.devtools = true;
