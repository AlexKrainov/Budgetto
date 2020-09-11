var AreaVue = new Vue({
    el: "#area-vue",
    data: {
        areas: [],
        area: {},

        isSaving: false,
        searchText: null,

    },
    watch: {
        searchText: function (newValue, oldValue) {
            if (newValue) {
                newValue = newValue.toLocaleLowerCase();
            }

            for (var i = 0; i < this.areas.length; i++) {
                this.areas[i].isShow_Filtered = this.areas[i].name.toLocaleLowerCase().indexOf(newValue) >= 0
                    || (this.areas[i].description && this.areas[i].description.toLocaleLowerCase().indexOf(newValue) >= 0);
            }
        }
    },
    mounted: function () {
        this.init();
    },
    methods: {
        init: function () {
            sendAjax("/Section/GetAllSectionForEdit", null, "GET")
                .then(function (result) {
                    if (result.isOk) {
                        AreaVue.areas = result.areas;
                    }
                });
        },
        selectArea: function (areaID, sectionID) {
            document.querySelectorAll("#area-vue .area-item").forEach(function (el, index) {
                el.classList.remove("active-item");
            });

            let area = this.areas.filter(x => x.id == areaID)[0];

            $(".area-item[data-area-id=" + area.id + "]").closest(".area-item").addClass("active-item");

            SectionVue.sections = area.sections;
            SectionVue.areaID = area.id;
            SectionVue.areaName = area.name;


            $("html, body").animate({
                scrollTop: $("#section-vue").offset().top - 75
            }, 1000);

            SectionVue.areas = this.areas.map(function (x) { return { name: x.name, id: x.id } });

            if (sectionID) {
                SectionVue.edit(area.sections.filter(x => x.id == sectionID)[0]);
            }
        },
        create: function () {
            this.area = {
                id: undefined,
                name: this.searchText,
                isShowInCollective: true,
                isShowOnSite: true,
                isShow_Filtered: true,
            };

            $("#modal-area").modal("show");
        },
        edit: function (area) {
            this.area = { ...area };

            $("#modal-area").modal("show");
        },
        save: function () {
            if (this.checkForm() == false) {
                return false;
            }

            this.isSaving = true;
            return sendAjax("/Section/SaveArea", this.area, "POST")
                .then(function (result) {
                    if (result.isOk) {
                        AreaVue.area = result.area;
                        let index = AreaVue.areas.findIndex(x => x.id == result.area.id);
                        if (index == -1) {
                            AreaVue.areas.push(result.area);
                        } else {
                            AreaVue.areas[index] = result.area;
                        }
                        if (SectionVue.areaID == result.area.id) {
                            SectionVue.areaName = result.area.name;
                        }
                        $("#modal-area").modal("hide");
                        AreaVue.isSaving = false;
                    }
                });
        },
        checkForm: function (e) {
            let isOk = true;

            if (!(this.area.name && this.area.name.length > 0)) {
                isOk = false;
            }
            if (isOk == false && e) {
                e.preventDefault();
            }
            return isOk;
        },
        remove: function (area) {
            if (!area.isGlobal) {
                sendAjax("/Section/RemoveArea?id=" + area.id, null, "POST")
                    .then(function (result) {
                        if (result.isOk && result.wasDeleted) {
                            toastr.success(result.text);
                            let index = AreaVue.areas.findIndex(x => x.id == result.id);
                            if (index != -1) {
                                AreaVue.areas.splice(index, 1);
                            }
                        } else if (result.isOk && result.wasDeleted == false) {
                            toastr.warning(result.text);
                        } else {
                            toastr.error(result.text);
                        }
                    });
            }
        },
        updateSectionComponent: function () {
            for (var i = 0; i < this.$children.length; i++) {
                let areaID = this.$children[i].dataId.replace("areaid_", "");

                let areaIndex = this.areas.findIndex(x => x.id == areaID);
                let childrenIndex = this.$children[i].updateSections(this.areas[areaIndex].sections);
            }
        },
        onChooseSection: function (section) {
            this.selectArea(section.areaID, section.id);
        }
    }
});

