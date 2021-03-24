if (document.location.href.indexOf("Start/Index") == -1) {
    var NotificationVue = new Vue({
        el: "#notification-vue",
        data: {
            notifications: [],
            anyNew: false,
            isEndLoadNotification: false,

            hubConnection: null,
            isSaving: false,
        },
        computed: {
        },
        mounted: function () {

            this.hubConnection = new signalR.HubConnectionBuilder()
                .withUrl("/NotificationHub") //, { transport: signalR.HttpTransportType.WebSockets | signalR.HttpTransportType.LongPolling })
                .withAutomaticReconnect()
                .configureLogging(signalR.LogLevel.Information)
                .build();

            this.hubConnection.start()
                .catch(function (err) {
                    return console.error(err.toString());
                });

            Object.defineProperty(WebSocket, 'OPEN', { value: 1, });
            this.hubConnection.on('Receive', this.receive);
            //hubConnection.serverTimeoutInMilliseconds = 1000 * 60; // 1000 * 60 * 10; // 10 minutes

            //console.log(this.hubConnection.connection.connectionId);

            //hubConnection.invoke('Send', "Hello SignalR", "Tom")
            //.catch(function (err) {
            //    return console.error(err.toString());
            //});
            setTimeout(
                this.load.bind(this, 10),
                1100
            );
        },
        methods: {
            load: function (take) {
                this.isSaving = true;

                return $.ajax({
                    type: "GET",
                    url: "/Notification/GetLast?skip=" + this.notifications.length + "&take=" + take,
                    contentType: "application/json",
                    dataType: 'json',
                    context: this,
                    success: function (result) {
                        if (result.isOk == true) {
                            this.isEndLoadNotification = result.notifications.length == 0;
                            this.notifications = this.notifications.concat(result.notifications);

                            this.anyNew = this.notifications.some(x => x.isRead == false);

                            this.isSaving = false;
                        }
                    },
                    error: function (result) {
                        console.log(result);
                        this.isSaving = false;
                    }
                });
            },
            openNotification: function () {

                if (this.notifications.some(x => x.isRead == false)) {
                    let ids = this.notifications.map(x => x.notificationID);

                    return $.ajax({
                        type: "POST",
                        url: "/Notification/SetFlagRead",
                        contentType: "application/json",
                        dataType: 'json',
                        data: JSON.stringify(ids),
                        context: this,
                        success: function (result) {
                            if (result.isOk == true) {
                                this.anyNew = false;

                                setTimeout(function () {
                                    for (var i = 0; i < NotificationVue.notifications.length; i++) {
                                        NotificationVue.notifications[i].isRead = true;
                                    }
                                }, 2000);
                            }
                        },
                        error: function (result) {
                            console.log(result);
                        }
                    });
                } else {
                    this.anyNew = false;
                }
            },
            receive: function (notification) {
                if (notification.notifyType == "warning") {
                    toastr.warning(notification.message, notification.title);
                } else if (notification.notifyType == "success") {
                    toastr.success(notification.message, notification.title);
                }

                this.anyNew = true;
                let index = this.notifications.findIndex(x => x.notificationID == notification.notificationID);
                if (index >= 0) {
                    this.notifications.splice(index, 1);
                }
                this.notifications.unshift(notification);
                //console.log(notification);
            },
            getDateByFormat: function (date, format) {
                return GetDateByFormat(date, format);
            },
        }
    });
}
