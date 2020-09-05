var LoginVue = new Vue({
    el: "#login-vue",
    data: {
        email: null,
        password: null,

        isHiddenPassword: true,
        isValidEmail: true,
        isValidPassword: true,
        isValidCode: true,
        textError: null,
        textMessage: null,

        login: {
            id: 0,
            isShow: true,
        },
        registration: {
            id: 1,
            isShow: false,
        },
        recoveryPassword: {
            id: 2,
            isShow: false,
        },
        recoveryPassword2: {
            id: 4,
            isShow: false,
            userID: null,
        },
        enterCode: {
            id: 3,
            isShow: false,
            code: null,
            canResend: false,
            seconds: 60,
            lastActionID: 0, //0-login, 1 - registration, 2 - recovery password
        },


        isSaving: false,
    },
    computed: {
        passwordValid: function () {
            return (this.password != null
                && this.password != ""
                && this.password.length >= 5);
        },
        emailValid: function () {
            return this.email && this.email.indexOf("@") > 0 && this.email.indexOf(".") > 0;
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
        this.changeView(this.login.id);
    },
    methods: {
        // login
        loginSave: function () {
            if (this.checkForm() == false) {
                return false;
            }

            this.isSaving = true;

            return $.ajax({
                type: "POST",
                url: "/Identity/Account/Login",
                context: this,
                data: JSON.stringify({ email: this.email, password: this.password }),
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (response) {
                    if (response.isOk == false) {
                        this.textError = response.textError;
                        this.isSaving = false;
                    } else {
                        if (response.emailID) {
                            this.enterCode.emailID = response.emailID;
                            this.changeView(this.login.id, this.enterCode.id);
                            this.isSaving = false;
                        } else if (response.href) {
                            document.location.href = response.href;
                        }
                    }
                    return response;
                },
                error: function (xhr, status, error) {
                    this.isSaving = false;
                    console.log(error);
                }
            });
        },
        //Registration
        checkForm: function (e) {
            let isOk = true;

            this.isValidPassword = this.passwordValid

            if (!this.isValidPassword) {
                isOk = false;
            }

            this.isValidEmail = this.emailValid;

            if (!this.isValidEmail) {
                isOk = false;
            }
            if (isOk == false && e) {
                e.preventDefault();
            }
            return isOk;
        },
        onRegistration: function () {
            if (this.checkForm() == false) {
                return false;
            }
            this.textError = null;
            this.isSaving = true;
            return $.ajax({
                type: "POST",
                url: "/Identity/Account/Registration",
                context: this,
                data: JSON.stringify({ email: this.email, password: this.password }),
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (response) {
                    if (response.isOk == false) {
                        this.textError = response.message;
                        this.isSaving = false;
                    } else {
                        //document.location.href = response.href;
                        if (response.isShowCode) {
                            this.enterCode.emailID = response.emailID;
                            this.changeView(this.registration.id, this.enterCode.id);
                            this.isSaving = false;
                        } else {
                            document.location.href = response.href;
                        }
                    }
                    return response;
                },
                error: function (xhr, status, error) {
                    this.isSaving = false;
                    console.log(error);
                }
            });
        },

        //Recovery
        onRecoveryPassword: function () {

            if (this.emailValid == false) {
                this.isValidEmail = false;
                return false;
            }
            this.isValidEmail = true;
            this.textError = null;

            this.isSaving = true;
            return $.ajax({
                type: "POST",
                url: "/Identity/Account/RecoveryPassword",
                context: this,
                data: JSON.stringify({ email: this.email }),
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (response) {
                    if (response.isOk == false) {
                        this.textError = response.message;
                    } else {
                        this.enterCode.emailID = response.emailID;
                        this.changeView(this.recoveryPassword.id, this.enterCode.id);
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
        onSetNewPassword: function () {
            if (this.passwordValid == false) {
                this.isValidPassword = false;
                return false;
            }
            this.isValidPassword = true;

            this.isSaving = true;
            return $.ajax({
                type: "POST",
                url: "/Identity/Account/RecoveryPassword2",
                context: this,
                data: JSON.stringify({ id: this.recoveryPassword2.userID, newPassword: this.password }),
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (response) {
                    if (response.isOk == false) {
                        this.textError = response.message;
                        this.isSaving = false;
                    } else {
                        document.location.href = response.href;
                    }
                    return response;
                },
                error: function (xhr, status, error) {
                    this.isSaving = false;
                    console.log(error);
                }
            });

        },

        checkCode: function () {
            if (!(this.enterCode.emailID && this.enterCode.code && this.enterCode.code.length == 4)) {
                this.isValidCode = false;
                return false;
            }
            this.isValidCode = true;
            this.textError = null;

            this.isSaving = true;

            return $.ajax({
                type: "POST",
                url: "/Identity/Account/CheckCode",
                context: this,
                data: JSON.stringify(this.enterCode),
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (response) {
                    if (response.isOk == false) {
                        this.textError = response.message;
                        this.isSaving = false;
                    } else {
                        if (response.canChangePassword) {
                            this.recoveryPassword2.userID = response.userID;
                            this.changeView(this.checkCode.id, this.recoveryPassword2.id);
                            this.isSaving = false;
                        } else {
                            document.location.href = response.href;
                        }
                    }
                    return response;
                },
                error: function (xhr, status, error) {
                    this.isSaving = false;
                    console.log(error);
                }
            });
        },

        changeView: function (fromViewID, toViewID) {
            this.isValidEmail = true;
            this.isValidPassword = true;
            this.isValidCode = true;
            this.textError = null;
            this.textMessage = null;

            switch (toViewID) {
                case this.login.id:
                    this.login.isShow = true;
                    this.registration.isShow = false;
                    this.recoveryPassword.isShow = false;
                    this.enterCode.isShow = false;
                    this.recoveryPassword2.isShow = false;
                    break;
                case this.registration.id:
                    this.login.isShow = false;
                    this.registration.isShow = true;
                    this.recoveryPassword.isShow = false;
                    this.recoveryPassword2.isShow = false;
                    this.enterCode.isShow = false;
                    break;
                case this.recoveryPassword.id:
                    this.login.isShow = false;
                    this.registration.isShow = false;
                    this.recoveryPassword.isShow = true;
                    this.recoveryPassword2.isShow = false;
                    this.enterCode.isShow = false;
                    break;
                case this.enterCode.id:
                    this.enterCode.lastActionID = fromViewID;

                    this.login.isShow = false;
                    this.registration.isShow = false;
                    this.recoveryPassword.isShow = false;
                    this.recoveryPassword2.isShow = false;
                    this.enterCode.isShow = true;

                    this.showResendCodeButton();
                    break;
                case this.recoveryPassword2.id:
                    this.login.isShow = false;
                    this.registration.isShow = false;
                    this.recoveryPassword.isShow = false;
                    this.recoveryPassword2.isShow = true;
                    this.enterCode.isShow = false;
                    break;
                default:
                    break;
            }
        },
        showResendCodeButton: function () {
            LoginVue.enterCode.canResend = false;

            //setTimeout(function () {
            //    LoginVue.enterCode.canResend = true;
            //}, 60000)

            var timeleft = 60;
            var downloadTimer = setInterval(function () {
                if (timeleft <= 0) {
                    clearInterval(downloadTimer);
                    LoginVue.enterCode.seconds = timeleft;
                    LoginVue.enterCode.canResend = true;
                } else {
                    LoginVue.enterCode.seconds = timeleft;
                }
                timeleft -= 1;
            }, 1000);
        },
        resend: function () {
            if (!this.enterCode.emailID) {
                return false;
            }

            if (this.enterCode.seconds > 0) {
                return false;
            }

            this.checkCode.code = null;
            this.textError = null;
            this.textMessage = null;
            this.isSaving = true;

            return $.ajax({
                type: "POST",
                url: "/Identity/Account/Resend",
                context: this,
                data: JSON.stringify(this.enterCode),
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (response) {
                    if (response.isOk == false) {
                        this.textError = response.message;
                        document.location.href = response.href;
                    } else {
                        this.enterCode.emailID = response.emailID;
                        //this.changeView(this.registration.id, this.enterCode.id);
                        this.showResendCodeButton();
                        this.isSaving = false;
                    }
                    return response;
                },
                error: function (xhr, status, error) {
                    this.isSaving = false;
                    console.log(error);
                }
            });
        }
    }
});