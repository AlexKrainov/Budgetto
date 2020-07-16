const dateFormatForFilter = "DD.MM.YYYY HH:mm",
    dateFormatForServer = "DD.MM.YYYY HH:mm",
    dateFormatForCalendar = "DD.MM.YYYY",
    dateFormatForTimeLine = "DD.MM.YYYY",
    datetimeFormatForTimeLine = "DD.MM.YYYY HH:mm:ss",
    timeFormatForTimeLine = "HH:mm:ss",
    timeFormatForTimeLineDelay = "mm:ss:ms",
    separatorDateRange = " - ";

var vueTimeline;
var gMapOptions;
// https://cal-heatmap.com/

$(function () {
    filter.init_filter();

    //WinMove(); // draggble .ibox

    vueTimeline = new Vue({
        el: "#timeline_records",
        data: {
            records: [],
            currentDate: ""
        },
        updated: function () {
            this.$nextTick(function () {
                // Code that will run only after the
                // entire view has been re-rendered

                //timeline.after_loading(false);// worked very bad
            });
        },
        methods: {
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
                RecordVue.recordComponent.editByElement(record, timeline.calendar_day_click_after_change_record);

                //RecordVue.recordComponent.edit(record.id);
            },
            remove: function (record) {

                return $.ajax({
                    type: "POST",
                    url: "/Budget/RemoveRecord",
                    data: JSON.stringify(record),
                    context: record,
                    contentType: "application/json",
                    dataType: 'json',
                    success: function (response) {
                        record.isDeleted = response.isOk;
                        //calendar.after_loading(response);
                    }
                });
            },
            recovery: function(record) {
                return $.ajax({
                    type: "POST",
                    url: "/Budget/RecoveryRecord",
                    data: JSON.stringify(record),
                    context: record,
                    contentType: "application/json",
                    dataType: 'json',
                    success: function (response) {
                        record.isDeleted = !response.isOk;
                        //calendar.after_loading(response);
                    }
                });
            },
            onClickCollapse: function () {
                record.isShowCollapse = !record.isShowCollapse;
            }
        }
    });

    // #region button up, event click

    $(".go_to_up").click(function (event) {
        $("html, body").animate({
            scrollTop: 0
        }, "slow");
        event.preventDefault();
    })

    // #endregion

    // #region resize calendar

    timeline.search_click();
});

var filter = {
    date: {},
    data: {
        startDate: moment().subtract(6, 'days').startOf('day'),
        endDate: moment()
    },
    init_filter: function () {
        $("#Sections").select2();


        $("#filter").change(function () {
            timeline.search_click();
        });
    },
    serialize: function (isReloadCalendar) {
        filter.data = $("#filter").serializeObject();
        filter.data.Count = $("#Count").val('0').val();
        filter.data.isAmount = $("#isAmount").prop("checked");
        filter.data.isConsiderCollection = $("#isConsiderCollection").prop("checked");

        if ($("button[data-year=" + moment().get("year") + "]").length > 0 && isReloadCalendar) {
            calendar.before_loading($("button[data-year=" + moment().get("year") + "]"), moment().get("year"));
        }
    },
    selectAll: function () {
        $("#Sections option").prop("selected", true);
        $("#Sections").trigger("change");
    },
    unselectAll: function () {
        $("#Sections").val(null).trigger("change");
    }
}

var timeline = {
    isLoading: true, //flag for loading ajax query 
    search_click: function () {
        filter.serialize(true);
        timeline.init();
        //calendar.calHeatMap.destroy();
    },
    calendar_day_click_after_change_record(dateTimeOfPayment) {
        dateTimeOfPayment = moment(dateTimeOfPayment);
        timeline.calendar_day_click(dateTimeOfPayment.date(), dateTimeOfPayment.get("months"), dateTimeOfPayment.get("year"));
    },
    calendar_day_click: function (day, month, year) {
        filter.serialize(false);
        filter.data.StartDate = moment(new Date(year, month, day, 00, 00)).format();//dateFormatForServer);
        filter.data.EndDate = moment(new Date(year, month, day, 23, 59, 59)).format();//dateFormatForServer);
        timeline.init();
    },
    init: function () {
        this.isLoading = false;
        vueTimeline.records = [];
        vueTimeline.currentDate = "";
        this.up_element("_", "_");
        this.loading_records().then(function () {
            timeline.scroll_event();
        });
    },
    before_loading: function () {
        timeline.toggleSpinner(true);
        timeline.isLoading = true;
        $("#to_up").show();

        filter.data.Count = $("#Count").val();
    },
    loading_records: function () {
        timeline.before_loading();
        return $.ajax({
            type: "POST",
            url: "/Budget/LoadingRecordsForCalendar",
            data: JSON.stringify(filter.data),
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
        timeline.scroll_event_destroy();
        timeline.toggleSpinner(false);
    },
    draw_records: function (response) {
        $("#Count").val(vueTimeline.records.length + response.take);
        setTimeout(function () {
            vueTimeline.add(response.data);

            if (response.isEnd == true) {
                timeline.end_loading();
            } else {
                timeline.scroll_event();
            }
            timeline.after_loading(false);
        }, 400);

        timeline.up_element($("#Count").val(), response.count);
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

        //let _filter = $("#filter").serializeObject();
        //_filter.Year = year;
        filter.data.Year = year;
        calendar.loading_dates(filter.data);
    },
    loading_dates: function (_filter) {
        return $.ajax({
            type: "POST",
            url: "/Budget/GetCountRecordsByYear",
            data: JSON.stringify(_filter),
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
            displayLegend: true,
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
            itemName: ["запись", "записей"],
            onClick: calendar.day_click,
            onComplete: calendar.onComplete
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