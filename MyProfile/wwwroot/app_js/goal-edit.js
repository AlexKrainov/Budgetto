var GoalEditVue = new Vue({
    el: "#goal-edit-vue",
    data: {
        //edit
        goal: {},
        flatpickrStart: {},
        flatpickrEnd: {},

        record: {},
        flatpickrDateTime: {},
        isSaving: false,
        showHistoryItems: 0,
        isShowButtonHistoryItems: true,
        isGanaral: true,
    },
    watch: {},
    mounted: function () {
    },
    methods: {
        getDateByFormat: function (date, format) {
            return GetDateByFormat(date, format);
        },
        edit: function (goal, isHistory) {
            this.showHistoryItems = 0;
            this.isGanaral = !(isHistory == true);
            if (goal) {
                this.goal = { ...goal };
                this.showHistory(10);
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
        save: function () {
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
        showHistory: function (count) {
            this.showHistoryItems += count;

            for (var i = 0; i < this.goal.records.length; i++) {
                if (this.showHistoryItems == i) {
                    return;
                }
                this.goal.records[i].isShow = true;
            }
            this.isShowButtonHistoryItems = false;
        },
        addMoney: function (goal) {
            this.record = {
                goalID: goal.id,
                dateTimeOfPayment: GetDateByFormat(moment(), "YYYY/MM/DD"),
            };

            this.goal = goal;

            let dateConfig = GetFlatpickrRuConfig(this.record.dateTimeOfPayment, GetDateByFormat(this.goal.dateStart, "YYYY/MM/DD"),
                GetDateByFormat(this.goal.dateEnd, "YYYY/MM/DD"));
            this.flatpickrStart = flatpickr('#dateTimeOfPayment', dateConfig);

            $("#modal-goal-add-money").modal("show");
        },
        saveMoney: function () {
            if (this.checkAddForm() == false) {
                return false;
            }
            this.isSaving = true;

            return sendAjax("/Goal/SaveRecord", this.record, "POST")
                .then(function (result) {
                    if (result.isOk == true) {

                        GoalListVue.load();
                        //let goalIndex = GoalEditVue.goals.findIndex(x => x.id == result.goal.id);
                        //if (goalIndex >= 0) {
                        //    GoalEditVue.goals[goalIndex] = result.goal;
                        //} else {
                        //    GoalEditVue.goals.push(result.goal);
                        //    setTimeout(function () {
                        //        GoalListVue.msnry.addItems($("div[data-id=goal_" + result.goal.id + "]"));
                        //        GoalListVue.msnry.layout();
                        //    }, 100);
                        //}
                        $("#modal-goal-add-money").modal("hide");
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
            }
            if (!(this.goal.expectationMoney && (this.goal.expectationMoney > 0 || this.goal.expectationMoney.length > 0))) {
                isOk = false;
            }
            if (isOk == false && e) {
                e.preventDefault();
            }
            return isOk;
        },
        checkAddForm: function (e) {
            let isOk = true;

            if (!(this.record.dateTimeOfPayment && this.record.dateTimeOfPayment.length > 0)) {
                isOk = false;
            }
            if (!(this.record.total && (this.record.total > 0 || this.record.total.length > 0))) {
                isOk = false;
            }
            if (isOk == false && e) {
                e.preventDefault();
            }
            return isOk;
        },
    }
});

Vue.config.devtools = true;
