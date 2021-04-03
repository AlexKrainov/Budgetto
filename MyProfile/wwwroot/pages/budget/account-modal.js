
var MainAccountVue = new Vue({
    el: "#main-account-modal-vue",
    data: {
        account: {
            id: undefined,
            currentyID: 1,
            bankTypeID: -1,
            isDeleted: false,
            cardID: null,
            cardName: null,
            isCash: false,
            currencyID: null,
        },
        bankTypes: [],
        currencyInfos: Metadata.currencies,

        isSaving: false
    },
    watch: {
        "account.bankTypeID": function (newValue) {
            if (newValue != undefined) {
                //this.banks = this.bankTypes.filter(x => x.id == newValue)[0].banks;
            }
        }
    },
    mounted: function () {
        setTimeout(function () {
            $.ajax({
                type: "GET",
                url: "/Account/GetEnvironmentForMain",
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                context: MainAccountVue,
                success: function (response) {
                    if (response.isOk) {
                        this.bankTypes = response.bankTypes
                    }
                    return response;
                },
                error: function (xhr, status, error) {
                    console.log(error);
                }
            });
        }, 1000);
    },
    methods: {
        edit: function (account, isCash) {
            if (account) {
                this.account = account;
                this.account.isCash = account.accountType == 1;
            } else {
                this.account = {
                    id: undefined,
                    currentyID: 1,
                    name: undefined,
                    bankTypeID: 1,
                    isDeleted: false,
                    isCash: isCash,
                    cardID: null,
                    cardName: null,
                    cardLogo: null,
                    currencyID: null,
                };
            }
            //setTimeout(function () {
            //    $("#main-account-bank").trigger('change').val(MainAccountVue.account.bankID).select2({
            //        dropdownCssClass: "d-block",
            //        selectionCssClass: "d-block",
            //    });
            //}, 300);
            $("#main-account-bank").parent().removeClass("is-invalid");
            $("#modal-main-account").modal("show");
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

            if (!this.account.balance) {
                this.account.balance = 0;
            }

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
                        $("#modal-main-account").modal("hide");
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
                $("#main-account-name").addClass("is-invalid");
            } else {
                $("#main-account-name").removeClass("is-invalid");
            }

            if (this.account.isCash == false) {
                this.account.bankID = $("#main-account-bank").val() * 1;
                if (this.account.bankID && this.account.bankID > 0) {
                    $("#main-account-bank").parent().removeClass("is-invalid");
                } else {
                    isOk = false;
                    $("#main-account-bank").parent().addClass("is-invalid");
                }
            }

            if (isOk == false && e) {
                e.preventDefault();
            }
            return isOk;
        },
        removeOrRecovery: function (account) {
            let el_id = "#main_account_" + account.id;
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
                                toastr.error("Ошибка во время удаления.");
                            } else {
                                toastr.error("Ошибка во время восстановления.");
                            }
                            this.account.isDeleted = !this.account.isDeleted;
                            HideLoading("#main_account_" + this.account.id);
                            return;
                        }
                        this.account.isDeleted = response.isDeleted;

                        if (this.account.accountType == 1) {
                            if (this.account.isDeleted) {
                                toastr.success("Вы успешно наличные");
                            } else {
                                toastr.success("Вы успешно восстановили наличные");
                            }
                        } else {
                            if (this.account.isDeleted) {
                                toastr.success("Вы успешно удалили организацию");
                            } else {
                                toastr.success("Вы успешно восстановили организацию");
                            }
                        }
                    }
                    HideLoading("#main_account_" + this.account.id);
                    return response;
                },
                error: function (xhr, status, error) {
                    toastr.error("Ошибка во время удаления");
                    console.log(error);
                }
            });
        },
        showHide: function (account, isHide) {

            AccountVue.showHide(JSCopyObject(account), isHide);
        },
        selectedCard: function () {
            let card = $("#main_account_card").select2("data")[0];
            this.account.cardID = card.id;
            this.account.cardName = card.name;
            this.account.cardLogo = card.logo;

            if (!this.account.bankID) {
                this.account.bankID = card.bankID;
                this.account.bankName = card.bankName;
                this.account.bankLogo = card.bankLogoRectangle;
                this.account.isSVG = card.bankLogoRectangle ? card.bankLogoRectangle.indexOf("svg") > 0 : false;

                $("#main-account-bank option").remove().val("");
                $("#main-account-bank").append(`<option value="${card.bankID}">${card.bankName}</option>`).select2("data", { id: card.bankID, text: card.bankName });
                $("#main-account-bank").trigger('change');
            }
        },
        unselectedCard: function () {
            this.account.cardID = null;
            this.account.cardName = null;
            this.account.cardLogo = null;
        },
        selectedBank: function () {
            let bank = $("#main-account-bank").select2("data")[0];
            this.account.bankID = bank.id;
            this.account.bankName = bank.name;
            this.account.bankLogo = bank.logoRectangle;
            this.account.isSVG = bank.logoRectangle ? bank.logoRectangle.indexOf("svg") > 0 : false;
            this.$forceUpdate();
        },
        unselectedBank: function () {
            this.account.bankID = null;
            this.account.bankName = null;
            this.account.bankLogo = null;
            this.account.isSVG = false;
            this.$forceUpdate();
        },

    }
});

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
            isCountTheBalance: true,
            isCountBalanceInMainAccount: true,
            //resetCashBackDate: null,
            isDeleted: false,
            paymentSystemID: null,
            cardID: null,
            cardName: null,
            cardLogo: null,
            isEmptyCard: false,
            timeListID: 3
        },

        bankTypes: [],
        accountTypes: [],
        paymentSystems: [],

        currencyInfos: Metadata.currencies,
        isSaving: false,
        showAttention: false,
    },
    watch: {
        "account.cachBackBalance": function (newValue) {
            if (!newValue) {
                this.account.cachBackBalance = 0;
            }
        },
        "account.bankTypeID": function (newValue) {
            console.log(newValue);
            if (newValue != undefined || newValue != null) {
                this.accountTypes = this.bankTypes.filter(x => x.id == newValue)[0].accountTypes;
            } else if (newValue == null) {
                this.accountTypes = [{
                    accountType: 1,
                    icon: 'pe-7s-cash',
                    banks: null,
                    name: 'Наличные',
                    id: 1
                }];
            }
        },
        "account.accountType": function (newValue) {
            if (!(this.account.id >= 0)) {
                if ([6, 7, 8].indexOf(newValue) >= 0) {
                    this.account.isCountTheBalance = false;
                    this.account.isCountBalanceInMainAccount = false;
                } else {
                    this.account.isCountTheBalance = true;
                    this.account.isCountBalanceInMainAccount = true;
                }
            }
        }
    },
    mounted: function () {
        setTimeout(function () {

            $.ajax({
                type: "GET",
                url: "/Account/GetEnvironment",
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                context: AccountVue,
                success: function (response) {
                    if (response.isOk) {
                        this.bankTypes = response.bankTypes;
                        this.paymentSystems = response.paymentSystems;
                    }
                    return response;
                },
                error: function (xhr, status, error) {
                    console.log(error);
                }
            });
        }, 1000);
    },
    methods: {

        edit: function (account, mainAccount) {
            if (account) {
                this.account = account;
                this.account.isEmptyCard = true;

                if (this.account.cardID) {
                    $("#account_card option").remove().val("");
                    $("#account_card").append(`<option value="${this.account.cardID}">${this.account.cardName}</option>`).select2("data", { id: this.account.cardID, text: this.account.cardName });
                    $("#account_card").trigger('change');
                    this.account.isEmptyCard = false;
                }

            } else {
                this.account = {
                    id: undefined,
                    parentID: mainAccount.id,
                    accountType: mainAccount.accountType,
                    accountIcon: mainAccount.accountIcon,
                    bankID: mainAccount.bankID,
                    bankLogo: mainAccount.bankLogo,
                    bankTypeID: mainAccount.bankTypeID,
                    bankName: mainAccount.bankName,
                    paymentSystemID: null,
                    currency: {},
                    currencyID: -1,
                    balance: null,
                    cachBackBalance: 0,
                    cashBackForAllPercent: 1,
                    isCountTheBalance: true,
                    isCountBalanceInMainAccount: true,
                    //resetCashBackDate: null,
                    isDeleted: false,
                    isCash: mainAccount.accountType == 1,
                    cardID: null,
                    cardName: null,
                    cardLogo: null,
                    timeListID: 3
                };

                this.account.currencyID = UserInfo.Currency.ID;
                this.account.currency = this.currencyInfos[this.currencyInfos.findIndex(x => x.id == this.account.currencyID)];

                $("#account_card option").remove().val("");
            }

            setTimeout(function () {

                //Timeout needs to set disabled for dateStart
                let dateConfig = GetFlatpickrRuConfig(AccountVue.account.dateStart,undefined, new Date);
                flatpickr('#account-date-start', dateConfig);

                dateConfig = GetFlatpickrRuConfig(AccountVue.account.expirationDate);
                flatpickr('#expirationDate', dateConfig);

                //if (this.account.accountType == 2) {

                //    let dateConfig = GetFlatpickrRuConfig_Month(this.account.expirationDate);
                //    this.flatpickrExpirationDate = flatpickr('#expirationDate', dateConfig);

                //} else if (this.account.accountType == 8 || this.account.accountType == 6) {

                //    let dateConfig = GetFlatpickrRuConfig(this.account.dateStart);
                //    flatpickr('#account-date-start', dateConfig);

                //    dateConfig = GetFlatpickrRuConfig(this.account.expirationDate);
                //    flatpickr('#expirationDate', dateConfig);
                //}

                $("#account-name").removeClass("is-invalid");
                $("#modal-account").modal("show");
            }, 100);
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
                type: "GET",
                url: "/Account/Toggle?accountID=" + this.account.id,
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                context: this,
                success: function (response) {
                    HideLoading(el_id);

                    if (response.isOk) {
                        //toastr.success("Данные счета обновлены");
                        this.account.isHide = isHide;

                        if (typeof (BudgetVue) == "object") {
                            for (var i = 0; i < BudgetVue.accounts.length; i++) {
                                if (this.account.parentID == null) {
                                    if (BudgetVue.accounts[i].id == this.account.id) {
                                        BudgetVue.accounts[i].isHideCurrentAccount = isHide;
                                        break;
                                    }
                                } else {
                                    let index = BudgetVue.accounts[i].accounts.findIndex(x => x.id == this.account.id);

                                    if (index >= 0) {
                                        BudgetVue.accounts[i].accounts[index].isHideCurrentAccount = isHide;
                                        break;
                                    }
                                }
                            }
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

            if (!this.account.balance) {
                this.account.balance = 0;
            }

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

            if (this.account.accountType == 7) {

                if (!this.account.dateStart) {
                    isOk = false;
                    $("#account-date-start").next().addClass("is-invalid");
                } else {
                    $("#account-date-start").next().removeClass("is-invalid");
                }

                if (!this.account.expirationDate) {
                    isOk = false;
                    $("#expirationDate").next().addClass("is-invalid");
                } else {
                    $("#expirationDate").next().removeClass("is-invalid");
                }

                if (!this.account.interestRate) {
                    isOk = false;
                    $("#interestRate").addClass("is-invalid");
                } else {
                    $("#interestRate").removeClass("is-invalid");
                }
            }

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
        },
        setExpirationDate: function (years) {
            if (this.account.dateStart) {
                let m = moment();

                if (this.account.dateStart.length <= 10) {
                    m = moment(this.account.dateStart, 'YYYY/MM/DD');
                } else {
                    m = moment(this.account.dateStart);
                }

                this.account.expirationDate = m.add(years, "year").format();

                dateConfig = GetFlatpickrRuConfig(this.account.expirationDate);
                flatpickr('#expirationDate', dateConfig);
            }
        },
        selectedCard: function () {
            let card = $("#account_card").select2("data")[0];
            this.account.cardID = card.id;
            this.account.cardName = card.name;
            this.account.cardLogo = card.logo;

            if (!this.account.name) {
                this.account.name = card.name;
            }
        },
        unselectedCard: function () {
            this.account.cardID = null;
            this.account.cardName = null;
            this.account.cardLogo = null;
        },
    }
});

var AccountTransferVue = new Vue({
    el: "#account-transfer-modal-vue",
    data: {
        accountsFrom: [],
        accountsTo: [],

        accountFrom: { currencyID: 1 },
        accountTo: { currencyID: 1 },

        value: null,
        currencyValue: 1,
        endValue: null,
        newValueFrom: null,
        newValueTo: null,
        comment: null,

        showEmpty: false,

        currencyInfos: Metadata.currencies,
        isSaving: false
    },
    watch: {
        value: function (newValue) {
            this.refreshValue();
        },
        accountFrom: function (newValue) {
            //let selectedCurrencyID = newValue.currencyID;
            this.accountTo = {};

            for (var i = 0; i < this.accountsTo.length; i++) {
                this.accountsTo[i].isSelected = false;
                this.accountsTo[i].isDisabled = this.accountsTo[i].id == newValue.id;
            }
            this.showEmpty = this.accountsTo.filter(x => x.isDisabled).length == this.accountsTo.length;

            for (var i = 0; i < this.accountsTo.length; i++) {
                if (this.accountsTo[i].isDisabled == false) {
                    this.accountsTo[i].isSelected = true;
                    this.accountTo = this.accountsTo[i];
                    break;
                }
            }

            //let accountsCurrency = this.accountsTo.filter(x => x.currencyID == selectedCurrencyID);
            //if (accountsCurrency.length >= 2) {
            //    let indexTo = accountsCurrency.findIndex(x => x.id != newValue.id);
            //    this.accountTo = accountsCurrency[indexTo];
            //    accountsCurrency[indexTo].isSelected = true;
            //}
            this.refreshValue();
        },
        accountTo: function (newValue) {
            this.refreshValue();
        },
        currencyValue: function (newValue) {
            this.refreshValue();
        }
    },
    computed: {
    },
    mounted: function () {

    },
    methods: {
        transferMoney: function (accounts, accountID) {
            this.newValueFrom = null;
            this.newValueTo = null;
            this.value = null;

            this.accountsFrom = JSCopyArray(accounts.filter(x => x.isDeleted == false));
            this.accountsTo = JSCopyArray(accounts.filter(x => x.isDeleted == false));

            let indexFrom = this.accountsFrom.findIndex(x => x.id == accountID);
            this.accountFrom = this.accountsFrom[indexFrom];

            //let indexTo = this.accountsTo.findIndex(x => x.id != accountID);
            //this.accountTo = this.accountsTo[indexTo];

            $("#modal-account-transfer").modal("show");
        },
        getCurrencyValue: function (account, val) {
            if (account.currency) {
                return new Intl.NumberFormat(account.currency.specificCulture, { style: 'currency', currency: account.currency.codeName }).format(val)
            }
            return null;
        },
        refreshValue: function () {
            if (this.accountFrom.currencyID != this.accountTo.currencyID) {
                this.endValue = this.getValueWithCurrency();
            } else {
                this.endValue = this.value * 1;
            }
            if (this.value > 0) {
                this.newValueFrom = this.accountFrom.balance - this.value;
                this.newValueTo = this.accountTo.balance + this.endValue;
            } else if (this.value < 0) {
                this.value = 0;
            }
        },
        getValueWithCurrency: function () {
            if (this.accountFrom.currency.codeName == "RUB") {
                return this.value / (this.currencyValue * 1);
            } else {
                return this.value * (this.currencyValue * 1);
            }
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
                    value: this.value,
                    comment: this.comment,
                    endValue: this.endValue,
                    currencyValue: this.currencyValue
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