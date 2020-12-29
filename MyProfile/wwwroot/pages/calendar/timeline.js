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
        filter: { Count: 0, isAmount: true, isConsiderCollection: false, isCount: false, Year: moment().get("year"), sections: [] },
        searchSection: null,
    },
    updated: function () {
        this.$nextTick(function () {
            // Code that will run only after the
            // entire view has been re-rendered

            //timeline.after_loading(false);// worked very bad
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
                    timeline.search_click();
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
                        var input = document.querySelector('input[name="tags"]');
                        var tagify = new Tagify(input, {
                            whitelist: this.userTags,
                            enforceWhitelist: true,
                            dropdown: {
                                maxItems: 20,           // <- mixumum allowed rendered suggestions
                                classname: "tags-look", // <- custom classname for this dropdown, so it could be targeted
                                enabled: 0,             // <- show suggestions on focus
                                closeOnSelect: false    // <- do not hide the suggestions dropdown once an item has been selected
                            }
                        });
                    }
                    return response;
                },
                error: function (xhr, status, error) {
                    console.log(error);
                }
            });
            //$("#filter").change(function () {
            //    timeline.search_click();
            //});
        },
        changeSwitch: function (val) {
            timeline.search_click();
        },
        onChooseSection: function (section) {
            timeline.search_click();
        },
        serialize: function (isReloadCalendar) {

            this.filter.sections = this.sections.filter(x => x.isSelected).map(x => x.id);

            if ($("button[data-year=" + moment().get("year") + "]").length > 0 && isReloadCalendar) {
                calendar.before_loading($("button[data-year=" + moment().get("year") + "]"), moment().get("year"));
            }
        },
        selectAll: function () {
            for (var i = 0; i < this.sections.length; i++) {
                this.sections[i].isSelected = true;
            }
        },
        selectAllTags: function () {
           
        },
        unselectAllTags: function () {

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
            RecordVue.editByElement(record, timeline.calendar_day_click_after_change_record);

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

var timeline = {
    isLoading: true, //flag for loading ajax query 
    countRefresh: 0,
    search_click: function () {
        timeline.countRefresh += 1;

        //if (timeline.countRefresh == 1) {
        setTimeout(
            function () {
                if (timeline.countRefresh <= 1) {
                    vueTimeline.serialize(true);
                    timeline.init();
                    timeline.countRefresh = 0;
                } else {
                    timeline.countRefresh -= 1;
                }
            },
            1000);
        //}
        //else {
        //    timeline.canRefresh = false;
        //    vueTimeline.serialize(true);
        //    timeline.init();
        //}
        //calendar.calHeatMap.destroy();
    },
    calendar_day_click_after_change_record: function (dateTimeOfPayment) {
        dateTimeOfPayment = moment(dateTimeOfPayment);
        timeline.calendar_day_click(dateTimeOfPayment.date(), dateTimeOfPayment.get("months"), dateTimeOfPayment.get("year"));
        calendar.before_loading($("button[data-year=" + moment().get("year") + "]"), moment().get("year"));
    },
    calendar_day_click: function (day, month, year) {
        vueTimeline.serialize(false);
        vueTimeline.filter.StartDate = moment(new Date(year, month, day, 00, 00)).format();//dateFormatForServer);
        vueTimeline.filter.EndDate = moment(new Date(year, month, day, 23, 59, 59)).format();//dateFormatForServer);
        timeline.init();
    },
    init: function () {
        this.isLoading = false;
        vueTimeline.records = [];
        vueTimeline.currentDate = "";
        this.up_element("_", "_");
        this.loading_records();
        //    .then(function () {
        //    timeline.scroll_event();
        //});
    },
    before_loading: function () {
        timeline.toggleSpinner(true);
        timeline.isLoading = true;
        $("#to_up").show();
    },
    loading_records: function () {
        timeline.before_loading();
        return $.ajax({
            type: "POST",
            url: "/Budget/LoadingRecordsForCalendar",
            data: JSON.stringify(vueTimeline.filter),
            contentType: "application/json",
            dataType: 'json',
            success: function (response) {
                timeline.draw_records(response);
            }
        });
    },
    after_loading: function (isShowSpinner) {
        timeline.toggleSpinner(isShowSpinner);
        timeline.isLoading = false;
    },
    end_loading: function () {
        timeline.isLoading = true;
        //timeline.scroll_event_destroy();
        timeline.toggleSpinner(false);
    },
    draw_records: function (response) {
        vueTimeline.filter.Count = vueTimeline.records.length + response.take;
        setTimeout(function () {
            vueTimeline.add(response.data);

            if (response.isEnd == true) {
                timeline.end_loading();
            } else {
                // timeline.scroll_event();
            }
            timeline.after_loading(false);
        }, 400);

        timeline.up_element(vueTimeline.filter.Count, response.count);
    },
    scroll_event: function () {
        $(window).scroll(function (event) {
            var scrollHeight = $(document).height();
            var scrollPosition = Math.ceil($(window).height() + $(window).scrollTop()); // округляем большую сторону (bug in FireFox)
            if (scrollPosition - scrollHeight === 0 && timeline.isLoading == false) {
                event.preventDefault();
                timeline.loading_records();
            }
        });
    },
    scroll_event_destroy: function () {
        $(window).off("scroll"); // stop scroll
    },
    up_element: function (have, count) {
        $("#to_up .badge").text(have + "/" + count);
    },
    toggleSpinner: function (isShowSpinner) {
        if (isShowSpinner) {
            ShowLoading('#filter');
        } else {
            HideLoading('#filter');
        }
    }
}

var calendar = {
    calHeatMap: undefined,
    range: 12,
    sizes: [],
    resize: function () {
        let sizeContainer = parseInt($("#cal-heatmap-container").width());
        for (var i = 0; i < calendar.sizes.length; i++) {
            if (sizeContainer <= calendar.sizes[i].size.to && sizeContainer >= calendar.sizes[i].size.from) {
                calendar.range = calendar.sizes[i].size.range;
            }
        }
    },
    before_loading: function (obj, year) {

        $(obj).parent().find("button").removeClass("active");
        $(obj).addClass("active")

        calendar.toggleSpinner(true);
        if (calendar.calHeatMap != undefined) {
            calendar.calHeatMap.destroy();
            $("#cal-heatmap").empty();
        }

        calendar.loading_dates();
    },
    loading_dates: function (_filter) {
        return $.ajax({
            type: "POST",
            url: "/Budget/GetCountRecordsByYear",
            data: JSON.stringify(vueTimeline.filter),
            contentType: "application/json",
            dataType: 'json',
            success: function (response) {
                calendar.after_loading(response);
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
        timeline.calendar_day_click(m.get("date"), m.get("month"), m.get("year"));
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
    toggleSpinner: function (isShowSpinner) {

    }
}

$.getJSON("/json/timeline-calendar-resize.json", function () { })
    .done(function (json) {
        calendar.sizes = json.calendar_size;
        calendar.resize();
    });