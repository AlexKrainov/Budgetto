var TaskVue = new Vue({
    el: "#task-vue",
    data: {
        //filter
        rangeStart: null,
        rangeEnd: null,
        statuses: [1, 2, 3, 4, 5],

        row: [],
        forceTaskIDs: [1, 2, 3, 4, 5, 9, 10, 11],
        //metadata
        dataTable: undefined,
        tableAjax: null,
    },
    computed: {},
    watch: {},
    mounted: function () {
        let z = new Date;
        z.setDate(z.getDate() - 1);
        let dateConfig = GetFlatpickrRuConfigWithTime(z);
        this.rangeStart = moment(z).format("YYYY-MM-DDTHH:mm:ss");
        flatpickr('#range-start', dateConfig);

        dateConfig = GetFlatpickrRuConfigWithTime(moment().add(1, "days").toDate());
        this.rangeEnd = moment().format("YYYY-MM-DDTHH:mm:ss");
        flatpickr('#range-end', dateConfig);

        this.search();
    },
    methods: {
        search: function () {
            if (this.dataTable) {//sometimes bug with : Cannot read property 'mData' of undefined
                this.dataTable = undefined;
                $("#table").DataTable().destroy();
            }
            let filterData = {};
            filterData.rangeStart = this.rangeStart;
            filterData.rangeEnd = this.rangeEnd;
            filterData.userIDs = $("#emails").val();

            if (!filterData.rangeStart) {
                let z = new Date;
                z.setDate(z.getDate() - 1);
                filterData.rangeStart = moment(z).format("YYYY-MM-DDTHH:mm:ss");
            }
            if (!filterData.rangeEnd) {
                filterData.rangeEnd = moment().format("YYYY-MM-DDTHH:mm:ss");
            }

            this.tableAjax = $.ajax({
                type: "POST",
                context: this,
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                url: "/Admin/UserActions/GetData",
                data: JSON.stringify(filterData),
                success: function (json) {
                    this.row = json.data;

                    setTimeout(function () {

                        TaskVue.dataTable = $("#table").DataTable({
                            'order': [[1, 'desc']],
                            columnDefs: [
                                { "targets": 1, "type": "date" }
                            ],
                        });

                        setTimeout(function () {
                            $('[data-toggle="tooltip"]').tooltip();
                        }, 500);
                    }, 100);

                    return json.data;
                },
                error: function (jqXHR, status, exception) {
                    console.log(jqXHR);
                    console.log(exception);
                    TaskVue.isDisabled(false);
                }
            });

            //$("#statuses")
            //	.select2()
            //	.on('select2:select', this.selectedStatus)
            //	.on('select2:unselect', this.selectedStatus);//unselectedStatus);
        },
        getTD: function (data) {
            let html = "";

            let actions = "<span class='float-right'>";

            //if (this.forceTaskIDs.indexOf(data.id) >= 0) {
            //    actions += `<a href="javascript:void(0)" class="btn btn-default btn-xs icon-btn md-btn-flat product-tooltip" title="Force start" data-toggle="tooltip"
            //        onclick="TaskVue.forceStart(${data.id})">
            //        <i class="ion ion-md-play"></i></a>`;
            //}

            html += "<td>" + data.id + actions + "</span></td > ";

            if (data.currentDateTime) {
                html += "<td>" + moment(data.currentDateTime).format("DD.MM.YYYY HH:mm") + "</td>";
            } else {
                html += "<td></td>";
            }
            html += "<td>" + data.actionName + "</td>";
            html += "<td>" + data.userName + "</td>";
            html += "<td>" + data.email + "</td>";
            html += "<td>" + (data.comment ? data.comment : "") + "</td>";

            return html;
        },
        selectedStatus: function () {
            this.statuses = $("#statuses").select2("data").map(function (x) { return x.id });
        },
        //unselectedStatus: function () {
        //	this.statuses = $("#status-select").select2("data").map(function (x) { return x.id });

        //}
        selectRow: function (dataDeletionLogId) {
            let deletionObject = this.data.filter(x => x.DataDeletionLogId == dataDeletionLogId);

            if (deletionObject) {
                this.deletionObject = deletionObject[0];
                $("#table tr").removeClass("ListViewDefaultStyleMouseOverRow")
                $("#table tr a[data-id=" + this.deletionObject.DataDeletionLogId + "]").parent().parent().addClass("ListViewDefaultStyleMouseOverRow");
            }
        },
        getDateByFormat: function (date, format) {
            if (date && moment(date).isValid()) {
                return moment(date).format(format);
            }
            return "";
        }
    }
});
