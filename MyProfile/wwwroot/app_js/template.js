var TemplateVue = new Vue({
	el: "#template-columns",
	data: {
		template: [],
		sections: [],

		counterColumn: -100,
		counterTemplateBudgetSections: -100,

		column: {},


	},
	watch: {
		"template.name": function (newValue, oldValue) {

		},
	},
	mounted: function () {
		this.init()
			.then(function () {
				TemplateVue.loadBAranAndRType();
			});
	},
	methods: {
		init: function () {
			return sendAjax("/Template/GetData/3", null, "GET")
				.then(function (result) {
					if (result.isOk = true) {
						TemplateVue.template = result.template;
						$("#templateName").val(result.template.name);
					}
				});
		},
		loadBAranAndRType: function () {
			return sendAjax("/Section/GetAllSectionByPerson", null, "GET")
				.then(function (result) {
					if (result.isOk = true) {
						TemplateVue.sections = result.sections;

					}
				});
		},
		removeColumn: function (columnID) {
			let columnIndex = this.template.columns.findIndex(x => x.id == columnID);
			if (columnIndex) {
				this.template.columns.splice(columnIndex, 1);
			}
		},
		addColumn: function () {

			$("#modalDataTypeColumn").modal("show");

			this.column = {
				"id": this.counterColumn++,
				"name": "New column",
				"order": this.template.columns.length,
				"isShow": true,
				"totalAction": 1,
				"formula": "",
				templateBudgetSections: []
			};
			//this.template.columns.push({
			//	"id": this.counterColumn++,
			//	"name": "New column",
			//	"order": this.template.columns.length,
			//	"isShow": true,
			//	"totalAction": "SUM",
			//	"formula": "",
			//	templateBudgetSections: []
			//})

		},
		addColumn_Complete: function () {
			$("#modalDataTypeColumn").modal("hide");
			this.template.columns.push(this.column);
			//$("input[name=data-column-type]:selected").removeProp("selected");
			this.column = {};
		},
		addColumnOption_step1: function (column) {
			this.column = column;
			$('.selectpicker').selectpicker("destroy").selectpicker('refresh');
			$("#modals-slide").modal("show");
		},
		addColumnOption_step2: function () {

			let budgetSectionName = $("#budgetArea option:selected").text();
			let budgetSectionID = $("#budgetArea").val();

			if (this.column.templateBudgetSections
				.find(x => x.budgetSectionName == budgetSectionName || x.budgetSectionID == budgetSectionID) != undefined) {
				$("#budgetArea")
					.next()
					.addClass("form-control is-invalid")
					.after('<div class="invalid-tooltip">Please provide a valid state.</div>');

				return;
			} else {
				let $divInvalidTooltip = $("#budgetArea")
					.next()
					.removeClass("form-control is-invalid")
					.next();

				if ($divInvalidTooltip.prop("class").indexOf("is-invalid") > 0) {
					$divInvalidTooltip.remove();
				}
			}

			if (this.column.formula.length > 0) {
				this.column.formula += " + <span class='badge badge-secondary'>" + budgetSectionName + "</span>";
			} else {
				this.column.formula += "<span class='badge badge-secondary'>" + budgetSectionName + "</span>";
			}

			this.column.templateBudgetSections.push({
				id: this.counterTemplateBudgetSections++,
				budgetSectionID: budgetSectionID,
				budgetSectionName: budgetSectionName
			});
			$("#modals-slide").modal("hide");
		},
		saveTemplate: function () {
			return sendAjax("/Template/Save", this.template, "POST")
				.then(function (result) {
					if (result.isOk = true) {
						TemplateVue.template = result.template;
					}
				});
		}
	}
});


//var BillingEventVue = new Vue({
//	el: "#billing-event-notify",
//	data: {
//		productID: null,
//		billingEvent: {},
//	},
//	computed: {
//		loadingElement: function () {
//			return $('#billing-event-notify').find('.ibox-content');
//		}
//	},
//	mounted: function () {
//		this.productID = $("#productid").val();
//		this.loadBillingEvent();
//	},
//	methods: {
//		loadBillingEvent: function () {
//			this.loadingElement.toggleClass('sk-loading');
//			sendAjax("/Product/BillingEventNotification/" + this.productID, null, "GET")
//				.then(function (result) {
//					if (result && result.data != null) {
//						BillingEventVue.billingEvent = result.data;
//					}
//					BillingEventVue.loadBillingEvent.toggleClass('sk-loading');
//				});
//		}

//	}
//});

Vue.config.devtools = true;
