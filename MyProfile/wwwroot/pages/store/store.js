var StoreVue = new Vue({
    el: "#store-vue",
    data: {
        paymentHistoryID: null,

        isSaving: false,
        textMessage: null,
        textError: null,
    },
    watch: {
    },
    mounted: function () {
    },
    methods: {
        pay: function () {
            let obj = {
                tariff: "Free"
            };

            return $.ajax({
                type: "Get",
                url: "/Store/PayStart",
                context: this,
                data: JSON.stringify(obj),
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (response) {
                    if (response.isOk) {
                        this.paymentHistoryID = response.paymentHistoryID;
                        this.textMessage = "Оплата прошла успешно";
                    } else {
                        this.textError = "Извините, оплата не прошла";
                    }
                    return response;
                },
                error: function (xhr, status, error) {
                    this.isSaving = false;
                    console.log(error);
                }
            });
        },
        returnFromPayment: function () {
            return $.ajax({
                type: "Get",
                url: "/Store/PayFinish?paymentID=" + this.paymentHistoryID,
                context: this,
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (response) {
                    if (response.isOk) {

                    }
                    return response;
                },
                error: function (xhr, status, error) {
                    this.isSaving = false;
                    console.log(error);
                }
            });
        }
    }
});
