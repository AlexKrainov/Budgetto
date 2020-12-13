String.prototype.replaceAll = function (search, replace) {
    return this.split(search).join(replace);
}

//send ajax by jquery , by default method send = "POST"
function sendAjax(url, value = null, type = "POST") {
    return $.ajax({
        type: type,
        url: url,
        data: value ? JSON.stringify(value) : null,
        contentType: 'application/json',
        dataType: 'json',
        success: function (response) {
            return response;
        },
        error: function (xhr, status, error) {
            console.log(error);
        }
    });
}

function parseQueryString() {

    var str = window.location.search;
    var objURL = {};

    str.replace(
        new RegExp("([^?=&]+)(=([^&]*))?", "g"),
        function ($0, $1, $2, $3) {
            objURL[$1] = $3;
        }
    );
    return objURL;
};

//return string by 3 digits {### ### ###}
function numberOfThreeDigits(str) {
    return str ? str.toString().replace(/(\d)(?=(\d\d\d)+([^\d]|$))/g, '$1 ') : "";
}

function GetDateByFormat(date, format) {
    if (date && moment(date).isValid()) {
        return moment(date).format(format);
    }
    return "";
}


var TemplateColumnTypeEnum = Object.freeze({
    Undefined: 0,
    BudgetSection: 1,
    DaysForMonth: 2,
    MonthsForYear: 3,
    YearForYear: 4,
    Percent: 5,
    Comment: 6,
    WeekForMonth: 7
});

var FooterActionTypeEnum = Object.freeze({
    Undefined: 0,
    Sum: 1,
    Avr: 2,
    Min: 3,
    Max: 4
});

var FormulaFieldTypeEnum = Object.freeze({
    Undefined: 0,
    Section: 1,
    Number: 2,
    Mark: 3,
    Parentheses: 4,
    Days: 5
});

var PeriodTypeEnum = Object.freeze({
    Undefined: 0,
    Month: 1,
    Weeks: 2,
    Year: 3,
    Years10: 4
});

var PaletteColorPicker = ["#1abc99", "#16a085", "#2ecc71", "#27ae60", "#3498db", "#2980b9", "#9b59b6", "#8e44ad", "#34495e", "#2c3e50", "#f1c40f", "#f39c12", "#e67e22", "#d35400", "#e74c3c", "#c0392b", "#ecf0f1", "#bdc3c7", "#95a5a6", "#7f8c8d"];

function GetRandomColor() {
    return PaletteColorPicker[GetRandomInt(PaletteColorPicker.length)];
}

function GetRandomInt(max) {
    return Math.floor(Math.random() * Math.floor(max));
}

function ParseXml(xml, arrayTags) {
    var dom = null;
    if (window.DOMParser) {
        dom = (new DOMParser()).parseFromString(xml, "text/xml");
    }
    else if (window.ActiveXObject) {
        dom = new ActiveXObject('Microsoft.XMLDOM');
        dom.async = false;
        if (!dom.loadXML(xml)) {
            throw dom.parseError.reason + " " + dom.parseError.srcText;
        }
    }
    else {
        throw "cannot parse xml string!";
    }

    function isArray(o) {
        return Object.prototype.toString.apply(o) === '[object Array]';
    }

    function parseNode(xmlNode, result) {
        if (xmlNode.nodeName == "#text") {
            var v = xmlNode.nodeValue;
            if (v.trim()) {
                result['#text'] = v;
            }
            return;
        }

        var jsonNode = {};
        var existing = result[xmlNode.nodeName];
        if (existing) {
            if (!isArray(existing)) {
                result[xmlNode.nodeName] = [existing, jsonNode];
            }
            else {
                result[xmlNode.nodeName].push(jsonNode);
            }
        }
        else {
            if (arrayTags && arrayTags.indexOf(xmlNode.nodeName) != -1) {
                result[xmlNode.nodeName] = [jsonNode];
            }
            else {
                result[xmlNode.nodeName] = jsonNode;
            }
        }

        if (xmlNode.attributes) {
            var length = xmlNode.attributes.length;
            for (var i = 0; i < length; i++) {
                var attribute = xmlNode.attributes[i];
                jsonNode[attribute.nodeName] = attribute.nodeValue;
            }
        }

        var length = xmlNode.childNodes.length;
        for (var i = 0; i < length; i++) {
            parseNode(xmlNode.childNodes[i], jsonNode);
        }
    }

    var result = {};
    for (let i = 0; i < dom.childNodes.length; i++) {
        parseNode(dom.childNodes[i], result);
    }

    return result;
}

//Template methods
function TemplateGetLinkForView(template) {
    if (template.periodTypeID == PeriodTypeEnum.Month) {
        return `/Budget/Month?templateID=${template.id}`;
    } else if (template.periodTypeID == PeriodTypeEnum.Year) {
        return `/Budget/Year?templateID=${template.id}`;
    } else {
        return '/Budget/Index/' + template.id;
    }
}

function GetRusName(n, text_forms) {
    n = Math.abs(n) % 100; var n1 = n % 10;
    if (n > 10 && n < 20) { return text_forms[2]; }
    if (n1 > 1 && n1 < 5) { return text_forms[1]; }
    if (n1 == 1) { return text_forms[0]; }
    return text_forms[2];
}

function GetFlatpickrRuConfig_Month(date, minDate, maxDate) {
    return {
        altInput: true,
        locale: "ru",
        defaultDate: date,
        minDate: minDate,
        maxDate: maxDate,
        disableMobile: "true",
        plugins: [
            new monthSelectPlugin({
                shorthand: true, //defaults to false
                dateFormat: "Y/m/d", //defaults to "F Y"
                altFormat: "F Y", //defaults to "F Y"
                theme: "light" // defaults to "light"
            })
        ],
    };
}

