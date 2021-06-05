
var TemplateListVue = new Vue({
    el: "#template-list-vue",
    data: {
        templates: [],
        activeTemplatePeriodTypeID: -1,
        search: null,
        msnry: {},

        preparedTemplate: {},
        template: {},
        isDetailsView: false,
        isDetailsViewTable: true,

        //payment
        currentCount: 0,
        limitCount: 0,

        isSaving: false,
    },
    watch: {
        search: function (newValue, oldValue) {
            if (!newValue) {
                this.templates.forEach(function (el, index) { el.isShow = true; });
            }

            for (var i = 0; i < this.templates.length; i++) {
                this.templates[i].isShow = this.templates[i].name.indexOf(newValue) >= 0;
            }
        }
    },
    mounted: function () {
        let baseEl = document.getElementById("template-list-vue");
        this.currentCount = baseEl.getAttribute("data-current-count") * 1;
        this.limitCount = baseEl.getAttribute("data-limit-count") * 1;

        this.init()
            .then(function () {
                TemplateListVue.msnry = new Masonry('#templates', {
                    itemSelector: '.masonry-item:not(.d-none)',
                    columnWidth: '.masonry-item-sizer',
                    originLeft: true,
                    horizontalOrder: true
                });
            });
    },
    methods: {
        init: function () {

            this.refreshTemplates();

            return sendAjax("/Template/GetPreparedTemplates", null, "GET")
                .then(function (result) {
                    if (result.isOk == true) {
                        TemplateListVue.preparedTemplate = result.preparedTemplate;
                    }
                });
        },
        refreshTemplates: function () {
            sendAjax("/Template/GetTemplates", null, "GET")
                .then(function (result) {
                    if (result.isOk == true) {
                        TemplateListVue.templates = result.templates;
                    }
                });
        },
        getLinkForView: function (template) {
            return TemplateGetLinkForView(template);
        },
        getDateByFormat: function (date, format) {
            return GetDateByFormat(date, format);
        },
        reload: function () {
            setTimeout(function () {
                TemplateListVue.msnry.layout();
            }, 15);
        },
        getRusName: function (num) {
            return GetRusName(num, ['Колонка', 'Колонки', 'Колонок']);
        },
        remove: function (template) {
            ShowLoading('#template_' + template.id);

            return $.ajax({
                type: "POST",
                url: "/Template/Remove",
                data: JSON.stringify(template),
                context: template,
                contentType: "application/json",
                dataType: 'json',
                success: function (response) {
                    template.isDeleted = response.isOk;
                    if (template.isDeleted) {
                        TemplateListVue.currentCount -= 1;
                    }
                    HideLoading('#template_' + template.id);
                }
            });
        },
        recovery: function (template) {
            ShowLoading('#template_' + template.id);

            return $.ajax({
                type: "POST",
                url: "/Template/Recovery",
                data: JSON.stringify(template),
                context: template,
                contentType: "application/json",
                dataType: 'json',
                success: function (response) {
                    if (response.isOk) {
                        template.isDeleted = !response.isOk;
                        if (template.isDeleted == false) {
                            TemplateListVue.currentCount += 1;
                        }
                    }
                    HideLoading('#template_' + template.id);
                }
            });
        },
        toggleTemplate: function (templateID) {
            ShowLoading('#template_' + templateID);
            return $.ajax({
                type: "GET",
                url: "/Template/ToggleTemplate?id=" + templateID,
                contentType: "application/json",
                dataType: 'json',
                context: templateID,
                success: function (response) {
                    HideLoading('#template_' + this);
                    if (response.isOk = true) {
                        TemplateListVue.init();
                    }
                },
                error: function () {
                    HideLoading('#template_' + this);
                }
            });
        },

        saveTemplate: function (template, isEdit) {
            this.isSaving = true;
            let url = '/Template/SavePreparedTemplate';

            if (isEdit) {
                url = '/Template/SavePreparedTemplateAndEdit';
            }

            return $.ajax({
                type: "POST",
                url: url,
                context: this,
                data: JSON.stringify(template),
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (result) {
                    if (result.isOk) {
                        toastr.success("Шаблон сохранен успешно");
                        $("#prepared-template-model").modal("hide");

                        this.isDetailsView = false;
                        this.currentCount += 1;

                        if (result.href) {
                            window.document.location.href = result.href;
                        } else {
                            this.refreshTemplates();
                        }
                    } else {
                        toastr.error("Не удалось сохранить шаблон.");
                    }
                    this.isSaving = false;
                    return true;
                },
                error: function (xhr, status, error) {
                    this.isSaving = false;
                    console.log(error);
                }
            });
        },
        showTemplate: function (template) {
            this.template = template;
            this.isDetailsView = true;
            this.isDetailsViewTable = true;
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
        selectPeriod: function (period) {

            for (var i = 0; i < this.preparedTemplate.periods.length; i++) {
                this.preparedTemplate.periods[i].isSelected = false;
            }
            period.isSelected = true;

        },
        showPreparedModel: function() {
            this.isDetailsView = false;

            $('#prepared-template-model').modal('show');
        }
    }
});


