var StartVue = new Vue({
    el: "#start-vue",
    data: {
        //1 
        userInfo: {
            name: "Alexey",
        },

        //2
        areas: [{ id: 0, name: "Расходы", codeName: "Spendings", sections: [] },
        { id: 1, name: "Доходы", codeName: "Earnings", sections: [] },
        { id: 2, name: "Инвестиции", codename: "Investments", sections: [] }], //2
        sectionSource: [],//2
        section: {}, //2
        userSectionSource: [],//2
        icons: [],//2
        colors: [],//2

        //3
        template: {
            columns: []
        },//3
        sections: [],//3
        column: {},//3
        startColumnsName: "Новая колонка",//3

        //4
        limit: {},
        limits: [], //4
        periodTypes: [{ codeName: "Days", id: 1, isUsing: false, name: "на месяц" }, { codeName: "Month", id: 3, isUsing: false, name: "на год" }],

        //5
        goal: {},
        goals: [],//5
        flatpickrStart: {},
        flatpickrEnd: {},

        isSaving: false,
        errorMessage: null,
        counter: -999,
        elWizard: {},
    },
    watch: {
    },
    computed: {
        sectionComponent_step2: function () {
            return this.$children[0];
        },
    },
    mounted: function () {
        this.elWizard = $('#start-vue').smartWizard({
            autoAdjustHeight: false,
            backButtonSupport: false,
            useURLhash: false,
            showStepURLhash: false,
            lang: { // Language variables for button
                next: 'Вперед',

            },
            toolbarSettings: {
                toolbarPosition: 'bottom', // none, top, bottom, both
                toolbarButtonPosition: 'center', // left, right, center
                showNextButton: true, // show/hide a Next button
                showPreviousButton: false, // show/hide a Previous button
                toolbarExtraButtons: [
                    $('<button type="button"></button>')
                        .text('Пропустить')
                        .addClass('btn btn-info')
                        .attr("v-on:click", "skip"),
                    $('<button type="button"></button>')
                        .text('Завершить')
                        .addClass('btn btn-info')
                        .attr("v-on:click", 'finish')
                ] // Extra buttons to show on toolbar, array of jQuery input/buttons elements
            },
        });
        this.elWizard.on("leaveStep", function (e, anchorObject, stepIndex, stepDirection) {
            if (stepDirection == "backward") {
                return false;
            }

            let canGo = true;
            StartVue.errorMessage = null;

            if (stepIndex == 0 && stepDirection == "forward") {

                //UserInfo
                if (StartVue.userInfo.name && StartVue.userInfo.name.length >= 2) {
                    $("#userInfoName").removeClass("is-invalid");
                    canGo = true;
                    StartVue.saveUserInfo();
                } else {
                    $("#userInfoName").addClass("is-invalid");
                    canGo = false;
                }
            }

            if (stepIndex == 1 && stepDirection == "forward") {//stepIndex == 1
                canGo = false;

                //Checking for any selected sections
                for (var i = 0; i < StartVue.areas.length; i++) {
                    if (StartVue.areas[i].sections.length > 0) {
                        canGo = true;
                        continue;
                    }
                }
                if (canGo) {
                    StartVue.saveSections();
                    StartVue.startStep3();
                } else {
                    StartVue.errorMessage = "Нужно выбрать хоты бы одну категорию.";
                }
            }

            if (stepIndex == 2 && stepDirection == "forward") {//stepIndex == 2
                canGo = false;

                if (StartVue.template.columns.length >= 2) {
                    StartVue.saveTemplate();
                    canGo = true;
                }
            }

            if (stepIndex == 3 && stepDirection == "forward") {//stepIndex == 3

                if (StartVue.limits.length > 0) {
                    StartVue.saveLimits();
                    canGo = true;
                }
            }

            if (stepIndex == 4 && stepDirection == "forward") {//stepIndex == 4

                if (StartVue.goals.length > 0) {
                    StartVue.saveGoals();
                    canGo = true;
                }
            }

            if (stepIndex == 5 && stepDirection == "forward") {//stepIndex == 4
                canGo = false;
            }

            return canGo;
        });

        //todo set timeline
        this.loadSections();
        // this.ONLY_FOR_TEST();

        $.getJSON("/json/font-awesome.json", function (json) {
            StartVue.icons = json;
        });
        $.getJSON("/json/colors-section.json", function (json) {
            StartVue.colors = json;
        });
        $("#section-description, #choose-area, #show-on-the-main-page, #limit-comment, #goal-comment, #gaol-is-finish").hide();
    },
    methods: {
        skip: function () {
            this.elWizard("next");
        },
        finish: function () {
            this.elWizard("next");
        },

        //1
        saveUserInfo: function () {
            this.isSaving = true;
            $("#userName").val(this.userInfo.name);

            return $.ajax({
                type: "POST",
                url: "/Start/SaveUserInfo",
                context: this,
                data: JSON.stringify(this.userInfo),
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (result) {
                    if (result.isOk) {
                        this.isSaving = false;
                        this.userInfo = result.user;
                        UserInfo = result.user;
                    }
                    return true;
                },
                error: function (xhr, status, error) {
                    this.isSaving = false;
                    console.log(error);
                }
            });
        },

        //2
        loadSections: function () {
            this.isSaving = true;
            return $.ajax({
                type: "GET",
                url: "/Start/LoadSections",
                context: this,
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (result) {
                    if (result.isOk) {
                        this.isSaving = false;
                        this.sectionSource = result.sections;
                        //this.userSectionSource = result.userSections;
                        ////ToDo: check if user have section and if a section is has codename
                        //for (var i = 0; i < this.userSectionSource.length; i++) {

                        //    if (this.userSectionSource[i].areaName = "Расходы") {
                        //        this.areas[0].sections.push(this.userSectionSource[i]);
                        //    } else if (this.userSectionSource[i].areaName = "Доходы") {
                        //        this.areas[1].sections.push(this.userSectionSource[i]);
                        //    } else if (this.userSectionSource[i].areaName = "Инвестиции") {
                        //        this.areas[2].sections.push(this.userSectionSource[i]);
                        //    }

                        //}
                    }
                    return true;
                },
                error: function (xhr, status, error) {
                    this.isSaving = false;
                    console.log(error);
                }
            });
        },
        onChooseSection: function (section) {
            if (section.isSelected) {
                this.areas[section.areaID].sections.push(section);
            } else {
                this.areas[section.areaID].sections.splice(this.areas[section.areaID].sections.indexOf(x => x.id == section.id), 1);
            }
        },
        selectAllSections: function () {
            this.sectionComponent_step2.selectAllSections();
        },
        addSection: function () {
            this.section = {
                id: this.counter++,
                name: null,
                areaID: 0,
                areaName: "Расходы",
                cssBackground: null,
                isShow: true,
                isShowInCollective: true,
                isShowOnSite: true,
                isShow_Filtered: true,
                sectionTypeID: 2,
                hasRecords: true,
                cssIcon: null,
                isSelected: true,
                canEdit: false,
                codeName: "test",
                collectiveSections: [],
                description: null,
                isUpdated: false,
                sectionTypeName: "Расходы",
                cssColor: '',
            };
            this.chooseColor();
            $("#accordion2-2, #accordion2-1").removeClass("show");
            this.changeSectionType(2);

            $("#modal-section").modal("show");
        },
        onEditSection: function (section) {
            console.log(section.sectionTypeID);
            this.section = { ...section };

            this.chooseColor(this.section.cssBackground);
            //this.changeSectionType(this.section.sectionTypeID);
            $("#accordion2-2, #accordion2-1").removeClass("show");
            $("#modal-section").modal("show");
        },
        removeSection: function () {
            $("#modal-section").modal("hide");
            let index = this.userSectionSource.findIndex(x => x.id == this.section.id);

            if (index > -1) {
                this.userSectionSource.splice(index, 1);

                for (var i = 0; i < this.areas.length; i++) {
                    index = this.areas[i].sections.findIndex(x => x.id == this.section.id);

                    if (index >= 0) {
                        this.areas[i].sections.splice(index, 1);
                        continue;
                    }
                }
            }
        },
        saveSection: function () {
            let index = this.userSectionSource.findIndex(x => x.id == this.section.id);

            if (this.section.sectionTypeID == 1) {
                this.section.areaID = 1;
                this.section.areaName = "Доходы";
            } else if (this.section.sectionTypeID == 2) {
                this.section.areaID = 0;
                this.section.areaName = "Расходы";
            } else if (this.section.sectionTypeID == 3) {
                this.section.areaID = 2;
                this.section.areaName = "Инвестиции";
            }
            this.section.hasRecords = false; //we're making removable section

            if (index == -1) {//create
                this.userSectionSource.push(this.section);
            } else {//update
                this.userSectionSource[index].sectionTypeID = this.section.sectionTypeID;
                this.userSectionSource[index].name = this.section.name;
                this.userSectionSource[index].cssBackground = this.section.cssBackground;
                this.userSectionSource[index].cssColor = this.section.cssColor;
                this.userSectionSource[index].cssIcon = this.section.cssIcon;
                this.userSectionSource[index].areaID = this.section.areaID;
                this.userSectionSource[index].areaName = this.section.areaName;
                //= { ...this.section };

                for (var i = 0; i < this.areas.length; i++) {
                    index = this.areas[i].sections.findIndex(x => x.id == this.section.id);

                    if (index >= 0) {
                        this.areas[i].sections.splice(index, 1);
                        continue;
                    }
                }
            }
            this.areas[this.section.areaID].sections.push(this.section);

            $("#modal-section").modal("hide");
            this.section = {};
        },
        selectIcon: function (item) {
            this.section.cssIcon = item.nameClass;
            //this.searchIcon = '';
            $("#accordion2-2, #accordion2-1").removeClass("show");
        },
        checkForm: function (e) {
            let isOk = true;

            if (!(this.section.name && this.section.name.length > 0)) {
                isOk = false;
            }
            if (isOk == false && e) {
                e.preventDefault();
            }
            return isOk;
        },
        changeSectionType: function (val) {
            this.section.sectionTypeID = val;// (val == this.section.sectionTypeID ? null : val);
        },
        chooseColor: function (cssBackground) {
            for (var i = 0; i < this.colors.length; i++) {
                this.colors[i].selected = false;
            }

            if (cssBackground) {
                let index = this.colors.findIndex(x => x.background == cssBackground);

                if (index != -1) {

                    this.colors[index].selected = true;
                    this.section.cssColor = this.colors[index].color;
                    this.section.cssBackground = this.colors[index].background;
                }

                $("#accordion2-2, #accordion2-1").removeClass("show");
            } else {
                this.section.cssColor = '';
                this.section.cssBackground = '';
            }
        },
        saveSections: function () {
            this.isSaving = true;

            return $.ajax({
                type: "POST",
                url: "/Start/SaveSections",
                context: this,
                data: JSON.stringify(this.areas),
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (result) {
                    if (result.isOk) {
                        this.isSaving = false;
                        this.sections = result.sections;
                    }
                    return true;
                },
                error: function (xhr, status, error) {
                    this.isSaving = false;
                    console.log(error);
                }
            });
        },

        //3
        ONLY_FOR_TEST: function () {
            return $.ajax({
                type: "GET",
                url: "/Section/GetSectins",
                data: null,
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                context: this,
                success: function (result) {
                    if (result.isOk) {
                        this.sections = result.sections;
                    }
                    return result;
                },
                error: function (xhr, status, error) {
                    console.log(error);
                }
            }, this);
        },
        startStep3: function () {
            if (this.template.columns.length == 0) {
                column = {
                    id: -7777,
                    name: "Дни",
                    order: this.template.columns.length,
                    isShow: false,
                    totalAction: 0,
                    formula: [],
                    templateBudgetSections: [],
                    placeAfterCommon: 0,
                    format: 'dd',
                    templateColumnType: TemplateColumnTypeEnum.DaysForMonth
                };
                this.template.columns.push(column);
            }
        },
        addColumn: function () {
            if (this.template.columns
                && this.template.columns.length > 1
                && this.template.columns[this.template.columns.length - 1].templateBudgetSections.length == 0) {
                this.errorMessage = "Колонка должна создержать хоты бы одну категорию.";
                return false;
            } else {
                this.errorMessage = "";
            }

            this.column = {
                id: this.counter++,
                name: this.startColumnsName,
                order: this.template.columns.length,
                isShow: true,
                totalAction: 0,
                formula: [],
                templateBudgetSections: [],
                placeAfterCommon: 0,
                format: '',
            };
            this.template.columns.push(this.column);

            setTimeout(function () {
                $(".lists").scrollCenter("#add-column", 300);
            }, 100);
        },
        selectColumn: function (column) {
            if (column.id != -7777) {
                this.column = column;
            }
        },
        templateOnSelectedSection: function (section) {
            if (this.column && this.column.formula) {
                this.errorMessage = "";

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
                    id: this.counter++,
                    counter: section.id,
                    sectionName: section.name,
                });

                setTimeout(function () {
                    $('[data-toggle="popover"]').popover('disable').popover('enable').popover("show");
                }, 250);

            }
        },
        getTitleCell: function (column) {
            let content = "";
            for (var i = 0; i < column.templateBudgetSections.length; i++) {
                if (i == 0) {
                    content = " " + column.templateBudgetSections[i].sectionName;
                } else {
                    content += " + " + column.templateBudgetSections[i].sectionName;
                }
            }
            return content;
        },
        removeSectionInColumn: function (templateBudgetSection, indexSection, indexColumn) {

            // let column = this.template.columns[indexColumn];

            this.column.templateBudgetSections.splice(indexSection, 1);
            this.column.formula = [];

            for (var i = 0; i < this.column.templateBudgetSections.length; i++) {
                let section = this.column.templateBudgetSections[i];
                if (this.column.formula.length > 0) {
                    this.column.formula.push({ id: null, value: "+", type: FormulaFieldTypeEnum.Mark });
                    this.column.formula.push({ id: section.counter, value: `[ ${section.sectionName} ]`, type: FormulaFieldTypeEnum.Section });
                } else {
                    this.column.formula.push({ id: section.counter, value: `[ ${section.sectionName} ]`, type: FormulaFieldTypeEnum.Section });
                }
            }
        },
        saveTemplate: function () {
            if (this.validTemplate() == false) {
                return false;
            }
            this.isSaving = true;
            return sendAjax("/Start/SaveTemplate", this.template, "POST")
                .then(function (result) {
                    if (result.isOk == true) {
                        StartVue.template = result.template;
                        StartVue.errorMessage = null;
                    }
                    StartVue.isSaving = false;
                    $('[data-toggle="popover"]').popover('hide');
                });
        },
        validTemplate: function () {
            let isOk = true;

            if (this.template.columns.length == 0) {
                isOk = false;
                this.errorMessage = "Шаблон должен содержать хотя бы одну колонку.";
            } else {
                this.errorMessage = null;
            }

            return isOk;
        },

        //4
        editLimit: function (limit) {
            if (this.periodTypes.length == 0) {
                return;
            }

            if (limit) {
                this.limit = { ...limit };
            } else {
                this.limit = { id: this.counter++, periodName: '', periodTypeID: -1, isShowOnDashboard: true };

                $("#limitSections").val(null).select2();

                this.limit.periodTypeID = this.periodTypes[0].id;
                this.limit.periodName = this.periodTypes[0].name;
            }

            if (this.limit.sections) {
                $("#limitSections").val(this.limit.sections.map(x => x.id)).select2();
            } else {
                $("#limitSections").select2();
            }

            //find limit by id 
            $("#modal-limit").modal("show");
        },
        saveLimit: function () {
            this.limit.newSections = $("#limitSections").select2("val").map(function (x) { return { id: x } });
            this.limit.sections = [];

            for (var i = 0; i < this.limit.newSections.length; i++) {
                let index = this.sections.findIndex(x => x.id == this.limit.newSections[i].id);

                if (index != -1) {
                    this.limit.sections.push(this.sections[index]);
                }
            }

            if (this.checkLimitForm() == false) {
                return false;
            }

            let index = this.limits.findIndex(x => x.id == this.limit.id);
            if (index == -1) {
                this.limits.push(this.limit);
            } else {
                this.limits[index] = this.limit;
            }
            $("#modal-limit").modal("hide");
        },
        checkLimitForm: function (e) {
            let isOk = true;
            if (this.limit.newSections.length == 0) {
                $("#limitSections").addClass("is-invalid");
                $("#limitSections").next().addClass("is-invalid");
                isOk = false;
            }
            else {
                $("#limitSections").removeClass("is-invalid");
                $("#limitSections").next().removeClass("is-invalid");
            }

            if (!(this.limit.name && this.limit.name.length > 0)) {
                $("#limit-name").addClass("is-invalid");
                isOk = false;
            } else {
                $("#limit-name").removeClass("is-invalid");
            }
            if (!(this.limit.limitMoney && (this.limit.limitMoney > 0 || this.limit.limitMoney.length > 0))) {
                isOk = false;
                $("[name=limitMoney]").addClass("is-invalid");
            } else {
                $("[name=limitMoney]").removeClass("is-invalid");
            }

            if (isOk == false && e) {
                e.preventDefault();
            }
            return isOk;
        },
        saveLimits: function () {
            this.isSaving = true;
            return sendAjax("/Start/SaveLimits", this.limits, "POST")
                .then(function (result) {
                    if (result.isOk == true) {

                    } else {
                        console.log(result.message);
                    }
                    StartVue.isSaving = false;
                });
        },
        removeLimit: function (limit) {
            let index = this.limits.findIndex(x => x.id == limit.id);
            if (index != -1) {
                this.limits.splice(index, 1);
            }
        },

        //5
        editGoal: function (goal) {
            if (goal) {
                this.goal = { ...goal };
            } else {
                this.goal = { id: this.counter++ };
                this.goal.dateStart = GetDateByFormat(moment(), "YYYY/MM/DD");
            }

            let startConfig = GetFlatpickrRuConfig(this.goal.dateStart);
            startConfig.onChange = function (selectedDates, dateStr, instance) {
                StartVue.flatpickrEnd.config.minDate = dateStr;
            };
            let endConfig = GetFlatpickrRuConfig(this.goal.dateEnd);
            endConfig.onChange = function (selectedDates, dateStr, instance) {
                StartVue.flatpickrStart.config.maxDate = dateStr;
            };

            this.flatpickrStart = flatpickr('#date-start', startConfig);
            this.flatpickrEnd = flatpickr('#date-end', endConfig);

            //find goal by id 
            $("#modal-goal").modal("show");
        },
        getDateByFormat: function (date, format) {
            return GetDateByFormat(date, format);
        },
        removeGoal: function (goal) {
            let index = this.goals.findIndex(x => x.id == goal.id);
            if (index != -1) {
                this.goals.splice(index, 1);
            }
        },
        saveGoal: function () {
            if (this.checkGoalForm() == false) {
                return false;
            }

            let index = this.goals.findIndex(x => x.id == this.goal.id);
            if (index == -1) {
                this.goals.push(this.goal);
            } else {
                this.goals[index] = this.goal;
            }
            $("#modal-goal").modal("hide");
        },
        checkGoalForm: function (e) {
            let isOk = true;

            if (!(this.goal.name && this.goal.name.length > 0)) {
                isOk = false;
                $("#goal-name").addClass("is-invalid");
            } else {
                $("#goal-name").removeClass("is-invalid");
            }
            if (!(this.goal.expectationMoney && (this.goal.expectationMoney > 0 || this.goal.expectationMoney.length > 0))) {
                isOk = false;
                $("#goal-expectationMoney").addClass("is-invalid");
            } else {
                $("#goal-expectationMoney").removeClass("is-invalid");
            }
            if (isOk == false && e) {
                e.preventDefault();
            }
            return isOk;
        },
        saveGoals: function () {
            this.isSaving = true;
            return sendAjax("/Start/SaveGoals", this.goals, "POST")
                .then(function (result) {
                    if (result.isOk == true) {

                    } else {
                        console.log(result.message);
                    }
                    StartVue.isSaving = false;
                });
        },

    }
});


