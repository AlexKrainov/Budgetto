var StartVue = new Vue({
    el: "#start-vue",
    data: {
        //1 
        userInfo: {
            name: null,
            workHours: 0,
            allWorkHours: 0,
            allWorkDays: 0,
        },

        //2
        areas: [],
        //    [{ id: 0, name: "Расходы", codeName: "Spendings", sections: [] },
        //{ id: 1, name: "Доходы", codeName: "Earnings", sections: [] }], //2
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
        isShowButtonSkip: false,
    },
    watch: {
    },
    computed: {
        sectionComponent_step2: function () {
            return this.$children[0];
        },
    },
    mounted: function () {
        let elWizard = $('#start-vue').smartWizard({
            autoAdjustHeight: false,
            backButtonSupport: false,
            useURLhash: false,
            showStepURLhash: false,
            keyNavigation: false,
            lang: { // Language variables for button
                next: 'Вперед',
            },
            toolbarSettings: {
                toolbarPosition: 'bottom', // none, top, bottom, both
                toolbarButtonPosition: 'center', // left, right, center
                showNextButton: true, // show/hide a Next button
                showPreviousButton: false, // show/hide a Previous button
            },
            keyboardSettings: {
                keyNavigation: false, // Enable/Disable keyboard navigation(left and right keys are used if enabled)
            },
        });

        elWizard.on("leaveStep", function (e, anchorObject, stepIndex, stepDirection) {
            if (stepDirection == "backward") {
                return false;
            }

            let canGo = true;
            StartVue.errorMessage = null;
            StartVue.isShowButtonSkip = false;

            if (stepIndex == 0 && stepDirection == "forward") {

                //UserInfo
                if (StartVue.checkUserForm()) {
                    $("#userInfoName").removeClass("is-invalid");
                    canGo = true;
                    StartVue.saveUserInfo();

                    $('#work-hours, #work-days').attr({
                        "title": "", "data-original-title": ""
                    });
                    $('#work-hours').tooltip('dispose');
                    $('#work-days').tooltip('dispose');
                    // $('[data-toggle="tooltip"]').tooltip('show');
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
                    StartVue.errorMessage = "Нужно выбрать хотя бы одну категорию.";
                }
            }

            if (stepIndex == 2 && stepDirection == "forward") {//stepIndex == 2

                if (StartVue.validTemplate()) {
                    StartVue.saveTemplate();
                    StartVue.isShowButtonSkip = true;
                } else {
                    canGo = false;
                }
            }

            if (stepIndex == 3 && stepDirection == "forward") {//stepIndex == 3

                if (StartVue.limits.length > 0) {
                    StartVue.saveLimits();
                }
                StartVue.isShowButtonSkip = true;
            }

            if (stepIndex == 4 && stepDirection == "forward") {//stepIndex == 4

                if (StartVue.goals.length > 0) {
                    StartVue.saveGoals();
                }
                StartVue.showStep5();
            }

            if (stepIndex == 5 && stepDirection == "forward") {
                canGo = false;
                console.log("finish");
            }

            $(".modal-backdrop").remove();

            return canGo;
        });

        setTimeout(function () {
            $(".sw-btn-next").parent().removeClass("btn-group").addClass("d-inline-flex align-items-center");
            $(".sw-btn-next").removeClass("btn-secondary").addClass("btn-primary").before($("#button-skip"));
            $(".sw-btn-next").after($("#button-finish"));
        }, 100);

        //todo set timeline
        this.loadUserInfo();
        this.loadSections();
        // this.ONLY_FOR_TEST();

        $.getJSON("/json/font-awesome.json", function (json) {
            StartVue.icons = json;
        });
        $.getJSON("/json/colors-section.json", function (json) {
            StartVue.colors = json;
        });
        $("#section-description, #choose-area, #show-on-the-main-page, #limit-comment, #goal-comment, #gaol-is-finish").hide();
        $(".layout-sidenav-toggle").remove();
        $(".isCashback-section").remove();
    },
    methods: {
        skip: function () {
            if ($('#start-vue').smartWizard("getStepIndex") == 4) {
                this.showStep5();
            }
            $('#start-vue').smartWizard("next");
        },
        showStep5: function () {
            this.isShowButtonSkip = false;//bug
            $("#button-skip").hide();
            $(".sw-btn-next").hide(); //.removeClass("btn-secondary disabled").addClass("btn-primary").val("Завершить");
            $("#button-finish").show();
        },
        finish: function () {
            StartVue.isSaving = true;
            return sendAjax("/Start/Finish", "GET")
                .then(function (result) {
                    if (result.isOk == true) {
                        document.location.href = "/Budget/Month";
                    }
                    StartVue.isSaving = false;
                });
        },

        //1
        loadUserInfo: function () {
            this.isSaving = true;

            return $.ajax({
                type: "GET",
                url: "/Start/LoadUserInfo",
                context: this,
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (result) {
                    if (result.isOk) {
                        this.isSaving = false;
                        this.userInfo = result.userInfo;
                        this.userInfo.timeZoneClient = Intl.DateTimeFormat().resolvedOptions().timeZone;
                    }
                    //$('[data-toggle="tooltip"]').tooltip('show');
                    return true;
                },
                error: function (xhr, status, error) {
                    this.isSaving = false;
                    console.log(error);
                }
            });
        },
        saveUserInfo: function () {
            this.isSaving = true;

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
                        $("#userName").html(this.userInfo.name);
                    }
                    return true;
                },
                error: function (xhr, status, error) {
                    this.isSaving = false;
                    console.log(error);
                }
            });
        },
        checkUserForm: function () {
            let isOk = true;

            let str = this.userInfo.name;
            str = str ? str.replaceAll(" ", "") : "";

            if (str.length == 0) {
                $("#userInfoName").addClass("is-invalid");
                isOk = false;
            } else {
                $("#userInfoName").removeClass("is-invalid");
                isOk = true;
            }

            return isOk;
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
                        this.$nextTick(() => {
                            //$("#baseSections>.ion-ios-list").click();
                            $(".ion-ios-list").click();
                            $("#baseSections>.div-change-view").hide();
                        });
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
                let index = this.areas.findIndex(x => x.id == section.areaID);
                if (index < 0) {
                    this.areas.push({ id: section.areaID, name: section.areaName, sections: [] });
                    index = this.areas.length - 1;
                }
                this.areas[index].sections.push(section);
            } else {
                let index = this.areas.findIndex(x => x.id == section.areaID);
                this.areas[index].sections.splice(this.areas[index].sections.findIndex(x => x.id == section.id), 1);
                $('[data-toggle="tooltip"]').tooltip('dispose');
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
                codeName: null,
                collectiveSections: [],
                description: null,
                isUpdated: false,
                sectionTypeName: "Расходы",
                cssColor: '',
                canRemove: true,
            };
            this.chooseColor();
            $("#accordion2-2, #accordion2-1").removeClass("show");
            this.changeSectionType(2);

            $("#modal-section").modal("show");
        },
        onEditSection: function (section) {
            console.log(section.sectionTypeID);
            this.section = JSCopyObject(section);

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
            if (this.validSection()) {


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
                    //JSCopyObject(goal);
                    this.userSectionSource[index].sectionTypeID = this.section.sectionTypeID;
                    this.userSectionSource[index].name = this.section.name;
                    this.userSectionSource[index].cssBackground = this.section.cssBackground;
                    this.userSectionSource[index].cssColor = this.section.cssColor;
                    this.userSectionSource[index].cssIcon = this.section.cssIcon;
                    this.userSectionSource[index].areaID = this.section.areaID;
                    this.userSectionSource[index].areaName = this.section.areaName;


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
            }
        },
        validSection: function (e) {
            let isOk = true;

            if (!(this.section.name && this.section.name.length > 0)) {
                isOk = false;
                $("#section-name").addClass("is-invalid");
            } else {
                $("#section-name").removeClass("is-invalid");
            }

            let str = this.section.name;
            str = str ? str.replaceAll(" ", "") : "";
            if (str.length == 0) {
                isOk = false;
                $("#section-name").addClass("is-invalid");
            } else {
                $("#section-name").removeClass("is-invalid");
            }

            if (isOk == false && e) {
                e.preventDefault();
            }
            return isOk;
        },
        selectIcon: function (item) {
            this.section.cssIcon = item.nameClass;
            //this.searchIcon = '';
            $("#accordion2-2, #accordion2-1").removeClass("show");
        },
        checkSectionForm: function (e) {
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
            ShowLoading("#smartwizard-1-step-3");

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
                    HideLoading("#smartwizard-1-step-3");
                    return true;
                },
                error: function (xhr, status, error) {
                    this.isSaving = false;
                    console.log(error);
                    HideLoading("#smartwizard-1-step-3");
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
                this.errorMessage = "Колонка должна содержать хотя бы одну категорию.";
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
                templateColumnType: TemplateColumnTypeEnum.BudgetSection
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
        removeColumnName: function (templateColumn) {
            if (templateColumn.name == 'Новая колонка') {
                templateColumn.name = '';
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
                    sectionID: section.id,
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
                    this.column.formula.push({ id: section.sectionID, value: `[ ${section.sectionName} ]`, type: FormulaFieldTypeEnum.Section });
                } else {
                    this.column.formula.push({ id: section.sectionID, value: `[ ${section.sectionName} ]`, type: FormulaFieldTypeEnum.Section });
                }
            }
        },
        saveTemplate: function () {
            this.isSaving = true;
            $('[data-toggle="popover"]').popover('hide');

            return sendAjax("/Start/SaveTemplate", this.template, "POST")
                .then(function (result) {
                    if (result.isOk == true) {
                        StartVue.template = result.template;
                        StartVue.errorMessage = null;
                    }
                    StartVue.isSaving = false;
                });
        },
        validTemplate: function () {
            let isOk = true;
            this.errorMessage = null;

            if (this.template.columns.length < 2) {
                isOk = false;
                this.errorMessage = "Шаблон должен содержать хотя бы 2 колонки.";
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

        //4
        editLimit: function (limit) {
            if (this.periodTypes.length == 0) {
                return;
            }

            if (limit) {
                this.limit = JSCopyObject(limit);
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
                this.limits[index].name = this.limit.name;
                this.limits[index].periodTypeID = this.limit.periodTypeID;
                this.limits[index].periodName = this.limit.periodName;
                this.limits[index].newSections = this.limit.newSections;
                this.limits[index].sections = this.limit.sections;
                this.limits[index].limitMoney = this.limit.limitMoney;
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

            let str = this.limit.name;
            str = str ? str.replaceAll(" ", "") : "";
            if (str.length == 0) {
                isOk = false;
                $("#limit-name").addClass("is-invalid");
            } else {
                $("#limit-name").removeClass("is-invalid");
            }

            if (this.limit.limitMoney && this.limit.limitMoney > 0) {
                $("[name=limitMoney]").removeClass("is-invalid");
            } else {
                isOk = false;
                $("[name=limitMoney]").addClass("is-invalid");
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
                this.goal = JSCopyObject(goal);
            } else {
                this.goal = { id: this.counter++ };
                this.goal.dateStart = GetDateByFormat(moment(), "YYYY/MM/DD");
                this.goal.isShow_BudgetMonth = true;
                this.goal.isShow_BudgetYear = true;
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
                this.goals[index].name = this.goal.name;
                this.goals[index].dateStart = this.goal.dateStart;
                this.goals[index].dateEnd = this.goal.dateEnd;
                this.goals[index].expectationMoney = this.goal.expectationMoney;
                this.goals[index].isShow_BudgetMonth = this.goal.isShow_BudgetMonth;
                this.goals[index].isShow_BudgetYear = this.goal.isShow_BudgetYear;
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


            let str = this.goal.name;
            str = str ? str.replaceAll(" ", "") : "";
            if (str.length == 0) {
                isOk = false;
                $("#goal-name").addClass("is-invalid");
            } else {
                $("#goal-name").removeClass("is-invalid");
            }

            if (this.goal.expectationMoney && this.goal.expectationMoney > 0) {
                $("#goal-expectationMoney").removeClass("is-invalid");
            } else {
                isOk = false;
                $("#goal-expectationMoney").addClass("is-invalid");
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


