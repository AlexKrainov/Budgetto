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
            addRecord: function () {
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