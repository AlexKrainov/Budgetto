﻿var ReminderVue = new Vue({
    el: "#reminder-vue",
    data: {
        reminders: [],
        reminder: {},
        dateTime: null,
        flatpickrReminder: {},

        searchText: null,
        isSaving: false,
        isShowModal: false,
        isNew: true,
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
    },
    mounted: function () {
    },
    methods: {
        showReminders: function (dateTime) {
            this.dateTime = dateTime;
            this.close();

            return this.loadTimeLine(dateTime);
        },
        addReminders: function (dateTime) {
            this.dateTime = dateTime;
            this.close();

            this.edit();

            return this.loadTimeLine(dateTime);
        },
        loadTimeLine: function (date) {
            return $.ajax({
                type: "GET",
                url: "/Reminder/LoadReminders?currentDate=" + date,
                contentType: "application/json",
                dataType: 'json',
                context: this,
                success: function (response) {
                    this.reminders = response.data;
                    $("#modalReminderTimeLine").modal("show");
                }
            });
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

        closeTimeline() {
            this.clearAllStyle();
        },

        GetDateByFormat: function (date, format) {
            return GetDateByFormat(date, format);
        },
        edit: function (reminder) {
            this.isShowModal = true;

            this.reminder.title = null;
            this.reminder.description = null;
            this.reminder.cssIcon = null;
            this.reminder.isRepeat = false;
            this.reminder.repeatEvery = null;

            if (reminder) {
                this.isNew = false;
                this.reminder = { ...reminder };

                let config = GetFlatpickrRuConfig(this.reminder.dateReminder);
                this.flatpickrReminder = flatpickr('#reminderDateReminder', config);
            } else {
                this.isNew = true;
                this.reminder.dateReminder = this.dateTime;

                let config = GetFlatpickrRuConfig(this.dateTime);
                this.flatpickrReminder = flatpickr('#reminderDateReminder', config);
            }

            this.chooseReminderIcon(this.reminder.cssIcon);
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
                        let index = this.reminders.findIndex(x => x.id == response.data.id);
                        //Check date
                        if (index == -1) {
                            this.reminders.push(response.data);
                        } else {
                            this.reminders[index] = response.data;
                        }
                        BudgetVue.refresh("onlyTable");

                        this.close();
                    } else {

                    }

                    this.isSaving = false;
                },
                error: function () {
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

            return isOk;
        },
        remove: function (reminder) {

            ShowLoading('#reminder_' + reminder.id);

            return $.ajax({
                type: "POST",
                url: "/Reminder/Remove",
                data: JSON.stringify(reminder),
                context: reminder,
                contentType: "application/json",
                dataType: 'json',
                success: function (response) {
                    reminder.isDeleted = response.isDeleted;
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
                    reminder.isDeleted = !response.isRecovery;
                    HideLoading('#reminder_' + reminder.id);
                    BudgetVue.refresh("onlyTable");
                }
            });
        },
    }
});

Vue.config.devtools = true;