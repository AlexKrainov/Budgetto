var GoalEditVue = new Vue({
    el: "#goal-edit-vue",
    data: {
        //edit
        goal: {},
        flatpickrStart: {},
        flatpickrEnd: {},

        record: {},
        isSaving: false,
    },
    watch: {},
    mounted: function () {
    },
    methods: {
        getDateByFormat: function (date, format) {
            return GetDateByFormat(date, format);
        },
        edit: function (goal) {
            if (goal) {
                this.goal = { ...goal };
                
            } else {
                this.goal.dateStart = GetDateByFormat(moment(), "YYYY/MM/DD");
            }

            let startConfig = GetFlatpickrRuConfig(this.goal.dateStart);
            startConfig.onChange = function (selectedDates, dateStr, instance) {
                console.log(dateStr);
                GoalEditVue.flatpickrEnd.config.minDate = dateStr;
            };
            let endConfig = GetFlatpickrRuConfig(this.goal.dateEnd);
            endConfig.onChange = function (selectedDates, dateStr, instance) {
                GoalEditVue.flatpickrStart.config.maxDate = dateStr;
            };

            this.flatpickrStart = flatpickr('#date-start', startConfig);
            this.flatpickrEnd = flatpickr('#date-end', endConfig);

            //find goal by id 
            $("#modal-goal").modal("show");
        },
        saveGoal: function () {
            if (this.checkForm() == false) {
                return false;
            }

            this.isSaving = true;

            return sendAjax("/Goal/Save", this.goal, "POST")
                .then(function (result) {
                    if (result.isOk == true) {

                        GoalListVue.load();
                        let goalIndex = GoalListVue.goals.findIndex(x => x.id == result.goal.id);
                        if (goalIndex >= 0) {
                            GoalListVue.goals[goalIndex] = result.goal;
                        } else {
                            GoalListVue.goals.push(result.goal);
                            setTimeout(function () {
                                GoalListVue.msnry.addItems($("div[data-id=goal_" + result.goal.id + "]"));
                                GoalListVue.msnry.layout();
                            }, 100);
                        }
                        $("#modal-goal").modal("hide");
                    } else {
                        console.log(result.message);
                    }
                    GoalEditVue.isSaving = false;
                });
        },
      
        checkForm: function (e) {
            let isOk = true;

            if (!(this.goal.name && this.goal.name.length > 0)) {
                isOk = false;
                $("#goal-name").addClass("is-valid");
            } else {
                $("#goal-name").removeClass("is-valid");
            }
            if (!(this.goal.expectationMoney && (this.goal.expectationMoney > 0 || this.goal.expectationMoney.length > 0))) {
                isOk = false;
                $("#goal-expectationMoney").addClass("is-valid");
            } else {
                $("#goal-expectationMoney").removeClass("is-valid");
            }
            if (isOk == false && e) {
                e.preventDefault();
            }
            return isOk;
        },
        addMoney: function (goal, isHistory) {

            GoalAddMoneyVue.addMoney(goal, isHistory);
        },
    }
});

