Vue.component("vue-record-component", {
	template: `<div class="row" v-bind:id="id" v-bind:name="name">
<button type="button" data-toggle="modal" data-target="#modals-default">open</button>
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
							<input type="text" class="form-control" id="record-date" v-model="dateTimeOfPayment">
						</div>
					</div>
					<div class="form-row">
						<div class="form-group col">
							<label class="form-label">Money</label>
							<div class="input-group">
								<input type="text" class="form-control" id="money" data-role="tagsinput">
								<div class="input-group-prepend">
									<span v-on:click="" class="input-group-text cursor-pointer"><i class="fa fa-ruble-sign"></i></span>
								</div>
								<div class="input-group-prepend">
									<span class="input-group-text cursor-pointer text-muted"><i class="fa fa-dollar-sign"></i></span>
								</div>
								<div class="input-group-prepend">
									<span class="input-group-text cursor-pointer text-muted"><i class="fa fa-euro-sign"></i></span>
								</div>
							</div>
						</div>
					</div>
					<div class="form-row">
						<div class="form-group col-6">
							<div class="row" v-for="record in records" v-show="record.isCorrect">
								<div class="col-6 mb-3">
									{{ record.tag }}
									{{ record.tag != record.money ? getMoney(record.money) : '' }}
									<span class="text-muted"></span>
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
							<label class="form-label">Description</label>
							<div class="input-group">

								<textarea class="form-control"></textarea>
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
		aftersave: Event,
	},
	data: function () {
		return {
			dateTimeOfPayment: null,
			records: [],

			//components
			flatpickr: {},
			tagify: {},

			//state
			isSaving: false,
		}
	},
	mounted: function () {

		this.flatpickr = flatpickr('#record-date', {
			altInput: true,
			//dateFormat: 'd.m.Y',
			defaultDate: Date.now()
		});
		//https://github.com/yairEO/tagify#events
		//https://yaireo.github.io/tagify/
		//https://rawgit.com/joewalnes/filtrex/master/example/colorize.html
		//https://github.com/joewalnes/filtrex/blob/master/example/colorize.js
		var elementMoney = document.getElementById("money");
		this.tagify = new Tagify(elementMoney, {
			transformTag: this.transformTag,
			duplicates: true,
		});

		this.tagify.on('remove', this.removeTag);
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

			this.records.push(
				{
					isCorrect: isCorrect,
					money: total,
					tag: item.value,
					sectionID: -1,
					sectionName: ""
				});
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
						}
						return result;
					},
					error: function (xhr, status, error) {
						console.log(error);
					}
				}, this);

			}
		},
		onChooseSection: function (section) {
			let record = this.records.find(x => x.isCorrect && x.sectionID == -1);

			if (record) {
				record.sectionID = section.id;
				record.sectionName = section.name
			}
		},
		getMoney: function (money) {
			return '= ' + numberOfThreeDigits(money)
		}
	}
});