var TemplateVue = new Vue({
    el: "#template-columns",
    data: {
        template: [],
        sections: [],
        selectedSelections: [],

        counterNewColumn: -100,
        counterTemplateBudgetSections: -100,

        column: {},

        periodType: -1,
        isSavingTemplate: false,
        errorMessage: null,
        startColumnsName: "Новая колонка",

        footerActions: [],
    },
    watch: {
        "template.name": function (newValue, oldValue) {

        },
        "template.columns": function () {
            this.selectedSelections = [];
            let selections = [];

            for (var columnsIndex = 0; columnsIndex < this.template.columns.length; columnsIndex++) {
                for (var sectionIndex = 0; sectionIndex < this.template.columns[columnsIndex].templateBudgetSections.length; sectionIndex++) {
                    selections.push(this.template.columns[columnsIndex].templateBudgetSections[sectionIndex].sectionID);
                }
            }

            for (var i = 0; i < selections.length; i++) {
                let filtered = this.selectedSelections.filter(x => x.id == selections[i]);
                if (filtered.length == 0) {
                    this.selectedSelections.push({ id: selections[i], count: selections.filter(x => x == selections[i]).length });
                }
            }
        }
    },
    computed: {
        sectionComponent: function () {
            return this.$children[0];
        },
    },
    mounted: function () {
        this.init()
            .then(function () {
                TemplateVue.loadBAranAndRType();

                sendAjax("/Common/GetFoolterAction", null, "GET")
                    .then(function (result) {
                        if (result.isOk == true) {
                            TemplateVue.footerActions = result.data;
                        }
                    });
            });
    },
    methods: {

        init: function () {
            let templateID = $("#templateID").val();
            return sendAjax("/Template/GetTemplate/" + templateID, null, "GET")
                .then(function (result) {
                    if (result.isOk == true) {
                        TemplateVue.template = result.template;

                        $("#templateName").val(result.template.name);
                        TemplateVue.refreshDragNDrop();
                    }
                });
        },
        loadBAranAndRType: function () {
            return sendAjax("/Section/GetAllAreaAndSectionByPerson", null, "GET")
                .then(function (result) {
                    if (result.isOk == true) {
                        TemplateVue.sections = result.areas;

                    }
                });
        },
        onChooseSection: function (section) {

        },
        refreshDragNDrop: function () {
            dragula(
                Array.prototype.slice.call(document.querySelectorAll('.lists')),
                {
                    moves: function (el, source, handle, sibling) {
                        console.log(arguments);
                        return !el.classList.contains("ignore-dnd") || !handle.classList.contains("ignore-dnd");
                    },
                    accepts: function (el, target, source, sibling) {//?
                        console.log(arguments);
                        return !el.classList.contains("ignore-dnd");
                    },
                    //invalid
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
        addColumn: function () {

            $("#modalDataTypeColumn").modal("show");

            this.column = {
                id: this.counterNewColumn++,
                name: this.startColumnsName,
                order: this.template.columns.length,
                isShow: true,
                totalAction: 0,
                formula: [],
                templateBudgetSections: [],
                placeAfterCommon: 0,
                format: '',
            };
        },
        addColumn_Complete: function () {
            $("#modalDataTypeColumn").modal("hide");
            this.template.columns.push(this.column);

            if (TemplateColumnTypeEnum.BudgetSection == this.column.templateColumnType) {
                this.column.totalAction = 1;
                this.addColumnOption_step1(this.column);
            } if (TemplateColumnTypeEnum.DaysForMonth == this.column.templateColumnType) {
                this.column.name = "Дни";
                this.column.format = "dd";
            } if (TemplateColumnTypeEnum.MonthsForYear == this.column.templateColumnType) {
                this.column.name = "Месяц";
            }
        },
        addColumnOption_step1: function (column) {
            this.column = column;
            $('.selectpicker').selectpicker("destroy").selectpicker('refresh');
            $("#section-modal").modal("show");
        },
        addColumnOption_step2: function (section) {

            if (this.column.formula.length > 0) {
                this.column.formula.push({ id: null, value: "+", type: FormulaFieldTypeEnum.Mark });
                this.column.formula.push({ id: section.id, value: `[ ${section.name} ]`, type: FormulaFieldTypeEnum.Section });
            } else {
                this.column.formula.push({ id: section.id, value: `[ ${section.name} ]`, type: FormulaFieldTypeEnum.Section });
            }

            if (this.column.name == this.startColumnsName) {
                this.column.name = section.name;
            }


            this.column.templateBudgetSections.push({
                id: this.counterTemplateBudgetSections++,
                sectionID: section.id,
                sectionName: section.name,
            });

            //work with vue-section-component
            let selectedSelectionsIndex = this.selectedSelections.findIndex(x => x.id == section.id);
            if (selectedSelectionsIndex != -1) {
                this.selectedSelections[selectedSelectionsIndex].count = this.selectedSelections[selectedSelectionsIndex].count + 1;
            } else {
                this.selectedSelections.push({ id: section.id, count: 1 });
            }

            $("#section-modal").modal("hide");
        },
        saveTemplate: function (saveAs) {
            let method = 'Save';
            if (saveAs == true) {
                method = 'SaveAs';
            }
            this.isSavingTemplate = true;
            return sendAjax("/Template/" + method, this.template, "POST")
                .then(function (result) {
                    if (result.isOk == true) {
                        TemplateVue.template = result.template;
                        TemplateVue.errorMessage = null;
                    } else {
                        if (result.nameAlreadyExist) {
                            //Show message
                            TemplateVue.errorMessage = result.errorMessage;
                        }
                    }
                    TemplateVue.isSavingTemplate = false;
                });
        },
        change: function (event) {
            console.log(event);
        },
        openFormula: function (columnID) {
            //FormulaVue.el.tagsinput("destroy");
            FormulaVue.fields = [];
            FormData.formula = [];
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

        },
        removeSectionInColumn: function (templateBudgetSection, indexSection, indexColumn) {

            let column = this.template.columns[indexColumn];

            //work with vue-section-component
            let section = column.templateBudgetSections[indexSection];
            let selectedSelectionsIndex = this.selectedSelections.findIndex(x => x.id == section.sectionID);
            if (selectedSelectionsIndex != -1) {
                if (this.selectedSelections[selectedSelectionsIndex].count == 1) {
                    this.selectedSelections.splice(selectedSelectionsIndex, 1);
                } else {
                    this.selectedSelections[selectedSelectionsIndex].count = this.selectedSelections[selectedSelectionsIndex].count - 1;
                }
            }

            column.templateBudgetSections.splice(indexSection, 1);
            column.formula = [];

            for (var i = 0; i < column.templateBudgetSections.length; i++) {
                let section = column.templateBudgetSections[i];
                if (column.formula.length > 0) {
                    column.formula.push({ id: null, value: "+", type: FormulaFieldTypeEnum.Mark });
                    column.formula.push({ id: section.sectionID, value: `[ ${section.sectionName} ]`, type: FormulaFieldTypeEnum.Section });
                } else {
                    column.formula.push({ id: section.sectionID, value: `[ ${section.sectionName} ]`, type: FormulaFieldTypeEnum.Section });
                }
            }


            //let foundIndex = true;
            //while (foundIndex) {

            //    let formulaIndex = column.formula.findIndex(x => x.id == section.sectionID);
            //    if (formulaIndex >= 0) {
            //        if (formulaIndex == 0) {
            //            //ToDo
            //        }
            //        column.formula.slice(formulaIndex, 1);
            //    } else {
            //        foundIndex = false;
            //    }
            //}
        },
        getLinkForView: function (template) {
            return GetLinkForView(template);
        },

    }
});


var FormulaVue = new Vue({
    el: "#modal-formula",
    data: {
        fields: [],
        formula: [],

        isValid: null,
        el: Object,
        columnID: null,
    },
    watch: {},
    computed: {},
    mounted: function () {
        this.el = $('#formula');

        this.el.tagsinput({
            //cancelConfirmKeysOnEmpty: true,
            allowDuplicates: true,
            tagClass: function (item) {
                switch (item.type) {
                    case FormulaFieldTypeEnum.Number: return 'badge badge-primary';
                    case FormulaFieldTypeEnum.Mark: return 'badge badge-success';
                    case FormulaFieldTypeEnum.Parentheses: return 'badge badge-success';
                    case FormulaFieldTypeEnum.Section: return 'badge badge-default';
                    //case 'Asia': return 'badge badge-warning';
                }
            },
            itemValue: 'value',
            itemText: 'value',
            trimValue: true
        });
        this.el.on('beforeItemRemove', function (event) {
            var tag = event.item;

            let index = FormulaVue.formula.findIndex(x => x == tag);
            console.log(index);
            if (index >= 0) {
                FormulaVue.formula.splice(index, 1);

            }
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
            //let lastItem = this.formula.pop();
            //console.log(lastItem);

            this.el.tagsinput("remove", this.formula.pop());
        },
        removeAll: function () {
            this.el.tagsinput('removeAll');
            this.formula = [];
        },
        add: function (event, type) {
            //if (type == FormulaFieldTypeEnum.Number) {

            //    var lastFormulaItem = this.formula[this.formula.length - 1];

            //    if (lastFormulaItem.type == FormulaFieldTypeEnum.Number) {

            //        this.el.tagsinput("remove", lastFormulaItem);
            //        lastFormulaItem.value += event.target.innerHTML;
            //        this.el.tagsinput('add', lastFormulaItem);
            //        return;

            //    }
            //}
            let formulaElement = { id: null, value: event.target.value, type: type };
            this.formula.push(formulaElement);
            this.el.tagsinput('add', formulaElement);
        },
        addField: function (field) {
            let formulaElement = { id: field.sectionID, value: `[ ${field.name} ]`, type: FormulaFieldTypeEnum.Section };

            //add mark "+" if last item in the formula is section or days
            if (this.formula && this.formula.length > 0) {
                let lastFormulaElement = this.formula[this.formula.length - 1];
                if (lastFormulaElement.type == FormulaFieldTypeEnum.Section || lastFormulaElement.type == FormulaFieldTypeEnum.Days) {

                    let formulaElement = { id: null, value: "+", type: FormulaFieldTypeEnum.Mark };
                    this.formula.push(formulaElement);
                    this.el.tagsinput('add', formulaElement);
                }
            }

            this.formula.push(formulaElement);
            this.el.tagsinput('add', formulaElement);
        },
        addPeriod: function (formulaFieldTypeEnumID) {
            if (formulaFieldTypeEnumID == FormulaFieldTypeEnum.Days) {
                let formulaElement = { id: null, value: "[ День ]", type: formulaFieldTypeEnumID };
                this.formula.push(formulaElement);
                this.el.tagsinput('add', formulaElement);
            }
        },
        refreshFormula: function () {
            this.el.tagsinput('removeAll');

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
        },
        isUsing: function (field) {
            return this.formula.findIndex(x => x.id == field.sectionID) >= 0;
        },
    }
});


