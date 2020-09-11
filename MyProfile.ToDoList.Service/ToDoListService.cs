using Microsoft.EntityFrameworkCore;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView.ToDoList;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
using MyProfile.User.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyProfile.ToDoList.Service
{
    using ToDoList = MyProfile.Entity.Model.ToDoList;
    using ToDoListFolder = MyProfile.Entity.Model.ToDoListFolder;

    public class ToDoListService
    {
        private IBaseRepository repository;
        private UserLogService userLogService;

        public ToDoListService(IBaseRepository baseRepository,
            UserLogService userLogService)
        {
            this.repository = baseRepository;
            this.userLogService = userLogService;
        }

        public async Task<List<FolderListModelView>> GetListFolders()
        {
            var currentUser = UserInfo.Current;
            var todate = DateTime.Now.Date;

            var folders = await repository.GetAll<ToDoListFolder>(x => x.UserID == currentUser.ID)
                .Select(x => new FolderListModelView
                {
                    ID = x.ID,
                    Title = x.Title,
                    CssIcon = x.CssIcon,
                    Lists = x.ToDoLists
                        .Where(y => y.IsDeleted == false)
                        .OrderByDescending(z => z.IsFavorite)
                        .ThenByDescending(z => z.DateEdit)
                        .Select(y => new ToDoFolderList
                        {
                            ID = y.ID,
                            FolderID = y.ToDoListFolderID,
                            PeriodTypeID = y.PeriodTypeID,
                            Title = y.Title,
                            DateEdit = y.DateEdit,
                            DateCreate = y.DateCreate,
                            IsShowInCollective = y.VisibleElement.IsShowInCollective,
                            IsOwner = y.ToDoListFolder.UserID == currentUser.ID,
                            IsFavorite = y.IsFavorite,
                            IsNewToday = y.DateCreate.Date == todate,
                            IsEditToday = y.DateCreate.Date != todate && y.DateEdit.Date == todate,
                            Items = y.ToDoListItems
                                .OrderByDescending(p => p.IsDone)
                                .ThenBy(p => p.DateCreate)
                                .Select(z => new ToDoListItemModelView
                                {
                                    DateCreate = z.DateCreate,
                                    DateEdit = z.DateEdit,
                                    ID = z.ID,
                                    IsDone = z.IsDone,
                                    Text = z.Text,
                                    IsOwner = z.OwnerUserID == currentUser.ID,
                                })
                        })

                })
                .ToListAsync();

            return folders;
        }

        public async Task<bool> RemoveFolder(FolderListModelView folder)
        {
            var currentUser = UserInfo.Current;

            try
            {
                var folderDB = await repository.GetAll<ToDoListFolder>(x => x.UserID == currentUser.ID && x.ID == folder.ID)
                    .FirstOrDefaultAsync();

                if (folderDB != null)
                {
                    var visibleElementIDs = folderDB.ToDoLists.Select(x => x.VisibleElementID).ToList();

                    await repository.DeleteAsync<ToDoListFolder>(folderDB, true);

                    foreach (var visibleElementID in visibleElementIDs)
                    {
                        await repository.DeleteAsync<VisibleElement>(visibleElementID);
                    }
                    await repository.SaveAsync();
                    await userLogService.CreateUserLog(currentUser.UserSessionID, UserLogActionType.ToDoListFolder_Delete);
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> ToggleFavorite(int listID, bool isFavorite)
        {
            var currentUserID = UserInfo.Current.ID;

            try
            {
                var list = await repository.GetAll<ToDoList>(x => x.ToDoListFolder.UserID == currentUserID && x.ID == listID).FirstOrDefaultAsync();
                list.IsFavorite = isFavorite;
                await repository.UpdateAsync(list, true);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public async Task<ToDoFolderList> GetListByID(int id)
        {
            var currentUserID = UserInfo.Current.ID;
            var todate = DateTime.Now.Date;

            return await repository.GetAll<ToDoList>(x => x.ID == id)
                        .Select(y => new ToDoFolderList
                        {
                            ID = y.ID,
                            FolderID = y.ToDoListFolderID,
                            PeriodTypeID = y.PeriodTypeID,
                            Title = y.Title,
                            IsShowInCollective = y.VisibleElement.IsShowInCollective,
                            IsOwner = y.ToDoListFolder.UserID == currentUserID,
                            IsFavorite = y.IsFavorite,
                            DateEdit = y.DateEdit,
                            DateCreate = y.DateCreate,
                            IsNewToday = y.DateCreate.Date == todate,
                            IsEditToday = y.DateCreate.Date != todate && y.DateEdit.Date == todate,
                            Items = y.ToDoListItems
                                .OrderByDescending(p => p.IsDone)
                                .ThenBy(p => p.DateEdit)
                                .Select(z => new ToDoListItemModelView
                                {
                                    DateCreate = z.DateCreate,
                                    DateEdit = z.DateEdit,
                                    ID = z.ID,
                                    IsDone = z.IsDone,
                                    Text = z.Text,
                                    IsOwner = z.OwnerUserID == currentUserID,
                                })
                        })
                        .FirstOrDefaultAsync();
        }

        public async Task<bool> CreateOrUpdateFolder(FolderListModelView folder)
        {
            var currentUser = UserInfo.Current;

            try
            {
                if (folder.ID == 0)
                {
                    var folderDB = new ToDoListFolder
                    {
                        Title = folder.Title,
                        CssIcon = folder.CssIcon,
                        UserID = currentUser.ID,
                    };
                    await repository.CreateAsync(folderDB, true);
                    await userLogService.CreateUserLog(currentUser.UserSessionID, UserLogActionType.ToDoListFolder_Create);
                    folder.ID = folderDB.ID;

                }
                else
                {
                    var folderDB = await repository.GetAll<ToDoListFolder>(x => x.ID == folder.ID && x.UserID == currentUser.ID).FirstOrDefaultAsync();

                    folderDB.Title = folder.Title;
                    folderDB.CssIcon = folder.CssIcon;

                    await repository.UpdateAsync(folderDB, true);
                    await userLogService.CreateUserLog(currentUser.UserSessionID, UserLogActionType.ToDoListFolder_Edit);
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> CreateOrUpdateList(ToDoFolderList list)
        {
            var currentUser = UserInfo.Current;
            var now = DateTime.Now.ToUniversalTime();

            try
            {
                if (list.ID == 0)
                {
                    List<ToDoListItem> listItems = new List<ToDoListItem>();

                    foreach (var item in list.Items)
                    {
                        Guid? doneUserID = null;

                        if (item.IsDone)
                        {
                            doneUserID = currentUser.ID;
                        }

                        listItems.Add(new ToDoListItem
                        {
                            DateCreate = now,
                            DateEdit = now,
                            DoneUserID = doneUserID,
                            OwnerUserID = currentUser.ID,
                            IsDone = item.IsDone,
                            Text = item.Text,
                        });
                    }

                    ToDoList toDoList = new ToDoList
                    {
                        DateCreate = now,
                        DateEdit = now,
                        PeriodTypeID = 1,
                        Title = list.Title,
                        ToDoListFolderID = list.FolderID,
                        VisibleElement = new Entity.Model.VisibleElement
                        {
                            IsShowInCollective = list.IsShowInCollective, // currentUser.IsAllowCollectiveBudget,
                            IsShow_BudgetMonth = list.PeriodTypeID == (int)PeriodTypesEnum.Month,
                            IsShow_BudgetYear = list.PeriodTypeID == (int)PeriodTypesEnum.Year,
                        },
                        ToDoListItems = listItems,
                    };

                    await repository.CreateAsync(toDoList, true);
                    await userLogService.CreateUserLog(currentUser.UserSessionID, UserLogActionType.ToDoListList_Create);

                    list.ID = toDoList.ID;
                }
                else
                {
                    var todoList = await repository.GetAll<ToDoList>(x => x.ID == list.ID).FirstOrDefaultAsync();

                    todoList.PeriodTypeID = list.PeriodTypeID;
                    todoList.Title = list.Title;
                    todoList.ToDoListFolderID = list.FolderID;
                    todoList.IsFavorite = list.IsFavorite;
                    todoList.VisibleElement.IsShowInCollective = list.IsShowInCollective;
                    todoList.VisibleElement.IsShow_BudgetMonth = list.PeriodTypeID == (int)PeriodTypesEnum.Month;
                    todoList.VisibleElement.IsShow_BudgetYear = list.PeriodTypeID == (int)PeriodTypesEnum.Year;

                    List<ToDoListItem> items = new List<ToDoListItem>();

                    foreach (var item in list.Items.Where(x => x.IsDeleted == false))
                    {
                        var itemDB = todoList.ToDoListItems.FirstOrDefault(x => x.ID == item.ID);
                        Guid? doneUserID = null;

                        if (item.IsDone)
                        {
                            doneUserID = currentUser.ID;
                        }

                        if (itemDB == null)
                        {
                            items.Add(new ToDoListItem
                            {
                                DateCreate = now,
                                DateEdit = now,
                                DoneUserID = doneUserID,
                                IsDone = item.IsDone,
                                OwnerUserID = currentUser.ID,
                                Text = item.Text,
                                ToDoListID = todoList.ID,
                            });
                        }
                        else
                        {
                            items.Add(new ToDoListItem
                            {
                                DateCreate = itemDB.DateCreate,
                                DateEdit = itemDB.IsDone != item.IsDone ? now : itemDB.DateEdit,
                                DoneUserID = doneUserID,
                                IsDone = item.IsDone,
                                OwnerUserID = itemDB.OwnerUserID,
                                Text = item.Text,
                                ToDoListID = todoList.ID,
                            });
                        }
                    }

                    if (todoList.ToDoListItems.Count() != 0)
                    {
                        await repository.DeleteRangeAsync(todoList.ToDoListItems, true);
                    }

                    todoList.ToDoListItems = items;

                    await repository.UpdateAsync(todoList, true);
                    await userLogService.CreateUserLog(currentUser.UserSessionID, UserLogActionType.ToDoListList_Edit);
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        #region Remove and recovery
        public async Task<bool> RemoveItem(ToDoListItemModelView item)
        {
            var currentID = UserInfo.Current.ID;

            try
            {
                var itemDB = await repository.GetAll<ToDoListItem>(x => x.OwnerUserID == currentID && x.ID == item.ID).FirstOrDefaultAsync();

                if (itemDB != null)
                {
                    await repository.DeleteAsync(itemDB, true);
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> Recovery(int listID)
        {
            var currentUser = UserInfo.Current;
            var now = DateTime.Now.ToUniversalTime();

            try
            {
                var listDB = await repository.GetAll<ToDoList>(x => x.ID == listID && x.ToDoListFolder.UserID == currentUser.ID).FirstOrDefaultAsync();

                if (listDB != null)
                {
                    listDB.IsDeleted = false;
                    listDB.DateEdit = now;

                    await repository.UpdateAsync(listDB, true);
                    await userLogService.CreateUserLog(currentUser.UserSessionID, UserLogActionType.ToDoListList_Recovery);
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> RemoveListsByIDs(List<ItemDelete> listIDs)
        {
            var currentUser = UserInfo.Current;
            var now = DateTime.Now.ToUniversalTime();
            var onlyIDs = listIDs.Select(x => x.ID);

            var lists = await repository.GetAll<ToDoList>(x => x.ToDoListFolder.UserID == currentUser.ID && onlyIDs.Contains(x.ID)).ToListAsync();

            try
            {
                for (int i = 0; i < lists.Count; i++)
                {
                    lists[i].IsDeleted = true;
                    lists[i].DateEdit = now;
                }
                await repository.SaveAsync();

                for (int i = 0; i < listIDs.Count; i++)
                {
                    listIDs[i].IsDeleted = true;
                }
                await userLogService.CreateUserLog(currentUser.UserSessionID, UserLogActionType.ToDoListList_Delete);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        #endregion
    }
}
