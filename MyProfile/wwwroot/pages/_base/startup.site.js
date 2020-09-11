//const urls_dont_show_loading = [
//    '/event/loadingeventlogs',
//    '/event/index',
//    '/event/getcounteventbyyear',
//    '/taskboard/getscheduleitemtooltip',
//    '/taskboard/loadingschedule',
//    '/taskboard/refreshschedule',
//    '/taskboard/getdata',
//    '/taskboard/getmapdata',
//    //"/admins/edit",
//    '/admins/getadminurlimage',
//    '/profile/getadmininfo',
//    '/profile/imageedit',
//    '/profile/getadmininfo',
//    '/logs/logdata',
//    '/logs/operationinfo',
//    '/logs/objecttype',
//    '/logs/adminimages',
//    '/contract/getdevicesettings',
//    '/contract/setdevicesettings',
//    '/contract/getdevicestatus',
//    '/contract/getestimateddaysleft',
//    '/status/getfulldatawithcharts',
//    '/reseller/statistics',
//];

//$(document).ajaxSend(function (event, response, sender) {

//    //отключено отображение loading, когда работаем с select2
//    if (sender.url.toLocaleLowerCase().indexOf("selectors") > 0) {
//        return;
//    }
//    let url = sender.url.toLocaleLowerCase();
//    if (urls_dont_show_loading.some(x => url.indexOf(x) != -1) == false) {
//        $('#page-wrapper').addClass('sk-loading');

//        // #region url-исключения
//        //После первого отображения лоадера, добавляем исключения.

//        //if (urls_dont_show_loading.indexOf("/event/loadingeventlogs") == -1) {
//        //	urls_dont_show_loading.push("/event/loadingeventlogs");
//        //}

//        //#endregion
//    }
//});

//$(document).ajaxComplete(function () {
//    $('#page-wrapper').removeClass('sk-loading');

//    $('#wrapper').tooltip({
//        selector: "[data-toggle=tooltip]",
//        container: "body",
//    });
//});

//$(document).on('preInit.dt', function (e, settings) {
//    let dataTable = new $.fn.dataTable.Api(settings);

//    if ($(dataTable.table().node()).hasClass('searchHighlight') || // table has class
//        settings.oInit.searchHighlight || // option specified
//        $.fn.dataTable.defaults.searchHighlight                    // default set
//    ) {
//        dataTable.on('draw', function () {
//            if (dataTable.state && dataTable.state().search && dataTable.state().search.smart) {
//                dataTable.state.clear();
//            }
//            highlightDataTable(dataTable);
//        });
//        dataTable.on('column-visibility.dt', function () {
//            highlightDataTable(dataTable);
//        });
//    }
//});

//$(function () {
//    // #region notification
//    toastr.options = {
//        closeButton: true,
//        debug: false,
//        progressBar: true,
//        positionClass: "toast-bottom-right",
//        onclick: null,
//        showDuration: "400",
//        hideDuration: "1000",
//        timeOut: "7000",
//        extendedTimeOut: "1000",
//        showEasing: "swing",
//        hideEasing: "linear",
//        showMethod: "fadeIn",
//        hideMethod: "fadeOut"
//    }
//    // #endregion
//    if (document.location.href.toLowerCase().indexOf("taskboard/map") > 0
//        || document.location.href.toLowerCase().indexOf("admin/home") > 0) {
//        $(".navbar-minimalize").click();
//    }
//});

//$(document).ready(function () {
//    $.validator.methods.date = function (value, element) {
//        return this.optional(element) || moment(value, "DD.MM.YYYY", true).isValid();
//    }
//    $.validator.methods.number = function (value, element) {
//        return this.optional(element) || /^[-]?\d{0,3}([\,,\.]\d{0,7}){0,1}/.test(value);
//    }
//    $('[data-toggle="tooltip"]').tooltip();
//});

$(document).ready(function () {
    // Auto update layout
    window.layoutHelpers.setAutoUpdate(true);

    // Collapse menu
    if ($('#layout-sidenav').hasClass('sidenav-horizontal') || window.layoutHelpers.isSmallScreen()) {
        return;
    }

    try {
        window.layoutHelpers.setCollapsed(
            localStorage.getItem('layoutCollapsed') === 'true',
            false
        );
    } catch (e) { }

    // Initialize sidenav
    $('#layout-sidenav').each(function () {
        new SideNav(this, {
            orientation: $(this).hasClass('sidenav-horizontal') ? 'horizontal' : 'vertical'
        });
    });

   
    // Initialize sidenav togglers
    $('body').on('click', '.layout-sidenav-toggle', function (e) {
        e.preventDefault();
        window.layoutHelpers.toggleCollapsed();
        if (!window.layoutHelpers.isSmallScreen()) {
            try { localStorage.setItem('layoutCollapsed', String(window.layoutHelpers.isCollapsed())); } catch (e) { }
        }
    });

    //if ($('html').attr('dir') === 'rtl') {
    //    $('#layout-navbar .dropdown-menu').toggleClass('dropdown-menu-right');
    //}



    if ($.fn.dataTable) {
        $.fn.dataTable.ext.type.detect.unshift(tablePreOrder);
        $.fn.dataTable.ext.type.order['money-pre'] = tableOrder;
        $.fn.dataTable.ext.type.order['day-pre'] = tableOrder;
    }
});




var RecordVue = new Vue({
    el: "#record-container",
    data: {
        callback: null,
    },
    computed: {
        recordComponent: function () {
            return this.$children[0];
        }
    },
    methods: {
        addRecord: function myfunction() {
            if (RecordVue.callback) {
                return this.recordComponent.showModel(undefined, RecordVue.callback);
            } else {
                return this.recordComponent.showModel();
            }
        },
        showModel: function (dateTime, callback) {
            return this.recordComponent.showModel(dateTime, callback);
        },
        editByElement: function (record, callback, args) {
            return this.recordComponent.editByElement(record, callback, args);
        },
        updateSectionComponent: function () {
            this.recordComponent.sectionComponent.load();
        },
        refreshSections: function () {
            this.recordComponent.sectionComponent.load();
        }
    }
});



