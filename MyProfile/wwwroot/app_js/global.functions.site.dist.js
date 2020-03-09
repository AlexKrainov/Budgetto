(function(e, a) { for(var i in a) e[i] = a[i]; }(window, /******/ (function(modules) { // webpackBootstrap
/******/ 	// The module cache
/******/ 	var installedModules = {};
/******/
/******/ 	// The require function
/******/ 	function __webpack_require__(moduleId) {
/******/
/******/ 		// Check if module is in cache
/******/ 		if(installedModules[moduleId]) {
/******/ 			return installedModules[moduleId].exports;
/******/ 		}
/******/ 		// Create a new module (and put it into the cache)
/******/ 		var module = installedModules[moduleId] = {
/******/ 			i: moduleId,
/******/ 			l: false,
/******/ 			exports: {}
/******/ 		};
/******/
/******/ 		// Execute the module function
/******/ 		modules[moduleId].call(module.exports, module, module.exports, __webpack_require__);
/******/
/******/ 		// Flag the module as loaded
/******/ 		module.l = true;
/******/
/******/ 		// Return the exports of the module
/******/ 		return module.exports;
/******/ 	}
/******/
/******/
/******/ 	// expose the modules object (__webpack_modules__)
/******/ 	__webpack_require__.m = modules;
/******/
/******/ 	// expose the module cache
/******/ 	__webpack_require__.c = installedModules;
/******/
/******/ 	// define getter function for harmony exports
/******/ 	__webpack_require__.d = function(exports, name, getter) {
/******/ 		if(!__webpack_require__.o(exports, name)) {
/******/ 			Object.defineProperty(exports, name, { enumerable: true, get: getter });
/******/ 		}
/******/ 	};
/******/
/******/ 	// define __esModule on exports
/******/ 	__webpack_require__.r = function(exports) {
/******/ 		if(typeof Symbol !== 'undefined' && Symbol.toStringTag) {
/******/ 			Object.defineProperty(exports, Symbol.toStringTag, { value: 'Module' });
/******/ 		}
/******/ 		Object.defineProperty(exports, '__esModule', { value: true });
/******/ 	};
/******/
/******/ 	// create a fake namespace object
/******/ 	// mode & 1: value is a module id, require it
/******/ 	// mode & 2: merge all properties of value into the ns
/******/ 	// mode & 4: return value when already ns object
/******/ 	// mode & 8|1: behave like require
/******/ 	__webpack_require__.t = function(value, mode) {
/******/ 		if(mode & 1) value = __webpack_require__(value);
/******/ 		if(mode & 8) return value;
/******/ 		if((mode & 4) && typeof value === 'object' && value && value.__esModule) return value;
/******/ 		var ns = Object.create(null);
/******/ 		__webpack_require__.r(ns);
/******/ 		Object.defineProperty(ns, 'default', { enumerable: true, value: value });
/******/ 		if(mode & 2 && typeof value != 'string') for(var key in value) __webpack_require__.d(ns, key, function(key) { return value[key]; }.bind(null, key));
/******/ 		return ns;
/******/ 	};
/******/
/******/ 	// getDefaultExport function for compatibility with non-harmony modules
/******/ 	__webpack_require__.n = function(module) {
/******/ 		var getter = module && module.__esModule ?
/******/ 			function getDefault() { return module['default']; } :
/******/ 			function getModuleExports() { return module; };
/******/ 		__webpack_require__.d(getter, 'a', getter);
/******/ 		return getter;
/******/ 	};
/******/
/******/ 	// Object.prototype.hasOwnProperty.call
/******/ 	__webpack_require__.o = function(object, property) { return Object.prototype.hasOwnProperty.call(object, property); };
/******/
/******/ 	// __webpack_public_path__
/******/ 	__webpack_require__.p = "";
/******/
/******/
/******/ 	// Load entry module and return exports
/******/ 	return __webpack_require__(__webpack_require__.s = "./wwwroot/app_js/global.functions.site.js");
/******/ })
/************************************************************************/
/******/ ({

/***/ "./wwwroot/app_js/global.functions.site.js":
/*!*************************************************!*\
  !*** ./wwwroot/app_js/global.functions.site.js ***!
  \*************************************************/
/*! no static exports found */
/***/ (function(module, exports) {

eval("function highlightDataTable(dataTable) {//var body = $(dataTable.table().body());\n  //body.unhighlight();\n  //if (dataTable.rows({ filter: 'applied' }).data().length) {\n  //    body.highlight(dataTable.search());\n  //}\n}\n\nString.prototype.replaceAll = function (search, replace) {\n  return this.split(search).join(replace);\n}; //send ajax by jquery , by default method send = \"POST\"\n\n\nfunction sendAjax(url) {\n  var value = arguments.length > 1 && arguments[1] !== undefined ? arguments[1] : null;\n  var type = arguments.length > 2 && arguments[2] !== undefined ? arguments[2] : \"POST\";\n  return $.ajax({\n    type: type,\n    url: url,\n    data: value ? JSON.stringify(value) : null,\n    contentType: 'application/json; charset=utf-8',\n    dataType: 'json',\n    success: function success(response) {\n      return response;\n    },\n    error: function error(xhr, status, _error) {\n      console.log(_error);\n    }\n  });\n}\n\nfunction parseQueryString() {\n  var str = window.location.search;\n  var objURL = {};\n  str.replace(new RegExp(\"([^?=&]+)(=([^&]*))?\", \"g\"), function ($0, $1, $2, $3) {\n    objURL[$1] = $3;\n  });\n  return objURL;\n}\n\n; //return string by 3 digits {### ### ###}\n\nfunction numberOfThreeDigits(str) {\n  return str ? str.toString().replace(/(\\d)(?=(\\d\\d\\d)+([^\\d]|$))/g, '$1 ') : \"\";\n}\n\ndocument.LocalizationLanguageFlags = [{\n  id: 2,\n  internationalCode: 'en-US',\n  src16: '/Images/flags/16/United-States.png'\n}, {\n  id: 3,\n  internationalCode: 'it-IT',\n  src16: '/Images/flags/16/Italy.png'\n}, {\n  id: 9,\n  internationalCode: 'es-ES',\n  src16: '/Images/flags/16/Spain.png'\n}, {\n  id: 10,\n  internationalCode: 'pt-PT',\n  src16: '/Images/flags/16/Portugal.png'\n}, {\n  id: 15,\n  internationalCode: 'fr-FR',\n  src16: '/Images/flags/16/France.png'\n}, {\n  id: 20,\n  internationalCode: 'de-DE',\n  src16: '/Images/flags/16/Germany.png'\n}];\n\nfunction GetFlat16(languageID) {\n  var language = document.LocalizationLanguageFlags.find(function (x) {\n    return x.id == languageID;\n  });\n\n  if (language) {\n    return language.src16;\n  } else {\n    return \"\";\n  }\n} //Change color to the opposite\n\n\nfunction InvertColor(hexTripletColor) {\n  var color = hexTripletColor;\n  color = color.substring(1); // remove #\n\n  color = parseInt(color, 16); // convert to integer\n\n  color = 0xFFFFFF ^ color; // invert three bytes\n\n  color = color.toString(16); // convert to hex\n\n  color = (\"000000\" + color).slice(-6); // pad with leading zeros\n\n  color = \"#\" + color; // prepend #\n\n  return color;\n}\n\n//# sourceURL=webpack:///./wwwroot/app_js/global.functions.site.js?");

/***/ })

/******/ })));