var ReminderVue = new Vue({
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
        searchText: function (newValue, oldValue) {
            if (newValue) {
                newValue = newValue.toLocaleLowerCase();
            }

            for (var i = 0; i < this.reminders.length; i++) {
                let record = this.reminders[i];

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
        showReminders: function (dateTime) {
            this.dateTime = dateTime;

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
            $(".reminder-icons i." + cssIcon).addClass("active");
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
                this.reminder = reminder;

                let config = GetFlatpickrRuConfig(this.reminder.dateReminder);
                this.flatpickrReminder = flatpickr('#dateReminder', config);
            } else {
                this.isNew = true;
                this.reminder.dateReminder = this.dateTime;

                let config = GetFlatpickrRuConfig(this.dateTime);
                this.flatpickrReminder = flatpickr('#dateReminder', config);
            }

            this.chooseReminderIcon(this.reminder.cssIcon);
        },
        close: function () {
            this.isShowModal = false;
            this.reminder = {};
        },
        remove: function (reminder) {

            ShowLoading('#reminder_' + record.id);

            return $.ajax({
                type: "POST",
                url: "/Reminder/Remove",
                data: JSON.stringify(reminder),
                context: reminder,
                contentType: "application/json",
                dataType: 'json',
                success: function (response) {
                    record.isDeleted = response.isOk;
                    HideLoading('#reminder_' + reminder.id);
                }
            });
        },

        save: function () {
            //HideLoading('#record_' + record.id);

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
                        if (index == -1) {
                            this.reminders.push(response.data);
                        } else {
                            this.reminders[index] = response.data;
                        }
                        this.isShowModal = false;
                        BudgetVue.refresh("onlyTable");
                    }

                    this.isSaving = true;

                },
                error: function () {
                    this.isSaving = true;

                }
            });
        },
        checkValid: function () {


            return true;
        }
    }
});

Vue.config.devtools = true;
