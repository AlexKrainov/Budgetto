Vue.component("vue-record-component", {
    template: `<div class="row" v-bind:id="id" v-bind:name="name">
    <div class="modal modal-top fade" id="modal-record">
        <div class="modal-dialog modal-lg">
            <article class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">
                        Добавление записи
                    </h5>
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">×</button>
                </div>
                <div class="modal-body">
                    <div class="form-row">
                        <div class="form-group col">
                            <div class="input-group">
                                <span class="input-group-prepend">
                                    <button v-on:click="addDays(-1)" class="btn btn-default modal-record-date-button" type="button" title="Минус 1 день"><i class="fa fa-angle-left font-size-large" aria-hidden="true"></i></button>
                                </span>
                                <input type="text" class="form-control record-date" id="record-date" v-model="dateTimeOfPayment">
                                <span class="input-group-append">
                                    <button v-on:click="addDays(1)" class="btn btn-default modal-record-date-button" type="button" title="Плюс 1 день"><i class="fa fa-angle-right font-size-large" aria-hidden="true"></i></button>
                                </span>
                            </div>
                        </div>
                    </div>
                    <section id="record-add-section" v-show="isShowHistory == false">

                        <div class="form-row">
                            <div class="form-group col">
                                <div class="input-group">
                                    <input type="text" class="form-control" id="money">
                                    <div class="input-group-prepend">
                                        <button type="button" class="btn btn-default dropdown-toggle" data-toggle="dropdown">{{ currentCurrency.icon }}</button>
                                        <div class="dropdown-menu">
                                            <a class="dropdown-item"
                                               href="javascript:void(0)"
                                               v-for="currencyInfo in currencyInfos"
                                               v-bind:class="currentCurrencyID == currencyInfo.id ? 'active' : ''"
                                               v-on:click="changeCurrency(currencyInfo)">{{ currencyInfo.icon }}</a>

                                        </div>
                                    </div>
                                </div>
                                <small class="text-muted">Чтобы изменить сумму, нажмите на нее два раза</small>
                            </div>
                        </div>
                        <div id="currency-container" class="form-inline mb-4" v-show="currentCurrencyID != 1">
                            <label class="form-check mr-sm-2 mb-2 mb-sm-0">
                                <input class="form-check-input" type="checkbox"
                                       v-model="isUseBankRate"
                                       v-on:change="getRate">
                                <div class="form-check-label">
                                    Конвертировать по курсу ЦБ на текущую дату
                                </div>
                            </label>
                            <div class="input-group text-right">
                                <input type="number" class="form-control"
                                       v-model="exchangeRate"
                                       v-bind:disabled="isUseBankRate">
                                <div class="input-group-prepend">
                                    <div class="input-group-text">₽</div>
                                </div>
                            </div>
                        </div>
                        <div class="form-row">
                            <div class="form-group col-6">
                                <div class="row records" v-for="record in records" v-show="record.isCorrect">
                                    <div class="col-6 mb-3 record-item">
                                        <a href="javascript:void(0)"
                                           class="a-hover font-weight-bold font-size-large"
                                           v-bind:class="descriptionRecord == record ? 'text-primary' : 'text-secondary'"
                                           title="Добавить описание"
                                           v-on:click="descriptionRecord = record">
                                            {{ showRecord(record) }}
                                            <i class="fas badge badge-dot indicator fa-comment-dots has-comment" v-show="record.description != undefined && record.description != ''"></i>
                                        </a>
                                        <span class="record-item-actions cursor-pointer font-size-large ml-3">
                                            <span v-on:click="descriptionRecord = record">+ <i class="far fa-comment"></i></span>
                                        </span>
                                    </div>
                                    <div class="col-6 mb-3 text-right">
                                        <span class="text-muted">{{ record.sectionName }} </span>
                                        <span class="fa fa-trash remove-section-icon cursor-pointer ml-1"
                                              v-on:click="record.sectionID = -1; record.sectionName = '';"
                                              v-show="record.sectionName"></span>
                                        <span class="text-danger"
                                              v-show="isErrorSelectSection && !record.sectionName"><i class="ion ion-ios-alert"></i> Не выбрана категория</span>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group col-6">
                                <vue-section-component data-search-id="searchSection"
                                                       data-records-style="height: 200px; overflow-x: overlay;"
                                                       data-class="cards-small"
                                                       v-bind:is-show-filter="true"
                                                       v-on:onchoose="onChooseSection"></vue-section-component>
                            </div>
                        </div>
                        <div class="form-row " v-bind:class="descriptionRecord ? 'show-comment': 'hide-comment'">
                            <div class="form-group col">
                                <label class="form-label">Комментарий для {{ showRecord(descriptionRecord) }}</label>
                                <span class="comment-actions cursor-pointer ml-3">
                                    <i class="fa fa-trash"
                                       v-on:click="descriptionRecord.description = null;"
                                       v-show="descriptionRecord.description"></i>
                                </span>
                                <div class="input-group">
                                    <textarea class="form-control"
                                              v-model="descriptionRecord.description"></textarea>
                                </div>
                            </div>
                        </div>

                    </section>
                    <section id="history-records" v-show="isShowHistory">
                        <vue-record-history-component>
                        </vue-record-history-component>
                    </section>
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0)" v-show="isShowHistory == false" v-on:click="showHistory(true)">История</a>
                    <a href="javascript:void(0)" v-show="isShowHistory == true" v-on:click="showHistory(false)">Добавить запись</a>
                    <div class="form-group" style=" margin-left: 0px; margin-right: auto;" v-show="isShowCollectionElement">
                        <label class="custom-control custom-checkbox">
                            <input type="checkbox" class="custom-control-input" v-model="isShowInCollection">
                            <span class="custom-control-label">Show in collective budget</span>
                        </label>
                    </div>
                    <button class="btn btn-primary" type="button" 
                        v-bind:disabled="isSaving" 
                        v-on:click="save($emit)"
                        v-show="isShowHistory == false">
                        <span class="spinner-border" role="status" aria-hidden="true" v-show="isSaving"></span>
                        {{ isEditMode ? 'Редактировать': 'Добавить' }}
                    </button>
                    <button type="button" class="btn btn-default" data-dismiss="modal">Закрыть</button>
                </div>
            </article>
        </div>
    </div>
</div>
`,
    props: {
        id: String,
        name: String,

        //Events
        afterSave: Event,
        //showModel: Event,
    },
    data: function () {
        return {
            dateTimeOfPayment: null,
            isShowCollectionElement: true,
            isShowInCollection: true,
            records: [],
            counter: -99,

            currentCurrencyID: 1,
            currentCurrency: {},
            exchangeRate: null,
            exchangeNominal: 1,
            isUseBankRate: false,
            currencyInfos: null,

            descriptionRecord: "",

            isShowHistory: false,

            //components
            flatpickr: {},
            tagify: {},

            //state
            isSaving: false,
            isEditMode: false,
            isErrorSelectSection: false,
            after_save_callback: Event,
            after_save_callback_args: undefined,
        }
    },
    computed: {
        sectionComponent: function () {
            return this.$children[0];
        },
        historyComponent: function () {
            return this.$children[1];
        }
    },
    mounted: function () {

        let flatpickrConfig = GetFlatpickrRuConfig();
        flatpickrConfig.onChange = function (selectedDates, dateStr, instance) {
            if (RecordVue.recordComponent.isShowHistory) {
                RecordVue.recordComponent.historyComponent.dateTimeOfPayment =
                    RecordVue.recordComponent.flatpickr.latestSelectedDateObj.toLocaleDateString();
            }
        };
        this.flatpickr = flatpickr('#record-date', flatpickrConfig);

        //https://github.com/yairEO/tagify#events
        //https://yaireo.github.io/tagify/
        //https://rawgit.com/joewalnes/filtrex/master/example/colorize.html
        //https://github.com/joewalnes/filtrex/blob/master/example/colorize.js
        var elementMoney = document.getElementById("money");
        this.tagify = new Tagify(elementMoney, {
            transformTag: this.transformTag,
            duplicates: true,
            placeholder: "550 или 100+500 или 199.99",
            callbacks: {
                change: function () {
                    console.log("Change");
                    console.log(arguments);
                },
                blur: function () {
                    console.log("Blur");
                    console.log(arguments);
                },
            }
        });

        this.tagify
            .on('remove', this.removeTag);
        //.on('keydown', this.keydownTagify)

        this.loadCurrenciesInfo()
            .then(function () {

            });

        this.isShowCollectionElement = UserInfo.IsAllowCollectiveBudget;

        $('#modal-record').on('show.bs.modal', function () {

        });
        $('#modal-record').on('hide.bs.modal', function () {
            RecordVue.recordComponent.isShowHistory = false;
            if (RecordVue.recordComponent.isEditMode) {
                RecordVue.recordComponent.isEditMode = false;
                RecordVue.recordComponent.tagify.removeTags();
            }
        });
    },
    methods: {
        transformTag: function (item) {
            let total;
            let value = item.value;
            let isCorrect = false;

            //console.log("transformTag");
            //console.log(arguments);
            //remove dublicate after edit
            if (this.records.some(x => x.tag == item.value)) {
                return false;
            }

            //bug with 015
            if (value && value[0] == "0") {
                try {
                    value = value * 1;
                } catch (e) {
                    item.value = value;
                }
            }

            try {
                let func = compileExpression(value.toString());
                total = func("1");
                if (total) {
                    item.style = "--tag-bg: #02BC77";
                    isCorrect = true;
                } else {
                    item.style = "--tag-bg: #d9534f";
                }
            } catch (e) {
                item.style = "--tag-bg: #d9534f";
            }

            if (total) {
                total = Math.round(total * 100) / 100;
            }

            if (item.id == undefined) {
                item.id = this.counter++;

                let newRecords = {
                    id: item.id,
                    isCorrect: isCorrect,
                    money: total,
                    tag: item.value,
                    sectionID: -1,
                    sectionName: "",
                    description: undefined,
                    currencyRate: null,
                    currencyNominal: 1,
                    currencyID: this.currentCurrencyID,
                };

                this.records.push(newRecords);

                if (newRecords.isCorrect) {
                    this.descriptionRecord = newRecords;
                }
            } else {
                let el = this.records.find(x => x.id == item.id);
                el.money = total;
                el.tag = item.value;

            }
        },
        //keydownTagify: function (event) {
        //    console.log(event.detail.originalEvent.key);

        //    if (event.detail.originalEvent.key == ",") {
        //        event.detail.originalEvent.key = ".";
        //    }
        //},
        removeTag: function (event) {
            if (event.detail.data && event.detail.data.value) {
                let removeIndex = this.records.findIndex(x => x.tag == event.detail.data.value);
                if (removeIndex >= 0) {
                    if (this.descriptionRecord.tag == event.detail.data.value) {
                        this.descriptionRecord = "";
                    }
                    this.records.splice(removeIndex, 1);
                }
            }
        },
        showRecord: function (record) {
            if (this.currentCurrencyID != UserInfo.CurrencyID && this.exchangeRate && this.exchangeRate > 0) {
                if (this.currentCurrencyID != 1) {
                    record.currencyRate = this.exchangeRate;
                    record.currencyNominal = this.exchangeNominal;
                    record.currencyID = this.currentCurrencyID;
                }
                let tagValue;
                try {
                    record.money = CurrencyCalculateExpression(record.tag, this.exchangeRate);

                    func = compileExpression(record.tag);
                    tagValue = func("1");

                } catch (e) {

                }

                return `(${new Intl.NumberFormat(this.currentCurrency.specificCulture, { style: 'currency', currency: this.currentCurrency.codeName }).format(tagValue)}) 
            * ${new Intl.NumberFormat('ru-RU', { style: 'currency', currency: 'RUB' }).format(this.exchangeRate)} 
            = ${new Intl.NumberFormat('ru-RU', { style: 'currency', currency: 'RUB' }).format(record.money)}`;
            } else {
                //this needs for jump from dollar back to rub
                try {
                    let func = compileExpression(record.tag);
                    record.money = func("1");
                    record.currencyRate = null;
                    record.currencyNominal = 1;
                    record.currencyID = this.currentCurrencyID;
                } catch (e) { }
            }


            if (record.tag == record.money) {
                return new Intl.NumberFormat('ru-RU', { style: 'currency', currency: 'RUB' }).format(record.money);
            } else {
                return record.tag + ' = ' + new Intl.NumberFormat('ru-RU', { style: 'currency', currency: 'RUB' }).format(record.money);
            }

            //{ { record.tag == record.money ? getMoney(record.money) : record.tag + " " + currentCurrencyIcon } }
            //{ { record.tag != record.money ? '= ' + getMoney(record.money) " " + currentCurrencyIcon : '' } }
        },
        save: function (emit) {
            if (this.records && this.records.length > 0 && this.records.some(x => x.isCorrect)) {

                if (this.checkValidBeforeSave() == false) {
                    return false;
                }

                let obj = {
                    dateTimeOfPayment: this.flatpickr.latestSelectedDateObj.toLocaleDateString(),
                    isShowInCollection: this.isShowCollectionElement == false ? false : this.isShowInCollection,
                    records: this.records.filter(x => x.isCorrect)
                };

                this.isSaving = true;

                return $.ajax({
                    type: "POST",
                    url: "/Record/SaveRecords",
                    data: JSON.stringify(obj),
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'json',
                    context: this,
                    success: function (result) {
                        if (result.isOk == true) {
                            if (result.budgetRecord.records.findIndex(x => x.isSaved == false) == -1) {
                                this.clearAll();
                            } else {
                                let records = result.budgetRecord.records.filter(x => x.isSaved);
                                let tags = records.map(x => x.tag)
                                this.tagify.removeTags(tags);

                                //remove comment
                                if (records.findIndex(x => x.tag == this.descriptionRecord.tag) >= 0) {
                                    this.descriptionRecord = "";
                                }
                            }
                            this.$emit("afterSave", 123);
                            this.isSaving = false;

                            if (this.after_save_callback && typeof (this.after_save_callback) === "string") {

                                this.after_save_callback = window.getFunctionFromString(this.after_save_callback);
                            }

                            if (typeof (this.after_save_callback) === "function") {
                                try {
                                    if (this.after_save_callback_args != undefined) {
                                        this.after_save_callback.call(this, this.after_save_callback_args);
                                        this.this.after_save_callback_args = undefined
                                    } else {
                                        this.after_save_callback.call(this, result.budgetRecord.dateTimeOfPayment);
                                    }

                                } catch (e) {
                                    console.log(e);
                                }
                                // this.after_save_callback = null;
                            } else {
                                //if it's a budget page we need to update data

                            }
                            this.isEditMode = false;
                            this.sectionComponent.clearSearchTextValue();
                        }
                        return result;
                    },
                    error: function (xhr, status, error) {
                        console.log(error);
                        this.$emit("afterSave", 123);
                        this.isSaving = false;
                    }
                }, this);
            } else {
                return false;
            }
        },
        checkValidBeforeSave: function () {
            let isOk = true;
            this.isErrorSelectSection = false;

            for (var i = 0; i < this.records.length; i++) {
                if (this.records[i].sectionID == -1) {
                    this.isErrorSelectSection = true;
                }
            }

            return isOk;
        },
        editByID: function (id) {
            this.isSaving = true;

            return $.ajax({
                type: "GET",
                url: "/Record/GetByID/" + id,
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                context: this,
                success: function (result) {

                    this.clearAll();

                    if (result.isOk == true) {
                        this.records.push(result.record);
                        this.flatpickr.setDate(result.record.dateTimeOfPayment);
                        this.tagify.addTags([{ value: result.record.tag, id: result.record.id }]);
                        $("#modal-record").modal("show");
                    }

                    this.isSaving = false;
                    return result;
                },
                error: function (xhr, status, error) {
                    console.log(error);
                    this.isSaving = false;
                }
            }, this);
        },
        editByElement: function (record, callback, args) {
            this.clearAll();

            this.records.push(record);

            this.setCurrentCurrency(record.currencyID);
            this.exchangeRate = record.currencyRate;
            this.isShowInCollection = record.isShowForCollection;
            this.flatpickr.setDate(record.dateTimeOfPayment);
            this.tagify.addTags([{ value: record.tag, id: record.id }]);

            this.after_save_callback = callback;
            this.after_save_callback_args = args;
            this.isEditMode = true;
            $("#modal-record").modal("show");
        },
        onChooseSection: function (section) {
            let record = this.records.find(x => x.isCorrect && x.sectionID == -1);

            if (record) {
                record.sectionID = section.id;
                record.sectionName = section.name
            }
        },
        clearAll: function () {
            this.records = [];
            this.tagify.removeAllTags();
            this.descriptionRecord = "";
        },

        addDays: function (days) {
            var result = new Date(this.flatpickr.latestSelectedDateObj);
            result.setDate(result.getDate() + days);
            console.log(result);

            this.flatpickr.setDate(result, true);
        },
        changeCurrency: function (currencyInfo) {
            this.currentCurrency = currencyInfo;
            this.currentCurrencyID = currencyInfo.id;

            if (this.currentCurrencyID != UserInfo.currencyID) {
                this.isUseBankRate = true;
                this.getRate();
            } else {
                this.isUseBankRate = false;
                this.exchangeRate = null;

            }
        },
        getRate: function () {
            if (this.isUseBankRate) {
                this.getRateFromBank();
            }
            return;
        },
        getRateFromBank: function () {
            let dateInFormat = this.flatpickr.latestSelectedDateObj.toLocaleDateString();
            ShowLoading("#currency-container");
            return $.ajax({
                type: "GET",
                url: "/Budget/GetRateFromBank?link=" + this.currentCurrency.cbR_Link + "&date=" + dateInFormat, // "http://www.cbr.ru/scripts/XML_daily.asp?date_req=02/03/2002&VAL_NM_RQ=R01235",
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                context: this,
                success: function (response) {
                    if (response.isOk && response.response) {
                        let obj = ParseXml(response.response);
                        let cur = obj.ValCurs.Valute.find(x => x.ID == this.currentCurrency.codeName_CBR);
                        if (cur) {
                            this.exchangeRate = cur.Value["#text"].replaceAll(",", ".");
                        }
                    } else {
                        this.isUseBankRate = false;
                        toastr.error("Извините, не удалось подгрузить данные из ЦБ.");
                    }
                    HideLoading("#currency-container");
                    return response;
                },
                error: function (xhr, status, error) {
                    console.log(error);
                    HideLoading("#currency-container");
                }
            });
        },
        setCurrentCurrency: function (currencyID) {
            this.currentCurrencyID = currencyID;
            this.currentCurrency = this.currencyInfos.find(x => x.id == this.currentCurrencyID);
        },
        loadCurrenciesInfo: function () {
            return $.ajax({
                type: "GET",
                url: "/Common/GetCurrenciesInfo",
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                context: this,
                success: function (response) {
                    if (response.isOk) {
                        this.currencyInfos = response.data;

                        this.setCurrentCurrency(UserInfo.CurrencyID);
                    }

                    return response;
                },
                error: function (xhr, status, error) {
                    console.log(error);
                }
            });
        },
        getDateByFormat: function (date, format) {
            return GetDateByFormat(date, format);
        },
        showModel: function (dateTime, callback, args) {
            if (dateTime) {
                this.flatpickr.setDate(dateTime);
            } else {
                this.flatpickr.setDate("today");
            }

            this.after_save_callback = callback;
            this.after_save_callback_args = args;

            $("#modal-record").modal("show");
            document.getElementById("money").focus()
        },

        //History
        showHistory: function (isShow) {
            this.isShowHistory = isShow;
            if (isShow) {
                this.historyComponent.dateTimeOfPayment = this.flatpickr.latestSelectedDateObj.toLocaleDateString();
            }
        }
    }
});

//<span class="input-group-prepend">
//                                    <button v-on:click="addDays(-7)" class="btn btn-default modal-record-date-button" type="button" title="Минус 7 дней"><i class="fa fa-angle-double-left font-size-large" aria-hidden="true"></i></button>
//                                </span>
 //<span class="input-group-append">
 //                                   <button v-on:click="addDays(7)" class="btn btn-default modal-record-date-button" type="button" title="Плюс 7 дней"><i class="fa fa-angle-double-right font-size-large" aria-hidden="true"></i></button>
 //                               </span>