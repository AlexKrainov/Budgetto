Vue.component("vue-record-component", {
    template: `<div class="row" v-bind:id="id" v-bind:name="name">
	<div class="modal fade" id="modals-default">
		<div class="modal-dialog">
			<form class="modal-content">
				<div class="modal-header">
					<h5 class="modal-title">
						Payment
						<span class="font-weight-light">Information</span>
						<br>
						<small class="text-muted">We need payment information to process your order.</small>
					</h5>
					<button type="button" class="close" data-dismiss="modal" aria-label="Close">×</button>
				</div>
				<div class="modal-body">
					<div class="form-row">
						<div class="form-group col">
						<label class="form-label">Date</label>
							<div class="input-group">
							<span class="input-group-prepend">
								<button v-on:click="addDays(-1)" class="btn btn-default" type="button" title="Минус 1 день"><i class="fa fa-angle-left" aria-hidden="true"></i></button>
							</span>
							<span class="input-group-prepend">
								<button v-on:click="addDays(-7)" class="btn btn-default" type="button" title="Минус 7 дней"><i class="fa fa-angle-double-left" aria-hidden="true"></i></button>
							</span>
							<input type="text" class="form-control" id="record-date" v-model="dateTimeOfPayment">
							<span class="input-group-append">
								<button v-on:click="addDays(7)" class="btn btn-default" type="button" title="Плюс 7 дней"><i class="fa fa-angle-double-right" aria-hidden="true"></i></button>
							</span>
							<span class="input-group-append">
								<button v-on:click="addDays(1)" class="btn btn-default" type="button" title="Плюс 1 день"><i class="fa fa-angle-right" aria-hidden="true"></i></button>
							</span>
						</div>
						</div>
					</div>
					<div class="form-row">
						<div class="form-group col">
							<label class="form-label">Money</label>
							<div class="input-group">
								<input type="text" class="form-control" id="money" data-role="tagsinput">
								<div class="input-group-prepend">
									<button type="button" class="btn btn-default dropdown-toggle" data-toggle="dropdown">{{ currentCurrencyIcon }}</button>
									<div class="dropdown-menu">
										<a class="dropdown-item" v-bind:class="currentCurrencyID == 1 ? 'active' : ''" href="javascript:void(0)" v-on:click="changeCurrency(1,'₽')">₽</a>
										<a class="dropdown-item" v-bind:class="currentCurrencyID == 2 ? 'active' : ''" href="javascript:void(0)" v-on:click="changeCurrency(2,'€')">€</a>
										<a class="dropdown-item" v-bind:class="currentCurrencyID == 3 ? 'active' : ''" href="javascript:void(0)" v-on:click="changeCurrency(3,'$')">$</a>
									</div>
								</div>
							</div>
						</div>
					</div>
					<div class="form-row">
						<div class="form-group col-6">
							<div class="row" v-for="record in records" v-show="record.isCorrect">
								<div class="col-6 mb-3">
									<a href="javascript:void(0)" v-bind:class="descriptionRecord == record ? 'text-primary' : 'text-secondary'" title="Добавить описание" v-on:click="descriptionRecord = record">
									{{ record.tag == record.money ? getMoney(record.money) : record.tag }}
									{{ record.tag != record.money ? '= ' + getMoney(record.money) : '' }}
									<i class="fas fa-comment badge badge-dot indicator" v-show="record.description != undefined || record.description != ''"></i>
									</a>
								</div>
								<div class="col-6 mb-3 ">
									<span class="text-muted">{{ record.sectionName }} </span>
									<span class="fa fa-trash remove-section-icon cursor-pointer ml-1"
										  v-on:click="record.sectionID = -1; record.sectionName = '';"
										  v-show="record.sectionName"></span>
								</div>
							</div>
						</div>
						<div class="form-group col-6">
							<vue-section-component id="test2"
												   name="test"
												   data-search-id="searchSection"
												   data-search-style="max-width: 300px;"
												   v-on:onchoose="onChooseSection"></vue-section-component>
						</div>
					</div>
					<div class="form-row">
						<div class="form-group col">
							<label class="form-label">Комментарий к {{ descriptionRecord.tag }}</label>
							<div class="input-group">
								<textarea class="form-control" v-model="descriptionRecord.description"></textarea>
							</div>
						</div>
					</div>
				</div>
				<div class="modal-footer">
					<button class="btn btn-primary" type="button" disabled v-show="isSaving">
						<span class="spinner-border" role="status" aria-hidden="true"></span>
						Add
					</button>
					<button type="button" class="btn btn-primary" v-on:click="save($emit)" v-show="!isSaving">Add</button>

					<button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
				</div>
			</form>
		</div>
	</div>
</div>
`,
    props: {
        id: String,
        name: String,


        //Events
        aftersave: Event
    },
    data: function () {
        return {
            dateTimeOfPayment: null,
            records: [],
            counter: -99,
            currentCurrencyIcon: "₽",
            currentCurrencyID: 1,

            descriptionRecord: {},

            //components
            flatpickr: {},
            tagify: {},

            //state
            isSaving: false,
            after_save_callback: Event,
        }
    },
    mounted: function () {

        this.flatpickr = flatpickr('#record-date', {
            altInput: true,
            //dateFormat: 'd.m.Y',
            defaultDate: "today"// Date.now()
        });
        //https://github.com/yairEO/tagify#events
        //https://yaireo.github.io/tagify/
        //https://rawgit.com/joewalnes/filtrex/master/example/colorize.html
        //https://github.com/joewalnes/filtrex/blob/master/example/colorize.js
        var elementMoney = document.getElementById("money");
        this.tagify = new Tagify(elementMoney, {
            transformTag: this.transformTag,
            duplicates: true,
            placeholder: "550 или 100+500 или 199.99"
        });

        this.tagify.on('remove', this.removeTag);

        this.currentCurrency = UserInfo.Currency.Icon;
        this.currentCurrencyID = UserInfo.CurrencyID;
    },
    methods: {
        transformTag: function (item) {
            let total;
            let isCorrect = false;

            try {
                let func = compileExpression(item.value);
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


            if (!item.id) {
                item.id = this.counter++;

                this.records.push(
                    {
                        id: item.id,
                        isCorrect: isCorrect,
                        money: total,
                        tag: item.value,
                        sectionID: -1,
                        sectionName: "",
                        description: undefined,
                    });
            } else {
                let el = this.records.find(x => x.id == item.id);
                el.money = total;
                el.tag = item.value;
            }
        },
        removeTag: function (event) {
            if (event.detail.data && event.detail.data.value) {
                let removeIndex = this.records.findIndex(x => x.tag == event.detail.data.value);
                if (removeIndex >= 0) {
                    this.records.splice(removeIndex, 1);
                }
            }
        },
        save: function (emit) {
            if (this.records && this.records.length > 0 && this.records.some(x => x.isCorrect)) {

                let obj = {
                    dateTimeOfPayment: this.flatpickr.latestSelectedDateObj.toLocaleDateString(),
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
                        if (result.isOk = true) {
                            for (var i = 0; i < result.budgetRecord.records.length; i++) {
                                let record = result.budgetRecord.records[i];
                                if (record.isSaved) {
                                    let index = this.tagify.getTagIndexByValue(record.tag);
                                    if (index >= 0) {
                                        this.tagify.removeTag(record.tag);
                                    }
                                }
                            }
                            this.$emit("aftersave", 123);
                            this.isSaving = false;

                            if (typeof (this.after_save_callback) === "function") {
                                try {
                                    this.after_save_callback.call(this, result.budgetRecord.dateTimeOfPayment);
                                } catch (e) {
                                    console.log(e);
                                }
                            }
                        }
                        return result;
                    },
                    error: function (xhr, status, error) {
                        console.log(error);
                        this.$emit("aftersave", 123);
                        this.isSaving = false;
                    }
                }, this);
            }
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

                    if (result.isOk = true) {
                        this.records.push(result.record);
                        this.flatpickr.setDate(result.record.dateTimeOfPayment);
                        this.tagify.addTags([{ value: result.record.tag, id: result.record.id }]);
                        $("#modals-default").modal("show");
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
        editByElement: function (record, callback) {
            this.records.push(record);
            this.flatpickr.setDate(record.dateTimeOfPayment);
            this.tagify.addTags([{ value: record.tag, id: record.id }]);
            this.after_save_callback = callback;
            $("#modals-default").modal("show");
        },
        onChooseSection: function (section) {
            let record = this.records.find(x => x.isCorrect && x.sectionID == -1);

            if (record) {
                record.sectionID = section.id;
                record.sectionName = section.name
            }
        },
        getMoney: function (money) {
            return numberOfThreeDigits(money)
        },
        addDays: function (days) {
            var result = new Date(this.flatpickr.latestSelectedDateObj);
            result.setDate(result.getDate() + days);
            console.log(result);

            this.flatpickr.setDate(result, true);
        },
        changeCurrency: function (currencyID, icon) {
            this.currentCurrencyIcon = icon;
            this.currentCurrencyID = currencyID;
        }
    }
});