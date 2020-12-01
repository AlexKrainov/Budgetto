
var PageVue = new Vue({
    el: "#page-vue",
    data: {
        userSessionID: null,
        email: null,

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
            isLandingPage: true,
            path: "",
            dateCreate: null,


            continent_code: "",
            continent_name: "",
            info: "",
        },

    },
    computed: {
    },
    watch: {
    },
    mounted: function () {

        this.userSessionID = UserSessionID;

        this.person_data.userSessionID = this.userSessionID;
        this.person_data.referrer = document.referrer;

        if (ip == "::1") {
            this.person_data = { "userSessionID": this.userSessionID, "ip": "Local", "city": "Moscow", "country": "Russia", "location": "55.7522, 37.6156", "index": "111111", "browser_name": "Chrome", "browser_version": 85, "os_name": "Windows", "os_version": "10", "screen_size": "1536 x 864", "referrer": "", "isMobile": false, "isLoad": false, "isShow": false, "path": "/Identity/Account/Login", "dateCreate": null, "continent_code": "EU", "continent_name": "Europe", "info": "", "provider_info": "{\"asn\":\"AS8402\",\"name\":\"PJSC \\\"Vimpelcom\\\"\",\"domain\":\"veon.com\",\"route\":\"37.144.0.0/14\",\"type\":\"isp\"}", "threat": "{\"is_tor\":false,\"is_proxy\":false,\"is_anonymous\":false,\"is_known_attacker\":false,\"is_known_abuser\":false,\"is_threat\":false,\"is_bogon\":false}, isLandingPage: true" };
            $.ajax({
                type: "POST",
                url: "/Home/UpdateUserSession",
                context: PageVue,
                data: JSON.stringify(this.person_data),
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (response) {
                    PageVue.userSessionID = response.userSessionID;
                    return response;
                },
                error: function (xhr, status, error) {
                    console.log(error);
                }
            });
            this.initSettingsPage();
            return;
        }
        try {
            $.get(`https://api.ipdata.co/${ip}?api-key=65b72cf7f83e7582fd54e2c3fad07548678dfa6e363424eb773edbd5`, function (response) {


                PageVue.person_data.userSessionID = PageVue.userSessionID;
                PageVue.person_data.ip = response.ip;
                PageVue.person_data.city = response.city;
                PageVue.person_data.country = response.country_name;
                PageVue.person_data.continent_code = response.continent_code;
                PageVue.person_data.continent_name = response.continent_name;
                //PageVue.person_data.hostname = response.hostname;
                PageVue.person_data.location = response.latitude + ", " + response.longitude;
                PageVue.person_data.index = response.postal;
                PageVue.person_data.provider_info = JSON.stringify(response.asn);
                PageVue.person_data.current_user_time = response.current_time;
                PageVue.person_data.threat = JSON.stringify(response.threat);

                PageVue.get_browser_info();
                let jscd = window.jscd;

                PageVue.person_data.browser_name = jscd.browser;
                PageVue.person_data.browser_version = jscd.browserMajorVersion;
                PageVue.person_data.os_name = jscd.os;
                PageVue.person_data.os_version = jscd.osVersion;
                PageVue.person_data.isMobile = jscd.mobile;
                PageVue.person_data.screen_size = jscd.screen;
                PageVue.person_data.referrer = document.referrer;
                PageVue.person_data.path = document.location.pathname;

                return PageVue.person_data;
            }).then(function () {
                return $.ajax({
                    type: "POST",
                    url: "/Home/UpdateUserSession",
                    context: PageVue,
                    data: JSON.stringify(PageVue.person_data),
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'json',
                    success: function (response) {
                        PageVue.userSessionID = response.userSessionID;
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
                url: "/Home/UpdateUserSession",
                context: PageVue,
                data: JSON.stringify(this.person_data),
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (response) {
                    PageVue.userSessionID = response.userSessionID;
                    return response;
                },
                error: function (xhr, status, error) {
                    this.isSaving = false;
                    console.log(error);
                }
            });
        }
        this.initSettingsPage();
    },
    methods: {
        goToAppBudgetto: function (linkName) {
            return $.ajax({
                type: "GET",
                url: "/Home/GoToBudgetto?id=" + PageVue.userSessionID + "&linkName=" + linkName,
                context: this,
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (response) {
                    let str = "";
                    if (this.email) {
                        str = "&email=" + this.email;
                    }
                    document.location.href = "https://app.budgetto.org/Identity/Account/Login?isLandingPage=true&id=" + this.userSessionID + str;
                    return response;
                },
                error: function (xhr, status, error) {
                    let str = "";
                    if (this.email) {
                        str = "&email=" + this.email;
                    }
                    document.location.href = "https://app.budgetto.org/Identity/Account/Login?isLandingPage=true&id=" + this.userSessionID + str;
                    console.log(error);
                }
            });
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
        },
        go_to: function (name) {
            $("html, body").animate({
                scrollTop: $(name).offset().top - (name == "#how-it-works" ? 50 : 0)
            }, 1000);
        },
        showDocument: function (name) {
            return $.ajax({
                type: "GET",
                url: "/Home/ShowDocument?name=" + name,
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (response) {
                    return response;
                },
                error: function (xhr, status, error) {
                }
            });
        },
        showMore: function () {
            return $.ajax({
                type: "GET",
                url: "/Home/ShowMore",
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (response) {
                    return response;
                },
                error: function (xhr, status, error) {
                }
            });
        },
        initSettingsPage: function () {
            var navbarScrollThreshold = 20;
            var navbarBreakpoint = 992;

            var navbarCustomClasses = {
                default: {
                    variant: 'navbar-light',
                    classes: 'pt-lg-4'
                },
                alt: {
                    variant: 'bg-white',
                    classes: 'py-1'
                }
            };

            // Navbar scroll behaviour
            //

            var $navbar = $('.landing-navbar');
            var $navbarCollapse = $('#landing-navbar-collapse');

            $(document).on('scroll', function (e) {
                var scrollTop = $(document).scrollTop();

                if (scrollTop > navbarScrollThreshold && !$navbar.hasClass('landing-navbar-alt')) {
                    $navbar
                        .addClass('landing-navbar-alt')
                        .removeClass(navbarCustomClasses.default.variant + ' ' + navbarCustomClasses.default.classes)
                        .addClass(navbarCustomClasses.alt.variant + ' ' + navbarCustomClasses.alt.classes)
                        .find('> div')
                        .removeClass('container-fluid')
                        .addClass('container');
                } else if (scrollTop <= navbarScrollThreshold && $navbar.hasClass('landing-navbar-alt')) {
                    $navbar.removeClass('landing-navbar-alt')
                        .addClass(navbarCustomClasses.default.classes)
                        .removeClass(navbarCustomClasses.alt.classes)
                        .find('> div')
                        .addClass('container-fluid')
                        .removeClass('container');

                    if ($(window).outerWidth() >= navbarBreakpoint || !$navbarCollapse.hasClass('show')) {
                        $navbar
                            .addClass(navbarCustomClasses.default.variant)
                            .removeClass(navbarCustomClasses.alt.variant);
                    }
                }
            });

            $navbarCollapse.on('show.bs.collapse hidden.bs.collapse', function (e) {
                if ($navbar.hasClass('landing-navbar-alt')) return;

                $navbar[e.type === 'show' ? 'removeClass' : 'addClass'](
                    navbarCustomClasses.default.variant
                );

                $navbar[e.type === 'show' ? 'addClass' : 'removeClass'](
                    navbarCustomClasses.alt.variant
                );
            });

            $(window).on('resize', function () {
                if ($navbar.hasClass('landing-navbar-alt')) return;

                var sm = $(this).outerWidth() < navbarBreakpoint;
                var alt = $navbar.hasClass(navbarCustomClasses.alt.variant);

                if (sm && !alt && $navbarCollapse.hasClass('show')) {
                    $navbar
                        .removeClass(navbarCustomClasses.default.variant)
                        .addClass(navbarCustomClasses.alt.variant);
                } else if (!sm && alt) {
                    $navbar
                        .removeClass(navbarCustomClasses.alt.variant)
                        .addClass(navbarCustomClasses.default.variant);
                }
            });


            $('#landing-slider-parallax').each(function () {
                new Swiper(this, {
                    parallax: true,
                    autoHeight: true,
                    speed: 1000,
                    followFinger: false,
                    threshold: 50,
                    preventClicks: true,
                    navigation: {
                        nextEl: '#landing-slider-next',
                        prevEl: '#landing-slider-prev'
                    }
                });
            });

            // App preview
            //

            $('#landing-preview-slider').each(function () {
                new Swiper(this, {
                    slidesPerView: 3,
                    spaceBetween: 0,
                    threshold: 50,
                    speed: 400,
                    centeredSlides: true,
                    slideToClickedSlide: true,
                    breakpoints: {
                        992: {
                            slidesPerView: 1,
                            spaceBetween: 20
                        }
                    },
                    pagination: {
                        el: '.swiper-pagination',
                        clickable: true
                    }
                });
            });

            $('#shop-preview-slider').each(function () {
                new Swiper(this, {
                    slidesPerView: 3,
                    spaceBetween: 8,
                    threshold: 20,
                    navigation: {
                        nextEl: $('#shop-preview-slider-next')[0],
                        prevEl: $('#shop-preview-slider-prev')[0]
                    }
                });
            });

            $('#shop-preview-slider').on('click', 'a', function (e) {
                e.preventDefault();
                $('#shop-preview-slider .border-primary').removeClass('border-primary');
                $(this).addClass('border-primary');
                $('#shop-preview-image img').attr('src', $(this).find('img').attr('src'));
            });

            $('#shop-preview-image').on('click', function (e) {
                e.preventDefault();

                // Unset focus
                $(this).blur();

                var curLink = $(this).find('img')[0].src;
                var links = [];

                $('#shop-preview-slider').find('img').each(function () {
                    links.push(this.src);
                });

                window.blueimpGallery(links, {
                    container: '#shop-preview-lightbox',
                    carousel: false,
                    hidePageScrollbars: true,
                    disableScroll: true,
                    index: links.indexOf(curLink)
                });
            });
        }
    }
});