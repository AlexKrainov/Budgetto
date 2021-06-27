
var AccountListVue = new Vue({
    el: "#account-list-vue",
    data: {
        accounts: [],
        activeBankTypeID: -1,

        //edit
        account: { periodName: '', periodTypeID: -1, notifications: [] },
        //flatpickrStart: {},
        //flatpickrEnd: {},
        msnry: {},

        bankType: [
            { id: null, name: 'Наличные' },
            { id: 1, name: 'Банк' },
            { id: 2, name: 'Брокер' },
        ],
        isSaving: false,
        numberID: -1,
        errorMessage: null,
    },
    watch: {
    },
    computed: {},
    mounted: function () {
        this.load();

        $("#modal-account").on("hidden.bs.modal", function () {
           // AccountListVue.closeEditModal();
        });
    },
    methods: {
        load: function () {
            $.ajax({
                type: "GET",
                url: "/Account/GetAccounts?periodType=1",
                contentType: "application/json",
                dataType: 'json',
                context: this,
                success: function (response) {
                    this.accounts = response.accounts;

                    setTimeout(function () {
                        if (AccountListVue.msnry && AccountListVue.msnry.destroy != undefined) {
                            AccountListVue.msnry.destroy();
                        }
                        AccountListVue.msnry = new Masonry('#accounts', {
                            itemSelector: '.masonry-item:not(.d-none)',
                            columnWidth: '.masonry-item-sizer',
                            originLeft: true,
                            horizontalOrder: true
                        });
                    }, 100);
                },
                error: function (result) {
                    console.log(result);
                }
            });
        },

        reloadView: function (activeBankTypeID) {
            this.activeBankTypeID = activeBankTypeID;

            for (var i = 0; i < this.accounts.length; i++) {
                if (activeBankTypeID == -1) {
                    this.accounts[i].isShow = true;
                } else {
                    this.accounts[i].isShow = this.accounts[i].bankTypeID == this.activeBankTypeID;
                }
            }

            setTimeout(function () {
                AccountListVue.msnry.layout();
            }, 100);
        },

        editMainAccount: function (account, isCash) {
            if (account == undefined) {
                MainAccountVue.edit(undefined, isCash);
            } else {
                MainAccountVue.edit(JSCopyObject(account), isCash);
            }
        },
        editAccount: function (account, mainAccount) {
            if (account == undefined) {
                AccountVue.edit(undefined, mainAccount);
            } else {
                AccountVue.edit(JSCopyObject(account), mainAccount);
            }
        },
        removeOrRecoveryAccount: function (account) {
            AccountVue.removeOrRecovery(account);
        },
        removeOrRecoveryMainAccount: function (account) {
            MainAccountVue.removeOrRecovery(account);
        },
        showHideAccount: function (account, isHide) {
            return AccountVue.showHide(account, isHide);
        },
        transferMoney: function (accountID) {
            let accounts = [];
            for (var i = 0; i < this.accounts.length; i++) {
                accounts = accounts.concat(this.accounts[i].accounts);
            }
            AccountTransferVue.transferMoney(accounts, accountID);
        },
        showHideAccounts: function (mainAccount, isHide) {
            for (var i = 0; i < mainAccount.accounts.length; i++) {
                if (mainAccount.accounts[i].isHideCurrentAccount != isHide) {
                    mainAccount.accounts[i].isHideCurrentAccount = isHide;
                    AccountVue.showHide(mainAccount.accounts[i], isHide);
                }
            }
        },
        getDateByFormat: function (date, format) {
            return GetDateByFormat(date, format);
        },
    }
});


