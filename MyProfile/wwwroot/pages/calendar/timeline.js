const dateFormatForServer = "DD.MM.YYYY HH:mm",
    dateFormatForCalendar = "DD.MM.YYYY",
    dateFormatForTimeLine = "DD.MM.YYYY";
// https://cal-heatmap.com/

var vueTimeline = new Vue({
    el: "#timeline_records",
    data: {
        records: [],
        currentDate: "",
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
            tags: []
        },
        searchSection: null,
        countRefresh: 0,

        tagify: null,
    },
    updated: function () {
        this.$nextTick(function () {
            // Code that will run only after the
            // entire view has been re-rendered

            //vueTimeline.after_loading(false);// worked very bad
        });
    },
    mounted: function () {
        this.init_filter();
    },
    methods: {
        init_filter: function () {
            $.ajax({
                type: "GET",
                url: "/Section/GetSectins",
                data: null,
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                context: this,
                success: function (result) {
                    if (result.isOk) {
                        this.sections = result.sections;
                    }
                    this.serialize(true);
                    return result;
                },
                error: function (xhr, status, error) {
                    console.log(error);
                }
            }, this);

            $.ajax({
                type: "GET",
                url: "/Common/GetUserTags",
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                context: this,
                success: function (response) {
                    if (response.isOk) {

                        this.userTags = response.tags;

                        this.tagify = new Tagify(document.querySelector('input[name="tags"]'), {
                            whitelist: this.userTags,
                            enforceWhitelist: true,
                            delimiters: null,
                            dropdown: {
                                maxItems: 20,           // <- mixumum allowed rendered suggestions
                                classname: "tags-look", // <- custom classname for this dropdown, so it could be targeted
                                enabled: 0,             // <- show suggestions on focus
                                closeOnSelect: false    // <- do not hide the suggestions dropdown once an item has been selected
                            },
                            callbacks: {
                                remove: this.removeTag
                            }
                        });
                        this.selectAllTags();
                    }
                    return response;
                },
                error: function (xhr, status, error) {
                    console.log(error);
                }
            });
        },

        loadingDatesForCalendar: function () {
            vueTimeline.countRefresh += 1;

            setTimeout(
                function () {
                    if (vueTimeline.countRefresh <= 1) {

                        vueTimeline.records = [];
                        vueTimeline.currentDate = "";
                        vueTimeline.serialize(true);

                        vueTimeline.countRefresh = 0;
                    } else {
                        vueTimeline.countRefresh -= 1;
                    }
                },
                1000);
        },
        init: function () {
            vueTimeline.records = [];
            vueTimeline.currentDate = "";
            this.loading_records();
        },
        loading_records: function () {
            this.toggleSpinner(true);
            return $.ajax({
                type: "POST",
                url: "/Budget/LoadingRecordsForCalendar",
                data: JSON.stringify(vueTimeline.filter),
                contentType: "application/json",
                dataType: 'json',
                success: function (response) {
                    vueTimeline.draw_records(response);

                },
                error: function () {
                    vueTimeline.toggleSpinner(false);
                    toastr.error("Ошибка сервера. Повторите запрос позже.");
                }
            });
        },
        after_loading: function (isShowSpinner) {
            vueTimeline.toggleSpinner(isShowSpinner);
        },
        draw_records: function (response) {
            if (response.isOk == false) {
                return;
            }
            setTimeout(function () {
                vueTimeline.add(response.data);

                vueTimeline.after_loading(false);
            }, 400);
        },
        toggleSpinner: function (isShowSpinner) {
            if (isShowSpinner) {
                ShowLoading('#filter');
            } else {
                HideLoading('#filter');
            }
        },

        serialize: function (isReloadCalendar) {

            this.filter.sections = this.sections.filter(x => x.isSelected).map(x => x.id);
            this.filter.tags = this.userTags.filter(x => x.isShow == false).map(x => x.id);
            this.filter.Year = $(".active[data-year]").attr("data-year");

            if (this.filter.Year > 0 && isReloadCalendar) {
                calendar.before_loading(this.filter.Year);
            }
        },

        changeSwitch: function (val) {
            vueTimeline.loadingDatesForCalendar();
        },
        onChooseSection: function (section) {
            vueTimeline.loadingDatesForCalendar();
        },

        selectAllTags: function () {
            this.userTags = this.userTags.map(function (item) {
                item.isShow = false;
                return item;
            });
            this.tagify.addTags(this.userTags);
            vueTimeline.loadingDatesForCalendar();
        },
        unselectAllTags: function () {
            this.tagify.removeAllTags();
            for (var i = 0; i < this.userTags.length; i++) {
                this.userTags[i].isShow = true;
            }
            vueTimeline.loadingDatesForCalendar();
        },
        selectedTag: function (tag) {
            this.tagify.addTags([tag]);
            tag.isShow = false;
            vueTimeline.loadingDatesForCalendar();
        },
        removeTag: function (event) {
            console.log(event);
            if (event.detail.data && event.detail.data.id) {
                let removeIndex = this.userTags.findIndex(x => x.id == event.detail.data.id);
                if (removeIndex >= 0) {
                    this.userTags[removeIndex].isShow = true;
                }
            }
            vueTimeline.loadingDatesForCalendar();
        },
        selectAll: function () {
            for (var i = 0; i < this.sections.length; i++) {
                this.sections[i].isSelected = true;
            }
            vueTimeline.loadingDatesForCalendar();
        },
        unselectAll: function () {
            for (var i = 0; i < this.sections.length; i++) {
                this.sections[i].isSelected = false;
            }
            vueTimeline.loadingDatesForCalendar();
        },
        selectOnlyType: function (sectionTypeID) {
            for (var i = 0; i < this.sections.length; i++) {
                this.sections[i].isSelected = this.sections[i].sectionTypeID == sectionTypeID;
            }
            vueTimeline.loadingDatesForCalendar();
        },

        add: function (arr) {
            for (var i = 0; i < arr.length; i++) {
                arr[i].delay = 0;
                arr[i].date = "";

                // #region date
                let date = moment(arr[i].dateTimeOfPayment).format(dateFormatForTimeLine);

                if (date != this.currentDate) {
                    this.currentDate = date
                    arr[i].date = date;
                }

                // #endregion

                this.records.push(arr[i]);
            }
        },
        titleClick: function (event, eventID, latitude, longitude, address) {
            latitude = latitude * 1;
            longitude = longitude * 1;
            if (latitude == 0 && longitude == 0 && !address) {
                return;
            }
        },
        GetDateByFormat: function (date, format) {
            return GetDateByFormat(date, format);
        },
        edit: function (record) {
            RecordVue.editByElement(record, calendar.calendar_day_click_after_change_record);

            //RecordVue.recordComponent.edit(record.id);
        },
        descriptionBuilder: function (record) {
            return TagBuilder.toDescription(record);
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
                    //calendar.after_loading(response);
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
                    //calendar.after_loading(response);
                }
            });
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
        }
    }
});

