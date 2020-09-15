var HistoryVue = new Vue({
    el: "#history-vue",
    data: {
        records: [],
        dateTime: null,
        dateStart: null,
        dateEnd: null,

        searchText: null,
        sections: [],
        flatpickrStart: null,
        flatpickrEnd: null,
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
                    || (record.userName && record.userName.toLocaleLowerCase().indexOf(newValue) >= 0)
                    || record.money.toString().indexOf(newValue) >= 0
                    || record.rawData.indexOf(newValue) >= 0;
            }
        }
    },
    mounted: function () {
        $("#history-sections").select2();

        //$("#historyCollapse").change(function () {

        //});
    },
    methods: {
        showHistory: function (filter, dateTime) {
            this.dateTime = dateTime;//?
            this.unselectAll();

            return this.loadTimeLine(filter);
        },
        search: function () {
            let filter = {
                sections: $("#history-sections").val(),
                startDate: moment(this.flatpickrStart.latestSelectedDateObj).format(),
                endDate: moment(this.flatpickrEnd.latestSelectedDateObj).format(),
            };
            return this.loadTimeLine(filter);
        },
        loadTimeLine: function (filter) {

            this.dateStart = filter.startDate;
            this.dateEnd = filter.endDate;

            let dateConfig = GetFlatpickrRuConfig(moment(this.dateStart, "YYYY/MM/DD").toDate());
            //dateConfig.minDate = this.dateEnd;
            this.flatpickrStart = flatpickr('#dateHistoryStart', dateConfig);
            var dateConfig2 = GetFlatpickrRuConfig(moment(this.dateEnd, "YYYY/MM/DD").toDate());
            //dateConfig2.maxDate = this.dateStart;
            this.flatpickrEnd = flatpickr('#dateHistoryEnd', dateConfig2);

            ShowLoading('.records-timeline');

            return $.ajax({
                type: "POST",
                url: "/Budget/LoadingRecordsForTableView",
                data: JSON.stringify(filter),
                contentType: "application/json",
                dataType: 'json',
                context: this,
                success: function (response) {
                    this.records = response.data;
                    this.sections = response.sections;
                    $("#history-sections").val(this.sections
                        .filter(x => x.selected)
                        .map(x => x.id))
                        .trigger("change");

                    HideLoading('.records-timeline');
                    $("#modalTimeLine").modal("show");
                }
            });
        },
        selectAll: function () {
            $("#history-sections").val(this.sections
                .map(x => x.id))
                .trigger("change");
        },
        unselectAll: function () {
            $("#history-sections").val(null).trigger("change");
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
        },
        remove: function (record) {
            ShowLoading('#record_' + record.id);

            return $.ajax({
                type: "POST",
                url: "/Budget/RemoveRecord",
                data: JSON.stringify(record),
                context: record,
                contentType: "application/json",
                dataType: 'json',
                success: function (response) {
                    record.isDeleted = response.isOk;
                    HideLoading('#record_' + record.id);
                    BudgetVue.refreshAfterChangeRecords()
                    //calendar.after_loading(response);
                }
            });
        },
    }
});
