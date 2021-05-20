var HistoryVue = new Vue({
    el: "#history-vue",
    data: {
       
        searchText: null,
        sections: [],
        userTags: [],

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
    },
    watch: {
        searchText: function (newValue, oldValue) {
            this.historyComponent.searchText = newValue;
        }
    },
    computed: {
        historyComponent: function () {
            return this.$children[1];
        },
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
                console.log("TEST");
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

            let start = moment().startOf('month');
            let end = moment();

            $('#daterange').daterangepicker(
                GetDateRangePickerRuConfig(start, end),
                HistoryVue.setDateRange);

            HistoryVue.setDateRange(start, end);

            if (document.location.href.toLocaleLowerCase().indexOf("history/records") >= 0) {
                HistoryVue.search();
            }
        }, 1000);

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

            let start = moment(filter.startDate);
            let end = moment(filter.endDate);

            $('#daterange').daterangepicker(
                GetDateRangePickerRuConfig(start, end),
                HistoryVue.setDateRange);

            HistoryVue.setDateRange(start, end);

            this.search();
        },
        showLastHistory: function () {
            if ($("#modal-record").hasClass("show") == false) {
                this.search();
            }
        },
        search: function () {
            var drp = $('#daterange').data('daterangepicker');

            this.filter.sections = this.sections.filter(x => x.isSelected).map(x => x.id);
            this.filter.tags = this.userTags.filter(x => x.isShow == false).map(x => x.id);
            this.filter.startDate = drp.startDate.format();
            this.filter.endDate = drp.endDate.format();

            return this.historyComponent.loadHistory(this.filter);
        },
        setDateRange: function (start, end) {
            $('#daterange span').html(start.format('D MMMM YYYY') + ' - ' + end.format('D MMMM YYYY'));
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
        GetDateByFormat: function (date, format) {
            return GetDateByFormat(date, format);
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
    }
});