function GetFlatpickrRuConfig(date, minDate, maxDate) {
    return {
        altInput: true,
        locale: "ru",
        dateFormat: 'Y/m/d',
        defaultDate: date,
        minDate: minDate,
        maxDate: maxDate,
    };
}

function ShowLoading(selector) {
    var overlayBg = themeSettings.isDarkStyle()
        ? '#22252B'
        : '#fff';
    $(selector).block({
        message: '<div class="sk-wave sk-primary mx-auto"><div class="sk-wave-rect"></div><div class="sk-wave-rect"></div><div class="sk-wave-rect"></div><div class="sk-wave-rect"></div><div class="sk-wave-rect"></div></div>',
        css: {
            backgroundColor: 'transparent',
            border: '0',
            zIndex: 1081
        },
        overlayCSS: {
            backgroundColor: overlayBg,
            opacity: 0.8,
            zIndex: 1080
        }
    });
}
function HideLoading(selector) {
    $(selector).unblock();
}

function CurrencyCalculateExpression(rawData, exchangeRate) {
    let newValue = `(${rawData}) * ${exchangeRate}`;
    let func = compileExpression(newValue);
    return func("1");
}

function CalculateExpression(rawData) {
    let func = compileExpression(rawData);
    return func("1");
}

//table
function tableOrder(d) {
    let str = d.toString();
    let index = str.search("data-value");
    let subString = str.substring(12 + index);
    let indexSubString = subString.search('"');
    let dataValue = subString.substring(0, indexSubString);
    return dataValue * 1;
}
function tablePreOrder(d) {
    let str = d.toString();
    let index = str.search("data-type");
    let dataColumnType = str.substring(11 + index, 12 + index);

    switch (dataColumnType) {
        case TemplateColumnTypeEnum.BudgetSection:
            return "money";
        case TemplateColumnTypeEnum.DaysForMonth:
        case TemplateColumnTypeEnum.MonthsForYear:
        case TemplateColumnTypeEnum.YearForYear:
            return "day"
        default:
            return "money";
    }

    return 'money';
}


jQuery.fn.scrollCenter = function (elem, speed) {

    var active = jQuery(this).find(elem); // find the active element
    //var activeWidth = active.width(); // get active width
    var activeWidth = active.width() / 2; // get active width center

    //alert(activeWidth)

    //var pos = jQuery('#timepicker .active').position().left; //get left position of active li
    // var pos = jQuery(elem).position().left; //get left position of active li
    //var pos = jQuery(this).find(elem).position().left; //get left position of active li
    var pos = active.position().left + activeWidth; //get left position of active li + center position
    var elpos = jQuery(this).scrollLeft(); // get current scroll position
    var elW = jQuery(this).width(); //get div width
    //var divwidth = jQuery(elem).width(); //get div width
    pos = pos + elpos - elW / 2; // for center position if you want adjust then change this

    jQuery(this).animate({
        scrollLeft: pos
    }, speed == undefined ? 1000 : speed);
    return this;
};

// http://podzic.com/wp-content/plugins/podzic/include/js/podzic.js
jQuery.fn.scrollCenterORI = function (elem, speed) {
    jQuery(this).animate({
        scrollLeft: jQuery(this).scrollLeft() - jQuery(this).offset().left + jQuery(elem).offset().left
    }, speed == undefined ? 1000 : speed);
    return this;
};

function GetCookie(name) {
    const value = `; ${document.cookie}`;
    const parts = value.split(`; ${name}=`);
    if (parts.length === 2) return parts.pop().split(';').shift();
}

function CheckAuthenticated() {
    let val = { uid: UserInfo.ID, usid: UserInfo.UserSessionID, ue: UserInfo.Email };
    return $.ajax({
        type: "POST",
        url: "/Settings/CheckAuthenticated",
        data: JSON.stringify(val),
        contentType: 'application/json',
        dataType: 'json',
        success: function (response) {
            document.CheckAuthorization = false;
            return response;
        },
        error: function (xhr, status, error) {
            console.log(error);
        }
    });
}

function onTabFocus() {
    if (document.CheckAuthorization) {
        CheckAuthenticated();
    }
}
function onTabBlur() {
    if (document.CheckAuthorization) {
        CheckAuthenticated();
    }
}

function onKeyListener(e) {
    //console.log("click");
    //e = e || window.event;

    //if (e.keyCode == 116) {
    //    console.log("f5 pressed");
    //} else {
    //    console.log("Window closed");
    //}
}

//async/await
function onCloseWindow() {
    return $.ajax({
        type: "GET",
        url: "/Settings/LeaveSite?UserSessionID=" + UserInfo.UserSessionID,
        contentType: 'application/json',
        dataType: 'json'
    });
}

function ShowCookieOff() {
    $("#cookie-model").remove();
    return $.ajax({
        type: "GET",
        url: "/UserSettings/ShowCookieOff",
        contentType: 'application/json',
        dataType: 'json',
        success: function (response) {
            UserInfo.UserSettings.IsShowCookie = false;
            return response;
        },
        error: function (xhr, status, error) {
        }
    });
}

function ShowDocument(name) {
    return $.ajax({
        type: "GET",
        url: "/UserSettings/ShowDocument?name=" + name,
        contentType: 'application/json',
        dataType: 'json',
        success: function (response) {
            return response;
        },
        error: function (xhr, status, error) {
        }
    });
}

function JSCopyObject(src) {
    return Object.assign({}, src);
}