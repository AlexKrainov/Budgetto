Vue.component("new-vue-record-history-component", {
    template: `
<div class="ui-timeline ui-timeline-with-info">
    <div v-for="(groupRecord, index) in groupRecords">
        <div class="ui-timeline-separator text-big">
            <div class="ui-timeline-track-bg d-inline-block rounded small font-weight-semibold py-2 px-4"
                >Сегодня</div>
        </div>
        <div class="mb-2" style="margin-top: -57px;" 
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
                <div class="col-4 col-md-3 px-4 font-weight-semibold text-center">
                    Деньги

                </div>
            </div>
        </div>
        <div class="ui-timeline-item"
            v-for="record in groupRecord.records">
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
                            <img class="ui-payment-small dropdown-toggle" data-toggle="dropdown" style="margin-right: 10px;"
                                 v-show="record.account.accountType != 1 && record.account.cardLogo"
                                 v-bind:src="record.account.cardLogo"
                                 v-bind:title="record.account.name" />
                            <img class="ui-payment-small mr-1" alt=""
                                 v-show="record.account.accountType != 1 && record.account.bankImage && record.account.cardLogo == undefined"
                                 v-bind:src="record.account.bankImage"
                                 v-bind:title="record.account.name">
                            <i class="text-xlarge mt-1 text-primary mr-1"
                               v-show="record.account.accountType == 1 || (record.account.accountType != 1 && !record.account.bankImage)"
                               v-bind:class="record.account.accountIcon"
                               v-bind:title="record.account.name"></i>
                            <div class="media-body ml-3">
                                {{ record.account.name }}
                                <div class="text-muted small">
                                    <span>
                                        <del>
                                            <span class=" pt-1">
                                                2000&nbsp;₽
                                            </span>
                                        </del>
                                        &nbsp; -&gt;&nbsp;
                                        <span class=" pt-1">
                                            1500&nbsp;₽
                                        </span>

                                    </span>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col-6 col-md-3 px-3 pt-3 text-right">

                        <h5 class="">
                            <span class="text-muted1 small pt-1 text-success">
                                 {{ getCurrencyValue(record, record.cashback, false) }}
                            </span>
                            <a href="javascript:void(0)" class="font-weight-semibold">
                                {{ getCurrencyValue(record, record.money) }}
                            </a> <i class="fas fa-edit pl-2 edit-record cursor-pointer"></i> <i class="fas fa-trash pl-2 edit-record text-danger cursor-pointer"></i>
                        </h5>
                    </div>
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
    },
    data: function () {
        return {
            dateTimeOfPayment: null,
            isShowCollection: true,
            groupRecords: [],
            searchText: null,

            //state
            isSaving: false,
        }
    },
    watch: {
        dateTimeOfPayment: function (newValue, oldValue) {
            this.loadHistory();
        },
        searchText: function (newValue, oldValue) {
            if (newValue) {
                newValue = newValue.toLocaleLowerCase();
            }

            for (var i = 0; i < this.groupRecords.length; i++) {
                let record = this.groupRecords[i];

                record.isShowForFilter = record.sectionName.toLocaleLowerCase().indexOf(newValue) >= 0
                    || (record.description && record.description.toLocaleLowerCase().indexOf(newValue) >= 0)
                    || record.areaName.toLocaleLowerCase().indexOf(newValue) >= 0
                    || (record.userName && record.userName.toLocaleLowerCase().indexOf(newValue) >= 0)
                    || record.money.toString().indexOf(newValue) >= 0
                    || record.rawData.indexOf(newValue) >= 0;
            }
        }
    },
    mounted: function () {
        this.loadHistory();
    },
    methods: {
        loadHistory: function () {
            this.searchText = null;
            ShowLoading('#history-records');
            return $.ajax({
                type: "GET",
                url: "/History/LoadingGroupRecordsForByDate?date=2021-05-19T00:00:00",// + this.dateTimeOfPayment,
                contentType: "application/json",
                dataType: 'json',
                context: this,
                success: function (response) {
                    this.groupRecords = response.data;
                    HideLoading('#history-records');
                }
            });
            //return $.ajax({
            //    type: "GET",
            //    url: "/Budget/GetLastRecords?last=5",
            //    contentType: 'application/json; charset=utf-8',
            //    dataType: 'json',
            //    context: this,
            //    success: function (response) {
            //        if (response.isOk) {
            //            this.groupRecords = response.data;
            //        }

            //        return response;
            //    },
            //    error: function (xhr, status, error) {
            //        console.log(error);
            //    }
            //});
        },

        edit: function (record) {
            RecordVue.recordComponent.isShowHistory = false;
            if (typeof (BudgetVue) != "undefined") {
                RecordVue.editByElement(record, BudgetVue.refresh, "runtimeData");
            } else {
                RecordVue.editByElement(record);
            }
        },
        remove: function (record) {
            ShowLoading('#record_' + record.id);

            return $.ajax({
                type: "POST",
                url: "/Budget/RemoveRecord",
                data: JSON.stringify(record),
                context: record,
                contentType: "application/json",
                dataType: 'json',
                success: function (response) {
                    record.isDeleted = response.isOk;
                    HideLoading('#record_' + record.id);
                    BudgetVue.refreshAfterChangeRecords(response.dateTimeOfPayment)
                }
            });
        },
        recovery: function (record) {
            ShowLoading('#record_' + record.id);
            return $.ajax({
                type: "POST",
                url: "/Budget/RecoveryRecord",
                data: JSON.stringify(record),
                context: record,
                contentType: "application/json",
                dataType: 'json',
                success: function (response) {
                    record.isDeleted = !response.isOk;
                    HideLoading('#record_' + record.id);
                    BudgetVue.refreshAfterChangeRecords(response.dateTimeOfPayment)
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
        GetDateByFormat: function (date, format) {
            return GetDateByFormat(date, format);
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

        }
    }
});