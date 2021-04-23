
var SubScriptionListVue = new Vue({
    el: "#subscription-list-vue",
    data: {
        userSubScriptions: [],
        baseSubScriptions: [],
        subScriptions: [],
        subScription: {},

        option: {},
        pricings: [],

        msnry: {},

        isSaving: false,
        numberID: -1,
        activeViewID: 1,
    },
    watch: {

    },
    computed: {
        anySelectedPricing: function () {
            return this.pricings.some(x => x.isSelected);
        },
    },
    mounted: function () {
        this.userLoad();
        this.baseLoad();

        $("#modal-limit").on("hidden.bs.modal", function () {
            SubScriptionListVue.closeEditModal();
        });
    },
    methods: {
        userLoad: function () {
            return $.ajax({
                type: "GET",
                url: "/SubScription/GetUserSubScriptions",
                contentType: "application/json",
                dataType: 'json',
                context: this,
                success: function (result) {
                    if (result.isOk == true) {
                        this.userSubScriptions = result.userSubScriptions;

                        if (this.userSubScriptions.length == 0) {
                            this.activeViewID = 1;
                            this.subScriptions = this.baseSubScriptions;
                        } else {
                            this.activeViewID = -1;
                            this.subScriptions = this.userSubScriptions;
                        }

                        //setTimeout(function () {
                        //    if (SubScriptionListVue.msnry && SubScriptionListVue.msnry.destroy != undefined) {
                        //        SubScriptionListVue.msnry.destroy();
                        //    }
                        //    SubScriptionListVue.msnry = new Masonry('#limits', {
                        //        itemSelector: '.masonry-item:not(.d-none)',
                        //        columnWidth: '.masonry-item-sizer',
                        //        originLeft: true,
                        //        horizontalOrder: true
                        //    });
                        //}, 100);
                    }
                },
                error: function (result) {
                    console.log(result);
                }
            });
        },
        baseLoad: function () {
            return $.ajax({
                type: "GET",
                url: "/SubScription/GetBaseSubScriptions",
                contentType: "application/json",
                dataType: 'json',
                context: this,
                success: function (result) {
                    if (result.isOk == true) {
                        this.baseSubScriptions = result.baseSubScriptions;

                        if (this.activeViewID == 1) {
                            this.subScriptions = this.baseSubScriptions;
                        }

                        //setTimeout(function () {
                        //    if (SubScriptionListVue.msnry && SubScriptionListVue.msnry.destroy != undefined) {
                        //        SubScriptionListVue.msnry.destroy();
                        //    }
                        //    SubScriptionListVue.msnry = new Masonry('#limits', {
                        //        itemSelector: '.masonry-item:not(.d-none)',
                        //        columnWidth: '.masonry-item-sizer',
                        //        originLeft: true,
                        //        horizontalOrder: true
                        //    });
                        //}, 100);
                    }
                },
                error: function (result) {
                    console.log(result);
                }
            });
        },
        add: function (sub) {
            this.subScription = sub;

            for (var i = 0; i < sub.options.length; i++) {
                if (sub.options[i].isSelected) {
                    this.option = sub.options[i];
                    this.pricings = sub.options[i].pricings;
                    continue;
                }
            }
            $('#modal-subscription').modal('show');
        },
        selectOption: function (option) {
            if (this.subScription.userSubScriptionID > 0) {
                return;
            }
            this.option = option;
            for (var i = 0; i < this.subScription.options.length; i++) {
                let isSelected = option.id == this.subScription.options[i].id;
                this.subScription.options[i].isSelected = isSelected;

                if (isSelected) {
                    this.pricings = this.subScription.options[i].pricings;
                } else {
                    //ToDo unselect price.isSelected
                    //pricings = this.subScription.options[i].pricings
                }
            }
        },
        selectPrice: function (pricing) {
            if (this.subScription.userSubScriptionID > 0) {
                return;
            }
            for (var i = 0; i < this.pricings.length; i++) {
                let isSelected = pricing.id == this.pricings[i].id;
                this.pricings[i].isSelected = isSelected;
            }
            this.subScription.userTitle = this.subScription.title;// + ' ' + (this.option.title ? ('- ' + this.option.optionVariant) : this.option.optionVariant);
            this.subScription.userPrice = pricing.price;
            this.subScription.userPriceForMonth = pricing.pricePerMonth;
            this.subScription.userPricingID = pricing.id;
            this.subScription.userSubScriptionID = 0;

        },
        save: function () {

            if (this.checkValid() == false) {
                return;
            }

            this.isSaving = true;

            return $.ajax({
                type: "POST",
                url: "/SubScription/Save",
                data: JSON.stringify(this.subScription),
                context: this,
                contentType: "application/json",
                dataType: 'json',
                success: function (response) {
                    if (response.isOk) {
                        this.userLoad();

                        $('#modal-subscription').modal('hide');
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

            if (!(this.subScription.userTitle && this.subScription.userTitle.length > 0)) {
                $("#userTitle").addClass("is-invalid");
                isOk = false;
            } else {
                $("#userTitle").removeClass("is-invalid");
            }

            if (!(this.subScription.userPrice && (this.subScription.userPrice * 1) > 0)) {
                $("#userPrice").addClass("is-invalid");
                isOk = false;
            } else {
                $("#userPrice").removeClass("is-invalid");
            }

            if (!(this.subScription.userPriceForMonth && (this.subScription.userPriceForMonth * 1) > 0)) {
                $("#userPriceForMonth").addClass("is-invalid");
                isOk = false;
            } else {
                $("#userPriceForMonth").removeClass("is-invalid");
            }

            return isOk;
        },

        edit: function (sub) {
            this.subScription = sub;
            this.options = sub.options;

            for (var i = 0; i < sub.options.length; i++) {
                if (sub.options[i].isSelected) {
                    this.pricings = sub.options[i].pricings;
                }
            }

            $('#modal-subscription').modal('show');
        },

        remove: function (sub) {
            if (this.isSaving) {
                return;
            }
            this.isSaving = true;
            return $.ajax({
                type: "POST",
                url: "/SubScription/Remove?id=" + sub.userSubScriptionID,
                context: this,
                contentType: "application/json",
                dataType: 'json',
                success: function (response) {
                    if (response.isOk) {
                        sub.isDeleted = true;
                    }
                    this.isSaving = false;
                },
                error: function () {
                    this.isSaving = false;
                }
            });

        },
        recovery: function (sub) {
            if (this.isSaving) {
                return;
            }
            this.isSaving = true;
            return $.ajax({
                type: "POST",
                url: "/SubScription/Recovery?id=" + sub.userSubScriptionID,
                context: this,
                contentType: "application/json",
                dataType: 'json',
                success: function (response) {
                    if (response.isOk) {
                        sub.isDeleted = false;
                    }
                    this.isSaving = false;
                },
                error: function () {
                    this.isSaving = false;
                }
            });
        }
    }
});