//component version: v1.0

Vue.component("vue-select2-component", {
    template: `<select :id="dataId" :name="dataName" :class="dataClass" :style="dataStyle"  v-model="selected" v-on:click="onchange"></select>`, //
    props: { //transferred variables
        dataId: String,
        //dataName: String,
        dataTitle: String, // for tooltip
        items: Array,
        dataSelectedItems: Array, //set selected items by default, example [{id:3, text: "Name"}]
        selectedPropertyName: String, // property name to show in selected item
        metadata: Array, // placeholder and options, for example [{text: 'User login', isOption: true, isPlaceholder: true, propertyName: 'Login'}]
        url: String, //for ajax query
        //value: Array, // value for v-model from the outside
        value: [Array, Object],
        dataWidth: String,
        dataStyle: {
            type: String,
            default: "width: 100%"
        },
        dataClass: {
            type: String,
            default: "form-control" //form-control crm-crash-list-contract-filter"
        },
        language: {
            type: String
        },
        multiple: { //one or more selected items
            type: Boolean,
            default: true
        },
        minimumInputLength: { //minimum input char for search in server
            type: Number,
            default: 3
        },
        isSaveLocalstorage: { // save or not to localstorage selected items 
            type: Boolean,
            default: true
        },
        postParams: Object,
        childRelatedSelectId: String, //set related element (select2), after change this.select2 child will be empty
        onSelected: [Event, Function],
        onUnselected: [Event, Function],
    },
    watch: {
        ///Проставляем самостоятельно select2 если v-model-объект изменился
        value: function (newValue, oldValue) {
            if ($(this.$el).attr("isUpdated") == true || $(this.$el).attr("isUpdated") == "true") { //Ставим флаг, чтобы не было бесконечного цикла
                $(this.$el).removeAttr("isUpdated");
                return;
            }

            if (newValue != null) {
                if (newValue.id != undefined) { //Если объект
                    if (this.items == undefined) {
                        $(this.$el)
                            .attr("isUpdated", true) //Ставим флаг, чтобы не было бесконечного цикла
                            .find('option')
                            .remove()
                            .end()
                            .append('<option value="' + newValue.id + '">' + (newValue[this.selectedPropertyName] || newValue[i].text || newValue[i].id) + '</option>')
                            .val(newValue.id)
                            .trigger('change');
                    } else {
                        $(this.$el)
                            .attr("isUpdated", true) //Ставим флаг, чтобы не было бесконечного цикла
                            .val(newValue.id)
                            .trigger('change');
                    }
                    return;
                } else if (newValue.length > 0) { //Если массив
                    let options = "";
                    let IDs = [];

                    for (var i = 0; i < newValue.length; i++) {
                        options += '<option value="' + newValue[i].id + '">' + (newValue[i][this.selectedPropertyName] || newValue[i].text || newValue[i].id) + '</option>';
                        IDs.push(newValue[i].id);
                    }

                    if (this.items == undefined) {
                        $(this.$el)
                            .attr("isUpdated", true) //Ставим флаг, чтобы не было бесконечного цикла
                            .find('option')
                            .remove()
                            .end()
                            .append(options)
                            .val(IDs)
                            .trigger('change');
                    } else {
                        $(this.$el)
                            .attr("isUpdated", true) //Ставим флаг, чтобы не было бесконечного цикла
                            .val(IDs)
                            .trigger('change');
                    }
                    return;
                } else {
                    //Очищаем select2 от selected options
                    if (this.items == undefined) {
                        $(this.$el)
                            .attr("isUpdated", true) //Ставим флаг, чтобы не было бесконечного цикла
                            .find('option')
                            .remove()
                            .end()
                            .trigger('change');
                    } else {
                        $(this.$el)
                            .attr("isUpdated", true) //Ставим флаг, чтобы не было бесконечного цикла
                            .trigger('change');
                    }
                    return;
                }
            }
        }
    },
    computed: { //computed properties
        dataName: function () {
            if (this.multiple == true) {
                return this.dataId + "[]";
            } else {
                return this.dataId;
            }
        },
        selectedItems: function () {
            let filter = localStorage.getItem(this.localStorageString);
            if (this.isSaveLocalstorage == true && filter) {
                return JSON.parse(filter); //[this.dataId];
            } else if (this.dataSelectedItems != undefined && this.dataSelectedItems.length > 0 && this.dataSelectedItems[0] != "") {
                return this.dataSelectedItems;
            } else {
                return undefined;
            }
        },
        placeholder: function () {
            let placeholder = "";
            if (this.metadata && this.metadata.length > 0) {
                for (var i = 0; i < this.metadata.length; i++) {
                    if (this.metadata[i].isPlaceholder == true) {
                        placeholder += this.metadata[i].text + ", ";
                    }
                }
                if (placeholder.length > 2) {
                    placeholder = placeholder.substring(0, placeholder.length - 2);
                }
            }
            return placeholder;
        },
        localStorageString: function () { //key for set/get cache of selected items
            return window.location.pathname + "#" + this.dataId;
        }
    },
    data: function () { //method starts before rendering
        //this.isPostBack = true; // чтобы не срабатывало первое событие: change

        return {
            selected: this.value,
            isPostBack: false,

        };
    },
    mounted: function () { //method starts after loading
        let $this = this;

        let select2_obj = $(this.$el).select2({
            ajax: this.items != undefined ? null : {
                url: this.url,
                type: "GET",
                dataType: 'json',
                delay: 350,
                data: function (params) {
                    let searchParams = {
                        searchString: params.term, // search term
                        page: params.page || 1
                    };
                     let resultParams = $.extend(true, $this.postParams, searchParams);
                    //for (var id in $this.postParams) {
                    //    if (searchParams.hasOwnProperty[id] == false) {
                    //        searchParams[id] = [];
                    //    }
                    //    if (typeof ($this.postParams) == "object") {
                    //        searchParams[id] = $this.postParams[id] ? $this.postParams[id].map(x => x.id) : [];
                    //    } else {
                    //        searchParams[id] = $this.postParams[id] ? $this.postParams[id].map(x => x.id) : [];
                    //    }
                    //}

                    for (var prop in resultParams) {// searchParams) { //remove all property with null and undefined
                        if (resultParams[prop] == null || resultParams[prop] == undefined) {
                            delete resultParams[prop];
                        }
                    }
                    return resultParams;//searchParams;
                },
                processResults: function (data, params) {
                    params.page = params.page || 1;

                    return {
                        results: data.items,
                        pagination: {
                            more: (params.page * 20) < data.count
                        }
                    };
                },
                cache: true
            },
            matcher: this.items == undefined ? null : this.customSearch,
            width: this.dataWidth,
            //debug: true,
            //theme: "bootstrap4",
            //dropdownCssClass: this.dataClass,
            //selectionCssClass: this.dataClass,
            data: this.items || this.selectedItems,
            multiple: this.multiple,
            language: this.language,
            placeholder: "", //this.placeholder
            allowClear: true,
            escapeMarkup: function (markup) {
                return markup;
            }, // let our custom formatter work
            minimumInputLength: this.minimumInputLength,
            templateResult: this.templateResult,
            templateSelection: this.templateSelection
        })
            .change(this.changeSelection)
            .on('select2:select', function (e) {
                //$this.$emit("onselect", e);
                if (typeof ($this.onSelected) === "function") {
                    $this.onSelected(e,);
                }
            })
            .on('select2:unselect', function (e) {
                if (typeof ($this.onUnselected) === "function") {
                    $this.onUnselected(e);
                }
            }); //after change selection, save to localstorage new selectedItems

        if (this.selectedItems) {
            select2_obj.val(this.selectedItems ? this.selectedItems.map(x => x.id) : undefined).trigger("change"); //this.selectedItems
        }

        if (this.minimumInputLength < 1) {
            select2_obj.parent().find(".select2-search__field").click(function (event) {
                event.stopPropagation();
                select2_obj.parent().find(".select2-selection__rendered").click();
            });
        }
        if (this.placeholder && this.placeholder.length > 3) {
            $($this.$el.parentElement).after('<span class="text-muted m-b block text-placeholder" style="margin-top:-15px;">' + this.placeholder + '</span>')
        }

        if (!this.selectedItems && this.items) {
            select2_obj.val(null).trigger("change");
        }
    },
    methods: {
        //кастомный поиск, если у select есть options
        customSearch: function (params, option) {
            let search = params.term;
            if (!search) {
                //Если ничего не приходит из строки поиска, выдаем все options
                return option;
            }
            if (params._type && params._type === "query") {
                search = search.replaceAll(" ", "").toLocaleLowerCase();
                for (const prop in option) {
                    if (option.hasOwnProperty(prop) && prop !== "element" && option[prop]) {

                        let value = option[prop].toString().replaceAll(" ", "").toLocaleLowerCase();
                        if (value.indexOf(search) != -1) {
                            return option;
                        }
                    }
                }
            }
            return null;
        },
        templateResult: function (repo) { //show values in option elements
            if (repo.loading == true) {
                return repo.text;
            }

            var markup = "<div class='clearfix select2-result-block-container'>";

            for (var i = 0; i < this.metadata.length; i++) {
                if (this.metadata[i].isLogo) {
                    let logo = repo[this.metadata.filter(x => x.isLogo)[0].propertyName];
                    if (logo) {
                        markup += "<img src='" + logo + "' style='height: 30px;' alt='' class='mr-2'>"
                    }

                } else if (this.metadata[i].isOption == true && repo[this.metadata[i].propertyName]) {
                    markup += "<div class='select2-result-block display-inline'>"
                        + (this.metadata[i].text
                        ? ("<span class='select2-option-title small text-muted'>" + this.metadata[i].text + ":</span> ")
                            : "")
                        + "<span class='select2-option-value'>" + repo[this.metadata[i].propertyName] + "</span></div> <br />";
                }
            }
            markup += "</div>";
            return markup;
        },
        templateSelection: function (repo) { //show value in select input

            if (this.selectedPropertyName != undefined) {
                $(repo.element).attr('selected-property-name', repo[this.selectedPropertyName]);
            }
            return repo[this.selectedPropertyName] || repo.text || repo.id;
        },
        changeSelection: function () { //save to localStorage selected options
            console.log("change");
            // #region serialize selection items
            if (this.isSaveLocalstorage == true && this.isPostBack == false) {
                let data = [];
                let selection_option = $(this.$el).select2("data");
                for (var i = 0; i < selection_option.length; i++) {
                    data.push({
                        id: selection_option[i].id,
                        text: selection_option[i][this.selectedPropertyName] || selection_option[i].text
                    });
                };
                localStorage.setItem(this.localStorageString, JSON.stringify(data));
            }
            // #endregion

            //Не обращать внимание на ошибку-предупреждение
            //Avoid mutating a prop directly since the value will be overwritten whenever the parent component re-renders. Instead, use a data or computed property based on the prop's value. Prop being mutated: "value"
            //this.value = $(this.$el).select2("data");

            //
            //$(this.$el).click();

            this.isPostBack = false;

            if (this.childRelatedSelectId) {
                $("#" + this.childRelatedSelectId).val("").trigger("change");
            }
        },
        onchange: function () {
            this.$emit('input', this.value);
        },
        refresh: function (value) {
            this.value = value;
            $(this.$el).val(this.value ? this.value.map(x => x.id) : this.value).trigger("change");
        },
        //Opportunity add selected option by vue
        //toSelectedOptions = Can be array [{id:1,text: "somthing",...}]
        addSelectedOption: function (toSelectedOptions) {
            let options = '';
            let allOptions = $(this.$el)
                .find("option")
                .map((index, item) => (
                    {
                        id: item.value,
                        text: item.text
                    }))
                .toArray();

            let selectedOptionIDs = $(this.$el)
                .find("option:selected")
                .map((index, item) => (item.value))
                .toArray();

            for (var i = 0; i < toSelectedOptions.length; i++) {
                if (allOptions.length == 0 || selectedOptionIDs.findIndex(x => x == toSelectedOptions[i].id) == -1) {
                    let selectedPropertyName = toSelectedOptions[i][this.selectedPropertyName] || toSelectedOptions[i].text || toSelectedOptions[i].id;
                    if (allOptions.findIndex(x => x.id == toSelectedOptions[i].id) == -1) {
                        options += '<option value="' + toSelectedOptions[i].id + '" selected-property-name="' + selectedPropertyName + '">' + selectedPropertyName + '</option>';
                    }
                    selectedOptionIDs.push(toSelectedOptions[i].id);
                }
            }

            if (options != '') {
                $(this.$el)
                    .append(options)
                    .val(selectedOptionIDs)
                    .trigger("change");
            } else {
                $(this.$el)
                    .val(selectedOptionIDs)
                    .trigger("change");
            }
        }
    }
});
