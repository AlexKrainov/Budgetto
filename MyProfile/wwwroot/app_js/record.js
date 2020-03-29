var RecoredVue = new Vue({
	el: "#record",
	data: {
		sections: [],
		dateTimeOfPayment: null,
		money: null,
		description: null,
		searchSection: null,

		money: [], //{type: 'number', value: 300}
		records: [],
		tagify: null,
		total: 0,
		flatpickr: null,

		isSaving: false,
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

			RecoredVue.records.push(
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
				let removeIndex = RecoredVue.records.findIndex(x => x.tag == event.detail.data.value);
				if (removeIndex >= 0) {
					RecoredVue.records.splice(removeIndex, 1);
				}
			}
		},
		save: function () {
			if (this.records && this.records.length > 0 && this.records.some(x => x.isCorrect)) {

				let obj = {
					dateTimeOfPayment: RecoredVue.flatpickr.latestSelectedDateObj.toJSON(),
					records: this.records.filter(x => x.isCorrect)
				};

				this.isSaving = true;

				return sendAjax("/Record/SaveRecords", obj, "POST")
					.then(function (result) {
						if (result.isOk = true) {
							for (var i = 0; i < result.budgetRecord.records.length; i++) {
								let record = result.budgetRecord.records[i];
								if (record.isSaved) {
									let index = RecoredVue.tagify.getTagIndexByValue(record.tag);
									if (index >= 0) {
										RecoredVue.tagify.removeTag(record.tag);
									}
								}
							}
							RecoredVue.isSaving = false;
						}
					});

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