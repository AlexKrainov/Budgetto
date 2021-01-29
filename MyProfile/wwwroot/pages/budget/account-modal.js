
var AccountVue = new Vue({
    el: "#account-modal-vue",
    data: {
        account: {
            id: undefined,
            accountType: 1,
            currency: {},
            currencyID: -1,
            balance: 0,
            cachBackBalance: 0,
            cashBackForAllPercent: 1,
            //resetCashBackDate: null,
            isDeleted: false
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
                    accountType: 1,
                    currency: {},
                    currencyID: -1,
                    balance: 0,
                    cachBackBalance: 0,
                    cashBackForAllPercent: 1,
                    //resetCashBackDate: null,
                    isDeleted: false
                };

                this.account.currencyID = UserInfo.Currency.ID;
                this.account.currency = this.currencyInfos[this.currencyInfos.findIndex(x => x.id == this.account.currencyID)];
            }


            let dateConfig = GetFlatpickrRuConfig_Month(this.account.expirationDate);
            this.flatpickrExpirationDate = flatpickr('#expirationDate', dateConfig);

            $("#account-name").removeClass("is-invalid");
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
                    HideLoading(el_id);

                    if (response.isOk) {
                        //toastr.success("Данные счета обновлены");
                        this.account.isHide = isHide;

                        if (typeof (BudgetVue) == "object" && isHide) {
                            let index = BudgetVue.accounts.findIndex(x => x.id == this.account.id);
                            BudgetVue.accounts.splice(index, 1);
                            BudgetVue.accounts.push(this.account);
                        } else {
                            let hideAccounts = BudgetVue.accounts.filter(x => x.isHide);
                            let notHideAccounts = BudgetVue.accounts.filter(x => x.isHide == false);

                            BudgetVue.accounts = notHideAccounts.concat(hideAccounts);
                        }
                    }
                    $('[data-toggle="tooltip"]').tooltip();
                    return response;
                },
                error: function (xhr, status, error) {
                    console.log(error);
                }
            });
        },
        save: function () {

            if (this.checkForm() == false) {
                return;
            }

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
        checkForm: function (e) {
            let isOk = true;

            if (!(this.account.name && this.account.name.length > 0)) {
                isOk = false;
                $("#account-name").addClass("is-invalid");
            } else {
                $("#account-name").removeClass("is-invalid");
            }

            //if (this.goal.expectationMoney && this.goal.expectationMoney > 0) {
            //    $("#goal-expectationMoney").removeClass("is-invalid");
            //} else {
            //    isOk = false;
            //    $("#goal-expectationMoney").addClass("is-invalid");
            //}

            if (isOk == false && e) {
                e.preventDefault();
            }
            return isOk;
        },
        removeOrRecovery: function (account) {
            if (typeof (BudgetVue) == "object" && this.account.isDeleted == false) {
                let lengthOfDeleted = BudgetVue.accounts.filter(x => x.isDeleted == false).length;

                if (lengthOfDeleted <= 1) {
                    toastr.error("Нельзя удалить все счета, должен оставаться хотя бы один.");
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
                        if (!this.account.isDeleted == response.isDeleted) {
                            if (this.account.isDeleted) {
                                toastr.error("Ошибка во время удаления счета. Возможно, вы удаляете последний счет.");
                            } else {
                                toastr.error("Ошибка во время восстановления счета.");
                            }
                            this.account.isDeleted = !this.account.isDeleted;
                            HideLoading("#account_" + this.account.id);
                            return;
                        }
                        this.account.isDeleted = response.isDeleted;
                        this.account.isDefault = false;

                        //refresh budget vue
                        if (typeof (BudgetVue) == "object") {
                            let index = BudgetVue.accounts.findIndex(x => x.id == response.accountIDWithIsDefault);
                            if (index != -1 && response.accountIDWithIsDefault != -1) {
                                BudgetVue.accounts[index].isDefault = true;
                            }
                            BudgetVue.loadSummaries();
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


var AccountTransferVue = new Vue({
    el: "#account-transfer-modal-vue",
    data: {
        accountsFrom: [],
        accountsTo: [],

        accountFrom: {},
        accountTo: {},

        value: null,
        newValueFrom: null,
        newValueTo: null,

        showEmpty: false,

        currencyInfos: Metadata.currencies,
        isSaving: false
    },
    watch: {
        value: function (newValue) {
            if (newValue > 0) {
                this.newValueFrom = this.accountFrom.balance - newValue * 1;
                this.newValueTo = this.accountTo.balance + newValue * 1;
            } else {
                this.newValueFrom = null;
                this.newValueTo = null;
            }
        },
        accountFrom: function (newValue) {
            let selectedCurrencyID = newValue.currencyID;
            this.accountTo = {};

            for (var i = 0; i < this.accountsTo.length; i++) {
                this.accountsTo[i].isDisabled = this.accountsTo[i].currencyID != selectedCurrencyID || this.accountsTo[i].id == newValue.id;
                this.accountsTo[i].isSelected = false;
            }
            this.showEmpty = this.accountsTo.filter(x => x.isDisabled).length == this.accountsTo.length;

            let accountsCurrency = this.accountsTo.filter(x => x.currencyID == selectedCurrencyID);
            if (accountsCurrency.length >= 2) {
                let indexTo = accountsCurrency.findIndex(x => x.id != newValue.id);
                this.accountTo = accountsCurrency[indexTo];
                accountsCurrency[indexTo].isSelected = true;
            }
        },
        accountTo: function (newValue) {

        },
    },
    computed: {
    },
    mounted: function () {

    },
    methods: {
        transferMoney: function (accounts, accountID) {
            this.accountsFrom = JSCopyArray(accounts);
            this.accountsTo = JSCopyArray(accounts);

            let indexFrom = this.accountsFrom.findIndex(x => x.id == accountID);
            this.accountFrom = this.accountsFrom[indexFrom];

            //let indexTo = this.accountsTo.findIndex(x => x.id != accountID);
            //this.accountTo = this.accountsTo[indexTo];

            $("#modal-account-transfer").modal("show");
        },
        getCurrencyValue: function (val) {
            if (this.accountFrom.currency) {
                return new Intl.NumberFormat(this.accountFrom.currency.specificCulture, { style: 'currency', currency: this.accountFrom.currency.codeName }).format(val)
            }
            return null;
        },

        save: function () {
            this.isSaving = true;

            $.ajax({
                type: "POST",
                url: "/Account/TransferMoney",
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify({
                    accountFromID: this.accountFrom.id,
                    accountToID: this.accountTo.id,
                    value: this.value
                }),
                dataType: 'json',
                context: this,
                success: function (response) {
                    if (response.isOk) {

                        toastr.success("Деньги переведены");

                        if (typeof (BudgetVue) == "object") {
                            BudgetVue.refresh("onlySummery");
                        }
                        $("#modal-account-transfer").modal("hide");
                    }
                    this.isSaving = false;
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
