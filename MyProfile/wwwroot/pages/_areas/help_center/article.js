$(document).ready(function () {
    if ($('html').hasClass('dark-style')) {
        $('.navbar').removeClass('bg-white').addClass('bg-dark')
    }
});

var ArticleVue = new Vue({
    el: "#help-article",
    data: {
        articleID: null,
        article: {},
        topArticles: [],
    },
    watch: {},
    mounted: function () {
        this.articleID = $("#help-arcticle").data("article-id");
        this.loadArticle();
    },
    methods: {
        loadArticle: function () {
            //ToDo show loading for comment, topArticle and etc
            //ShowLoading
            return $.ajax({
                type: "GET",
                url: "/Help/Center/Load?id=" + this.articleID,
                context: this,
                contentType: "application/json",
                dataType: 'json',
                success: function (response) {
                    if (response.isOk) {
                        this.article = response.article;
                        this.topArticles = response.topArticles;
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


