var z = Vue.component("vue-section-component", {
	template: `<div v-bind:id="id" v-bind:name="name">
				<input
					type='search'
					class="form-control form-control-sm"
					v-on:change='onchange'/>
					<div class="demo-inline-spacing" >
                        <a 
							href="javascript:void(0)" 
							class="badge badge-pill badge-default"
							v-for="section in dataSectionItems"
							v-on:click="$emit('onchoose', section)"
						>{{ section.name }}</a>
                        
                    </div>
				</div>`,
	//: name='dataSearchName'
	props: {
		dataSearchId: String,
		id:String,
		name:String,
		//dataSearchName: String,
	
		onchoose: Event,

	},
	//computed: {

	//},
	data: function () {
		return {
			count: 0,
			dataSectionItems: [],
		}
	},
	mounted: function () {
		return $.ajax({
			type: "GET",
			url: "/Section/GetSectins",
			data: null,
			contentType: 'application/json; charset=utf-8',
			dataType: 'json',
			context: this,
			success: function (result) {
				if (result.isOk) {
					this.dataSectionItems = result.sections;
				}
				return result;
			},
			error: function (xhr, status, error) {
				console.log(error);
			}
		}, this);
	},
	methods: {
		onchange: function () {

		},
		//selectSection: function (val) {
		//	this.$emit('onchooseSection', val)
		//	this.onchooseSection(val);
		//}
	}
});