var HistoryVue = new Vue({
    el: "#history-vue",
    data: {
        records: [],
        dateTime: null,

        searchText: null,
    },
    watch: {
        searchText: function (newValue, oldValue) {
            if (newValue) {
                newValue = newValue.toLocaleLowerCase();
            }

            for (var i = 0; i < this.records.length; i++) {
                let record = this.records[i];

                record.isShowForFilter = record.sectionName.toLocaleLowerCase().indexOf(newValue) >= 0
                    || (record.description && record.description.toLocaleLowerCase().indexOf(newValue) >= 0)
                    || record.areaName.toLocaleLowerCase().indexOf(newValue) >= 0
                    || (record.userName && record.userName.toLocaleLowerCase().indexOf(newValue) >= 0);
            }
        }
    },
    mounted: function () {
    },
    methods: {
        showHistory: function (filter, dateTime) {
            this.dateTime = dateTime;

            return this.loadTimeLine(filter);
        },
        loadTimeLine: function (filter) {
            return $.ajax({
                type: "POST",
                url: "/Budget/LoadingRecordsForTableView",
                data: JSON.stringify(filter),
                contentType: "application/json",
                dataType: 'json',
                context: this,
                success: function (response) {
                    this.records = response.data;
                    $("#modalTimeLine").modal("show");
                }
            });
        },
        closeTimeline() {
            this.clearAllStyle();
        },
        getCurrencyValue: function (record) {
            let value = new Intl.NumberFormat(UserInfo.Currency.SpecificCulture, { style: 'currency', currency: UserInfo.Currency.CodeName }).format(record.money);
            if (UserInfo.Currency.ID != record.currencyID) {
                try {
                    value += " (" + new Intl.NumberFormat(record.currencySpecificCulture, { style: 'currency', currency: record.currencyCodeName }).format(CalculateExpression(record.tag)) + ")";
                } catch (e) {
                }
            }
            return value;
        },
        GetDateByFormat: function (date, format) {
            return GetDateByFormat(date, format);
        },
        add: function () {
            $("#modalTimeLine").modal("hide");
            return RecordVue.showModel(this.dateTime, 'BudgetVue.refreshAfterChangeRecords');
        },
        edit: function (record) {
            RecordVue.editByElement(record, BudgetVue.refreshAfterChangeRecords);

            $("#modalTimeLine").modal("hide");
        }
    }
});
