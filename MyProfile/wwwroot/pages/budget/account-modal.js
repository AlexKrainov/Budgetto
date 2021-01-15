
var AccountVue = new Vue({
    el: "#account-modal-vue",
    data: {
        account: {
            id: undefined,
            accountType: 1,
            currency: {},
            currencyID: -1,
            //resetCashBackDate: null
        },

        accountTypes: [],
        banks: [],
        flatpickrExpirationDate: null,

        currencyInfos: Metadata.currencies,
        isSaving: false
    },
    watch: {

    },
    mounted: function () {

        $.ajax({
            type: "GET",
            url: "/Account/GetEnvironment",
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            context: this,
            success: function (response) {
                if (response.isOk) {
                    this.accountTypes = response.accountTypes;
                    this.banks = response.banks;
                }
                return response;
            },
            error: function (xhr, status, error) {
                console.log(error);
            }
        });
    },
    methods: {

        edit: function (account) {
            if (account) {
                this.account = account;
            } else {
                this.account = {
                    id: undefined,
                    accountType: 1
                };

                this.account.currencyID = UserInfo.Currency.ID;
                this.account.currency = this.currencyInfos[this.currencyInfos.findIndex(x => x.id == this.account.currencyID)];
            }
            let dateConfig = GetFlatpickrRuConfig_Month(this.account.expirationDate);
            this.flatpickrExpirationDate = flatpickr('#expirationDate', dateConfig);

            $("#modal-account").modal("show");
        },
        showHide: function (account, isHide) {
            this.account = account;

            let el_id = "#account_" + account.id;
            ShowLoading(el_id);
            //bug with .dropdown-menu
            $(el_id)
                .find(".dropdown-menu")
                .removeClass("show");

            return $.ajax({
                type: "POST",
                url: "/Account/ShowHide",
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify(this.account),
                dataType: 'json',
                context: this,
                success: function (response) {
                    if (response.isOk) {
                        //toastr.success("Данные счета обновлены");
                        this.account.isHide = isHide;

                    }
                    HideLoading(el_id);
                    return response;
                },
                error: function (xhr, status, error) {
                    console.log(error);
                }
            });
        },
        save: function () {
            this.isSaving = true;

            $.ajax({
                type: "POST",
                url: "/Account/Save",
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify(this.account),
                dataType: 'json',
                context: this,
                success: function (response) {
                    if (response.isOk) {

                        //toastr.success("Данные счета обновлены");

                        if (typeof (BudgetVue) == "object") {
                            BudgetVue.refresh("onlySummery");
                        }
                        if (typeof (RecordVue) == "object") {
                            RecordVue.recordComponent.loadAccounts();
                        }
                        $("#modal-account").modal("hide");
                    }
                    this.isSaving = false;
                    return response;
                },
                error: function (xhr, status, error) {
                    this.isSaving = false;
                    console.log(error);
                }
            });
        },
        removeOrRecovery: function (account) {
            if (typeof (BudgetVue) == "object" && this.account.isDeleted == false) {
                let lengthOfDeleted = BudgetVue.accounts.filter(x => x.isDeleted).length;

                if (BudgetVue.accounts.length - lengthOfDeleted <= 1) {
                    toastr.error("Нельзя удалить все счета, должен оставаться хотябы один.");
                    return;
                }
            }
            let el_id = "#account_" + account.id;
            ShowLoading(el_id);
            //bug with .dropdown-menu
            $(el_id).find(".dropdown-menu").removeClass("show");

            this.account = account;
            this.account.isDeleted = !this.account.isDeleted;

            $.ajax({
                type: "POST",
                url: "/Account/RemoveOrRecovery",
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify(this.account),
                dataType: 'json',
                context: this,
                success: function (response) {
                    if (response.isOk) {
                        this.account.isDeleted = response.isDeleted;
                        this.account.isDefault = false;

                        //refresh budget vue
                        if (typeof (BudgetVue) == "object") {
                            let index = BudgetVue.accounts.findIndex(x => x.id == response.accountIDWithIsDefault);
                            if (index != -1 && response.accountIDWithIsDefault != -1) {
                                BudgetVue.accounts[index].isDefault = true;
                            }
                        }
                        //refresh record metadata
                        if (typeof (RecordVue) == "object") {
                            RecordVue.recordComponent.loadAccounts();
                        }
                        if (this.account.isDeleted) {
                            toastr.success("Вы успешно удалили счет");
                        } else {
                            toastr.success("Вы успешно восстановили счет");
                        }
                    }
                    HideLoading("#account_" + this.account.id);
                    return response;
                },
                error: function (xhr, status, error) {
                    toastr.error("Ошибка во время удаления счета");
                    console.log(error);
                }
            });
        },
        changeCurrency: function (currency) {
            this.account.currencyID = currency.id;
            this.account.currency = currency;
            this.$forceUpdate();
        }
    }
});