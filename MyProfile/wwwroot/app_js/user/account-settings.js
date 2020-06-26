var AccountSettingsVue = new Vue({
    el: "#account-settings",
    data: {
        user: {},
        oldEmail: null,

        isSaving: false,
    },
    computed: {
        validName: function () {
            return this.user.name && this.user.name.length > 2;
        },
        validEmail: function () {
            return this.user.email && this.user.email.indexOf("@") > 0 && this.user.email.indexOf(".") > 0;
        }
    },
    watch: {
    },
    mounted: function () {
        this.loadUser();
    },
    methods: {
        loadUser: function (id) {
            return sendAjax("/Identity/Account/LoadUserSettings", null, "GET")
                .then(function (result) {
                    AccountSettingsVue.user = result.user;
                    AccountSettingsVue.oldEmail = result.user.email;
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
                return sendAjax("/Identity/Account/SaveUserInfo", this.user, "POST")
                    .then(function (result) {
                        AccountSettingsVue.user = result.user;
                        AccountSettingsVue.oldEmail = result.user.email;
                        AccountSettingsVue.isSaving = false;
                    });
            } else {
                console.log("not save");
            }
        }

    }
});




