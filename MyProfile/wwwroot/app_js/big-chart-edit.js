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
                if (ChartEditVue.chart.fields.length == 0) {

                }
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

            if (this.chart.id == 0) {
                setTimeout(function () {
                    $('#big-chart-edit-vue').smartWizard("next");
                }, 1000);
            } else {
                //init control
                setTimeout(function () {

                    for (var i = 0; i < ChartEditVue.chart.fields.length; i++) {
                        let el_name = "#chart_section_" + ChartEditVue.chart.fields[i].id;
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

                let el_name = "#chart_section_" + ChartEditVue.fieldsCount;
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
