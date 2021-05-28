var UserSessionsVue = new Vue({
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
        this.rangeEnd = (moment().add(1, "days")).format("YYYY-MM-DDTHH:mm:ss");
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
                url: "/Admin/UserSessions/GetData",
                data: JSON.stringify(filterData),
                success: function (json) {
                    this.row = json.data;

                    setTimeout(function () {

                        UserSessionsVue.dataTable = $("#table").DataTable({
                            //'order': [[0, 'desc']],
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
                    UserSessionsVue.isDisabled(false);
                }
            });

            //$("#statuses")
            //	.select2()
            //	.on('select2:select', this.selectedStatus)
            //	.on('select2:unselect', this.selectedStatus);//unselectedStatus);
        },
        getTD: function (data) {
            let html = "";

            let device = "<span class='float-right'>";
            let isLendingPage = "";
            if (data.isLandingPage) {
                isLendingPage = '<div class="badge badge-outline-warning float-right">Lending page</div>';
            }
            if (data.isPhone) {
                device += '<i class="lnr lnr-smartphone"></i>';
            }
            if (data.isTablet) {
                device += '<i class="lnr lnr-tablet"></i>';
            }
            if (!data.isPhone && !data.isTablet) {
                device += '<i class="lnr lnr-laptop"></i>';
            }

            html += "<td>" + moment(data.enterDate).format("DD.MM.YYYY HH:mm") + " " + isLendingPage + "</td>";
            if (data.userName && data.email) {
                html += "<td>" + data.userName + "(" + data.email + ")</td>";
            } else if (data.userName) {
                html += "<td>" + data.userName + "</td>";
            } else if (data.email) {
                html += "<td>" + data.email + "</td>";
            } else {
                html += "<td></td>";
            }
            html += "<td>" + data.ip + "(" + data.ipCounter + ")</td>";
            html += "<td>" + data.place + "</td>";
            html += "<td>" + data.browerName + " " + data.oS_Name + "</td>";
            html += "<td>" + data.screenSize + device + "</span></td>";
            html += "<td>" + data.referrer + "</td>";

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
