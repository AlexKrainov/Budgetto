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
        addMoney: function (goal, isHistory) {
            GoalEditVue.addMoney(goal, isHistory);
        },
        edit: function (goal) {
            GoalEditVue.edit(goal);
        },
        reloadView: function () {
            setTimeout(function () {
                GoalListVue.msnry.layout();
            }, 15);
        },
        remove: function (goal) {
            ShowLoading('#goal_' + goal.id);

            return $.ajax({
                type: "POST",
                url: "/Goal/Remove",
                data: JSON.stringify(goal),
                context: goal,
                contentType: "application/json",
                dataType: 'json',
                success: function (response) {
                    goal.isDeleted = response.isOk;
                    HideLoading('#goal_' + goal.id);
                }
            });
        },
        recovery: function (goal) {
            ShowLoading('#goal_' + goal.id);

            return $.ajax({
                type: "POST",
                url: "/Goal/Recovery",
                data: JSON.stringify(goal),
                context: goal,
                contentType: "application/json",
                dataType: 'json',
                success: function (response) {
                    goal.isDeleted = !response.isOk;
                    HideLoading('#goal_' + goal.id);
                }
            });
        },
    }
});


