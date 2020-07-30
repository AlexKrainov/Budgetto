Vue.component("vue-section-component", {
	template: `<div >
				<input type='search'
					class="form-control form-control-sm"
					v-on:input="onsearch"/>
					<div class="inline-spacing" style="height: 200px; overflow-x: overlay;" id='record-sections' >
                      <a 
						href="javascript:void(0)" 
						v-for="section in dataSectionItems"
						v-on:click="$emit('onchoose', section)"
						v-bind:title="section.description"
						v-show="section.isShow"
						v-bind:class="['badge', 'badge-pill', section.cssColor == undefined ? 'badge-default' : '' ]"
						v-bind:style="'box-shadow: 0 0 0 1px '+ section.cssColor +' inset; color: '+ section.cssColor +';background-color: '+ section.cssBackground">
							<i v-bind:class="section.cssIcon"></i>
							{{ section.name }}</a>
	                        
                    </div>
				</div>`,
	props: {
		dataSearchId: String,
		id:String,
		name:String,
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
		new PerfectScrollbar(document.getElementById('record-sections'));
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
		onsearch: function (event) {
			for (var i = 0; i < this.dataSectionItems.length; i++) {
				this.dataSectionItems[i].isShow = this.dataSectionItems[i].name.toLocaleLowerCase().indexOf(event.target.value.toLocaleLowerCase()) >= 0;
			}
		},
	}
});