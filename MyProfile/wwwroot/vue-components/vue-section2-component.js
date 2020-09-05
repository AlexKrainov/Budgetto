Vue.component("vue-section-component", {
    template: `
<div v-bind:id="dataId" >
    <input type='search'
           class="form-control form-control-sm"
           v-on:input="onsearch" 
            v-model="searchText"
            v-show="isShowFilter"/>
    <div class="cards" v-bind:style="searchText ? '' : dataRecordsStyle"
        v-bind:class="dataClass">
        <div class="card-section card cursor-pointer"
             v-for="section in sections"
             v-on:click="$emit('onchoose', section)"
             v-bind:title="section.description"
             v-show="section.isShow"
             v-bind:style="'color: '+ section.cssColor +';background-color: '+ section.cssBackground">
            <div class="cards-container card-body d-flex align-items-center ">
                <i class="icon-large opacity-75" v-bind:class="section.cssIcon"></i>
                <div class="card-section-text ml-2">
                    <div class="section-name">{{section.name}}</div>
                    <div class="area-name opacity-75" style="margin-top: -5px;">
                        {{ section.areaName }}
                        <div class="ml-2" style="display: inline-block;" v-show="section.collectiveSections.length > 0">
                            <i class="oi oi-layers"></i> +{{section.collectiveSections.length}}
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>`,
    props: {
        dataSearchId: String,
        dataId: String,
        name: String,
        onchoose: Event,
        isShowFilter: {
            type: Boolean,
            default: false,
        },
        dataItems: Array,//[{"id":9,"name":"Расходы (продукты)","description":"test groceries","cssIcon":"fas fa-shopping-cart","cssColor":"rgba(24,28,33,0.8)","areaID":5,"areaName":"Основные раходы","isUpdated":false,"collectiveSections":[],"sectionTypeID":null,"sectionTypeName":null,"recordCount":139,"isShow":true,"hasRecords":false,"cssBackground":"#ffab91"}]
        dataRecordsStyle: {
            type: String,
            default: ""
        },
        dataClass: {
            type: String,
            default: "cards-small" //   cards-small/cards-medium/cards-big
        },
    },
    //computed: {
    //},
    data: function () {
        return {
            count: 0,
            sections: [],
            searchText: null,
        }
    },
    watch: {
        dataItems: function (newValue) {
            console.log("watch");
            this.updateSections();
        }
    },
    mounted: function () {
        new PerfectScrollbar(document.getElementsByClassName('cards')[0]);

        if (this.dataItems == undefined) {
            this.load();
        } else {
            this.updateSections();
        }
    },
    methods: {
        load: function () {
            return $.ajax({
                type: "GET",
                url: "/Section/GetSectins",
                data: null,
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                context: this,
                success: function (result) {
                    if (result.isOk) {
                        this.sections = result.sections;
                    }
                    return result;
                },
                error: function (xhr, status, error) {
                    console.log(error);
                }
            }, this);
        },
        onsearch: function (event) {
            let value = event.target.value.toLocaleLowerCase();

            for (var i = 0; i < this.sections.length; i++) {
                this.sections[i].isShow = this.sections[i].name.toLocaleLowerCase().indexOf(value) >= 0;
            }
        },
        updateSections: function (newItems) {
            if (newItems) {
                this.sections = newItems;
            } else {
                this.sections = this.dataItems;
            }
            this.$forceUpdate();
        }
    }
});