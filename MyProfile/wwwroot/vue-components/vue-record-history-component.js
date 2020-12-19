Vue.component("vue-record-history-component", {
    template: `<div class="row m-0" v-bind:id="id" v-bind:name="name">
	 <div class="input-group w-100">
         <div class="input-group-prepend">
             <div class="input-group-text ion ion-ios-search"></div>
         </div>
         <input type="text" class="form-control" placeholder="Поиск" v-model="searchText">
     </div>
     <div class=" card m-4" style="width: 100%;">
         <ul class="list-group list-group-flush records-timeline" style=" margin-left: -5px;">
             <li class="list-group-item py-4 record-item"
                 v-for="record in records"
                 v-bind:id="'record_'+record.id"
                 v-show="record.isShowForFilter"
                 v-bind:style="'border-left-color: '+record.cssBackground+' !important;'">
                 <div class="media flex-wrap" v-if="record.isDeleted == false">
                     <div class="">
                         <i class="record-icon" v-bind:class="record.cssIcon"></i>
                     </div>
                     <div class="media-body ml-sm-4">
                         <h5 class="mb-2">
                             <i class="fas fa-trash pl-2 edit-record float-right text-danger cursor-pointer" v-on:click="remove(record)" v-if="record.isOwner"></i>
                             <a href="javascript:void(0)" class="float-right font-weight-semibold ml-3" v-on:click="edit(record)" v-if="record.isOwner">
                                 {{ getCurrencyValue(record) }}
                                 <i class="fas fa-edit pl-2 edit-record"></i>
                             </a>
                             <div href="javascript:void(0)" class="float-right font-weight-semibold ml-3" v-else-if="!record.isOwner">
                                 {{ getCurrencyValue(record) }}
                             </div>
                             <div class="text-body">
                                 {{ record.areaName }} > {{ record.sectionName }}
                             </div>
                             <div class="text-muted small pt-1">
                                 <i class="ion ion-md-time text-primary"></i>
                                 <span>{{ GetDateByFormat(record.dateTimeOfPayment, 'DD.MM.YYYY') }} </span>
                             </div>
                         </h5>
                         <div v-html="descriptionBuilder(record)"> </div>
                         <div class="mt-2">
                             <div class="card-title with-elements">
                                 <div class="card-title-elements ml-md-auto">
                                     <a data-toggle="collapse" v-bind:href="'#collapse_info_' + record.id" class="d-block ml-3"><i class="collapse-icon"></i></a>
                                 </div>
                             </div>
                             <div class="collapse" v-bind:id="'collapse_info_' + record.id">
                                 <span class="badge badge-outline-default text-muted" v-show="record.isConsider" title="Семеный"><i class="fas fa-users"></i></span>
                                 <span class="badge badge-outline-default text-muted"><i class="ion ion-md-time text-primary"></i>  Создание: {{ GetDateByFormat(record.dateTimeCreate, 'DD.MM.YYYY') }}</span>
                                 <span class="badge badge-outline-default text-muted"><i class="ion ion-md-time text-primary"></i>  Отредактирован: {{ GetDateByFormat(record.dateTimeEdit, 'DD.MM.YYYY') }}</span>
                                 <span class="badge badge-outline-default text-muted">
                                     <i class="ion ion-md-create text-primary"></i>  Внесенное число: {{ record.rawData }}
                                 </span>
                             </div>

                         </div>
                     </div>
                 </div>
                 <div class="media flex-wrap" v-else-if="record.isDeleted">
                     <div class="media-body ml-sm-4">
                         <i class="fa fa-undo pl-2 float-right text-success cursor-pointer" v-on:click="recovery(record)"></i>
                         <h5 class="mb-2 pr-4">
                             <div href="javascript:void(0)" class="float-right font-weight-semibold ml-3 deleted-item">
                                 {{ getCurrencyValue(record) }}
                             </div>
                             <div class="text-body deleted-item">
                                 {{ record.areaName }} > {{ record.sectionName }}
                             </div>
                             <div class="text-muted small pt-1 deleted-item">
                                 <i class="ion ion-md-time text-primary"></i>
                                 <span>{{ GetDateByFormat(record.dateTimeOfPayment, 'DD.MM.YYYY') }} </span>
                             </div>
                         </h5>
                         <div class="deleted-item">{{ record.description }}</div>
                     </div>
                 </div>
             </li>

         </ul>
     </div>
</div>
`,
    props: {
        id: String,
        name: String,

        //Events
        afterSave: Event,
        //showModal: Event,
    },
    data: function () {
        return {
            dateTimeOfPayment: null,
            isShowCollection: true,
            records: [],
            searchText: null,

            showAddRecord: false,

            //state
            isSaving: false,
            after_save_callback: Event,
        }
    },
    watch: {
        dateTimeOfPayment: function (newValue, oldValue) {
            console.log(newValue);
            this.loadHistory();
        },
        searchText: function (newValue, oldValue) {
            if (newValue) {
                newValue = newValue.toLocaleLowerCase();
            }

            for (var i = 0; i < this.records.length; i++) {
                let record = this.records[i];

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

    },
    methods: {
        loadHistory: function () {
            this.searchText = null;
            ShowLoading('#history-records');
            return $.ajax({
                type: "GET",
                url: "/Budget/LoadingRecordsForByDate?date=" + this.dateTimeOfPayment,
                contentType: "application/json",
                dataType: 'json',
                context: this,
                success: function (response) {
                    this.records = response.data;
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
            //            this.records = response.data;
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
            if (BudgetVue != undefined) {
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
                    //calendar.after_loading(response);
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
                }
            });
        },
        getCurrencyValue: function (record) {
            let value = new Intl.NumberFormat(UserInfo.Currency.SpecificCulture, { style: 'currency', currency: UserInfo.Currency.CodeName }).format(record.money);
            if (UserInfo.Currency.ID != record.currencyID) {
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
        }
    }
});