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