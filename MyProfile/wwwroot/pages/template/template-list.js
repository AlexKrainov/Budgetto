
var TemplateListVue = new Vue({
    el: "#template-list-vue",
    data: {
        templates: [],
        activeTemplatePeriodTypeID: -1,
        search: null,
        msnry: {},
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

            return sendAjax("/Template/GetTemplates", null, "GET")
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
                    template.isDeleted = !response.isOk;
                    HideLoading('#template_' + template.id);
                }
            });
        },
    }
});