var calendar = {
    calHeatMap: undefined,
    range: 12,
    sizes: [],
    calendar_day_click_after_change_record: function (dateTimeOfPayment) {
        dateTimeOfPayment = moment(dateTimeOfPayment);
        calendar.calendar_day_click(dateTimeOfPayment.date(), dateTimeOfPayment.get("months"), dateTimeOfPayment.get("year"));
        calendar.before_loading($("button[data-year=" + moment().get("year") + "]"), moment().get("year"));
    },
    calendar_day_click: function (day, month, year) {
        vueTimeline.serialize(false);
        vueTimeline.filter.StartDate = moment(new Date(year, month, day, 00, 00)).format();//dateFormatForServer);
        vueTimeline.filter.EndDate = moment(new Date(year, month, day, 23, 59, 59)).format();//dateFormatForServer);
        vueTimeline.init();
    },
    before_loading: function (year) {

        $("[data-year]").removeClass("active");
        $("[data-year=" + year + "]").addClass("active");

        if (calendar.calHeatMap != undefined) {
            calendar.calHeatMap.destroy();
            $("#cal-heatmap").empty();
        }

        vueTimeline.serialize(false);
        calendar.loading_dates();
    },
    loading_dates: function () {
        vueTimeline.toggleSpinner(true);
        return $.ajax({
            type: "POST",
            url: "/Budget/GetCountRecordsByYear",
            data: JSON.stringify(vueTimeline.filter),
            contentType: "application/json",
            dataType: 'json',
            success: function (response) {
                calendar.after_loading(response);
                vueTimeline.toggleSpinner(false);
            }
        });
    },
    after_loading: function (response) {
        var parser = function (data) { //формируем объект ввида { 12341234: 4, 412314212: 10 }, где 12341234 дата в формате (unix, Timestamp (seconds)), и 4 - count
            let stats = {};
            let d, c;
            let all_dates_in_year = calendar.getAllDatesInYear(response.year);

            jQuery.each(all_dates_in_year, function (index, element) {

                let element_from_server = data.find(x => moment(x.date).format("YYYY-MM-DD") == element.dateFormat);

                d = moment(element.date).unix(); //переводим все полученный с сервера даты в Timestamp (seconds)

                if (element_from_server == undefined) {
                    c = 0; // если count будет  ноль, тогда для этих ячеек будет применять цвет: { empty: "#f3f3f4" }
                } else {
                    c = element_from_server.count;
                }
                stats[d] = c;
            });

            return stats;
        };

        // #region range button show\hide
        let nextSelector = "#domainDynamicDimension-next",
            previousSelector = "#domainDynamicDimension-previous";

        if (calendar.range == 12) {
            nextSelector = false;
            previousSelector = false;
            $("#btn_move_calendar").hide();
        } else {
            $("#btn_move_calendar").show();
        }

        // #endregion
        let itemName = [];
        if ($("#isAmount").prop("checked")) {
            itemName = ["₽", "₽"]
        } else {
            itemName = ["запись", "записей"]
        }

        calendar.calHeatMap = new CalHeatMap();
        calendar.calHeatMap.init({
            start: new Date(response.year, 0),
            range: calendar.range,
            //itemSelector: "#cal-heatmap", by default
            domain: "month",
            subDomain: "day",
            cellSize: 15, //20, //12,
            cellRadius: 3,
            domainGutter: 5, //Space between each domain, in pixel
            //animationDuration: 800,
            subDomainTextFormat: "%d", //set day number
            considerMissingDataAsZero: true,
            //domainDynamicDimension: false,// расстояние между месяцами
            data: response.dates, // Dates Array
            afterLoadData: parser, // Parser function
            highlight: ["now"], //отображаем сегодняшнее число 
            displayLegend: false,// true,
            legend: response.legend, // [5, 7, 9, 11, 13, 15, 17, 19, 21, 23, 25, 27, 29, 31, 33, 35, 37, 39],
            legendColors: {
                min: "#DAF1FF", //ffffff",
                max: "#26B4FF",
                empty: "#f3f3f4", //"#EFF7FB",
                base: "#DAF1FF",
                overflow: "#DAF1FF"
                // Will use the CSS for the missing keys
            },
            tooltip: true,
            nextSelector: nextSelector,
            previousSelector: previousSelector,
            subDomainDateFormat: function (date) {
                return moment(date).format(dateFormatForCalendar); // Use the moment library to format the Date
            },
            itemName: itemName,
            onClick: calendar.day_click,
            onComplete: calendar.onComplete,
            domainLabelFormat: calendar.domainLabelFormat,
        });
    },
    day_click: function (date, count) {
        if (count == 0) {
            return true;
        }
        let m = moment(date);
        calendar.calendar_day_click(m.get("date"), m.get("month"), m.get("year"));
    },
    onComplete: function () {
    },
    domainLabelFormat: function (date) {
        return moment(date).format("MMMM");
    },
    getAllDatesInYear: function (year) {
        let dates = [];
        for (var month = 0; month <= 12; month++) {
            var date = new Date(year, month, 1);
            while (date.getMonth() === month) {
                dates.push({
                    date: moment(date),
                    count: 0,
                    dateFormat: moment(date).format("YYYY-MM-DD")
                });
                date.setDate(date.getDate() + 1);
            }
        }
        return dates;
    },
    resize: function () {
        let sizeContainer = parseInt($("#cal-heatmap-container").width());
        for (var i = 0; i < calendar.sizes.length; i++) {
            if (sizeContainer <= calendar.sizes[i].size.to && sizeContainer >= calendar.sizes[i].size.from) {
                calendar.range = calendar.sizes[i].size.range;
            }
        }
    },
}

$.getJSON("/json/timeline-calendar-resize.json", function () { })
    .done(function (json) {
        calendar.sizes = json.calendar_size;
        calendar.resize();
    });