var SectionVue = new Vue({
    el: "#section-vue",
    data: {
        sections: [],
        section: {},

        areaID: null,
        areaName: null,
        areas: [], //for select

        icons: [],
        searchIcon: '',
        searchText: null,
        isSaving: false,

        //https://materializecss.com/color.html
        colors: [
            {
                id: 1,
                background: '#ffebee',
                color: "rgba(24,28,33,0.8)",
                border: '',
                selected: false,
            }, {
                id: 2,
                background: '#ffcdd2',
                color: "rgba(24,28,33,0.8)",
                border: '',
                selected: false,
            }, {
                id: 3,
                background: '#ef9a9a',
                color: "rgba(24,28,33,0.8)",
                border: '',
                selected: false,
            },
            {
                id: 4,
                background: '#f44336',
                color: "#fff",
                border: '',
                selected: false,
            },
            {
                id: 5,
                background: '#fbe9e7',
                color: "rgba(24,28,33,0.8)",
                border: '',
                selected: false,
            },
            {
                id: 6,
                background: '#ffccbc',
                color: "rgba(24,28,33,0.8)",
                border: '',
                selected: false,
            },
            {
                id: 7,
                background: '#ffab91',
                color: "rgba(24,28,33,0.8)",
                border: '',
                selected: false,
            },
            {
                id: 8,
                background: '#ff8a65',
                color: "#fff",
                border: '',
                selected: false,
            },


            //Yellow
            {
                id: 9,
                background: '#ffe0b2',
                color: "rgba(24,28,33,0.8)",
                border: '',
                selected: false,
            }, {
                id: 10,
                background: '#ffcc80',
                color: "rgba(24,28,33,0.8)",
                border: '',
                selected: false,
            },
            {
                id: 11,
                background: '#ffb74d',
                color: "rgba(24,28,33,0.8)",
                border: '',
                selected: false,
            },
            {
                id: 12,
                background: '#ffa726',
                color: "#fff",
                border: '',
                selected: false,
            },

            //gray
            {
                id: 13,
                background: '#eeeeee',
                color: "rgba(24,28,33,0.8)",
                border: '',
                selected: false,
            },
            {
                id: 14,
                background: '#e0e0e0',
                color: "rgba(24,28,33,0.8)",
                border: '',
                selected: false,
            },
            {
                id: 15,
                background: '#bdbdbd',
                color: "rgba(24,28,33,0.8)",
                border: '',
                selected: false,
            },
            {
                id: 16,
                background: '#9e9e9e',
                color: "#fff",
                border: '',
                selected: false,
            },

            //green

            {
                id: 17,
                background: '#b9f6ca',
                color: "rgba(24,28,33,0.8)",
                border: '',
                selected: false,
            },
            {
                id: 18,
                background: '#69f0ae',
                color: "rgba(24,28,33,0.8)",
                border: '',
                selected: false,
            },
            {
                id: 19,
                background: '#00e676',
                color: "rgba(24,28,33,0.8)",
                border: '',
                selected: false,
            },
            {
                id: 20,
                background: '#00c853',
                color: "#fff",
                border: '',
                selected: false,
            },

            //blue
            {
                id: 21,
                background: '#e3f2fd',
                color: "rgba(24,28,33,0.8)",
                border: '',
                selected: false,
            },
            {
                id: 22,
                background: '#bbdefb',
                color: "rgba(24,28,33,0.8)",
                border: '',
                selected: false,
            },
            {
                id: 23,
                background: '#90caf9',
                color: "rgba(24,28,33,0.8)",
                border: '',
                selected: false,
            },
            {
                id: 24,
                background: '#2196f3',
                color: "#fff",
                border: '',
                selected: false,
            }],
    },
    watch: {
        searchIcon: function (newValue, oldValue) {
            if (newValue) {
                newValue = newValue.toLocaleLowerCase();
            }

            for (var i = 0; i < this.icons.length; i++) {
                this.icons[i].isShowOnSite = this.icons[i].nameClass.toLocaleLowerCase().indexOf(newValue) >= 0 || this.icons[i].name.toLocaleLowerCase().indexOf(newValue) >= 0;
            }
        },
        searchText: function (newValue, oldValue) {
            if (newValue) {
                newValue = newValue.toLocaleLowerCase();
            }

            for (var i = 0; i < this.areas.length; i++) {
                this.areas[i].isShow_Filtered = this.sections[i].name.toLocaleLowerCase().indexOf(newValue) >= 0
                    || (this.sections[i].description && this.sections[i].description.toLocaleLowerCase().indexOf(newValue) >= 0);
            }
        }
    },
    mounted: function () {
        $.getJSON("/json/font-awesome.json", function () { })
            .done(function (json) {
                SectionVue.icons = json;
            });
    },
    methods: {
        create: function () {
            this.section = {
                name: this.searchText,
                areaID: this.areaID,
                isShowInCollective: true,
                isShowOnSite: true,
                isShow_Filtered: true,
                sectionTypeID: null,
                hasRecords: false,
                cssIcon: null,
            };
            this.chooseColor();
            $("#accordion2-2, #accordion2-1").removeClass("show");
            this.changeSectionType(2)

            $("#modal-section").modal("show");
        },
        edit: function (section) {
            this.section = { ...section };

            $("#collectiveSections").select2();

            this.chooseColor(this.section.cssBackground);
            $("#accordion2-2, #accordion2-1").removeClass("show");

            //$("#cssColor").colorPick({
            //    'initialColor': this.section.cssColor ? this.section.cssColor : "#E5E9EB",
            //    //'palette': ['#5BADFF', "#F06568", "#3C9E71","#C6C6C6"],
            //    //'localizationColor': [],
            //    'onColorSelected': function () {
            //        //let title = '';
            //        //let localColor = this.localizationColor.find(x => x.ColorCode.toUpperCase() == this.color.toUpperCase());
            //        //if (localColor && localColor.ColorName) {
            //        //title = localColor.ColorName;
            //        SectionVue.section.cssColor = this.color;
            //        //$("#carColorLocal").text(title);
            //        //this.element.attr("title", title);
            //        this.element.css({ 'backgroundColor': this.color, 'color': this.color });
            //        //}
            //    }
            //});

            $("#modal-section").modal("show");
        },

        remove: function (section) {
            sendAjax("/Section/RemoveSection?id=" + section.id, null, "POST")
                .then(function (result) {
                    if (result.isOk && result.wasDeleted) {
                        toastr.success(result.text);
                        let index = SectionVue.sections.findIndex(x => x.id == result.id);
                        if (index != -1) {
                            SectionVue.sections.splice(index, 1);
                        }
                    } else if (result.isOk && result.wasDeleted == false) {
                        toastr.warning(result.text);
                    } else {
                        toastr.error(result.text);
                    }
                }).then(function () {
                    RecordVue.refreshSections();
                });
        },
        selectIcon: function (item) {
            this.section.cssIcon = item.nameClass;
            //this.searchIcon = '';
            $("#accordion2-2, #accordion2-1").removeClass("show");
        },
        save: function () {
            if (this.checkForm() == false) {
                return false;
            }
            this.isSaving = true;
            return sendAjax("/Section/SaveSection", this.section, "POST")
                .then(function (result) {
                    if (result.isOk) {
                        if (result.section.areaID == SectionVue.areaID) {

                            SectionVue.section = result.section;
                            let index = SectionVue.sections.findIndex(x => x.id == result.section.id);
                            let areaIndex = AreaVue.areas.findIndex(x => x.id == SectionVue.areaID)

                            if (index == -1) {
                                //SectionVue.sections.push(result.section);
                                AreaVue.areas[areaIndex].sections.push(result.section);
                            } else {
                                // SectionVue.sections[index] = result.section;
                                let sectionIndex = AreaVue.areas[areaIndex].sections.findIndex(x => x.id == result.section.id);
                                AreaVue.areas[areaIndex].sections[sectionIndex] = result.section;
                            }
                        } else {
                            let index = SectionVue.sections.findIndex(x => x.id == result.section.id);
                            let removeAreaIndex = AreaVue.areas.findIndex(x => x.id == SectionVue.areaID)

                            if (index != -1) {
                                SectionVue.sections.splice(index, 1);
                                let removeAreaSectionIndex = AreaVue.areas[removeAreaIndex].sections.findIndex(x => x.id == result.section.id);
                                AreaVue.areas[removeAreaIndex].sections.splice(removeAreaSectionIndex, 1);

                                let newAreaIndex = AreaVue.areas.findIndex(x => x.id == result.section.areaID);
                                AreaVue.areas[newAreaIndex].sections.push(result.section);

                            }
                        }
                        //AreaVue.$forceUpdate();//bug areaVuew doesn't update section in a view
                        $("#modal-section").modal("hide");
                        AreaVue.updateSectionComponent();
                    }
                    SectionVue.isSaving = false;
                })
                .then(function () {
                    RecordVue.refreshSections();
                });
        },
        checkForm: function (e) {
            let isOk = true;

            if (!(this.section.name && this.section.name.length > 0)) {
                isOk = false;
            }
            if (isOk == false && e) {
                e.preventDefault();
            }
            return isOk;
        },
        changeArea: function (event) {
            this.section.areaID = event.target.selectedOptions[0].value;
            this.section.areaName = event.target.selectedOptions[0].text;
        },
        changeSectionType: function (val) {
            this.section.sectionTypeID = (val == this.section.sectionTypeID ? null : val);
        },
        chooseColor: function (cssBackground) {
            for (var i = 0; i < this.colors.length; i++) {
                this.colors[i].selected = false;
            }

            if (cssBackground) {
                let index = this.colors.findIndex(x => x.background == cssBackground);

                this.colors[index].selected = true;
                this.section.cssColor = this.colors[index].color;
                this.section.cssBackground = this.colors[index].background;

                $("#accordion2-2, #accordion2-1").removeClass("show");
            } else {
                this.section.cssColor = '';
                this.section.cssBackground = '';
            }
        }
    }
});