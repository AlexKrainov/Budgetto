var AccountSettingsVue = new Vue({
    el: "#account-settings",
    data: {
        user: {},
        oldEmail: null,
        newPassword: null,
        newPassword2: null,

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
    },
    computed: {
        validName: function () {
            return this.user.name && this.user.name.length > 2;
        },
        validEmail: function () {
            return this.user.email && this.user.email.indexOf("@") > 0 && this.user.email.indexOf(".") > 0;
        },
        validPasswords: function () {
            return (this.newPassword == null && this.newPassword2 == null)
                || (this.newPassword != null && this.newPassword.length >= 5
                    && this.newPassword2 != null && this.newPassword2.length >= 5
                    && this.newPassword === this.newPassword2)
        },
        validCollectiveUserSearch: function () {
            return this.collectiveUserSearch.indexOf("@") > 0 && this.collectiveUserSearch.indexOf(".") > 0
        }
    },
    watch: {
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

        //Change password tab
        saveNewPassword: function () {
            if (this.validPasswords) {
                this.isSaving = true;

                return sendAjax("/Identity/Account/ChangePassword", this.newPassword, "POST")
                    .then(function (result) {
                        if (result.isOk) {
                            AccountSettingsVue.newPassword = null;
                            AccountSettingsVue.newPassword2 = null;
                            AccountSettingsVue.isSaving = false;
                        }
                    });
            } else {
                console.log("not save");
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




