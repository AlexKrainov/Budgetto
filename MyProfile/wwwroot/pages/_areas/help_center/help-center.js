var HelpVue = new Vue({
    el: "#help-vue",
    data: {
        menus: [],
        searchText: undefined,
    },
    watch: {
        searchText: function (newValue, oldValue) {

        },
    },
    mounted: function () {
        this.load();
    },
    methods: {
        load: function () {
            //ToDo show loading for comment, topArticle and etc
            //ShowLoading
            return $.ajax({
                type: "GET",
                url: "/Help/Center/LoadMenu?id=" + this.articleID,
                context: this,
                contentType: "application/json",
                dataType: 'json',
                success: function (response) {
                    if (response.isOk) {
                        this.menus = response.menus;
                    }
                    //HideLoading('#chart_' + chart.id);
                }
            });
        },
        getDateByFormat: function (date, format) {
            return GetDateByFormat(date, format);
        },
    }
});