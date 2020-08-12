var ToDoListVue = new Vue({
    el: "#todo-list-vue",
    data: {
        folders: [{ lists: [{ isDeleted: false, isDoneCount: 0 }] }],

        //Edit list
        list: { isDeleted: false, isDoneCount: 0 },
        text: null,

        //Edit item
        item: null,

        //Edit folder
        folder: { id: null },

        isEdit: false,
        isSaving: false,
        isFolderSaving: false,
        id: -500,
        searchListText: null,
        searchFolderText: null,
    },
    watch: {
        searchFolderText: function (newValue, oldValue) {
            if (newValue == "" || newValue == null || newValue == undefined) {
                for (var i = 0; i < this.folders.length; i++) {
                    this.folders[i].isShow = true;
                }
                return true;
            }

            if (newValue) {
                newValue = newValue.toLocaleLowerCase();
            }

            for (var i = 0; i < this.folders.length; i++) {
                let folder = this.folders[i];

                folder.isShow = folder.title.toLocaleLowerCase().indexOf(newValue) >= 0
                    || folder.lists.findIndex(x => x.title.toLocaleLowerCase().indexOf(newValue) != -1) >= 0;
            }
        },
        searchListText: function (newValue, oldValue) {
            if (newValue == "" || newValue == null || newValue == undefined) {
                for (var i = 0; i < this.folder.lists.length; i++) {
                    this.folder.lists[i].isShow = true;
                }
                return true;
            }

            if (newValue) {
                newValue = newValue.toLocaleLowerCase();
            }

            for (var i = 0; i < this.folder.lists.length; i++) {
                let list = this.folder.lists[i];

                list.isShow = list.title.toLocaleLowerCase().indexOf(newValue) >= 0
                    || list.items.findIndex(x => x.text != null && x.text.toLocaleLowerCase().indexOf(newValue) != -1) >= 0;

            }
        },
    },
    mounted: function () {
        this.load();

        $('.messages-sidebox-toggler').click(function (e) {
            $("a.messages-sidebox-toggler").
                e.preventDefault();
            $('.messages-wrapper, .messages-card').toggleClass('messages-sidebox-open');
        });
    },
    methods: {
        load: function () {
            ShowLoading("#listOfLists");

            return sendAjax("/ToDoList/GetLists", null, "GET")
                .then(function (result) {
                    if (result.isOk == true) {
                        ToDoListVue.folders = result.folders;

                        ToDoListVue.folder = ToDoListVue.folders[0];
                        HideLoading("#listOfLists");
                    }
                });
        },
        selecteFolder: function (folder) {
            this.folder = { ...folder };
            this.isEdit = false;
        },
        selecteList: function (list) {
            this.list = { ...list };
            this.isEdit = true;
        },

        edit: function (list) {
            if (list) {
                this.list = list;
            } else {
                this.list = {
                    items: [],
                    title: null,
                    folderID: this.folder.id,
                    id: 0,
                    isDeleted: false,
                    selected: false,
                    isFavorite: false,
                    isNewToday: true,
                    isEditToday: false,
                    isDoneCount: 0,
                }
            }

            this.isEdit = true;
        },
        toggleFavorite: function (list) {
            if (this.isSaving) {
                return false;
            }
            this.isSaving = true;
            list.isFavorite = !list.isFavorite;

            return $.ajax({
                type: "GET",
                url: "/ToDoList/ToggleFavorite?listID=" + list.id + "&isFavorite=" + list.isFavorite,
                context: {
                    $this: this, list: list
                },
                contentType: "application/json",
                dataType: 'json',
                success: function (response) {

                    if (response.isOk == false) {
                        this.list.isFavorite = !this.list.isFavorite;
                    }
                    this.$this.isSaving = false;
                },
                error: function () {
                    this.$this.isSaving = false;
                }
            });
        },

        saveList: function (isContinuousOnThisList) {
            if (this.checkListValid() == false) {
                return false;
            }

            if (!(this.list.title && this.list.title.length > 0)) {
                this.list.title = this.list.items[0].text;
            }

            this.isSaving = true;

            return $.ajax({
                type: "POST",
                url: "/ToDoList/EditList",
                data: JSON.stringify(this.list),
                context: {
                    $this: this, isContinuousOnThisList: isContinuousOnThisList
                },
                contentType: "application/json",
                dataType: 'json',
                success: function (response) {

                    if (response.isOk) {
                        this.$this.list = response.list;

                        if (this.$this.folder.id == response.list.folderID) {

                            let listIndex = this.$this.folder.lists.findIndex(x => x.id == response.list.id);
                            if (listIndex == -1) {
                                this.$this.folder.lists.push(response.list);
                            } else {
                                this.$this.folder.lists[listIndex] = response.list;
                            }
                        } else {
                            let listIndex = this.$this.folder.lists.findIndex(x => x.id == response.list.id);

                            if (listIndex >= 0) {
                                this.$this.folder.lists.splice(list, 1);

                                let folderIndex = this.$this.folders.findIndex(x => x.id == response.list.folderID);
                                if (folderIndex > 0) {
                                    this.$this.folders[folderIndex].lists.push = response.list;
                                    this.$this.folder = this.$this.folders[folderIndex];
                                } else {
                                    this.$this.load();
                                }
                            }
                        }
                    } else {
                        //ToDo error
                    }

                    this.$this.isEdit = this.isContinuousOnThisList;

                    this.$this.isSaving = false;
                },
                error: function () {
                    this.$this.isSaving = false;
                }
            });
        },
        checkListValid: function () {
            let isOk = true;
            $("#listTitle").removeClass("is-invalid")

            if (!(this.list.title && this.list.title.length > 0) && (this.list.items == undefined || this.list.items.length == 0)) {
                isOk = false;
                $("#listTitle").addClass("is-invalid")
            }

            return isOk;
        },
        removeLists: function () {

            let listsDelete = [];

            for (var i = 0; i < this.folder.lists.length; i++) {
                let list = this.folder.lists[i];
                if (list.selected) {
                    listsDelete.push({ id: list.id, isDeleted: false });
                    list.selected = false;
                    ShowLoading('#list_' + list.id);
                }
            }

            return $.ajax({
                type: "POST",
                url: "/ToDoList/RemoveList",
                data: JSON.stringify(listsDelete),
                context: listsDelete,
                contentType: "application/json",
                dataType: 'json',
                success: function (response) {

                    for (var i = 0; i < response.listIDs.length; i++) {

                        let deletedListID = response.listIDs[i];
                        let index = ToDoListVue.folder.lists.findIndex(x => x.id == deletedListID.id);
                        ToDoListVue.folder.lists[index].isDeleted = deletedListID.isDeleted;

                        HideLoading('#list_' + deletedListID.id);
                    }
                },
                error: function () {
                    for (var i = 0; i < this.length; i++) {

                        HideLoading('#list_' + this[i].id);
                    }
                }
            });
        },
        recovery: function (list) {
            ShowLoading('#list_' + list.id);

            return $.ajax({
                type: "GET",
                url: "/ToDoList/Recovery?listID=" + list.id,
                context: list,
                contentType: "application/json",
                dataType: 'json',
                success: function (response) {
                    this.isDeleted = !response.isOk;
                    HideLoading('#list_' + this.id);
                }
            });
        },

        //Items
        editItem: function (item) {
            this.text = item.text;
            this.item = item;
            $("#input-text").focus();
        },
        addItem: function () {
            if (!this.list.items) {
                this.list.items = [];
            }

            if (!this.text || this.text.length == 0) {
                $("#input-text").addClass("is-invalid");

                return false;
            } else {
                $("#input-text").removeClass("is-invalid");

            }

            if (this.item) {
                let index = this.list.items.findIndex(x => x.id == this.item.id);
                if (index != -1) {
                    this.list.items[index].text = this.text;
                }
            } else {
                this.list.items.push({
                    id: this.id,
                    text: this.text,
                    isDone: false,
                    isDeleted: false,
                });
                this.id++;
            }
            this.text = null;
            this.item = null;
        },
        clearItem: function () {
            if (this.item) {
                this.item = null;
            }
            this.text = null;
        },
        //removeItem: function (item) {
        //    if (item.id > 0) {
        //        item.isDeleted = true;
        //    } else {
        //        let index = this.list.items.findIndex(x => x.id == item.id);
        //        if (index != -1) {
        //            this.list.items.splice(index, 1);
        //        }
        //    }
        //},
        //removeItem: function (item) {
        //    if (item.id > 0) {

        //        return $.ajax({
        //            type: "POST",
        //            url: "/ToDoList/RemoveItem",
        //            data: JSON.stringify(item),
        //            context: this.item,
        //            contentType: "application/json",
        //            dataType: 'json',
        //            success: function (response) {
        //                if (response.isOk) {
        //                    ToDoListVue._removeItem(this);
        //                }
        //            },
        //            error: function () {
        //            }
        //        });
        //    } else {
        //        this._removeItem(item);
        //    }
        //},


        //Folder 
        editFolder: function (isEdit, folderID) {
            //FolderID
            if (isEdit == false) {
                let index = this.folders.findIndex(x => x.id == folderID);
                if (index != -1) {
                    this.folder = { ... this.folders[index] };
                    this.chooseFolderIcon(this.folder.cssIcon);
                    $("#modal-folder").modal("show");
                    return true;
                }
            }
            this.folder = {
                id: 0,
                title: null,
                cssIcon: null,
                lists: [],
                selected: false,
                isShow: true,
                isOwner: true,
            };


            $("#modal-folder").modal("show");
        },
        chooseFolderIcon: function (cssIcon) {
            let newCssIcon = cssIcon.replace(" ", ".");
            $(".folder-icons i").removeClass("active");
            $(".folder-icons i." + newCssIcon).addClass("active");
            this.folder.cssIcon = cssIcon;

        },
        saveFolder: function () {
            //HideLoading('#record_' + reminder.id);

            if (this.checkFolderValid() == false) {
                return false;
            }

            this.isFolderSaving = true;

            return $.ajax({
                type: "POST",
                url: "/ToDoList/EditFolder",
                data: JSON.stringify(this.folder),
                context: this,
                contentType: "application/json",
                dataType: 'json',
                success: function (response) {

                    if (response.isOk) {
                        let index = this.folders.findIndex(x => x.id == response.folder.id);
                        //Check date
                        if (index == -1) {
                            this.folders.push(response.folder);

                            if (this.isEdit && this.list != undefined) {
                                this.folder = response.folder;
                                this.list.folderID = this.folder.id;
                            }
                        } else {
                            this.folders[index].title = response.folder.title;
                            this.folders[index].cssIcon = response.folder.cssIcon;
                        }
                        //BudgetVue.refresh("onlyTable");

                        //this.close();
                        $("#modal-folder").modal("hide");
                    } else {

                    }

                    this.isFolderSaving = false;
                },
                error: function () {
                    this.isFolderSaving = false;

                }
            });
        },
        checkFolderValid: function () {
            let isOk = true;
            $("#folderTitle").removeClass("is-invalid")

            if (!(this.folder.title && this.folder.title.length > 0)) {
                isOk = false;
                $("#folderTitle").addClass("is-invalid")
            }

            return isOk;
        },
        removeFolder: function () {
            this.isFolderSaving = true;
            return $.ajax({
                type: "POST",
                url: "/ToDoList/RemoveFolder",
                data: JSON.stringify(this.folder),
                context: this,
                contentType: "application/json",
                dataType: 'json',
                success: function (response) {

                    if (response.isOk) {
                        let index = this.folders.findIndex(x => x.id == response.folder.id);
                        if (index != -1) {
                            this.folders.splice(index, 1);
                            this.folder = {};
                        }
                        $("#modal-folder").modal("hide");
                    } else {
                        //todo show error
                    }

                    this.isFolderSaving = false;
                },
                error: function () {
                    this.isFolderSaving = false;
                }
            });
        },



        //helpers
        keyPress: function (e) {
            if (e.keyCode == 13) {
                this.addItem();
            }
        },
        getDateByFormat: function (date, format) {
            return GetDateByFormat(date, format);
        },
        click: function (item) {
            item.isDone = !item.isDone;
        },
    }
});

Vue.config.devtools = true;
