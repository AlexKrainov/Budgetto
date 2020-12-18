﻿Vue.component("vue-record-component", {
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
                                <div class="input-group input-money">
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
                            <div class="form-group col-12 col-sm-12 col-md-6">
                                <div class="row records" v-for="record in records" v-show="record.isCorrect">
                                    <div class="col-6 col-sm-6 col-md-6 mb-3 record-item">
                                        <a href="javascript:void(0)"
                                           class="a-hover font-weight-bold font-size-large"
                                           v-bind:class="selectedRecord == record ? 'text-primary' : 'text-secondary'"
                                           title="Добавить описание"
                                           v-on:click="selectedRecord = record">
                                            {{ showRecord(record) }}
                                            <i class="fas badge badge-dot indicator fa-comment-dots has-comment" v-show="record.description != undefined && record.description != ''"></i>
                                        </a>
                                        <span class="record-item-actions cursor-pointer font-size-large ml-3">
                                            <span v-on:click="selectedRecord = record">+ <i class="far fa-comment"></i></span>
                                        </span>
                                    </div>
                                    <div class="col-6 col-sm-6 col-md-6 mb-3 text-right">
                                        <span class="text-muted">{{ record.sectionName }} </span>
                                        <span class="fa fa-trash remove-section-icon cursor-pointer ml-1"
                                              v-on:click="record.sectionID = -1; record.sectionName = '';"
                                              v-show="record.sectionName"></span>
                                        <span class="text-danger"
                                              v-show="isErrorSelectSection && !record.sectionName"><i class="ion ion-ios-alert"></i> Не выбрана категория</span>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group col-12 col-sm-12 col-md-6">
                                <vue-section-component data-search-id="searchSection"
                                                       data-records-style="max-height: 200px; overflow-x: overlay;"
                                                       data-class="cards-small"
                                                       v-bind:is-show-filter="true"
                                                       v-on:onchoose="onChooseSection"></vue-section-component>
                            </div>
                        </div>
{{ selectedRecord }}
                        <div class="form-row " v-bind:class="selectedRecord && selectedRecord.id ? 'show-comment': 'hide-comment'">
                            <div class="form-group col">
                                <label class="form-label">Комментарий для </label>
                                <span title="123" contenteditable="false" spellcheck="false" tabindex="-1" class="tagify__tag 1" id="-999" style="--tag-bg: #02BC77" __isvalid="true" value="123">
                                    <div>
                                       <span class="tagify__tag-text">{{ showRecord(selectedRecord) }}</span>
                                    </div>
                                </span>
                                <span class="comment-actions cursor-pointer ml-3" title="Очистить поле комментарий">
                                    <i class="fas fa-comment-slash"
                                       v-on:click="selectedRecord.description = null; selectedRecord.Tags = []; tagifyTagsClearAll()"
                                       v-show="selectedRecord.description"></i>
                                </span>
                                <div class="input-group">
                                    <textarea class="form-control" id="selectedRecord"
                                              v-model="selectedRecord.description"></textarea>
                                </div>
                            </div>
                        </div>
{{ selectedRecord.description }}
                    </section>
                    <section id="history-records" v-show="isShowHistory">
                        <vue-record-history-component>
                        </vue-record-history-component>
                    </section>
                    <div class="callout callout-danger" v-show="isAvailable == false">
                        У вас истек пробный период. <u><a href="/Store/Index" class="text-danger">Продлить</a></u>
                    </div>
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0)" style="position: absolute;left: 21px;" 
                        v-show="isShowHistory == false" 
                        v-on:click="showHistory(true)" >История</a>
                    <a href="javascript:void(0)" v-show="isShowHistory == true" v-on:click="showHistory(false)">Добавить запись</a>
                    <div class="form-group" style=" margin-left: 0px; margin-right: auto;" v-show="isShowCollectionElement">
                        <label class="custom-control custom-checkbox">
                            <input type="checkbox" class="custom-control-input" v-model="isShowInCollection">
                            <span class="custom-control-label">Show in collective budget</span>
                        </label>
                    </div>
                    <button class="btn btn-primary button-add-record" type="button" 
                        v-bind:disabled="isAvailable == false || isSaving || records.length == 0" 
                        v-on:click="save($emit)"
                        v-show="isShowHistory == false">
                        <span class="spinner-border" role="status" aria-hidden="true" v-show="isSaving"></span>
                        {{ isEditMode ? 'Редактировать': 'Добавить' }}
                    </button>
                    <button type="button" class="btn btn-default button-close-record" data-dismiss="modal">Закрыть</button>
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
        //showModal: Event,
    },
    data: function () {
        return {
            dateTimeOfPayment: null,
            isShowCollectionElement: true,
            isShowInCollection: true,
            records: [],
            userTags: [],

            counter: -999,

            currentCurrencyID: 1,
            currentCurrency: {},
            exchangeRate: null,
            exchangeNominal: 1,
            isUseBankRate: false,
            currencyInfos: null,

            selectedRecord: {
                tags: []
            },

            isShowHistory: false,

            //components
            flatpickr: {},
            tagify: {},
            tagifyTags: {},

            //state
            isSaving: false,
            isEditMode: false,
            isErrorSelectSection: false,
            after_save_callback: Event,
            after_save_callback_args: undefined,
            isAvailable: UserInfo.IsAvailable,
        }
    },
    watch: {
        selectedRecord: function (newValue, oldValue) {
            if (newValue == undefined || newValue == "" || newValue == null || newValue.id != oldValue.id) {
                this.tagifyTagsClearAll();

                if (typeof newValue.id === "number") {
                    this.tagifyTags.loadOriginalValues(newValue.description);
                }
            }
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
            let recordComponent = RecordVue.recordComponent;
            if (recordComponent.isShowHistory) {
                recordComponent.historyComponent.dateTimeOfPayment =
                    recordComponent.flatpickr.latestSelectedDateObj.toLocaleDateString();
            }
            if (recordComponent.currentCurrencyID != UserInfo.CurrencyID) {
                recordComponent.changeCurrency(recordComponent.currencyInfos.find(x => x.id == recordComponent.currentCurrencyID))
            }
        };
        this.flatpickr = flatpickr('#record-date', flatpickrConfig);

        //https://github.com/yairEO/tagify#events
        //https://yaireo.github.io/tagify/
        //https://rawgit.com/joewalnes/filtrex/master/example/colorize.html
        //https://github.com/joewalnes/filtrex/blob/master/example/colorize.js
        this.tagify = new Tagify(document.getElementById("money"), {
            transformTag: this.transformTag,
            duplicates: true,
            placeholder: "550 или 100+500 или 199.99",
            callbacks: {
                remove: this.removeTag
            }
        });

        this.loadTags()
            .then(function () {
                this.initTagifyTags();
            });

        this.loadCurrenciesInfo();
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
        //Records
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
                    tags: [],
                };

                this.records.push(newRecords);

                if (newRecords.isCorrect) {
                    this.selectedRecord = newRecords;
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
                    if (this.selectedRecord.tag == event.detail.data.value) {
                        this.selectedRecord = {};
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
                    ////bug with 015
                    let value = record.tag;

                    if (value && value[0] == "0") {
                        try {
                            value = value * 1;
                        } catch (e) {
                            item.value = value;
                        }
                    }
                    record.money = CurrencyCalculateExpression(value, this.exchangeRate);

                    let func = compileExpression(value.toString());
                    tagValue = func("1");

                } catch (e) {

                }

                return `(${new Intl.NumberFormat(this.currentCurrency.specificCulture, { style: 'currency', currency: this.currentCurrency.codeName })
                    .format(tagValue)}) 
            * ${new Intl.NumberFormat('ru-RU', { style: 'currency', currency: 'RUB' }).format(this.exchangeRate)} 
            = ${new Intl.NumberFormat('ru-RU', { style: 'currency', currency: 'RUB' }).format(record.money)}`;
            } else {
                //this needs for jump from dollar back to rub
                try {
                    //bug with 015
                    let value = record.tag;

                    if (value && value[0] == "0") {
                        try {
                            value = value * 1;
                        } catch (e) {
                            item.value = value;
                        }
                    }

                    let func = compileExpression(value.toString());
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

        // Tags
        initTagifyTags: function () {
            let el = document.getElementById("selectedRecord");
            this.tagifyTags = new Tagify(el, {
                //  mixTagsInterpolator: ["{{", "}}"],
                duplicates: true,
                mode: 'mix',  // <--  Enable mixed-content
                pattern: /@|#|!/,
                // Array for initial interpolation, which allows only these tags to be used
                whitelist: Array.from(this.concatArray(this.userTags, this.selectedRecord.tags)),
                editTags: {
                    clicks: 1,
                },
                dropdown: {
                    enabled: 0,
                    position: "text",
                    highlightFirst: true  // automatically highlights first sugegstion item in the dropdown
                },
                callbacks: {
                    add: this.tagifyTagsAdd,
                    input: this.tagifyTagsInput,
                    remove: this.tagifyTagsRemove,
                    'edit:updated': this.tagifyTagsEdit,
                },
            });
            //el.addEventListener('change', onChange);
            //el.addEventListener('keydown', onChange);

            //function onChange(e) {
            //    if (!e.target.value) {
            //        return;
            //    }
            //    console.log(e.target.value);
            //    RecordVue.recordComponent.selectedRecord.description = e.target.value;
            //}
        },
        tagifyTagsAdd: function (e) {
            let tag = e.detail.data;
            tag.value = tag.value.trim();

            if (tag.id == undefined) {//new tag
                tag.id = this.counter++;
                tag.title = tag.value;
                tag.toBeEdit = false;
            }

            this.selectedRecord.tags.push(tag);

            if (this.userTags.some(x => x.title == tag.title) == false) {//
                this.userTags.push(tag);
            }

            this.tagifyTags.settings.whitelist = this.userTags;
            this.selectedRecord.description = document.getElementById("selectedRecord").value;
        },
        tagifyTagsEdit: function (e) {
            let index = e.detail.index;
            let newValue = e.detail.data.value.trim();
            let oldValue = this.selectedRecord.tags[index].title;

            this.selectedRecord.tags[index].value = newValue;
            this.selectedRecord.tags[index].title = newValue;

            if (this.selectedRecord.tags[index].id > 0) {
                this.selectedRecord.tags[index].toBeEdit = true;
            }

            let indexUserTags = this.userTags.findIndex(x => x.id == e.detail.data.id);

            if (indexUserTags != -1) {
                this.userTags[indexUserTags].title = newValue;
                this.userTags[indexUserTags].value = newValue;
                this.userTags[indexUserTags].toBeEdit = this.selectedRecord.tags[index].toBeEdit;
            }

            this.tagifyTags.settings.whitelist = this.userTags;
            this.selectedRecord.description = document.getElementById("selectedRecord").value;

            //??
            //replaceTag
            //newValue = '"' + newValue + '"';
            //oldValue = '"' + oldValue + '"';
            for (var i = 0; i < this.records.length; i++) {
                for (var j = 0; j < this.records[i].tags.length; j++) {
                    if (this.selectedRecord.tags[index].id == this.records[i].tags[j].id) {
                        let record = this.records[i];
                        //console.log(record.description);
                        if (record.description) {
                            record.description = record.description.replaceAll(oldValue, newValue);
                        }
                        record.tags[j].value = newValue;
                        record.tags[j].title = newValue;
                    }
                }
            }

            return true;
        },
        tagifyTagsRemove: function (e) {
            this.selectedRecord.tags.splice(e.detail.index, 1);
            this.selectedRecord.description = document.getElementById("selectedRecord").value;
        },
        tagifyTagsInput: function (e) {
            var prefix = e.detail.prefix;

            // first, clean the whitlist array, because the below code, while not, might be async,
            // therefore it should be up to you to decide WHEN to render the suggestions dropdown
            //  tagify.settings.whitelist.length = 0;

            if (prefix) {
                //if (prefix == '@' || prefix == '#' || prefix == '!') {
                //    this.tagifyTags.settings.whitelist = this.concatArray(this.userTags, this.selectedRecord.tags);
                //}
                if (e.detail.value.length > 1)
                    this.tagifyTags.dropdown.show.call(this.tagifyTags, e.detail.value);
            }
            this.selectedRecord.description = document.getElementById("selectedRecord").value;
            return true;
        },
        tagifyTagsClearAll: function () {
            this.tagifyTags.removeAllTags();
        },

        save: function (emit) {
            if (this.isAvailable && this.records && this.records.length > 0 && this.records.some(x => x.isCorrect)) {

                if (this.checkValidBeforeSave() == false) {
                    return false;
                }

                let obj = {
                    dateTimeOfPayment: moment(this.flatpickr.latestSelectedDateObj).format("YYYY-MM-DDTHH:mm:ss"),
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

                                if (result.budgetRecord.records.length == 1) {
                                    if (this.isEditMode == false) {
                                        toastr.success("Запись добавлена успешно");
                                    } else {
                                        toastr.success("Запись отредактирована");
                                    }
                                } else {
                                    if (this.isEditMode == false) {
                                        toastr.success("Записи добавлены успешно");
                                    } else {
                                        toastr.success("Записи отредактированы");
                                    }
                                }
                                this.clearAll();
                            } else {
                                let records = result.budgetRecord.records.filter(x => x.isSaved);
                                let tags = records.map(x => x.tag)
                                this.tagify.removeTags(tags);

                                //remove comment
                                if (records.findIndex(x => x.tag == this.selectedRecord.tag) >= 0) {
                                    this.selectedRecord = "";
                                }

                                if (result.budgetRecord.records.some(x => x.isSaved)) {
                                    toastr.warning("Не все записи были сохранены");
                                } else {
                                    if (result.budgetRecord.records.length == 1) {
                                        toastr.error("Не удалось сохранить запись");
                                    } else {
                                        toastr.error("Не удалось сохранить записи");
                                    }
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
                        } else {
                            this.isSaving = false;
                        }

                        this.loadTags();

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
            this.selectedRecord = {};
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
            let dateInFormat = this.flatpickr.latestSelectedDateObj.toISOString();
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

        //Loads
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
        loadTags: function () {
            return $.ajax({
                type: "GET",
                url: "/Common/GetUserTags",
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                context: this,
                success: function (response) {
                    if (response.isOk) {
                        this.userTags = response.tags;
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
        showModal: function (dateTime, callback, args) {
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
                this.historyComponent.dateTimeOfPayment = moment(this.flatpickr.latestSelectedDateObj).format("YYYY-MM-DDTHH:mm:ss");
            }
        },

        //Helpers
        concatArray: function (ar1, ar2) {
            let arr = ar1;// JSCopyObject(ar1);
            for (var i = 0; i < ar2.length; i++) {
                if (ar1.some(x => x.title.toLocaleLowerCase() != ar2[i].title.toLocaleLowerCase())) {
                    arr.push(ar2[i]);
                }
            }
            return arr;
        }
    }
});

//<span class="input-group-prepend">
//                                    <button v-on:click="addDays(-7)" class="btn btn-default modal-record-date-button" type="button" title="Минус 7 дней"><i class="fa fa-angle-double-left font-size-large" aria-hidden="true"></i></button>
//                                </span>
 //<span class="input-group-append">
 //                                   <button v-on:click="addDays(7)" class="btn btn-default modal-record-date-button" type="button" title="Плюс 7 дней"><i class="fa fa-angle-double-right font-size-large" aria-hidden="true"></i></button>
 //                               </span>