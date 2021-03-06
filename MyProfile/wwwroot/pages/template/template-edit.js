var TemplateVue = new Vue({
    el: "#template-columns",
    data: {
        template: {},
        sections: [],
        selectedSelections: [],
        sectionSource: [],

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
                //TemplateVue.loadBAranAndRType();

                //sendAjax("/Common/GetFooterAction", null, "GET")
                //    .then(function (result) {
                //        if (result.isOk == true) {
                //            TemplateVue.footerActions = result.data;
                //        }
                //    });
            });
    },
    methods: {

        init: function () {
            let templateID = $("#templateID").val();
            return sendAjax("/Template/GetTemplate/" + templateID, null, "GET")
                .then(function (result) {
                    if (result.isOk == true) {
                        TemplateVue.template = result.template;

                        if (TemplateVue.template.id == 0 && parseQueryString()["periodTypeId"] != undefined) {
                            TemplateVue.template.periodTypeID = parseQueryString()["periodTypeId"] * 1;
                            if (TemplateVue.template.periodTypeID == PeriodTypeEnum.Month) {
                                TemplateVue.template.periodName = "Финансы на месяц";
                            } else if (TemplateVue.template.periodTypeID == PeriodTypeEnum.Year) {
                                TemplateVue.template.periodName = "Финансы на год";
                            }
                        }

                        $("#templateName").val(result.template.name);
                        TemplateVue.refreshDragNDrop();
                    }
                });
        },
        //loadBAranAndRType: function () {
        //    return sendAjax("/Section/GetAllAreaAndSectionByPerson", null, "GET")
        //        .then(function (result) {
        //            if (result.isOk == true) {
        //                TemplateVue.sections = result.areas;

        //            }
        //        });
        //},
        refreshDragNDrop: function () {
            var drake = dragula(
                Array.prototype.slice.call(document.querySelectorAll('.lists')),
                {
                    moves: function (el, container, handle) {
                        return handle.classList.contains('kanban-box');
                    },
                    //moves: function (el, source, handle, sibling) {
                    //    return !el.classList.contains("ignore-dnd") || !handle.classList.contains("ignore-dnd");
                    //},
                    //accepts: function (el, target, source, sibling) {//?
                    //    return !el.classList.contains("ignore-dnd");
                    //},
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
                $(".list+.ignore-dnd").insertAfter(".lists .list:last");
            });

            var scroll = autoScroll([
                window,
                document.querySelector('.lists'),
            ], {
                margin: 100,
                autoScroll: function () {
                    return this.down && drake.dragging;
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
                this.column.format = "MM.yyyy";
            }
        },
        addColumnOption_step1: function (column) {
            this.column = column;
            $('.selectpicker').selectpicker("destroy").selectpicker('refresh');
            $("#section-modal").modal("show");

            let sections = this.sectionComponent.sections;
            for (var i = 0; i < sections.length; i++) {
                sections[i].isSelected = column.templateBudgetSections.findIndex(x => x.sectionID == sections[i].id) != -1;
            }
        },
        addColumnOption_step2: function (section) {

            let indexSetionInColumn = this.column.templateBudgetSections.findIndex(x => x.sectionID == section.id);
            if (indexSetionInColumn >= 0) {
                this.removeSectionInColumn(null, indexSetionInColumn, this.column.order);
                return;
            }

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

            //$("#section-modal").modal("hide");
        },
        changeType: function () {
            this.template.columns = [];
        },
        saveTemplate: function (saveAs, saveAndGoToView) {
            if (this.validTemplate() == false) {
                toastr.error("Не удалось сохранить шаблон");
                return false;
            }

            let method = 'Save';
            if (saveAs == true) {
                method = 'SaveAs';
            }
            this.isSavingTemplate = true;
            this._saveAndGoToView = saveAndGoToView;

            return $.ajax({
                type: "POST",
                url: "/Template/" + method,
                context: this,
                data: JSON.stringify(this.template),
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (result) {
                    if (result.isOk) {
                        toastr.success("Шаблон сохранен успешно");
                        this.template = result.template;
                        this.errorMessage = null;

                        if (this._saveAndGoToView) {
                            window.document.location.href = TemplateGetLinkForView(this.template);
                        }
                    } else {
                        if (result.nameAlreadyExist) {
                            //Show message
                            toastr.error("Не удалось сохранить шаблон");
                        }
                        this.errorMessage = result.errorMessage;
                    }
                    this.isSavingTemplate = false;
                    return true;
                },
                error: function (xhr, status, error) {
                    this.isSaving = false;
                    console.log(error);
                }
            });
        },
        validTemplate: function () {
            let isOk = true;
            this.errorMessage = null;

            let str = this.template.name;
            str = str ? str.replaceAll(" ", "") : "";
            if (str.length == 0) {
                isOk = false;
                $("#template-name").addClass("is-invalid");
            } else {
                $("#template-name").removeClass("is-invalid");
            }

            if (this.template.columns.length == 0) {
                isOk = false;
                this.errorMessage = "Шаблон должен содержать хотя бы одну колонку.";
            }

            if (this.template.columns.length > 0
                && this.template.columns.findIndex(x => x.templateColumnType == TemplateColumnTypeEnum.BudgetSection && x.templateBudgetSections.length == 0) > -1) {
                isOk = false;
                this.errorMessage = "Шаблон должен содержать колонки хотя бы с одной категорией.";
            }

            if (this.template.columns.length > 0
                && this.template.columns.findIndex(x => x.name.length == 0) > -1) {
                isOk = false;
                this.errorMessage = "Название у колонок обязательно.";
            }

            return isOk;
        },
        saveAndGoToView: function () {
            this.saveTemplate(false, true);
        },
        change: function (event) {

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


$(function () {

    // Drag&Drop

    dragula(
        Array.prototype.slice.call(document.querySelectorAll('.kanban-box'))
    );

    // RTL

    if ($('html').attr('dir') === 'rtl') {
        $('.kanban-board-actions .dropdown-menu').removeClass('dropdown-menu-right');
    }

});

