$(document).ready(function () {
    // Auto update layout
    window.layoutHelpers.setAutoUpdate(true);

    if (window.layoutHelpers.isSmallScreen()) {
        window.layoutHelpers.setCollapsed(true, false);
        localStorage.setItem('layoutCollapsed', "true");
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
    $('.layout-sidenav-toggle').on('click', function (e) {

        window.layoutHelpers.toggleCollapsed();
        e.preventDefault();
        if (!window.layoutHelpers.isSmallScreen()) {
            try { localStorage.setItem('layoutCollapsed', String(window.layoutHelpers.isCollapsed())); } catch (e) { }
        }
    });

    if ($.fn.dataTable) {
        $.fn.dataTable.ext.type.detect.unshift(tablePreOrder);
        $.fn.dataTable.ext.type.order['money-pre'] = tableOrder;
        $.fn.dataTable.ext.type.order['day-pre'] = tableOrder;
    }

    //#region auth
    document.CheckAuthorization = false;
    setInterval(function () {
        document.CheckAuthorization = true;
    }, 300000); //5 mins

    $(window).focus(onTabFocus);
    $(window).blur(onTabBlur);
    //$(window).bind("beforeunload", onCloseWindow);
    $(window).on("unload", onCloseWindow);
    document.onkeydown = onKeyListener;
    document.onkeypress = onKeyListener
    document.onkeyup = onKeyListener;
    //#endregion


    $(document).click(function (event) {
        //Close .page-settings when click outside the model
        if ($(event.target).closest(".page-settings").length == 0) {
            $(".page-settings").removeClass("theme-settings-open");
        }
    });

    if (window.layoutHelpers.isSmallScreen()) {
        $("#help-container").remove();
    }
});

if (document.location.href.indexOf("Start/Index") == -1) {
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
                    return this.recordComponent.showModal(undefined, RecordVue.callback);
                } else {
                    return this.recordComponent.showModal();
                }
            },
            showModal: function (dateTime, callback) {
                return this.recordComponent.showModal(dateTime, callback);
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
}

if (document.location.href.indexOf("localhost") != -1
    || document.location.href.indexOf("testmybudget") != -1) {
    Vue.config.devtools = true;
}

