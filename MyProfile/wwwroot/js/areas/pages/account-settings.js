﻿var AccountSettingsVue = new Vue({
    el: "#account-settings",
    data: {
        user: {},
        oldEmail: null,
        newPassword: null,

        //Collective tab
        collectiveSearchedUser: {},
        collectiveUserSearch: '',
        isFoundCollectiveUser: null,
        isActiveUsersTab: true,
        offerSent: false,
        collectiveUsers: [],
        collectiveRequests: [],
        offers: [],

        //metadate
        isSaving: false,
        isValidPassword: true,
        isHiddenPassword: true,
    },
    computed: {
        validName: function () {
            return this.user.name && this.user.name.length > 2;
        },
        validEmail: function () {
            return this.user.email && this.user.email.indexOf("@") > 0 && this.user.email.indexOf(".") > 0;
        },
        validPassword: function () {
            return (this.newPassword != null
                && this.newPassword != ""
                && this.newPassword.length >= 5);
        },
        validCollectiveUserSearch: function () {
            return this.collectiveUserSearch.indexOf("@") > 0 && this.collectiveUserSearch.indexOf(".") > 0
        }
    },
    watch: {
        isHiddenPassword: function (newValue, oldValue) {
            if (newValue) {
                $("input[name=password]").prop("type", "password");
            } else {
                $("input[name=password]").prop("type", "text");
            }
        }
    },
    mounted: function () {
        this.loadUser();
    },
    methods: {
        //General settings tab
        loadUser: function (id) {
            return sendAjax("/Identity/Account/LoadUserSettings", null, "GET")
                .then(function (result) {
                    AccountSettingsVue.user = result.user;
                    AccountSettingsVue.oldEmail = result.user.email;

                    AccountSettingsVue.refreshCollectiveList();
                    AccountSettingsVue.checkOffers();
                });
        },
        resendConfirmEmail: function () {
            if (this.validEmail) {
                this.isSaving = true;
                return sendAjax("/Identity/Account/ResendConfirmEmail", null, "GET")
                    .then(function (result) {
                        AccountSettingsVue.isSaving = false;
                    });
            }
        },
        saveUserInfo: function (isSendEmail) {
            if (this.validName && this.validEmail) {
                this.isSaving = true;

                if (this.user.email != this.oldEmail) {
                    this.user.isConfirmEmail = false;
                }

                return sendAjax("/Identity/Account/SaveUserInfo", this.user, "POST")
                    .then(function (result) {
                        AccountSettingsVue.user = result.user;
                        AccountSettingsVue.oldEmail = result.user.email;
                        AccountSettingsVue.isSaving = false;
                    });
            } else {
                console.log("not save");
            }
        },
        encodeImageFileAsURL: function (event) {
            var file = event.target.files[0];
            var reader = new FileReader();
            reader.onloadend = function () {
                AccountSettingsVue.user.imageBase64 = reader.result;
                //$("#photo").attr("src", reader.result);
                //console.log('RESULT', reader.result)
            }
            reader.readAsDataURL(file);
        },


        //Change password tab
        saveNewPassword: function () {
            if (this.validPassword) {
                this.isValidPassword = true;
                this.isSaving = true;

                return sendAjax("/Identity/Account/ChangePassword", this.newPassword, "POST")
                    .then(function (result) {
                        if (result.isOk) {
                            AccountSettingsVue.newPassword = null;
                            AccountSettingsVue.isSaving = false;
                        }
                    });
            } else {
                console.log("not save");
                this.isValidPassword = false;
            }
        },

        //Collective budget tab
        changeStatusCollectiveBudget: function () {
            this.user.isAllowCollectiveBudget = !this.user.isAllowCollectiveBudget;

            return sendAjax("/Identity/Account/ChangeStatusCollectiveBudget", this.user.isAllowCollectiveBudget, "POST")
                .then(function (result) {
                    AccountSettingsVue.refreshCollectiveList()
                        .then(function () {
                            AccountSettingsVue.checkOffers();
                        });
                });
        },
        searchUser: function () {
            this.isSaving = true;
            this.isFoundCollectiveUser = null;
            return sendAjax("/Identity/Account/SearchUser?email=" + this.collectiveUserSearch, null, "GET")
                .then(function (result) {
                    if (result.isOk) {
                        AccountSettingsVue.isSaving = false;
                        AccountSettingsVue.isFoundCollectiveUser = result.user != null;

                        if (AccountSettingsVue.isFoundCollectiveUser) {
                            AccountSettingsVue.collectiveSearchedUser = result.user;
                        }
                    }
                });
        },
        sendOffer: function (email) {
            this.isSaving = true;
            return sendAjax("/Identity/Account/SendOffer?email=" + email, null, "GET")
                .then(function (result) {
                    if (result.isOk) {
                        AccountSettingsVue.isSaving = false;
                        AccountSettingsVue.isFoundCollectiveUser = null;
                        AccountSettingsVue.offerSent = true;

                    }
                });
        },
        refreshCollectiveList: function () {
            return sendAjax("/Identity/Account/RefreshCollectiveList", null, "GET")
                .then(function (result) {
                    if (result.isOk) {
                        AccountSettingsVue.collectiveUsers = result.collectiveUsers;
                        AccountSettingsVue.collectiveRequests = result.collectiveRequests;

                    }
                });
        },
        leftCollectiveBudgetGroup: function () {
            return sendAjax("/Identity/Account/LeftCollectiveBudgetGroup", null, "GET")
                .then(function (result) {
                    if (result.isOk) {
                        //update lists
                        AccountSettingsVue.refreshCollectiveList();
                    }
                });
        },
        checkOffers: function () {
            return sendAjax("/Identity/Account/CheckOffers", null, "GET")
                .then(function (result) {
                    if (result.isOk) {
                        AccountSettingsVue.offers = result.offers;
                    }
                });
        },
        offerAction: function (offerID, action) {

            // ToDo: Attenchen show model 'You will auto left other collective group'
            return sendAjax("/Identity/Account/OfferAction?offerID=" + offerID + "&action=" + action, null, "GET")
                .then(function (result) {
                    if (result.isOk) {
                        AccountSettingsVue.offers = result.offers;

                        AccountSettingsVue.refreshCollectiveList()
                            .then(function () {
                                AccountSettingsVue.checkOffers();
                            });
                    }
                });
        },







        getDateByFormat: function (date, format) {
            return GetDateByFormat(date, format);
        },
    }
});



