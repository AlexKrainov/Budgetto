Vue.component("new-vue-record-history-component", {
    template: `
<div class="ui-timeline ui-timeline-with-info">
    <div v-for="(groupRecord, index) in groupRecords">
        <div class="ui-timeline-separator text-big"
                v-show="isShowDate">
            <div class="ui-timeline-track-bg d-inline-block rounded small font-weight-semibold py-2 px-4">{{ getDate(groupRecord) }}</div>
        </div>
        <div class="mb-2" 
            v-bind:style="isShowDate ? 'margin-top: -57px;' : ''" 
            v-show="index == 0">
            <div class="row no-gutters align-items-center py-2">
                <div class="col-12 col-md-3 px-4 font-weight-semibold">
                    Категория
                </div>
                <div class="col-4 col-md-3
                font-weight-semibold px-4">
                    Комментарий
                </div>
                <div class="col-4 col-md-3 px-4 font-weight-semibold">Банк</div>
                <div class="col-4 col-md-3 px-4 font-weight-semibold text-right">
                    Кэшбэк/Сумма
                </div>
            </div>
        </div>
        <div class="ui-timeline-item"
            v-for="record in groupRecord.records"
            v-bind:id="'history-record-'+record.id"
            v-show="record.isShowForFilter"
            v-bind:class="[record.isDeleted ? ' deleted-item' : '']">
            <div class="ui-timeline-badge ui-w-50"
                    v-html="getLogoHtml(record)">
            </div>
            <div class="card pb-3 mb-2">
                <div class="row no-gutters align-items-center">
                    <div class="col-6 col-md-3 px-3 pt-3">
                        <a href="javascript:void(0)" class="text-body font-weight-semibold">{{ record.section.name }}</a><br>
                        <small class="text-muted">{{ record.section.areaName }}</small>
                    </div>
                    <div class="col-6 col-md-3 text-muted small px-3 pt-3"
                        v-html="descriptionBuilder(record)">
                    </div>
                    <div class="col-6 col-md-3 text-muted small px-3 pt-3">
                        <div class="media">
                           <span v-html="getCardLogoHtml(record)"></span>
                            <div class="media-body ml-2" style="margin-top: -4px;">
                                <span class="text-body font-weight-semibold text-big">{{ record.account.name }}</span>
                                <div class="text-muted small">
                                    <span>
                                        <del>
                                            <span class=" pt-1">
                                                 {{ getCurrencyValue(record, record.account.oldBalance) }}
                                            </span>
                                        </del>
                                        &nbsp; -&gt;&nbsp;
                                        <span class=" pt-1">
                                                 {{ getCurrencyValue(record, record.account.newBalance) }}
                                        </span>

                                    </span>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col-6 col-md-3 px-3 pt-3 text-right">

                        <h5 class="mb-0 pb-0">
                            <a href="javascript:void(0)" class="text-body font-weight-semibold">
                                {{ getCurrencyValue(record, record.money) }}
                            </a> 
                            <span class="text-muted1 small pt-1 text-success d-block">
                                 {{ getCurrencyValue(record, record.cashback, false) }}
                            </span>
                        </h5>
                        <div class="pt-2" style="margin-bottom: -10px;"
                            v-show="record.isDeleted == false">
                            <i class="fas fa-edit pl-2 edit-record cursor-pointer"
                                v-on:click="edit(record)"></i> 
                            <i class="fas fa-trash pl-2 edit-record text-danger cursor-pointer"
                                v-on:click="remove(record)"></i>
                        </div>
                        <div class="pt-2" style="margin-bottom: -10px;"
                            v-show="record.isDeleted == true">
                            <i class="fa fa-undo pl-2 float-right text-success cursor-pointer" 
                                v-on:click="recovery(record)"></i>
                        </div>
                    </div>
                </div>
            </div>
        </div>
   </div>
    <div class="card card-body mt-4" v-show="groupRecords && groupRecords.length > 0 && isShowFooter" id="history-footer"
                     style="position: fixed; right: 42px; top: 84%; width: 91.4%; z-index: 1;">
                    <div class="text-right">
                        <div style="display:inline-block" v-show="spendingInvestingMoney != 0">
                            <label class="text-muted font-weight-normal m-0">Итого инвестиций</label>
                            <div class="text-large">
                                <strong>
                                    {{ new Intl.NumberFormat('ru-RU', { style: 'currency', currency: 'RUB' }).format(spendingInvestingMoney) }}
                                </strong>
                            </div>
                        </div>
                        <div class="mx-4" style="display:inline-block" v-show="spendingErningMoney != 0">
                            <label class="text-muted font-weight-normal m-0">Итого доходов</label>
                            <div class="text-large">
                                <strong>
                                    {{ new Intl.NumberFormat('ru-RU', { style: 'currency', currency: 'RUB' }).format(spendingErningMoney) }}
                                </strong>
                            </div>
                        </div>
                        <div style="display:inline-block" v-show="spendingTotalMoney != 0">
                            <label class="text-muted font-weight-normal m-0">Итого расходов</label>
                            <div class="text-large">
                                <strong>
                                    {{ new Intl.NumberFormat('ru-RU', { style: 'currency', currency: 'RUB' }).format(spendingTotalMoney) }}
                                </strong>
                            </div>
                        </div>
                    </div>
                </div>
</div>
`,
    props: {
        dataSearchId: String,
        dataId: String,
        name: {
            type: String,
            default: "section-component"
        },
        onchoose: Event,
        onUpdateView: Event,
        isShowFilter: {
            type: Boolean,
            default: false,
        },
        dataItems: Array,//[{"id":9,"name":"Расходы (продукты)","description":"test groceries","cssIcon":"fas fa-shopping-cart","cssColor":"rgba(24,28,33,0.8)","areaID":5,"areaName":"Основные расходы","isUpdated":false,"collectiveSections":[],"sectionTypeID":null,"sectionTypeName":null,"recordCount":139,"isShow":true,"hasRecords":false,"cssBackground":"#ffab91"}]
        dataClass: {
            type: String,
            default: "" //   cards-small/cards-medium/cards-big
        },
        dataSelectedItemsCount: Array,//[{ id:9, count:1 }]
        dataIsSelection: {
            type: Boolean,
            default: false
        },
        isShowFooter: {
            type: Boolean,
            default: false
        },
        isShowModal: {
            type: Boolean,
            default: false
        },
        isShowDate: {
            type: Boolean,
            default: true
        },
    },
    data: function () {
        return {
            dateTimeOfPayment: null,
            groupRecords: [],
            isShowCollection: true,
            searchText: null,
            filter: {},

            spendingTotalMoney: 0,
            spendingErningMoney: 0,
            spendingInvestingMoney: 0,

            //state
            isSaving: false,
        }
    },
    watch: {
        dateTimeOfPayment: function (newValue, oldValue) {
            let filter = {
                isSearchAllUserSections: true,
                startDate: newValue,
                endDate: newValue
            };
            this.loadHistory(filter);
        },

        searchText: function (newValue, oldValue) {
            if (newValue) {
                newValue = newValue.toLocaleLowerCase();
            }

            for (var i = 0; i < this.groupRecords.length; i++) {
                let groupRecord = this.groupRecords[i];
                for (var j = 0; j < groupRecord.records.length; j++) {
                    let record = groupRecord.records[j];

                    record.isShowForFilter = record.section.name.toLocaleLowerCase().indexOf(newValue) >= 0
                        || (record.description && record.description.toLocaleLowerCase().indexOf(newValue) >= 0)
                        || record.section.areaName.toLocaleLowerCase().indexOf(newValue) >= 0
                        || record.money.toString().indexOf(newValue) >= 0
                        || record.rawData.indexOf(newValue) >= 0
                        || record.tags.find(x => x.value.toLocaleLowerCase().indexOf(newValue) >= 0);
                }
            }
        },
        groupRecords: {
            handler: function (newValue, oldValue) {
                if (this.isShowFooter == false) {
                    return;
                }
                this.spendingTotalMoney = 0;
                this.spendingErningMoney = 0;
                this.spendingInvestingMoney = 0;

                let records = [];
                for (var i = 0; i < this.groupRecords.length; i++) {
                    records = Array.prototype.concat.apply(records, this.groupRecords[i].records);
                }

                for (var i = 0; i < records.length; i++) {
                    if (records[i].isDeleted == false) {
                        if (records[i].sectionTypeID == 1) { //Earnings
                            this.spendingErningMoney += records[i].money * 1;
                        } else if (records[i].sectionTypeID == 2) { //Spendings
                            this.spendingTotalMoney += records[i].money * 1;
                        } else if (records[i].sectionTypeID == 3) { //Investments
                            this.spendingInvestingMoney += records[i].money * 1;
                        }
                    }
                }
            },
            deep: true
        }
    },
    mounted: function () {

    },
    methods: {
        loadHistory: function (filter) {
            this.searchText = null;
            ShowLoading('#history-records');
            this.filter = filter;

            return $.ajax({
                type: "POST",
                url: "/History/GetGroupRecords",// + this.dateTimeOfPayment,
                data: JSON.stringify(filter),
                contentType: "application/json",
                dataType: 'json',
                context: this,
                success: function (response) {
                    this.groupRecords = response.data;
                    HideLoading('#history-records');

                    if (this.isShowModal) {
                        $("#modalTimeLine").modal("show");
                    }
                }
            });
        },
        edit: function (record) {
            RecordVue.recordComponent.isShowHistory = false;
            if (typeof (BudgetVue) != "undefined") {
                RecordVue.editByElement(record, this.refresh, "runtimeData");
            } else {
                RecordVue.editByElement(record);
            }
        },
        remove: function (record) {
            ShowLoading('#history-record-' + record.id);

            return $.ajax({
                type: "POST",
                url: "/Budget/RemoveRecord",
                data: JSON.stringify(record),
                context: record,
                contentType: "application/json",
                dataType: 'json',
                success: function (response) {
                    record.isDeleted = response.isOk;
                    HideLoading('#history-record-' + record.id);

                    if (typeof (BudgetVue) != "undefined") {
                        BudgetVue.refreshAfterChangeRecords(response.dateTimeOfPayment)
                    }
                }
            });
        },
        recovery: function (record) {
            ShowLoading('#history-record-' + record.id);
            return $.ajax({
                type: "POST",
                url: "/Budget/RecoveryRecord",
                data: JSON.stringify(record),
                context: record,
                contentType: "application/json",
                dataType: 'json',
                success: function (response) {
                    record.isDeleted = !response.isOk;
                    HideLoading('#history-record-' + record.id);

                    if (typeof (BudgetVue) != "undefined") {
                        BudgetVue.refreshAfterChangeRecords(response.dateTimeOfPayment)
                    }
                }
            });
        },
        getCurrencyValue: function (record, money, isCashback) {
            let value = new Intl.NumberFormat(UserInfo.Currency.SpecificCulture, { style: 'currency', currency: UserInfo.Currency.CodeName }).format(money);
            if (UserInfo.Currency.ID != record.currencyID && isCashback == undefined) {
                try {
                    value += " (" + new Intl.NumberFormat(record.currencySpecificCulture, { style: 'currency', currency: record.currencyCodeName }).format(CalculateExpression(record.tag)) + ")";
                } catch (e) {
                }
            }
            return value;
        },
        descriptionBuilder: function (record) {
            return TagBuilder.toDescription(record);
        },
        getLogoHtml: function (record) {
            let html;
            if (record.tags.length > 0 && record.tags.some(x => x.companyLogo)) {
                let tag = record.tags.find(x => x.companyLogo);
                html = `<img src="${tag.companyLogo}" class="ui-w-50 rounded-circle" alt="">`
            } else {
                html = `<div class="ui-square rounded-circle text-white" style="background-color: ${record.section.cssBackground}">
                            <div class="d-flex ui-square-content">
                                <div class="m-auto text-large ${record.section.cssIcon}" style="color: ${record.section.cssColor}"></div>
                            </div>
                        </div>`;
            }

            return html;

        },
        getCardLogoHtml: function (record) {
            let html;
            if (record.account.accountType != 1 && record.account.cardLogo) {
                html = ` <img class="ui-payment-small dropdown-toggle" data-toggle="dropdown"
                                 src="${record.account.cardLogo}"
                                 title="${record.account.name}" />`;
            } else if (record.account.accountType != 1 && record.account.bankImage && record.account.cardLogo == undefined) {
                html = `<img class="ui-payment-small" alt=""
                                 src="${record.account.bankImage}"
                                 title="${record.account.name}">`
            } else {
                html = `<i class="text-xlarge mt-1 text-primary ${record.account.accountIcon}"
                               title="${record.account.name}"></i>`
            }
            return html;
        },
        getDate: function (groupRecord) {
            let date = moment(groupRecord.groupDate).format("DD.MM.YYYY");
            let today = moment().format("DD.MM.YYYY");
            let yesterday = moment().subtract(1, "days").format("DD.MM.YYYY");
            if (today == date) {
                return "Сегодня";
            } else if (yesterday == date) {
                return "Вчера";
            } else {
                return date;
            }
        },
    }
});