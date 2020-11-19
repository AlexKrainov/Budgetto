var LoginVue = new Vue({
    el: "#login-vue",
    data: {
        email: null,
        password: null,
        userSessionID: null,

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
        person_data: {
            ip: "",
            city: "",
            country: "",
            //hostname: "",
            location: "",
            index: "",
            browser_name: "",
            browser_version: "",
            os_name: "",
            os_version: "",
            screen_size: "",
            referrer: "",
            isMobile: false,
            isLoad: false,
            isShow: false,
            path: "",
            dateCreate: null,


            continent_code: "",
            continent_name: "",
            info: "",
        },
    },
    computed: {
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

        let ip = $('body').attr('client-ip');

        this.userSessionID = $("#login-vue").data("user-session-id");
        this.person_data.userSessionID = this.userSessionID;
        this.person_data.referrer = document.referrer;

        if (ip == "::1") {
            this.person_data = { "userSessionID": this.userSessionID, "ip": "Local", "city": "Moscow", "country": "Russia", "location": "55.7522, 37.6156", "index": "111111", "browser_name": "Chrome", "browser_version": 85, "os_name": "Windows", "os_version": "10", "screen_size": "1536 x 864", "referrer": "", "isMobile": false, "isLoad": false, "isShow": false, "path": "/Identity/Account/Login", "dateCreate": null, "continent_code": "EU", "continent_name": "Europe", "info": "", "provider_info": "{\"asn\":\"AS8402\",\"name\":\"PJSC \\\"Vimpelcom\\\"\",\"domain\":\"veon.com\",\"route\":\"37.144.0.0/14\",\"type\":\"isp\"}", "threat": "{\"is_tor\":false,\"is_proxy\":false,\"is_anonymous\":false,\"is_known_attacker\":false,\"is_known_abuser\":false,\"is_threat\":false,\"is_bogon\":false}" };
            return $.ajax({
                type: "POST",
                url: "/Identity/Account/Stat",
                context: LoginVue,
                data: JSON.stringify(this.person_data),
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (response) {
                    LoginVue.userSessionID = response.userSessionID;
                    return response;
                },
                error: function (xhr, status, error) {
                    console.log(error);
                }
            });
            return;

        }
        try {
            $.get(`https://api.ipdata.co/${ip}?api-key=65b72cf7f83e7582fd54e2c3fad07548678dfa6e363424eb773edbd5`, function (response) {


                LoginVue.person_data.userSessionID = LoginVue.userSessionID;
                LoginVue.person_data.ip = response.ip;
                LoginVue.person_data.city = response.city;
                LoginVue.person_data.country = response.country_name;
                LoginVue.person_data.continent_code = response.continent_code;
                LoginVue.person_data.continent_name = response.continent_name;
                //LoginVue.person_data.hostname = response.hostname;
                LoginVue.person_data.location = response.latitude + ", " + response.longitude;
                LoginVue.person_data.index = response.postal;
                LoginVue.person_data.provider_info = JSON.stringify(response.asn);
                LoginVue.person_data.current_user_time = response.current_time;
                LoginVue.person_data.threat = JSON.stringify(response.threat);

                LoginVue.get_browser_info();
                let jscd = window.jscd;

                LoginVue.person_data.browser_name = jscd.browser;
                LoginVue.person_data.browser_version = jscd.browserMajorVersion;
                LoginVue.person_data.os_name = jscd.os;
                LoginVue.person_data.os_version = jscd.osVersion;
                LoginVue.person_data.isMobile = jscd.mobile;
                LoginVue.person_data.screen_size = jscd.screen;
                LoginVue.person_data.referrer = document.referrer;
                LoginVue.person_data.path = document.location.pathname;

                return LoginVue.person_data;
            }).then(function () {
                return $.ajax({
                    type: "POST",
                    url: "/Identity/Account/Stat",
                    context: LoginVue,
                    data: JSON.stringify(LoginVue.person_data),
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'json',
                    success: function (response) {
                        LoginVue.userSessionID = response.userSessionID;
                        return response;
                    },
                    error: function (xhr, status, error) {
                        this.isSaving = false;
                        console.log(error);
                    }
                });
            });
        } catch (e) {
            return $.ajax({
                type: "POST",
                url: "/Identity/Account/Stat",
                context: LoginVue,
                data: JSON.stringify(this.person_data),
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (response) {
                    LoginVue.userSessionID = response.userSessionID;
                    return response;
                },
                error: function (xhr, status, error) {
                    this.isSaving = false;
                    console.log(error);
                }
            });
        }
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
                data: JSON.stringify({ email: this.email, password: this.password, userSessionID: this.userSessionID }),
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
                    this.textError = "Ошибка авторизации. Пожалуйста, попробуйте еще раз.";
                    console.log(error);
                }
            });
        },
        //Registration
        checkForm: function (e) {
            let isOk = true;

            this.isValidPassword = this.validPassword();
            isOk = this.isValidPassword;

            this.isValidEmail = this.emailValid;

            if (!this.isValidEmail) {
                isOk = false;
            }
            if (isOk == false && e) {
                e.preventDefault();
            }
            return isOk;
        },
        validPassword: function () {
            let isOk = true;

            if (this.password == null || this.password == "") {
                isOk = false;
                this.textError = "Пароль не может быть пустым.";
                return isOk;
            } else {
                this.textError = null;
            }

            if (this.password.length < 5) {
                isOk = false;
                this.textError = "Пароль должен содержать не менее 5 символов.";
                return isOk;
            } else {
                this.textError = null;
            }

            if (this.password.indexOf(' ') >= 0) {
                isOk = false;
                this.textError = "Пароль не должен содержать пробелов.";
            } else {
                this.textError = null;
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
                data: JSON.stringify({ email: this.email, password: this.password, userSessionID: this.userSessionID }),
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
                data: JSON.stringify({ email: this.email, userSessionID: this.userSessionID }),
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
            if (this.validPassword() == false) {
                this.isValidPassword = false;
                return false;
            }
            this.isValidPassword = true;

            this.isSaving = true;
            return $.ajax({
                type: "POST",
                url: "/Identity/Account/RecoveryPassword2",
                context: this,
                data: JSON.stringify({ id: this.recoveryPassword2.userID, newPassword: this.password, userSessionID: this.userSessionID }),
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
            this.enterCode.userSessionID = this.userSessionID;

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
            this.enterCode.userSessionID = this.userSessionID;

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
        },
        kaypress: function (e) {
            if (e.keyCode == 13) {
                e.preventDefault();

                if (this.login.isShow) {
                    //this.loginSave();
                    $("#login-btn").click();
                } else if (this.registration.isShow) {
                    $("#registraion-btn").click();
                    //this.onRegistration();
                } else if (this.recoveryPassword.isShow) {
                    this.onRecoveryPassword();
                    //$("#recovery-btn").click();
                } else if (this.recoveryPassword2.isShow) {
                    //this.onSetNewPassword();
                    $("#recovery2-btn").click();
                } else if (this.enterCode.isShow) {
                    //this.checkCode();
                    $("#checkCode-btn").click();
                }
            }
        },
        get_browser_info: function () {

            var unknown = '-';

            // screen
            var screenSize = '';
            if (screen.width) {
                width = (screen.width) ? screen.width : '';
                height = (screen.height) ? screen.height : '';
                screenSize += '' + width + " x " + height;
            }

            // browser
            var nVer = navigator.appVersion;
            var nAgt = navigator.userAgent;
            var browser = navigator.appName;
            var version = '' + parseFloat(navigator.appVersion);
            var majorVersion = parseInt(navigator.appVersion, 10);
            var nameOffset, verOffset, ix;

            // Opera
            if ((verOffset = nAgt.indexOf('Opera')) != -1) {
                browser = 'Opera';
                version = nAgt.substring(verOffset + 6);
                if ((verOffset = nAgt.indexOf('Version')) != -1) {
                    version = nAgt.substring(verOffset + 8);
                }
            }
            // Opera Next
            if ((verOffset = nAgt.indexOf('OPR')) != -1) {
                browser = 'Opera';
                version = nAgt.substring(verOffset + 4);
            }
            // Edge
            else if ((verOffset = nAgt.indexOf('Edge')) != -1) {
                browser = 'Microsoft Edge';
                version = nAgt.substring(verOffset + 5);
            }
            // MSIE
            else if ((verOffset = nAgt.indexOf('MSIE')) != -1) {
                browser = 'Microsoft Internet Explorer';
                version = nAgt.substring(verOffset + 5);
            }
            // Chrome
            else if ((verOffset = nAgt.indexOf('Chrome')) != -1) {
                browser = 'Chrome';
                version = nAgt.substring(verOffset + 7);
            }
            // Safari
            else if ((verOffset = nAgt.indexOf('Safari')) != -1) {
                browser = 'Safari';
                version = nAgt.substring(verOffset + 7);
                if ((verOffset = nAgt.indexOf('Version')) != -1) {
                    version = nAgt.substring(verOffset + 8);
                }
            }
            // Firefox
            else if ((verOffset = nAgt.indexOf('Firefox')) != -1) {
                browser = 'Firefox';
                version = nAgt.substring(verOffset + 8);
            }
            // MSIE 11+
            else if (nAgt.indexOf('Trident/') != -1) {
                browser = 'Microsoft Internet Explorer';
                version = nAgt.substring(nAgt.indexOf('rv:') + 3);
            }
            // Other browsers
            else if ((nameOffset = nAgt.lastIndexOf(' ') + 1) < (verOffset = nAgt.lastIndexOf('/'))) {
                browser = nAgt.substring(nameOffset, verOffset);
                version = nAgt.substring(verOffset + 1);
                if (browser.toLowerCase() == browser.toUpperCase()) {
                    browser = navigator.appName;
                }
            }
            // trim the version string
            if ((ix = version.indexOf(';')) != -1) version = version.substring(0, ix);
            if ((ix = version.indexOf(' ')) != -1) version = version.substring(0, ix);
            if ((ix = version.indexOf(')')) != -1) version = version.substring(0, ix);

            majorVersion = parseInt('' + version, 10);
            if (isNaN(majorVersion)) {
                version = '' + parseFloat(navigator.appVersion);
                majorVersion = parseInt(navigator.appVersion, 10);
            }

            // mobile version
            var mobile = /Mobile|mini|Fennec|Android|iP(ad|od|hone)/.test(nVer);

            // cookie
            var cookieEnabled = (navigator.cookieEnabled) ? true : false;

            if (typeof navigator.cookieEnabled == 'undefined' && !cookieEnabled) {
                document.cookie = 'testcookie';
                cookieEnabled = (document.cookie.indexOf('testcookie') != -1) ? true : false;
            }

            // system
            var os = unknown;
            var clientStrings = [
                { s: 'Windows 10', r: /(Windows 10.0|Windows NT 10.0)/ },
                { s: 'Windows 8.1', r: /(Windows 8.1|Windows NT 6.3)/ },
                { s: 'Windows 8', r: /(Windows 8|Windows NT 6.2)/ },
                { s: 'Windows 7', r: /(Windows 7|Windows NT 6.1)/ },
                { s: 'Windows Vista', r: /Windows NT 6.0/ },
                { s: 'Windows Server 2003', r: /Windows NT 5.2/ },
                { s: 'Windows XP', r: /(Windows NT 5.1|Windows XP)/ },
                { s: 'Windows 2000', r: /(Windows NT 5.0|Windows 2000)/ },
                { s: 'Windows ME', r: /(Win 9x 4.90|Windows ME)/ },
                { s: 'Windows 98', r: /(Windows 98|Win98)/ },
                { s: 'Windows 95', r: /(Windows 95|Win95|Windows_95)/ },
                { s: 'Windows NT 4.0', r: /(Windows NT 4.0|WinNT4.0|WinNT|Windows NT)/ },
                { s: 'Windows CE', r: /Windows CE/ },
                { s: 'Windows 3.11', r: /Win16/ },
                { s: 'Android', r: /Android/ },
                { s: 'Open BSD', r: /OpenBSD/ },
                { s: 'Sun OS', r: /SunOS/ },
                { s: 'Linux', r: /(Linux|X11)/ },
                { s: 'iOS', r: /(iPhone|iPad|iPod)/ },
                { s: 'Mac OS X', r: /Mac OS X/ },
                { s: 'Mac OS', r: /(MacPPC|MacIntel|Mac_PowerPC|Macintosh)/ },
                { s: 'QNX', r: /QNX/ },
                { s: 'UNIX', r: /UNIX/ },
                { s: 'BeOS', r: /BeOS/ },
                { s: 'OS/2', r: /OS\/2/ },
                { s: 'Search Bot', r: /(nuhk|Googlebot|Yammybot|Openbot|Slurp|MSNBot|Ask Jeeves\/Teoma|ia_archiver)/ }
            ];
            for (var id in clientStrings) {
                var cs = clientStrings[id];
                if (cs.r.test(nAgt)) {
                    os = cs.s;
                    break;
                }
            }

            var osVersion = unknown;

            if (/Windows/.test(os)) {
                osVersion = /Windows (.*)/.exec(os)[1];
                os = 'Windows';
            }

            switch (os) {
                case 'Mac OS X':
                    osVersion = /Mac OS X (10[\.\_\d]+)/.exec(nAgt)[1];
                    break;

                case 'Android':
                    osVersion = /Android ([\.\_\d]+)/.exec(nAgt)[1];
                    break;

                case 'iOS':
                    osVersion = /OS (\d+)_(\d+)_?(\d+)?/.exec(nVer);
                    osVersion = osVersion[1] + '.' + osVersion[2] + '.' + (osVersion[3] | 0);
                    break;
            }

            // flash (you'll need to include swfobject)
            /* script src="//ajax.googleapis.com/ajax/libs/swfobject/2.2/swfobject.js" */
            var flashVersion = 'no check';
            if (typeof swfobject != 'undefined') {
                var fv = swfobject.getFlashPlayerVersion();
                if (fv.major > 0) {
                    flashVersion = fv.major + '.' + fv.minor + ' r' + fv.release;
                }
                else {
                    flashVersion = unknown;
                }
            }
            window.jscd = {
                screen: screenSize,
                browser: browser,
                browserVersion: version,
                browserMajorVersion: majorVersion,
                mobile: mobile,
                os: os,
                osVersion: osVersion,
                cookies: cookieEnabled,
                flashVersion: flashVersion
            }
        }

    }
});