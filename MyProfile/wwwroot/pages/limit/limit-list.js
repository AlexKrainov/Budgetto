
var LimitListVue = new Vue({
    el: "#limit-list-vue",
    data: {
        limits: [],
        activeLimitPeriodTypeID: -1,

        //edit
        limit: { periodName: '', periodTypeID: -1, notifications: [] },
        //flatpickrStart: {},
        //flatpickrEnd: {},
        msnry: {},

        sections: [],
        periodTypes: [],
        isSaving: false,
        numberID: -1,
    },
    watch: {
        //'limit.periodTypeID': function (newValue, oldValue) {
        //    if (newValue == -1) {
        //        return;
        //    } else if (newValue == 1) {
        //        $("#date-start").next().show();
        //        $("#date-end").next().show();
        //    } else if (newValue == 3) {
        //        $("#date-start").next().hide();
        //        $("#date-end").next().hide();
        //    }
        //}
    },
    computed: {},
    mounted: function () {
        this.load()
            .then(function () {
                LimitListVue.loadSections();
                LimitListVue.loadPeriodTypes();

            });

        $("#modal-limit").on("hidden.bs.modal", function () {
            LimitListVue.closeEditModal();
        });
    },
    methods: {
        load: function () {
            return $.ajax({
                type: "GET",
                url: "/Limit/GetLimits",
                contentType: "application/json",
                dataType: 'json',
                context: this,
                success: function (result) {
                    if (result.isOk == true) {
                        LimitListVue.limits = result.limits;

                        setTimeout(function () {
                            if (LimitListVue.msnry && LimitListVue.msnry.destroy != undefined) {
                                LimitListVue.msnry.destroy();
                            }
                            LimitListVue.msnry = new Masonry('#limits', {
                                itemSelector: '.masonry-item:not(.d-none)',
                                columnWidth: '.masonry-item-sizer',
                                originLeft: true,
                                horizontalOrder: true
                            });
                        }, 100);
                    }
                },
                error: function (result) {
                    console.log(result);
                }
            });
        },
        loadSections: function () {
            return sendAjax("/Section/GetAllSectionByPerson", null, "GET")
                .then(function (result) {
                    if (result.isOk == true) {
                        LimitListVue.sections = result.sections;
                    }
                });
        },
        loadPeriodTypes: function () {
            return sendAjax("/Limit/GetPeriodTypes", null, "GET")
                .then(function (result) {
                    if (result.isOk == true) {
                        LimitListVue.periodTypes = result.periodTypes;
                    }
                });
        },
        edit: function (limit) {
            if (this.periodTypes.length == 0) {
                return;
            }

            if (limit) {
                this.limit = JSCopyObject(limit);
            } else {
                this.limit = { periodName: '', periodTypeID: -1, isShowOnDashboard: true, notifications: [] };

                $("#limitSections").val(null).select2();

                this.limit.periodTypeID = this.periodTypes[0].id;
                this.limit.periodName = this.periodTypes[0].name;
            }

            if (this.limit.sections) {
                $("#limitSections").val(this.limit.sections.map(x => x.id)).select2();
            } else {
                $("#limitSections").select2();
            }

            setTimeout(function () {
                $('[data-toggle="tooltip"]').tooltip();
            }, 1000);

            //find limit by id 
            $("#modal-limit").modal("show");
        },
        saveLimit: function () {
            this.limit.newSections = $("#limitSections").select2("val").map(function (x) { return { id: x } });

            if (this.checkForm() == false) {
                return false;
            }

            this.isSaving = true;
            return sendAjax("/Limit/Save", this.limit, "POST")
                .then(function (result) {
                    if (result.isOk == true) {

                        // LimitListVue.load();
                        let limitIndex = LimitListVue.limits.findIndex(x => x.id == result.limit.id);
                        if (limitIndex >= 0) {
                            LimitListVue.limits[limitIndex] = result.limit;
                        } else {
                            LimitListVue.limits.push(result.limit);
                            setTimeout(function () {
                                LimitListVue.msnry.addItems($("#limit_" + result.limit.id).parent());
                                LimitListVue.msnry.layout();
                            }, 100);
                        }

                        $("#modal-limit").modal("hide");
                    } else {
                        console.log(result.message);
                    }
                    LimitListVue.isSaving = false;
                });
        },
        checkForm: function (e) {
            let isOk = true;
            if (this.limit.newSections.length == 0) {
                $("#limitSections").addClass("is-invalid");
                $("#limitSections").next().addClass("is-invalid");
                isOk = false;
            }
            else {
                $("#limitSections").removeClass("is-invalid");
                $("#limitSections").next().removeClass("is-invalid");
            }

            if (!(this.limit.name && this.limit.name.length > 0)) {
                $("#limit-name").addClass("is-invalid");
                isOk = false;
            } else {
                $("#limit-name").removeClass("is-invalid");
            }

            let str = this.limit.name;
            str = str ? str.replaceAll(" ", "") : "";
            if (str.length == 0) {
                isOk = false;
                $("#limit-name").addClass("is-invalid");
            } else {
                $("#limit-name").removeClass("is-invalid");
            }

            if (this.limit.limitMoney && this.limit.limitMoney > 0) {
                $("[name=limitMoney]").removeClass("is-invalid");
            } else {
                isOk = false;
                $("[name=limitMoney]").addClass("is-invalid");
            }

            if (this.limit.notifications && this.limit.notifications.length > 0) {
                //if (this.limit.notifications.some(x => x.isMail == false && x.isSite == false && x.isTelegram == false && x.isDeleted == false)
                //    || this.limit.notifications.some(x => (x.price * 1) <= 0 && x.isDeleted == false)) {

                for (var i = 0; i < this.limit.notifications.length; i++) {
                    let notification = this.limit.notifications[i];
                    if ((notification.isMail == false && notification.isSite == false && notification.isTelegram == false && notification.isDeleted == false)
                        || ((notification.price * 1) <= 0 && notification.isDeleted == false)) {

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

            if (isOk == false && e) {
                e.preventDefault();
            }
            return isOk;
        },
        //finishLimit: function () {
        //    this.limit.dateEnd = GetDateByFormat((new Date()).setMonth((new Date()).getMonth() - 1), "YYYY/MM/DD");
        //    this.flatpickrEnd = flatpickr('#date-to', GetFlatpickrRuConfig_Month(this.limit.dateEnd));
        //},
        closeEditModal: function () {
            this.limit = { periodName: '', periodTypeID: -1, notifications: [] };
        },
        remove: function (limit) {
            ShowLoading('#limit_' + limit.id);

            return $.ajax({
                type: "POST",
                url: "/Limit/Remove",
                data: JSON.stringify(limit),
                context: limit,
                contentType: "application/json",
                dataType: 'json',
                success: function (response) {
                    limit.isDeleted = response.isOk;
                    HideLoading('#limit_' + limit.id);
                }
            });
        },
        recovery: function (limit) {
            ShowLoading('#limit_' + limit.id);
            return $.ajax({
                type: "POST",
                url: "/Limit/Recovery",
                data: JSON.stringify(limit),
                context: limit,
                contentType: "application/json",
                dataType: 'json',
                success: function (response) {
                    limit.isDeleted = !response.isOk;
                    HideLoading('#limit_' + limit.id);
                }
            });
        },

        getDateByFormat: function (date, format) {
            return GetDateByFormat(date, format);
        },
        reloadView: function () {
            setTimeout(function () {
                LimitListVue.msnry.layout();
            }, 100);
        },
        toggleLimit: function (limitID) {
            ShowLoading('#limit_' + limitID);
            return $.ajax({
                type: "GET",
                url: "/Limit/ToggleLimit?id=" + limitID,
                contentType: "application/json",
                dataType: 'json',
                context: limitID,
                success: function (response) {
                    HideLoading('#limit_' + this);
                    if (response.isOk = true) {
                        LimitListVue.limits[LimitListVue.limits.findIndex(x => x.id == this)].isShowOnDashboard = response.isShow;
                    }
                },
                error: function () {
                    HideLoading('#limit_' + this);
                }
            });
        },
        addNotification: function () {
            this.numberID -= 1;
            this.limit.notifications.push(
                {
                    isSite: false,
                    isMail: false,
                    isTelegram: false,
                    price: this.limit.limitMoney,
                    id: this.numberID,
                    isDeleted: false
                });

            setTimeout(function () {
                $('[data-toggle="tooltip"]').tooltip();
                $("#notification-" + LimitListVue.numberID).addClass("show");
            }, 500);
        },
        removeNotification: function (notification) {
            if (notification.id < 0) {
                let index = this.limit.notifications.findIndex(x => x.id == notification.id);
                this.limit.notifications.splice(index, 1);
            } else {
                notification.isDeleted = true;
            }
        }
    }
});


