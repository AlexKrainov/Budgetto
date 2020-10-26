var ChartEditVue = new Vue({
    el: "#big-chart-edit-vue",
    data: {
        chart: {
            chartTypeName: '',
            fields: [],
            isShowBudgetMonth: true,
            isShowBudgetYear: true
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

            if (stepIndex == 0 && stepDirection == "forward") {
                canGo = ChartEditVue.chart.chartTypeID > 0;
                if (canGo == false) {
                    $("#validChooseChartType").attr("style", "display:block");
                } else {
                    $("#validChooseChartType").removeAttr("style");
                }
            }

            if (stepIndex == 1 && stepDirection == "forward") { //check for empty fields
                canGo = ChartEditVue.chart.fields.length > 0 && ChartEditVue.validStep1();
            }

            return canGo;
        });

        let chartID = document.getElementById("big-chart-edit-vue").attributes["data-chart-id"].value;
        if (chartID) {
            this.loadChart(chartID);
        }
        this.loadSections();
    },
    methods: {
        loadChart: function (id) {
            return sendAjax("/Chart/LoadChart?id=" + id, null, "GET")
                .then(function (result) {
                    ChartEditVue.chart = result.chart;
                    ChartEditVue.selectChart(ChartEditVue.chart.chartTypeID);
                });
        },
        loadSections: function () {
            return sendAjax("/Section/GetAllSectionByPerson", null, "GET")
                .then(function (result) {
                    if (result.isOk == true) {
                        ChartEditVue.sections = result.sections;
                    }
                });
        },
        selectChart: function (chartTypeID) {
            let charts = document.querySelectorAll(".chart-list-item");
            for (var i = 0; i < charts.length; i++) {
                if (charts[i].attributes["data-chart-type"].value == chartTypeID) {
                    this.chart.chartTypeID = chartTypeID;
                    this.chart.chartTypeName = charts[i].attributes["data-chart-type-name"].value;
                    charts[i].className = 'chart-list-item active';
                } else {
                    charts[i].className = 'chart-list-item';
                }
            }


            if (this.chart.id == undefined || this.chart.id <= 0) {
                this.addField();
                setTimeout(function () {
                    $('#big-chart-edit-vue').smartWizard("next");
                }, 500);
            } else {
                //init control
                setTimeout(function () {

                    for (var i = 0; i < ChartEditVue.chart.fields.length; i++) {
                        let el_name = "#field_sections_" + ChartEditVue.chart.fields[i].id;
                        $(el_name).val(ChartEditVue.chart.fields[i].sections.map(x => x)).select2({ placeholder: 'Выбор категорий' });

                        el_name = "#chart_color_" + ChartEditVue.chart.fields[i].id;
                        $(el_name).colorPick({
                            'initialColor': ChartEditVue.chart.fields[i].cssColor,
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
                    }
                }, 10);
            }
        },
        addField: function () {
            this.fieldsCount += 1;
            this.chart.fields.push({ id: this.fieldsCount, name: "", cssColor: "" });

            //init control
            setTimeout(function () {

                let el_name = "#field_sections_" + ChartEditVue.fieldsCount;
                $(el_name).select2({ placeholder: 'Продукты, кафе, фастфуд' });

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
            if (this.validStep2() == false) {
                return false;
            }
            ShowLoading("#big-chart-edit-vue");
            this.isSaving = true;

            try {
                //for (var i = 0; i < this.chart.fields.length; i++) {
                //    let el_name = "#field_sections_" + this.chart.fields[i].id;
                //    this.chart.fields[i].sections = $(el_name).select2("val");
                //}

                return sendAjax("/Chart/Save", this.chart, "POST", this)
                    .then(function (result) {
                        if (result.isOk == true) {
                            this.chart = result.chart;
                        } else {
                            console.log(result.message);
                        }
                        ChartEditVue.isSaving = false;
                        HideLoading("#big-chart-edit-vue");
                        document.location.href = result.href;
                    });

            } catch (e) {
                ChartEditVue.isSaving = false;
                HideLoading("#big-chart-edit-vue");
            }
        },
        validStep1: function () {
            let canGo = true;

            for (var i = 0; i < this.chart.fields.length; i++) {
                let field = this.chart.fields[i];

                //validate name
                let selector = "#field_name_" + field.id;
                let $elName = $(selector);
                if (!(field.name && field.name.length > 0)) {
                    canGo = false;
                    $elName.addClass("is-invalid");
                } else {
                    $elName.removeClass("is-invalid");
                }

                //validate sections
                selector = "#field_sections_" + field.id;
                let $elSection = $(selector);
                field.sections = $elSection.val();
                if (!(field.sections && field.sections.length > 0)) {
                    canGo = false;
                    $elSection.addClass("is-invalid");
                    $elSection.next().find(".select2-selection").attr("style", "border-color: #d9534f;");
                } else {
                    $elSection.removeClass("is-invalid");
                    $elSection.next().find(".select2-selection").removeAttr("style", "border-color: #d9534f;");
                }
            }
            return canGo;
        },
        validStep2: function () {
            let canGo = true;
            if (this.chart.name && this.chart.name.length > 1) {
                $("#chartName").removeClass("is-invalid");
            } else {
                canGo = false;
                $("#chartName").addClass("is-invalid");
            }

            return canGo;
        },
        removeField: function (field) {
            let indexField = this.chart.fields.findIndex(x => x.id == field.id);
            this.chart.fields.splice(indexField, 1);
        },
        selectAll: function (fieldID) {
            let name = this.getSelectName(fieldID);
            $(name + " option").prop("selected", true);
            $(name).trigger("change");
        },
        unselectAll: function (fieldID) {
            let name = this.getSelectName(fieldID);
            $(name).val(null).trigger("change");
        },
        selectOnlyType: function (fieldID, sectionTypeID) {
            let name = this.getSelectName(fieldID);

            $(name).val(null);
            $(name).val(
                this.sections
                    .filter(x => x.sectionTypeID == sectionTypeID)
                    .map(x => x.id))
                .trigger("change");
        },
        getSelectName: function (fieldID) {
            return '#field_sections_' + fieldID;
        },


        getRusName: function (num) {
            return GetRusName(num, ['Поле', 'Поля', 'Полей']);
        }
    }
});


