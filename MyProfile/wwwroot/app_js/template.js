var TemplateVue = new Vue({
	el: "#template-columns",
	data: {
		template: [],
		sections: [],

		counterNewColumn: -100,
		counterTemplateBudgetSections: -100,

		column: {},

		periodType: -1,
		isSaveTemplate: false,

		footerActions: []
	},
	watch: {
		"template.name": function (newValue, oldValue) {

		},
	},
	mounted: function () {
		this.init()
			.then(function () {
				TemplateVue.loadBAranAndRType();

				sendAjax("/Common/GetFoolterAction", null, "GET")
					.then(function (result) {
						if (result.isOk = true) {
							TemplateVue.footerActions = result.data;
						}
					});
			});
	},
	methods: {

		init: function () {
			let templateID = document.getElementById("templateID").value;
			return sendAjax("/Template/GetTemplate/" + templateID, null, "GET")
				.then(function (result) {
					if (result.isOk = true) {
						TemplateVue.template = result.template;
						$("#templateName").val(result.template.name);
						TemplateVue.refreshDragNDrop();
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
		refreshDragNDrop: function () {
			dragula(
				Array.prototype.slice.call(document.querySelectorAll('.lists')),
				{
					moves: function (el) {
						return !el.classList.contains("ignore-dnd");
					},
					accepts: function (el) {//?
						return !el.classList.contains("ignore-dnd");
					},
				}
			).on('dragend', function (el) {
				let el_s = document.querySelectorAll(".list[columnid]");

				for (var i = 0; i < el_s.length; i++) {
					let columnID = el_s[i].attributes["columnid"].value * 1;
					if (columnID) {
						TemplateVue.template.columns.find(x => x.id == columnID).order = i;
					}
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
				"id": this.counterNewColumn++,
				"name": "New column",
				"order": this.template.columns.length,
				"isShow": true,
				"totalAction": 0,
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
				this.column.formula.push({ id: null, value: "+", type: FormulaFieldTypeEnum.Mark });
				this.column.formula.push({ id: sectionID, value: sectionName, type: FormulaFieldTypeEnum.Section });
			} else {
				this.column.formula.push({ id: sectionID, value: sectionName, type: FormulaFieldTypeEnum.Section });
			}

			this.column.templateBudgetSections.push({
				id: this.counterTemplateBudgetSections++,
				sectionID: sectionID,
				sectionName: sectionName
			});
			$("#modals-slide").modal("hide");
		},
		saveTemplate: function () {
			this.isSaveTemplate = true;
			return sendAjax("/Template/Save", this.template, "POST")
				.then(function (result) {
					if (result.isOk = true) {
						TemplateVue.template = result.template;
					}
					TemplateVue.isSaveTemplate = false;
				});
		},
		change: function (event) {
			console.log(event);
		},
		openFormula: function (columnID) {
			//FormulaVue.el.tagsinput("destroy");
			FormulaVue.columnID = columnID;
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
					FormulaVue.formula = [... this.template.columns[columnIndex].formula];
				}
				FormulaVue.init();
			}
		},
		setNewFormula: function (newFormula, columnID) {
			let columnIndex = this.template.columns.findIndex(x => x.id == columnID);
			if (columnIndex >= 0) {
				this.template.columns[columnIndex].formula = [...newFormula];
			}
		},
		GetDateByFormat: function (date, format) {
			return GetDateByFormat(date, format);
		},
		isBudgetSection: function (templateColumnType) {
			return templateColumnType == TemplateColumnTypeEnum.BudgetSection;
		},
		isDaysForMonth: function (templateColumnType) {
			return templateColumnType == TemplateColumnTypeEnum.DaysForMonth;
		},
		isMonthsForYear: function (templateColumnType) {
			return templateColumnType == TemplateColumnTypeEnum.MonthsForYear;
		},
		getFooterActionTypeValue: function (footerActionTypeEnum) {

		}

	}
});


var FormulaVue = new Vue({
	el: "#modal-formula",
	data: {
		fields: [],
		formula: [],

		number: ["0", "1", "2", "3", "4", "5", "6", "7", "8", "9"],
		mark: ["*", "/", "-", "+"],
		parentheses: ["(", ")"],

		isValid: null,
		el: Object,
		columnID: null,
	},
	watch: {
		formula: function (newValue, oldValue) {
			console.log("refresh");

		},
	},
	computed: {
	},
	mounted: function () {
		this.el = $('#formula');

		this.el.tagsinput({
			//cancelConfirmKeysOnEmpty: true,
			allowDuplicates: true,
			tagClass: function (item) {
				switch (item.type) {
					case FormulaFieldTypeEnum.Number: return 'badge badge-primary';
					case FormulaFieldTypeEnum.Mark: return 'badge badge-danger';
					case FormulaFieldTypeEnum.Parentheses: return 'badge badge-success';
					case FormulaFieldTypeEnum.Section: return 'badge badge-default';
					//case 'Asia': return 'badge badge-warning';
				}
			},

			itemValue: 'value',
			itemText: 'value',
		});
	},
	methods: {
		init: function () {
			this.refreshFormula();
			$("#modal-formula").modal("show");
			//$("#modal-formula").modal("hidden.bs.modal", function () {
			//	FormulaVue.fields = [];
			//	FormulaVue.formula = [];
			//});
		},

		removeLast: function () {
			this.el.tagsinput("remove", this.formula.pop());
		},
		add: function (event, type) {
			if (type == FormulaFieldTypeEnum.Number) {

				var lastFormulaItem = this.formula[this.formula.length - 1];

				if (lastFormulaItem.type == FormulaFieldTypeEnum.Number) {

					this.el.tagsinput("remove", lastFormulaItem);
					lastFormulaItem.value += event.target.innerHTML;
					this.el.tagsinput('add', lastFormulaItem);
					return;

				}
			}
			let formulaElement = { id: null, value: event.target.innerHTML, type: type };
			this.formula.push(formulaElement);
			this.el.tagsinput('add', formulaElement);
		},
		addField: function (field) {
			let formulaElement = { id: field.sectinoID, value: field.name, type: FormulaFieldTypeEnum.Section };
			this.formula.push(formulaElement);
			this.el.tagsinput('add', formulaElement);
		},
		addPeriod: function (formulaFieldTypeEnumID) {

			if (formulaFieldTypeEnumID == FormulaFieldTypeEnum.Days) {
				let formulaElement = { id: null, value: "Days", type: formulaFieldTypeEnumID };
				this.formula.push(formulaElement);
				this.el.tagsinput('add', formulaElement);
			}
		},
		refreshFormula: function () {
			//this.el.tagsinput('destroy');

			for (var i = 0; i < this.formula.length; i++) {
				this.el.tagsinput('add', this.formula[i]);
			}
		},
		save: function () {
			TemplateVue.setNewFormula(this.formula, this.columnID);
			$("#modal-formula").modal('hide');
			this.fields = [];
			this.formula = [];
			this.el.tagsinput('removeAll');
		}
	}
});

Vue.config.devtools = true;
