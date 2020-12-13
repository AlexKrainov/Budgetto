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


            //$("html, body").animate({
            //    scrollTop: $("#section-vue").offset().top - 75
            //}, 1000);

            SectionVue.areas = this.areas.map(function (x) { return { name: x.name, id: x.id } });

            if (sectionID > 0) {
                SectionVue.edit(area.sections.filter(x => x.id == sectionID)[0]);
            } else if (sectionID == -1) {
                SectionVue.create();
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
            this.area = JSCopyObject(area);

            $("#modal-area").modal("show");
        },
        save: function () {
            if (this.checkForm() == false) {
                return false;
            }

            this.isSaving = true;
            return $.ajax({
                type: "POST",
                url: "/Section/SaveArea",
                data: JSON.stringify(this.area),
                context: this,
                contentType: "application/json",
                dataType: 'json',
                success: function (result) {
                    if (result.isOk) {
                        this.area = result.area;
                        let index = this.areas.findIndex(x => x.id == result.area.id);
                        if (index == -1) {
                            this.areas.push(result.area);
                        } else {
                            this.areas[index] = result.area;
                        }
                        if (SectionVue.areaID == result.area.id) {
                            SectionVue.areaName = result.area.name;
                        }
                        $("#modal-area").modal("hide");
                        this.isSaving = false;
                    }
                }
            });
        },
        checkForm: function (e) {
            let isOk = true;

            if (!(this.area.name && this.area.name.length > 0)) {
                isOk = false;
            }

            let str = this.area.name;
            str = str ? str.replaceAll(" ", "") : "";
            if (str.length == 0) {
                isOk = false;
                $("#area-name").addClass("is-invalid");
            } else {
                $("#area-name").removeClass("is-invalid");
            }

            if (isOk == false && e) {
                e.preventDefault();
            }
            return isOk;
        },
        remove: function (area) {
            if (!area.isGlobal) {
                this.isSaving = true;
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
                        AreaVue.isSaving = false;
                    });
            }
        },
        updateSectionComponent: function () {
            for (var i = 0; i < this.$children.length; i++) {

                var areaID = this.$children[i].dataId.replace("areaid_", "");
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
        colors: [],
    },
    watch: {
        searchIcon: function (newValue, oldValue) {
            if (newValue) {
                newValue = newValue.toLocaleLowerCase();
            }

            for (var i = 0; i < this.icons.length; i++) {
                this.icons[i].isShowOnSite = this.icons[i].nameClass.toLocaleLowerCase().indexOf(newValue) >= 0
                    || this.icons[i].name.toLocaleLowerCase().indexOf(newValue) >= 0;
            }
        },
        searchText: function (newValue, oldValue) {
            if (newValue) {
                newValue = newValue.toLocaleLowerCase();
            }

            for (var i = 0; i < this.sections.length; i++) {
                this.sections[i].isShow_Filtered = this.sections[i].name.toLocaleLowerCase().indexOf(newValue) >= 0
                    || (this.sections[i].description && this.sections[i].description.toLocaleLowerCase().indexOf(newValue) >= 0);
            }
        }
    },
    mounted: function () {
        //because theiaStickySidebar cannot work with dialog inside container
        $("#modal-section").appendTo("#model-section-container");

        $.getJSON("/json/font-awesome.json", function (json) {
            SectionVue.icons = json;
        });
        $.getJSON("/json/colors-section.json", function (json) {
            SectionVue.colors = json;
        });


        jQuery('#section-vue').theiaStickySidebar({
            additionalMarginTop: 70,
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
                canRemove: false
            };
            this.chooseColor();
            $("#accordion2-2, #accordion2-1").removeClass("show");
            this.changeSectionType(2)

            $("#modal-section").modal("show");
        },
        edit: function (section) {
            this.section = JSCopyObject(section);

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

        removeSection: function (section) {
            this.isSaving = true;
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
                    SectionVue.isSaving = false;
                }).then(function () {
                    RecordVue.refreshSections();
                });
        },
        selectIcon: function (item) {
            this.section.cssIcon = item.nameClass;
            //this.searchIcon = '';
            $("#accordion2-2, #accordion2-1").removeClass("show");
        },
        saveSection: function () {
            if (this.checkForm() == false) {
                return false;
            }
            this.isSaving = true;
            return sendAjax("/Section/SaveSection", this.section, "POST")
                .then(function (result) {
                    if (result.isOk && result.section.isSaved) {
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
                        if (result.section.isUpdated) {
                            toastr.success("Категория отредактирована.");
                        } else {
                            toastr.success("Категория создана.");
                        }
                        $("#modal-section").modal("hide");
                        AreaVue.updateSectionComponent();
                    } else {
                        if (result.section.isUpdated) {
                            toastr.error("Ошибка при редактировании категории.");
                        } else {
                            toastr.error("Ошибка при создании категории.");
                        }
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
                $("#section-name").addClass("is-invalid");
            } else {
                $("#section-name").removeClass("is-invalid");
            }

            let str = this.section.name;
            str = str ? str.replaceAll(" ", "") : "";
            if (str.length == 0) {
                isOk = false;
                $("#section-name").addClass("is-invalid");
            } else {
                $("#section-name").removeClass("is-invalid");
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
            this.section.sectionTypeID = val;// (val == this.section.sectionTypeID ? null : val);
        },
        chooseColor: function (cssBackground) {
            for (var i = 0; i < this.colors.length; i++) {
                this.colors[i].selected = false;
            }

            if (cssBackground) {
                let index = this.colors.findIndex(x => x.background == cssBackground);

                if (index != -1) {

                    this.colors[index].selected = true;
                    this.section.cssColor = this.colors[index].color;
                    this.section.cssBackground = this.colors[index].background;
                }

                $("#accordion2-2, #accordion2-1").removeClass("show");
            } else {
                this.section.cssColor = '';
                this.section.cssBackground = '';
            }
        }
    }
});