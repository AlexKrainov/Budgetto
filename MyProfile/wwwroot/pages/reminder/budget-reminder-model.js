var ReminderVue = new Vue({
    el: "#reminder-vue",
    data: {
        reminders: [],
        reminder: { isRepeat: false, notifications: [] },
        dateTime: null,
        dateTimeFinish: null,
        month: -1,
        year: -1,
        flatpickrReminder: {},
        timezones: [],

        searchText: null,
        isSaving: false,
        isShowModal: false,
        isNew: true,
        isPast: false,
        numberID: -1,
    },
    watch: {
        //searchText: function (newValue, oldValue) {
        //    if (newValue) {
        //        newValue = newValue.toLocaleLowerCase();
        //    }

        //    for (var i = 0; i < this.reminders.length; i++) {
        //        let reminder = this.reminders[i];

        //        reminder.isShowForFilter = reminder.sectionName.toLocaleLowerCase().indexOf(newValue) >= 0
        //            || (reminder.description && reminder.description.toLocaleLowerCase().indexOf(newValue) >= 0)
        //            || reminder.areaName.toLocaleLowerCase().indexOf(newValue) >= 0
        //            || (reminder.userName && reminder.userName.toLocaleLowerCase().indexOf(newValue) >= 0);
        //    }
        //}
        "reminder.isRepeat": function () {
            if (this.reminder.isRepeat && this.reminder.repeatEvery == undefined) {
                this.reminder.repeatEvery = "Month";
            }
        },
        dateTime: function (newValue) {
            this.isPast = moment() > moment(newValue);
        },
    },
    mounted: function () {
    },
    methods: {
        showReminders: function (dateTime) {
            this.dateTime = dateTime;
            this.month = null;
            this.year = null;
            this.dateTimeFinish = null;
            this.close();

            return this.loadTimeLine(dateTime);
        },
        showRemindersByPeriod: function (month, year) {
            this.dateTime = null;
            this.month = month;
            this.year = year;
            this.close();

            return this.loadTimeLine();
        },
        addReminders: function (dateTime) {
            this.dateTime = dateTime;
            // this.close();

            if (this.timezones.length == 0) {
                this.loadTimezone()
                    .then(function () {
                        this.edit();

                        this.loadTimeLine(dateTime);
                    });
            } else {
                this.edit();

                this.loadTimeLine(dateTime);
            }
        },
        loadTimeLine: function () {
            return $.ajax({
                type: "GET",
                url: "/Reminder/LoadReminders?currentDate=" + this.dateTime + "&month=" + this.month + "&year=" + this.year,
                contentType: "application/json",
                dataType: 'json',
                context: this,
                success: function (response) {
                    this.month = null;
                    this.year = null;
                    this.dateTime = response.currentDate;
                    this.dateTimeFinish = response.dateTimeFinish;
                    this.reminders = response.data;
                    $("#modalReminderTimeLine").modal("show");

                    this.loadTimezone();
                }
            });
        },
        loadTimezone: function () {
            if (this.timezones.length == 0) {
                return $.ajax({
                    type: "GET",
                    url: "/Common/GetTimeZone",
                    contentType: "application/json",
                    dataType: 'json',
                    context: this,
                    success: function (response) {
                        this.timezones = response.timezone;
                    }
                });
            }
            return null; //Promise.resolve()
        },
        chooseReminderIcon: function (cssIcon) {
            $(".reminder-icons i").removeClass("active");

            if (!cssIcon) {
                return true;
            }

            let newCssIcon = cssIcon.replace(" ", ".");
            $(".reminder-icons i." + newCssIcon).addClass("active");
            this.reminder.cssIcon = cssIcon;
        },

        closeTimeline: function () {
            this.clearAllStyle();
        },
        edit: function (reminder) {
            this.isShowModal = true;

            this.reminder = { isRepeat: false, notifications: [] },

                this.reminder.id = -1;
            this.reminder.title = null;
            this.reminder.description = null;
            this.reminder.cssIcon = null;
            this.reminder.isRepeat = false;
            this.reminder.repeatEvery = null;
            this.reminder.notifications = [];

            if (reminder) {
                this.isNew = false;
                this.reminder = JSCopyObject(reminder);
                this.reminder.offSetClient = new Date().getTimezoneOffset();
                this.reminder.timeZoneClient = Intl.DateTimeFormat().resolvedOptions().timeZone;

                let config = GetFlatpickrRuConfigWithTime(this.reminder.dateReminder);
                //config.onChange = this.onChangeDateTime;
                flatpickr('#reminderDateReminder', config);

                setTimeout(function () {
                    for (var i = 0; i < ReminderVue.reminder.notifications.length; i++) {
                        if (moment(ReminderVue.reminder.notifications[i].expirationDateTime) < moment()) {
                            flatpickr('#expirationDateTime_' + ReminderVue.reminder.notifications[i].id,
                                GetFlatpickrRuConfigWithTime(ReminderVue.reminder.notifications[i].expirationDateTime));
                        } else {
                            flatpickr('#expirationDateTime_' + ReminderVue.reminder.notifications[i].id,
                                GetFlatpickrRuConfigWithTime(ReminderVue.reminder.notifications[i].expirationDateTime), new Date);
                        }
                    }

                }, 1000);
            } else {
                this.isNew = true;
                this.dateTime = this.dateTime.replace("T00", "T" + new Date().getHours())
                this.reminder.dateReminder = this.dateTime;
                this.reminder.offSetClient = new Date().getTimezoneOffset();
                this.reminder.timeZoneClient = Intl.DateTimeFormat().resolvedOptions().timeZone;
                this.reminder.olzonTZID = this.getTimeZoneID(this.reminder.timeZoneClient);

                let config = GetFlatpickrRuConfigWithTime(this.dateTime);
                config.onChange = this.onChangeDateTime;
                flatpickr('#reminderDateReminder', config);
            }

            this.chooseReminderIcon(this.reminder.cssIcon);
        },
        //onChangeDateTime: function (selectedDates, dateStr, instance) {
        //    this.isPast = moment() > moment(dateStr);
        //},
        getTimeZoneID: function (timezone) {
            let index = this.timezones.findIndex(x => x.olzonTZName == timezone);

            if (index != -1) {
                return this.timezones[index].olzonTZID;
            } else {
                index = this.timezones.findIndex(x => x.olzonTZName == "Europe/Moscow");
                return this.timezones[index].olzonTZID;
            }
        },
        close: function () {
            this.isShowModal = false;
            this.reminder = { isRepeat: false };
            this.chooseReminderIcon();
        },

        save: function () {
            //HideLoading('#record_' + reminder.id);

            if (this.checkValid() == false) {
                return false;
            }

            this.isSaving = true;

            return $.ajax({
                type: "POST",
                url: "/Reminder/Edit",
                data: JSON.stringify(this.reminder),
                context: this,
                contentType: "application/json",
                dataType: 'json',
                success: function (response) {

                    if (response.isOk) {
                        if (this.reminder.id > 0) {
                            let index = this.reminders.findIndex(x => x.id == response.data.id);
                            this.reminders[index] = response.data;
                        } else {
                            this.reminders.push(response.data);
                        }
                        BudgetVue.refresh("onlyTable");
                        BudgetVue.refresh("only-progress");

                        this.close();
                    }
                    this.isSaving = false;
                },
                error: function (error) {
                    console.log(error);
                    this.isSaving = false;

                }
            });
        },
        checkValid: function () {
            let isOk = true;
            $("#reminderTitle, #reminderDateReminder").removeClass("is-invalid")

            if (!(this.reminder.title && this.reminder.title.length > 0)) {
                isOk = false;
                $("#reminderTitle").addClass("is-invalid")
            }

            let str = this.reminder.title;
            str = str ? str.replaceAll(" ", "") : "";
            if (str.length == 0) {
                isOk = false;
                $("#reminderTitle").addClass("is-invalid");
            } else {
                $("#reminderTitle").removeClass("is-invalid");
            }

            if (!(this.reminder.cssIcon && this.reminder.cssIcon.length > 0)) {
                isOk = false;
                $("#reminderIcons").css("display", "block");
            } else {
                $("#reminderIcons").css("display", "none");
            }

            if (!(this.reminder.dateReminder && this.reminder.dateReminder.length > 0)) {
                isOk = false;
                $("#reminderDateReminder").addClass("is-invalid")
            }

            if (this.reminder.isRepeat && this.reminder.repeatEvery == null) {
                $("#repeatType").addClass("is-invalid");
                isOk = false;
            } else {
                $("#repeatType").removeClass("is-invalid");
            }

            if (this.reminder.notifications && this.reminder.notifications.length > 0) {
                //if (this.reminder.notifications.some(x => x.isMail == false && x.isSite == false && x.isTelegram == false && x.isDeleted == false)
                //    || this.reminder.notifications.some(x => (x.expirationDateTime * 1) <= 0 && x.isDeleted == false)) {

                for (var i = 0; i < this.reminder.notifications.length; i++) {
                    let notification = this.reminder.notifications[i];
                    if ((notification.isMail == false && notification.isSite == false && notification.isTelegram == false && notification.isDeleted == false)
                        || (!notification.expirationDateTime && notification.isDeleted == false)) {

                        $("#errorborder_" + notification.id).addClass("border-danger");
                        $("#texterror_" + notification.id).show();
                        isOk = false;
                    } else {

                        $("#errorborder_" + notification.id).removeClass("border-danger");
                        $("#texterror_" + notification.id).hide();
                    }
                }
                //} else {

                //}
            }

            return isOk;
        },
        remove: function (reminder) {
            if (reminder.isWasRepeat && reminder.isRepeat == false) {
                toastr.warning("Внимание! Если вы удалите это напоминание, удалится вся цепочка напоминаний.");
                reminder.isWasRepeat = false;
                return;
            }
            ShowLoading('#reminder_' + reminder.id);

            return $.ajax({
                type: "POST",
                url: "/Reminder/Remove",
                data: JSON.stringify(reminder),
                context: this,
                contentType: "application/json",
                dataType: 'json',
                success: function (response) {
                    let index = this.reminders.findIndex(x => x.id == response.data.id);
                    this.reminders[index].isDeleted = response.isDeleted;
                    //reminder.isDeleted = response.isDeleted;
                    //bug, don't change view removed/recovered
                    //ReminderVue.$forceUpdate();

                    if (this.reminder.id == reminder.id) {
                        this.close();
                    }
                    HideLoading('#reminder_' + reminder.id);
                    BudgetVue.refresh("onlyTable");
                }
            });
        },
        recovery: function (reminder) {

            ShowLoading('#reminder_' + reminder.id);

            return $.ajax({
                type: "POST",
                url: "/Reminder/Recovery",
                data: JSON.stringify(reminder),
                context: reminder,
                contentType: "application/json",
                dataType: 'json',
                success: function (response) {
                    let index = this.reminders.findIndex(x => x.id == response.data.id);
                    this.reminders[index].isDeleted = !response.isRecovery;
                    //reminder.isDeleted = !response.isRecovery;
                    HideLoading('#reminder_' + reminder.id);
                    BudgetVue.refresh("onlyTable");
                }
            });
        },
        GetDateByFormat: function (date, format) {
            return GetDateByFormat(date, format);
        },
        addNotification: function () {
            this.numberID -= 1;
            this.reminder.notifications.push(
                {
                    isSite: false,
                    isMail: false,
                    isTelegram: false,
                    expirationDateTime: this.reminder.dateReminder,
                    id: this.numberID,
                    isDeleted: false,
                    isRepeat: this.reminder.isRepeat,
                    repeatMinutes: 0,
                });

            setTimeout(function () {
                let index = ReminderVue.reminder.notifications.findIndex(x => x.id == ReminderVue.numberID);

                let config = GetFlatpickrRuConfigWithTime(ReminderVue.reminder.notifications[index].expirationDateTime, new Date);
                config.onChange = function (selectedDates, dateStr, instance) {
                    let duration = moment.duration(moment(dateStr).diff(moment(ReminderVue.reminder.dateReminder)));
                    let notificationID = instance.element.id.replace('expirationDateTime_', '') * 1;
                    let index = ReminderVue.reminder.notifications.findIndex(x => x.id == notificationID);
                    
                    ReminderVue.reminder.notifications[index].repeatMinutes = Math.round(duration.asMinutes() * -10) / 10;
                };
                flatpickr('#expirationDateTime_' + ReminderVue.reminder.notifications[index].id, config);

                $('[data-toggle="tooltip"]').tooltip();
                $("#notification-" + ReminderVue.reminder.notifications[index].id).addClass("show");
            }, 300);//300
        },
        removeNotification: function (notification) {
            if (notification.id < 0) {
                let index = this.reminder.notifications.findIndex(x => x.id == notification.id);
                this.reminder.notifications.splice(index, 1);
            } else {
                notification.isDeleted = true;
            }
        },
        //getMinutesFormat: function (notification) {
        //    return moment.utc().startOf('minutes').add({ minutes: notification.repeatMinutes }).format('H:mm');
        //}
    }
});


