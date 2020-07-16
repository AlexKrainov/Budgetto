var GoalListVue = new Vue({
    el: "#goal-list-vue",
    data: {
        goals: [],

        msnry: {},
        //edit
    },
    watch: {},
    mounted: function () {
        this.load()
            .then(function () {
            });
    },
    methods: {
        load: function () {
            return sendAjax("/Goal/GetGoals", null, "GET")
                .then(function (result) {
                    if (result.isOk == true) {
                        GoalListVue.goals = result.goals;

                        setTimeout(function () {
                            if (GoalListVue.msnry && GoalListVue.msnry.destroy != undefined) {
                                GoalListVue.msnry.destroy();
                            }
                            GoalListVue.msnry = new Masonry('#goals', {
                                itemSelector: '.masonry-item:not(.d-none)',
                                columnWidth: '.masonry-item-sizer',
                                originLeft: true,
                                horizontalOrder: true
                            });
                        }, 100);
                    }
                });
        },

        getDateByFormat: function (date, format) {
            return GetDateByFormat(date, format);
        },
        addMoney: function (goal) {
            GoalEditVue.addMoney(goal);
        },
        edit: function (goal, isHistory) {
            GoalEditVue.edit(goal, isHistory);
        },
        reloadView: function () {
            setTimeout(function () {
                GoalListVue.msnry.layout();
            }, 15);
        },
    }
});

Vue.config.devtools = true;
