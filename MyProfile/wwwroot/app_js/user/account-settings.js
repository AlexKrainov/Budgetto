var AccountSettingsVue = new Vue({
    el: "#account-settings",
    data: {
        user: {},
        oldEmail: null,
        newPassword: null,
        newPassword2: null,

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
        }

    }
});




