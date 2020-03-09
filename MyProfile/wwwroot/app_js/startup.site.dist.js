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
/******/ 	return __webpack_require__(__webpack_require__.s = "./wwwroot/app_js/startup.site.js");
/******/ })
/************************************************************************/
/******/ ({

/***/ "./wwwroot/app_js/startup.site.js":
/*!****************************************!*\
  !*** ./wwwroot/app_js/startup.site.js ***!
  \****************************************/
/*! no static exports found */
/***/ (function(module, exports) {

eval("//const urls_dont_show_loading = [\n//    '/event/loadingeventlogs',\n//    '/event/index',\n//    '/event/getcounteventbyyear',\n//    '/taskboard/getscheduleitemtooltip',\n//    '/taskboard/loadingschedule',\n//    '/taskboard/refreshschedule',\n//    '/taskboard/getdata',\n//    '/taskboard/getmapdata',\n//    //\"/admins/edit\",\n//    '/admins/getadminurlimage',\n//    '/profile/getadmininfo',\n//    '/profile/imageedit',\n//    '/profile/getadmininfo',\n//    '/logs/logdata',\n//    '/logs/operationinfo',\n//    '/logs/objecttype',\n//    '/logs/adminimages',\n//    '/contract/getdevicesettings',\n//    '/contract/setdevicesettings',\n//    '/contract/getdevicestatus',\n//    '/contract/getestimateddaysleft',\n//    '/status/getfulldatawithcharts',\n//    '/reseller/statistics',\n//];\n//$(document).ajaxSend(function (event, response, sender) {\n//    //отключено отображение loading, когда работаем с select2\n//    if (sender.url.toLocaleLowerCase().indexOf(\"selectors\") > 0) {\n//        return;\n//    }\n//    let url = sender.url.toLocaleLowerCase();\n//    if (urls_dont_show_loading.some(x => url.indexOf(x) != -1) == false) {\n//        $('#page-wrapper').addClass('sk-loading');\n//        // #region url-исключения\n//        //После первого отображения лоадера, добавляем исключения.\n//        //if (urls_dont_show_loading.indexOf(\"/event/loadingeventlogs\") == -1) {\n//        //\turls_dont_show_loading.push(\"/event/loadingeventlogs\");\n//        //}\n//        //#endregion\n//    }\n//});\n//$(document).ajaxComplete(function () {\n//    $('#page-wrapper').removeClass('sk-loading');\n//    $('#wrapper').tooltip({\n//        selector: \"[data-toggle=tooltip]\",\n//        container: \"body\",\n//    });\n//});\n//$(document).on('preInit.dt', function (e, settings) {\n//    let dataTable = new $.fn.dataTable.Api(settings);\n//    if ($(dataTable.table().node()).hasClass('searchHighlight') || // table has class\n//        settings.oInit.searchHighlight || // option specified\n//        $.fn.dataTable.defaults.searchHighlight                    // default set\n//    ) {\n//        dataTable.on('draw', function () {\n//            if (dataTable.state && dataTable.state().search && dataTable.state().search.smart) {\n//                dataTable.state.clear();\n//            }\n//            highlightDataTable(dataTable);\n//        });\n//        dataTable.on('column-visibility.dt', function () {\n//            highlightDataTable(dataTable);\n//        });\n//    }\n//});\n//$(function () {\n//    // #region notification\n//    toastr.options = {\n//        closeButton: true,\n//        debug: false,\n//        progressBar: true,\n//        positionClass: \"toast-bottom-right\",\n//        onclick: null,\n//        showDuration: \"400\",\n//        hideDuration: \"1000\",\n//        timeOut: \"7000\",\n//        extendedTimeOut: \"1000\",\n//        showEasing: \"swing\",\n//        hideEasing: \"linear\",\n//        showMethod: \"fadeIn\",\n//        hideMethod: \"fadeOut\"\n//    }\n//    // #endregion\n//    if (document.location.href.toLowerCase().indexOf(\"taskboard/map\") > 0\n//        || document.location.href.toLowerCase().indexOf(\"admin/home\") > 0) {\n//        $(\".navbar-minimalize\").click();\n//    }\n//});\n//$(document).ready(function () {\n//    $.validator.methods.date = function (value, element) {\n//        return this.optional(element) || moment(value, \"DD.MM.YYYY\", true).isValid();\n//    }\n//    $.validator.methods.number = function (value, element) {\n//        return this.optional(element) || /^[-]?\\d{0,3}([\\,,\\.]\\d{0,7}){0,1}/.test(value);\n//    }\n//    $('[data-toggle=\"tooltip\"]').tooltip();\n//});\n\n//# sourceURL=webpack:///./wwwroot/app_js/startup.site.js?");

/***/ })

/******/ })));