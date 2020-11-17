$(function () {

    $('.account-settings-multiselect').each(function () {
        $(this)
            .wrap('<div class="position-relative"></div>')
            .select2({
                dropdownParent: $(this).parent()
            });
    });

    $('.account-settings-tagsinput').tagsinput({ tagClass: 'badge badge-default' });

});


var AccountSettingsVue = new Vue({
    el: "#account-settings",
    data: {
        user: {
            userSettings: {},
            payment: {},
            imageBase64: null
        },
        oldTheme: "",
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
        code: null,

        //metadate
        isSaving: false,
        isValidPassword: true,
        isHiddenPassword: true,
        isValidCode: true,
        isShowCode: false,
        isShowSuccessChangedPassword: false,
        errorMessage: null
    },
    computed: {
        validName: function () {
            return this.user.name && this.user.name.length >= 2;
        },
        validEmail: function () {
            return this.user.email && this.user.email.indexOf("@") > 0 && this.user.email.indexOf(".") > 0;
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
        //User info
        loadUser: function (id) {
            return sendAjax("/Identity/Account/LoadUserSettings", null, "GET")
                .then(function (result) {
                    AccountSettingsVue.user = result.user;
                    AccountSettingsVue.oldTheme = result.user.userSettings.webSiteTheme;
                    AccountSettingsVue.oldEmail = result.user.email;

                    AccountSettingsVue.refreshCollectiveList();
                    AccountSettingsVue.checkOffers();
                });
        },
        resendConfirmEmail: function () {
            if (this.validEmail && this.isSaving == false) {
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

                return $.ajax({
                    type: "POST",
                    url: "/Identity/Account/SaveUserInfo",
                    context: this,
                    data: JSON.stringify(this.user),
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'json',
                    success: function (result) {
                        if (result.isOk) {
                            $("#userImageLink").prop("src", result.user.imageLink);
                            let userName = result.user.email;

                            if (result.user.name) {
                                userName = result.user.name;
                                if (result.user.lastName) {
                                    userName += " " + result.user.lastName;
                                }
                            }
                            
                            $("#userName").text(userName);

                            if (this.user.email != this.oldEmail) {
                                //this.user.isConfirmEmail = false;
                                this.isShowCode = true;
                            }

                            this.user = result.user;
                            this.oldEmail = result.user.email;
                            this.isSaving = false;

                        }
                        return true;
                    },
                    error: function (xhr, status, error) {
                        this.isSaving = false;
                        console.log(error);
                    }
                });
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
        checkCode: function () {
            if (!(this.code && this.code.length == 4)) {
                this.isValidCode = false;
                return false;
            }
            this.isValidCode = true;
            this.isSaving = true;

            return $.ajax({
                type: "GET",
                url: "/Identity/Account/CheckCodeAfterChangeEmail?code=" + this.code,
                context: this,
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (response) {
                    if (response.isOk) {
                        this.code = null;
                        this.isValidCode = true;
                        this.isShowCode = false;
                        this.user.isConfirmEmail = true;
                    } else {
                        this.isValidCode = false;
                    }
                    this.isSaving = false;
                    return response;
                },
                error: function (xhr, status, error) {
                    this.isSaving = false;
                    console.log(error);
                }
            });
        },

        //Change password tab
        saveNewPassword: function () {
            if (this.validPassword()) {
                this.isValidPassword = true;
                this.isSaving = true;
                this.isShowSuccessChangedPassword = false;

                return sendAjax("/Identity/Account/ChangePassword", this.newPassword, "POST")
                    .then(function (result) {
                        if (result.isOk) {
                            AccountSettingsVue.newPassword = null;
                            AccountSettingsVue.isSaving = false;
                            AccountSettingsVue.isShowSuccessChangedPassword = true;
                        }
                    });
            } else {
                console.log("not save");
                this.isValidPassword = false;
            }
        },
        validPassword: function () {
            let isOk = true;

            if (this.newPassword == null || this.newPassword == "") {
                isOk = false;
                this.errorMessage = "Пароль не может быть пустым.";
                return isOk;
            } else {
                this.errorMessage = null;
            }

            if (this.newPassword.length < 5) {
                isOk = false;
                this.errorMessage = "Пароль должен содержать не менее 5 символов.";
                return isOk;
            } else {
                this.errorMessage = null;
            }

            if (this.newPassword.indexOf(' ') >= 0) {
                isOk = false;
                this.errorMessage = "Пароль не должен содержать пробелов.";
            } else {
                this.errorMessage = null;
            }

            return isOk;
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

        //User settings
        saveUserSettings: function () {
            this.isSaving = true;

            UserInfo.UserSettings.webSiteTheme = this.user.userSettings.webSiteTheme;
            UserInfo.UserSettings.newsLetter = this.user.userSettings.newsLetter;

            return sendAjax("/Identity/Account/SaveUserSettings", this.user.userSettings, "POST")
                .then(function (result) {
                    AccountSettingsVue.isSaving = false;

                    if (AccountSettingsVue.oldTheme != AccountSettingsVue.user.userSettings.webSiteTheme) {
                        themeSettings.setStyle(AccountSettingsVue.user.userSettings.webSiteTheme)
                        //document.location.href = document.location.href;
                    }
                });
        },


        //Testing
        generatedRecords: function () {
            if (confirm("Вы уверены, что хотите сгенерировать данные ?")) {
                this.isSaving = true;
                return sendAjax("/Test/GenerateRecords", null, "GET")
                    .then(function (result) {
                        AccountSettingsVue.isSaving = false;
                    });
            }
        },
        setFlagConstructor: function () {
            if (confirm("Вы уверены, что хотите обнулить аккаунт ?")) {
                this.isSaving = true;
                return $.ajax({
                    type: "GET",
                    url: "/Test/SetFlagConstructor",
                    context: this,
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'json',
                    success: function (response) {
                        this.isSaving = false;
                        return response;
                    },
                    error: function (xhr, status, error) {
                        this.isSaving = false;
                        console.log(error);
                    }
                });
            }
        },



        getDateByFormat: function (date, format) {
            return GetDateByFormat(date, format);
        },
    }
});




