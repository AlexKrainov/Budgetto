
var LimitListVue = new Vue({
    el: "#limit-list-vue",
    data: {
        limits: [],
        activeLimitPeriodTypeID: -1,

        //edit
        limit: { periodName: '', periodTypeID: -1 },
        flatpickrStart: {},
        flatpickrEnd: {},
        msnry: {},

        sections: [],
        periodTypes: [],
        isSaving: false,
    },
    watch: {
        'limit.periodTypeID': function (newValue, oldValue) {
            if (newValue == -1) {
                return;
            } else if (newValue == 1) {
                $("#date-start").next().show();
                $("#date-end").next().show();
            } else if (newValue == 3) {
                $("#date-start").next().hide();
                $("#date-end").next().hide();
            }
        }
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
            return sendAjax("/Limit/GetLimits", null, "GET")
                .then(function (result) {
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
                this.limit = { ...limit };
            } else {
                this.limit = { periodName: '', periodTypeID: -1 };

                $("#limitSections").val(null).select2();

                this.limit.periodTypeID = this.periodTypes[0].id;
                this.limit.periodName = this.periodTypes[0].name;

                this.limit.dateStart = GetDateByFormat(moment(), "YYYY/MM/DD");
                this.limit.yearStart = moment().years();
                this.limit.yearEnd = null;

            }

            let startConfig = GetFlatpickrRuConfig_Month(this.limit.dateStart);
            startConfig.onChange = function (selectedDates, dateStr, instance) {
                LimitListVue.flatpickrEnd.config.minDate = dateStr;
            };
            let endConfig = GetFlatpickrRuConfig_Month(this.limit.dateEnd);
            endConfig.onChange = function (selectedDates, dateStr, instance) {
                LimitListVue.flatpickrStart.config.maxDate = dateStr;
            };

            this.flatpickrStart = flatpickr('#date-start', startConfig);
            this.flatpickrEnd = flatpickr('#date-end', endConfig);

            if (this.limit.sections) {
                $("#limitSections").val(this.limit.sections.map(x => x.id)).select2();
            } else {
                $("#limitSections").select2();
            }

            //find limit by id 
            $("#modal-limit").modal("show");
        },
        save: function () {
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
                                LimitListVue.msnry.addItems($("div[data-id=limit_" + result.limit.id + "]"));
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
                //$("#limitSections").addClass("is-invalid");
                //$("#limitSections").next().addClass("is-invalid");
                isOk = false;
            }
            //else {
            //    $("#limitSections").removeClass("is-invalid");
            //    $("#limitSections").next().removeClass("is-invalid");
            //}

            if (!(this.limit.name && this.limit.name.length > 0)) {
                isOk = false;
            }
            if (!(this.limit.limitMoney && (this.limit.limitMoney > 0 || this.limit.limitMoney.length > 0))) {
                isOk = false;
            }
            if (isOk == false && e) {
                e.preventDefault();
            }
            return isOk;
        },
        finishLimit: function () {
            this.limit.dateEnd = GetDateByFormat((new Date()).setMonth((new Date()).getMonth() - 1), "YYYY/MM/DD");
            this.flatpickrEnd = flatpickr('#date-to', GetFlatpickrRuConfig_Month(this.limit.dateEnd));
        },
        closeEditModal: function () {
            this.limit = { periodName: '', periodTypeID: -1 };
        },


        getDateByFormat: function (date, format) {
            return GetDateByFormat(date, format);
        },
        reloadView: function () {
            setTimeout(function () {
                LimitListVue.msnry.layout();
            }, 15);
        },
    }
});

Vue.config.devtools = true;
