
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
                                toastr.error("Ошибка во время удаления счета. Возможно вы удаляете последний счет.");
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