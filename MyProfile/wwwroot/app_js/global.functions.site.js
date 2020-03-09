function highlightDataTable(dataTable) {
	//var body = $(dataTable.table().body());

	//body.unhighlight();
	//if (dataTable.rows({ filter: 'applied' }).data().length) {
	//    body.highlight(dataTable.search());
	//}
}

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


document.LocalizationLanguageFlags = [
	{
		id: 2,
		internationalCode: 'en-US',
		src16: '/Images/flags/16/United-States.png'
	},
	{
		id: 3,
		internationalCode: 'it-IT',
		src16: '/Images/flags/16/Italy.png'
	},
	{
		id: 9,
		internationalCode: 'es-ES',
		src16: '/Images/flags/16/Spain.png'
	},
	{
		id: 10,
		internationalCode: 'pt-PT',
		src16: '/Images/flags/16/Portugal.png'
	},
	{
		id: 15,
		internationalCode: 'fr-FR',
		src16: '/Images/flags/16/France.png'
	},
	{
		id: 20,
		internationalCode: 'de-DE',
		src16: '/Images/flags/16/Germany.png'
	},
];

function GetFlat16(languageID) {
	let language = document.LocalizationLanguageFlags.find(x => x.id == languageID);
	if (language) {
		return language.src16;
	} else {
		return "";
	}
}

//Change color to the opposite
function InvertColor(hexTripletColor) {
	var color = hexTripletColor;
	color = color.substring(1); // remove #
	color = parseInt(color, 16); // convert to integer
	color = 0xFFFFFF ^ color; // invert three bytes
	color = color.toString(16); // convert to hex
	color = ("000000" + color).slice(-6); // pad with leading zeros
	color = "#" + color; // prepend #
	return color;
}