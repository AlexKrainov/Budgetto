var TaskVue = new Vue({
    el: "#task-vue",
    data: {
        //filter
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

        this.search();
    },
    methods: {
        search: function () {
            if (this.dataTable) {//sometimes bug with : Cannot read property 'mData' of undefined
                this.dataTable = undefined;
                $("#table").DataTable().destroy();
            }

            this.tableAjax = $.ajax({
                type: "POST",
                context: this,
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                url: "/Admin/SchedulerTask/GetData",
                data: function (params) {
                    let filterData = {};
                    //let range = $('#daterange').data('daterangepicker');
                    //filterData.dateFrom = range.startDate.format('YYYY-MM-DD HH:mm:ss');
                    //filterData.dateTo = range.endDate.format('YYYY-MM-DD HH:mm:ss');
                    //filterData.statuses = TaskVue.statuses;

                    params.filter = filterData;
                    return params;
                },
                success: function (json) {
                    this.row = json.data;

                    setTimeout(function () {

                        TaskVue.dataTable = $("#table").DataTable({
                            columnDefs: [
                                {
                                    targets: '_all',
                                    className: "column-min-width",
                                    "orderDataType": "dom-text-numeric",
                                    type: "num"
                                },
                            ],
                            colReorder: {
                                realtime: false
                            }
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

            if (this.forceTaskIDs.indexOf(data.id) >= 0) {
                actions += `<a href="javascript:void(0)" class="btn btn-default btn-xs icon-btn md-btn-flat product-tooltip" title="Force start" data-toggle="tooltip"
                    onclick="TaskVue.forceStart(${data.id})">
                    <i class="ion ion-md-play"></i></a>`;
            }

            html += "<td>" + data.id + actions + "</span></td > ";
            html += "<td>" + data.name + "</td>";

            if (data.firstStart) {
                html += "<td>" + moment(data.firstStart).format("DD.MM.YYYY HH:mm") + "</td>";
            } else {
                html += "<td></td>";
            }
            if (data.lastStart) {
                html += "<td>" + moment(data.lastStart).format("DD.MM.YYYY HH:mm") + "</td>";
            } else {
                html += "<td></td>";
            }
            if (data.nextStart) {
                let missed = '';
                if (data.isMissed) {
                    missed = '<a href="javascript:void(0)" class="badge badge-pill badge-danger">Missed</a>';
                }
                html += "<td>" + moment(data.nextStart).format("DD.MM.YYYY HH:mm") + " " + missed + "</td > ";
            } else {
                html += "<td></td>";
            }

            html += "<td data-id-status='" + data.id + "'>" + this.status(data.id, data.taskStatus) + "</td>";
            html += "<td>" + data.cronComment + "</td>";
            html += "<td>" + data.cronExpression + "</td>";
            //html += "<td>" + (data.comment ? data.comment : "") + "</td>";

            return html;
        },
        status: function (id, taskStatus) {
            let _class = 'btn-secondary';
            if (taskStatus == "New") {
                _class = "btn-primary";
            } else if (taskStatus == "InProcess") {
                _class = "btn-success";
            } else if (taskStatus == "Stop") {
                _class = "btn-warning";
            } else if (taskStatus == "Error") {
                _class = "btn-danger";
            }
            return '<div class="ticket-priority btn-group">' +
                '<button type="button" class="btn btn-xs md-btn-flat dropdown-toggle ' + _class + '" data-toggle="dropdown">' + taskStatus + '</button>' +
                '<div class="dropdown-menu">' +
                '<a class="dropdown-item" href="javascript:void(0)" onclick="TaskVue.setStatus(' + id + ', 1)">New</a>' +
                '<a class="dropdown-item" href="javascript:void(0)" onclick="TaskVue.setStatus(' + id + ', 2)">InProcess</a>' +
                '<a class="dropdown-item" href="javascript:void(0)" onclick="TaskVue.setStatus(' + id + ', 3)">Stop</a>' +
                '<a class="dropdown-item" href="javascript:void(0)" onclick="TaskVue.setStatus(' + id + ', 4)">Error</a>' +
                '</div>' +
                '</div>';
        },
        setStatus: function (taskID, newStatusID) {
            $.ajax({
                type: "Get",
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                url: "/Admin/SchedulerTask/SetNewStatus?taskID=" + taskID + "&newStatusID=" + newStatusID,
                context: this,
                success: function (json) {

                    if (json.isOk) {
                        $("td[data-id-status=" + taskID + "]").html(this.status(taskID, json.newTaskStatus));
                        toastr.success("Force start success");
                    } else {
                        toastr.error("Error");
                    }
                },
                error: function (jqXHR, status, exception) {
                    console.log(jqXHR);
                    console.log(exception);
                    TaskVue.isDisabled(false);
                }
            });
        },
        forceStart: function (taskID) {
            $.ajax({
                type: "Get",
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                url: "/Admin/SchedulerTask/ForceStart?taskID=" + taskID,
                context: this,
                success: function (json) {

                    if (json.isOk) {
                        toastr.success("Force start success");
                        this.search();
                    } else {
                        toastr.error("Error");
                    }

                    return json.data;
                },
                error: function (jqXHR, status, exception) {
                    console.log(jqXHR);
                    console.log(exception);
                    TaskVue.isDisabled(false);
                }
            });
        },
        getDateByFormat: function (date, format) {
            if (date && moment(date).isValid()) {
                return moment(date).format(format);
            }
            return "";
        }
    }
});
