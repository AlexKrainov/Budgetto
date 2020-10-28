var GoalAddMoneyVue = new Vue({
    el: "#goal-add-money",
    data: {
        goal: {},
        flatpickrStart: {},

        record: {},
        isSaving: false,
        showHistoryItems: 0,
        isShowButtonHistoryItems: true,
        isGanaral: true,
    },
    methods: {
        getDateByFormat: function (date, format) {
            return GetDateByFormat(date, format);
        },
        addMoney: function (goal, isHistory) {
            this.record = {
                goalID: goal.id,
                dateTimeOfPayment: GetDateByFormat(moment(), "YYYY/MM/DD"),
            };

            this.showHistoryItems = 0;
            this.isGanaral = isHistory == true ? false : true;
            this.goal = goal;
            this.showHistory(10);

            let dateConfig = GetFlatpickrRuConfig(
                this.record.dateTimeOfPayment,
                GetDateByFormat(goal.dateStart, "YYYY/MM/DD"),
                GetDateByFormat(goal.dateEnd, "YYYY/MM/DD"));
            this.flatpickrStart = flatpickr('#dateTimeOfPayment', dateConfig);

            $("#goal-add-money").modal("show");
        },
        saveMoney: function () {
            if (this.checkAddForm() == false) {
                return false;
            }
            this.isSaving = true;

            sendAjax("/Goal/SaveRecord", this.record, "POST")
                .then(function (result) {
                    if (result.isOk == true) {

                        if (typeof (GoalListVue) == "object") {
                            GoalListVue.load();
                        } else if (typeof (BudgetVue) == "object") {
                            BudgetVue.refresh("onlyGoal");
                        }
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
                        $("#goal-add-money").modal("hide");
                    } else {
                        console.log(result.message);
                    }
                    GoalAddMoneyVue.isSaving = false;
                });
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
    }
});








