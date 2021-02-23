
var HistoryVue = new Vue({
    el: "#history-vue",
    data: {
        records: [],
        spendingTotalMoney: 0,
        spendingErningMoney: 0,
        spendingInvestingMoney: 0,
        dateTime: null,
        filter: {
            isSection: true,
            Count: 0,
            isAmount: true,
            isConsiderCollection: false,
            isCount: false,
            Year: moment().get("year"),
            sections: [],
            tags: [],
            startDate: null,
            endDate: null
        },

        searchText: null,
        sections: [],
        userTags: [],
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
        },
        records: {
            handler: function (newValue, oldValue) {
                this.spendingTotalMoney = 0;
                this.spendingErningMoney = 0;
                this.spendingInvestingMoney = 0;

                for (var i = 0; i < this.records.length; i++) {
                    if (this.records[i].isDeleted == false) {
                        if (this.records[i].sectionTypeID == 1) { //Earnings
                            this.spendingErningMoney += this.records[i].money * 1;
                        } else if (this.records[i].sectionTypeID == 2) { //Spendings
                            this.spendingTotalMoney += this.records[i].money * 1;
                        } else if (this.records[i].sectionTypeID == 3) { //Investments
                            this.spendingInvestingMoney += this.records[i].money * 1;
                        }
                    }
                }
            },
            deep: true
        }
    },
    mounted: function () {
        setTimeout(function () {
            HistoryVue.sections = JSCopyArray(RecordVue.recordComponent.sectionComponent.sections);
            HistoryVue.userTags = JSCopyArray(RecordVue.recordComponent.userTags);

            HistoryVue.tagify = new Tagify(document.querySelector('input[name="tags"]'), {
                whitelist: HistoryVue.userTags,
                enforceWhitelist: true,
                delimiters: null,
                dropdown: {
                    maxItems: 20,           // <- mixumum allowed rendered suggestions
                    classname: "tags-look", // <- custom classname for this dropdown, so it could be targeted
                    enabled: 0,             // <- show suggestions on focus
                    closeOnSelect: false    // <- do not hide the suggestions dropdown once an item has been selected
                },
                callbacks: {
                    add: HistoryVue.addTag,
                    remove: HistoryVue.removeTag
                }
            });

            setTimeout(HistoryVue.unselectAllTags, 100);

            if (HistoryVue.sections.length == 0) {
                $.ajax({
                    type: "GET",
                    url: "/Section/GetSectins",
                    data: null,
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'json',
                    context: this,
                    success: function (result) {
                        if (result.isOk) {
                            HistoryVue.sections = result.sections;
                        }
                        return result;
                    },
                    error: function (xhr, status, error) {
                        console.log(error);
                    }
                }, this);
            }

        }, 2000);

        $('#modalTimeLine').on('hide.bs.modal', function () {
            $("#historyCollapse").removeClass("show");
        });
    },
    methods: {
        showHistory: function (filter, dateTime) {
            this.dateTime = dateTime;//?
            this.unselectAll();

            for (var i = 0; i < this.sections.length; i++) {
                this.sections[i].isSelected = filter.sections.some(x => x == this.sections[i].id);
            }

            this.filter.startDate = filter.startDate;
            this.filter.endDate = filter.endDate;

            let dateConfig = GetFlatpickrRuConfig(moment(this.filter.startDate, "YYYY/MM/DD").toDate());
            //dateConfig.minDate = this.dateEnd;
            this.flatpickrStart = flatpickr('#dateHistoryStart', dateConfig);
            var dateConfig2 = GetFlatpickrRuConfig(moment(this.filter.endDate, "YYYY/MM/DD").toDate());
            //dateConfig2.maxDate = this.dateStart;
            this.flatpickrEnd = flatpickr('#dateHistoryEnd', dateConfig2);

            this.search();
        },
        showLastHistory: function () {
            if ($("#modal-record").hasClass("show") == false) {
                this.search();
            }
        },
        search: function () {
            this.filter.sections = this.sections.filter(x => x.isSelected).map(x => x.id);
            this.filter.tags = this.userTags.filter(x => x.isShow == false).map(x => x.id);
            this.filter.startDate = moment(this.flatpickrStart.latestSelectedDateObj).format();
            this.filter.endDate = moment(this.flatpickrEnd.latestSelectedDateObj).format();

            return this.loadTimeLine();
        },
        loadTimeLine: function () {
            ShowLoading('.records-timeline');
            $("#modalTimeLine").modal("show");
            return $.ajax({
                type: "POST",
                url: "/Budget/LoadingRecordsForTableView",
                data: JSON.stringify(this.filter),
                contentType: "application/json",
                dataType: 'json',
                context: this,
                success: function (response) {
                    this.records = response.data;

                    HideLoading('.records-timeline');
                    //setTimeout(function () {
                    //    $('[data-toggle="tooltip"]').tooltip();
                    //}, 1000);
                }
            });
        },
        selectAll: function () {
            for (var i = 0; i < this.sections.length; i++) {
                this.sections[i].isSelected = true;
            }
        },
        unselectAll: function () {
            for (var i = 0; i < this.sections.length; i++) {
                this.sections[i].isSelected = false;
            }
        },
        selectOnlyType: function (sectionTypeID) {
            for (var i = 0; i < this.sections.length; i++) {
                this.sections[i].isSelected = this.sections[i].sectionTypeID == sectionTypeID;
            }
        },
        selectAllTags: function () {
            this.userTags = this.userTags.map(function (item) {
                item.isShow = false;
                return item;
            });
            this.tagify.addTags(this.userTags);
        },
        unselectAllTags: function () {
            this.tagify.removeAllTags();
            for (var i = 0; i < this.userTags.length; i++) {
                this.userTags[i].isShow = true;
            }
        },
        selectedTag: function (tag) {
            this.tagify.addTags([tag]);
            tag.isShow = false;
        },
        addTag: function (e) {
            let tag = e.detail.data;
            if (tag.isShow) {
                let index = this.userTags.findIndex(x => x.id == tag.id);
                if (index != -1) {
                    this.userTags[index].isShow = false;
                }
            }
        },
        removeTag: function (event) {
            if (event.detail.data && event.detail.data.id) {
                let removeIndex = this.userTags.findIndex(x => x.id == event.detail.data.id);
                if (removeIndex >= 0) {
                    this.userTags[removeIndex].isShow = true;
                }
            }
            // vueTimeline.loadingDatesForCalendar();
        },
        onChooseSection: function (section) {
            //vueTimeline.loadingDatesForCalendar();
        },
        closeTimeline: function () {
            this.clearAllStyle();
        },
        getCurrencyValue: function (record, money, isCashback) {
            let value = new Intl.NumberFormat(UserInfo.Currency.SpecificCulture, { style: 'currency', currency: UserInfo.Currency.CodeName }).format(money);
            if (UserInfo.Currency.ID != record.currencyID && isCashback == undefined) {
                try {
                    value += " (" + new Intl.NumberFormat(record.currencySpecificCulture, { style: 'currency', currency: record.currencyCodeName }).format(CalculateExpression(record.tag)) + ")";
                } catch (e) {
                }
            }
            return value;
        },
        descriptionBuilder: function (record) {
            return TagBuilder.toDescription(record);
        },
        GetDateByFormat: function (date, format) {
            return GetDateByFormat(date, format);
        },
        add: function () {
            $("#modalTimeLine").modal("hide");

            //setTimeout becase bug with focus
            setTimeout(
                RecordVue.showModal,
                750,
                this.dateTime,
                'BudgetVue.refreshAfterChangeRecords'
            );
        },
        edit: function (record) {
            $("#modalTimeLine").modal("hide");

            //setTimeout becase bug with focus
            setTimeout(
                RecordVue.editByElement,
                750,
                record,
                ['BudgetVue.refreshAfterChangeRecords', 'HistoryVue.showLastHistory']
            );
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
                    BudgetVue.refreshAfterChangeRecords(response.dateTimeOfPayment)
                }
            });
        },
        recovery: function (record) {
            ShowLoading('#record_' + record.id);
            return $.ajax({
                type: "POST",
                url: "/Budget/RecoveryRecord",
                data: JSON.stringify(record),
                context: record,
                contentType: "application/json",
                dataType: 'json',
                success: function (response) {
                    record.isDeleted = !response.isOk;
                    HideLoading('#record_' + record.id);
                    BudgetVue.refreshAfterChangeRecords(response.dateTimeOfPayment)
                }
            });
        },
    }
});
