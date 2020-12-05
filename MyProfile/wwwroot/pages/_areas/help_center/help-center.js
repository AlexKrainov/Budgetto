var HelpVue = new Vue({
    el: "#help-vue",
    data: {
        menus: [],
        searchText: undefined,
    },
    watch: {
        searchText: function (newValue, oldValue) {
            if (newValue) {
                newValue = newValue.toLocaleLowerCase();
            }

            for (var j = 0; j < this.menus.length; j++) {
                let articles = this.menus[j].articles;
                for (var i = 0; i < articles.length; i++) {
                    articles[i].isShow = articles[i].keyWords.search(newValue) >= 0
                        || articles[i].title.toLocaleLowerCase().includes(newValue);
                }

                this.menus[j].isShow = this.menus[j].articles.filter(x => x.isShow == true).length > 0;
            }
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
        selectedMenu: function (menu) {
            $("html, body").animate({
                scrollTop: $("#menu_" + menu.id).offset().top - 75
            }, 1000);
        },
    }
});