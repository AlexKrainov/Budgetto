Vue.component("vue-section-component", {
    template: `
        <div v-bind:id="dataId" >
            <input type='search'
                    name="sectionSearch"
                    v-model="searchText"
                    class="form-control form-control-sm"
                    v-on:input="onsearch" 
                    v-show="isShowFilter"/>
            <div class="cards" v-bind:style="searchText ? '' : dataRecordsStyle"
                v-bind:class="dataClass">
                <div class="card-section card"
                     v-for="section in sections"
                     v-bind:key="section.id"
                     v-on:click="onSelect(section)"
                     v-bind:title="section.description"
                     v-show="section.isShow"
                     v-bind:style="'color: '+ section.cssColor +';background-color: '+ section.cssBackground"
                     v-bind:class="[section.isSelected  ? 'selected-section' : 'not-selected-section', dataSectionClasses ]">
                    <span class="selected-section-count">{{ checkSelected(section) }}</span>
                    <div class="cards-container card-body d-flex align-items-center ">
                        <i class="icon-large opacity-75" v-bind:class="section.cssIcon"></i>
                        <div class="card-section-text ml-2">
                            <div class="section-name">{{section.name}}</div>
                            <div class="area-name opacity-75" style="margin-top: -5px;">
                                {{ section.areaName }}
                                <div class="ml-2" style="display: inline-block;" 
                                    v-show="section.collectiveSections && section.collectiveSections.length > 0">
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
        name: {
            type: String,
            default: "section-component"
        },
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
        dataSelectedItems: Array,//[{ id:9, count:1 }]
        dataIsSelection: {
            type: Boolean,
            default: false
        },
        dataSectionClasses: {
            type: String,
            default: "cursor-pointer"
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
        },
        checkSelected: function (section) {
            if (this.dataSelectedItems && this.dataSelectedItems.length > 0) {
                let p = this.dataSelectedItems.filter(x => x.id == section.id);
                if (p && p.length > 0) {
                    return p[0].count;
                }
            }
            return "";
        },
        clearSearchTextValue: function () {
            this.searchText = null;

            for (var i = 0; i < this.sections.length; i++) {
                this.sections[i].isShow = true;
            }

        },
        onSelect: function (section) {
            if (this.dataIsSelection) {
                section.isSelected = !section.isSelected;

            }
            this.$emit('onchoose', section);
        },
        selectAllSections: function () {
            for (var i = 0; i < this.sections.length; i++) {
                if (this.sections[i].isSelected == false) {
                    this.onSelect(this.sections[i]);
                }
                //this.sections[i].isSelected = true;
            }
        }
    }
});