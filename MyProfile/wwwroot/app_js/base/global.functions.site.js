String.prototype.replaceAll = function (search, replace) {
    return this.split(search).join(replace);
}

//send ajax by jquery , by default method send = "POST"
function sendAjax(url, value = null, type = "POST") {
    return $.ajax({
        type: type,
        url: url,
        data: value ? JSON.stringify(value) : null,
        contentType: 'application/json; charset=utf-8',
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
function GetLinkForView(template) {
    if (template.periodTypeID == 1) { //PeriodTypesEnum.Days
        return `/Budget/Month?templateID=${template.id}`;
    } else if (template.periodTypeID == 3) { //PeriodTypesEnum.Months
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
        plugins: [
            new monthSelectPlugin({
                shorthand: true, //defaults to false
                dateFormat: "yy/m/d", //defaults to "F Y"
                altFormat: "F Y", //defaults to "F Y"
                theme: "light" // defaults to "light"
            })
        ],
        onChange : function (selectedDates, dateStr, instance) {
            LimitListVue.flatpickrStart.config.maxDate = dateStr;
        },
    };
}
