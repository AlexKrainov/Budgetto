$(document).ready(function () {
    // Auto update layout
    window.layoutHelpers.setAutoUpdate(true);

    if (window.layoutHelpers.isSmallScreen()) {
        window.layoutHelpers.toggleCollapsed();
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
        console.log("click");
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

    document.CheckAuthorization = false;
    setInterval(function () {
        document.CheckAuthorization = true;
    }, 900000); //15 mins

    $(window).focus(onTabFocus);
    $(window).blur(onTabBlur);
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


Vue.config.devtools = true;

