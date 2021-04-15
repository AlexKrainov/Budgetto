using Microsoft.EntityFrameworkCore;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView.ToDoList;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
using MyProfile.User.Service;
using MyProfile.UserLog.Service;
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
                        .Select(y => new ToDoListModelView
                        {
                            ID = y.ID,
                            FolderID = y.ToDoListFolderID,
                            Title = y.Title,
                            DateEdit = y.DateEdit,
                            DateCreate = y.DateCreate,
                            IsShowInCollective = y.VisibleElement.IsShowInCollective,
                            IsOwner = y.ToDoListFolder.UserID == currentUser.ID,
                            IsFavorite = y.IsFavorite,
                            IsNewToday = y.DateCreate.Date == todate,
                            IsEditToday = y.DateCreate.Date != todate && y.DateEdit.Date == todate,
                            IsShow_BudgetMonth = y.VisibleElement.IsShow_BudgetMonth,
                            IsShow_BudgetYear = y.VisibleElement.IsShow_BudgetYear,
                            Items = y.ToDoListItems
                                .OrderBy(p => p.Order)
                                .Select(z => new ToDoListItemModelView
                                {
                                    DateCreate = z.DateCreate,
                                    DateEdit = z.DateEdit,
                                    ID = z.ID,
                                    IsDone = z.IsDone,
                                    Text = z.Text,
                                    IsOwner = z.OwnerUserID == currentUser.ID,
                                    Order = z.Order
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
                    await userLogService.CreateUserLogAsync(currentUser.UserSessionID, UserLogActionType.ToDoListFolder_Delete);
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        public void HideList(int listID, PeriodTypesEnum periodType)
        {
            var currentUserID = UserInfo.Current.ID;
            var todate = DateTime.Now.Date;

            var list = repository.GetAll<ToDoList>(x => x.ToDoListFolder.UserID == currentUserID && x.ID == listID)
                .FirstOrDefault();

            if (periodType == PeriodTypesEnum.Month)
            {
                list.VisibleElement.IsShow_BudgetMonth = !list.VisibleElement.IsShow_BudgetMonth;
                list.DateEdit = todate;
            }
            if (periodType == PeriodTypesEnum.Year)
            {
                list.VisibleElement.IsShow_BudgetYear = !list.VisibleElement.IsShow_BudgetYear;
                list.DateEdit = todate;
            }
            repository.Update(list, true);
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

        public List<ToDoListModelView> GetListByPeriodTypeID(PeriodTypesEnum periodType)
        {
            var currentUserID = UserInfo.Current.ID;
            var todate = DateTime.Now.Date;

            var lists = repository.GetAll<ToDoList>(x =>
                x.ToDoListFolder.UserID == currentUserID
                && x.IsDeleted == false
                && (periodType == PeriodTypesEnum.Month ? x.VisibleElement.IsShow_BudgetMonth : periodType == PeriodTypesEnum.Year ? x.VisibleElement.IsShow_BudgetYear : false))
                        .OrderBy(x => x.IsFavorite)
                        .Select(y => new ToDoListModelView
                        {
                            ID = y.ID,
                            FolderID = y.ToDoListFolderID,
                            Title = y.Title,
                            IsShowInCollective = y.VisibleElement.IsShowInCollective,
                            IsOwner = y.ToDoListFolder.UserID == currentUserID,
                            IsFavorite = y.IsFavorite,
                            DateEdit = y.DateEdit,
                            DateCreate = y.DateCreate,
                            IsNewToday = y.DateCreate.Date == todate,
                            IsEditToday = y.DateCreate.Date != todate && y.DateEdit.Date == todate,
                            IsShow_BudgetMonth = y.VisibleElement.IsShow_BudgetMonth,
                            IsShow_BudgetYear = y.VisibleElement.IsShow_BudgetYear,

                            Items = y.ToDoListItems
                                .OrderBy(p => p.Order)
                                .Select(z => new ToDoListItemModelView
                                {
                                    DateCreate = z.DateCreate,
                                    DateEdit = z.DateEdit,
                                    ID = z.ID,
                                    IsDone = z.IsDone,
                                    Text = z.Text,
                                    IsOwner = z.OwnerUserID == currentUserID,
                                    Order = z.Order
                                })
                        })
                        .ToList();

            for (int i = 0; i < lists.Count; i++)
            {
                decimal all = lists[i].Items.Count();
                decimal doneItems = lists[i].Items.Where(x => x.IsDone).Count();
                if (all != 0 && doneItems > 0)
                {
                    lists[i].Percent = Math.Round((doneItems / all * 100), 2);
                }
            }

            return lists;
        }

        public async Task<ToDoListModelView> GetListByID(int id)
        {
            var currentUserID = UserInfo.Current.ID;
            var todate = DateTime.Now.Date;

            var list = await repository.GetAll<ToDoList>(x => x.ID == id)
                        .Select(y => new ToDoListModelView
                        {
                            ID = y.ID,
                            FolderID = y.ToDoListFolderID,
                            Title = y.Title,
                            IsShowInCollective = y.VisibleElement.IsShowInCollective,
                            IsOwner = y.ToDoListFolder.UserID == currentUserID,
                            IsFavorite = y.IsFavorite,
                            DateEdit = y.DateEdit,
                            DateCreate = y.DateCreate,
                            IsNewToday = y.DateCreate.Date == todate,
                            IsEditToday = y.DateCreate.Date != todate && y.DateEdit.Date == todate,
                            IsShow_BudgetMonth = y.VisibleElement.IsShow_BudgetMonth,
                            IsShow_BudgetYear = y.VisibleElement.IsShow_BudgetYear,

                            Items = y.ToDoListItems
                                .OrderBy(p => p.Order)
                                .Select(z => new ToDoListItemModelView
                                {
                                    DateCreate = z.DateCreate,
                                    DateEdit = z.DateEdit,
                                    ID = z.ID,
                                    IsDone = z.IsDone,
                                    Text = z.Text,
                                    IsOwner = z.OwnerUserID == currentUserID,
                                    Order = z.Order
                                })
                        })
                        .FirstOrDefaultAsync();

            decimal all = list.Items.Count();
            decimal doneItems = list.Items.Where(x => x.IsDone).Count();
            if (all != 0 && doneItems > 0)
            {
                list.Percent = Math.Round((doneItems / all * 100), 2);
            }

            return list;
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
                    await userLogService.CreateUserLogAsync(currentUser.UserSessionID, UserLogActionType.ToDoListFolder_Create);
                    folder.ID = folderDB.ID;

                }
                else
                {
                    var folderDB = await repository.GetAll<ToDoListFolder>(x => x.ID == folder.ID && x.UserID == currentUser.ID).FirstOrDefaultAsync();

                    folderDB.Title = folder.Title;
                    folderDB.CssIcon = folder.CssIcon;

                    await repository.UpdateAsync(folderDB, true);
                    await userLogService.CreateUserLogAsync(currentUser.UserSessionID, UserLogActionType.ToDoListFolder_Edit);
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> CreateOrUpdateList(ToDoListModelView list)
        {
            var currentUser = UserInfo.Current;
            var now = DateTime.Now.ToUniversalTime();

            try
            {
                if (list.ID == 0)
                {
                    List<ToDoListItem> listItems = new List<ToDoListItem>();

                    foreach (var item in list.Items.OrderBy(x => x.Order))
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
                            Order = item.Order
                        });
                    }

                    ToDoList toDoList = new ToDoList
                    {
                        DateCreate = now,
                        DateEdit = now,
                        Title = list.Title,
                        ToDoListFolderID = list.FolderID,
                        VisibleElement = new Entity.Model.VisibleElement
                        {
                            //IsShowInCollective = list.IsShowInCollective, // currentUser.IsAllowCollectiveBudget,
                            IsShow_BudgetMonth = list.PeriodTypeIDs.Contains((int)PeriodTypesEnum.Month),
                            IsShow_BudgetYear = list.PeriodTypeIDs.Contains((int)PeriodTypesEnum.Year),
                        },
                        ToDoListItems = listItems,
                    };

                    await repository.CreateAsync(toDoList, true);
                    await userLogService.CreateUserLogAsync(currentUser.UserSessionID, UserLogActionType.ToDoListList_Create);

                    list.ID = toDoList.ID;
                }
                else
                {
                    var todoList = await repository.GetAll<ToDoList>(x => x.ID == list.ID).FirstOrDefaultAsync();

                    todoList.Title = list.Title;
                    todoList.ToDoListFolderID = list.FolderID;
                    todoList.IsFavorite = list.IsFavorite;
                    // todoList.VisibleElement.IsShowInCollective = list.IsShowInCollective;
                    todoList.VisibleElement.IsShow_BudgetMonth = list.PeriodTypeIDs.Contains((int)PeriodTypesEnum.Month);
                    todoList.VisibleElement.IsShow_BudgetYear = list.PeriodTypeIDs.Contains((int)PeriodTypesEnum.Year);

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
                                Order = item.Order
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
                                Order = item.Order
                            });
                        }
                    }

                    if (todoList.ToDoListItems.Count() != 0)
                    {
                        await repository.DeleteRangeAsync(todoList.ToDoListItems, true);
                    }

                    todoList.ToDoListItems = items;

                    await repository.UpdateAsync(todoList, true);
                    await userLogService.CreateUserLogAsync(currentUser.UserSessionID, UserLogActionType.ToDoListList_Edit);
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
                    await userLogService.CreateUserLogAsync(currentUser.UserSessionID, UserLogActionType.ToDoListList_Recovery);
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
                await userLogService.CreateUserLogAsync(currentUser.UserSessionID, UserLogActionType.ToDoListList_Delete);
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
