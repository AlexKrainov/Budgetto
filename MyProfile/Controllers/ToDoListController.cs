using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView.ToDoList;
using MyProfile.Identity;
using MyProfile.ToDoList.Service;
using MyProfile.User.Service;
using MyProfile.UserLog.Service;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyProfile.Controllers
{
    [Authorize]
    public class ToDoListController : Controller
    {
        private ToDoListService toDoListService;
        private UserLogService userLogService;

        public ToDoListController(ToDoListService toDoListService, UserLogService userLogService)
        {
            this.toDoListService = toDoListService;
            this.userLogService = userLogService;
        }

        public async Task<IActionResult> List()
        {
            await userLogService.CreateUserLogAsync(UserInfo.Current.UserSessionID, UserLogActionType.ToDoLists_Page);

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetLists()
        {
            var folders = await toDoListService.GetListFolders();

            return Json(new { isOk = true, folders });
        }

        [HttpPost]
        public async Task<IActionResult> EditFolder([FromBody] FolderListModelView folder)
        {
            var result = await toDoListService.CreateOrUpdateFolder(folder);

            return Json(new { isOk = result, folder });
        }


        [HttpPost]
        public async Task<IActionResult> EditList([FromBody] ToDoListModelView list)
        {
            var result = await toDoListService.CreateOrUpdateList(list);

            return Json(new { isOk = result, list = await toDoListService.GetListByID(list.ID) });
        }


        [HttpPost]
        public async Task<IActionResult> RemoveList([FromBody] List<ItemDelete> listIDs)
        {
            var result = await toDoListService.RemoveListsByIDs(listIDs);

            return Json(new { isOk = result, listIDs });
        }
        [HttpPost]
        public async Task<IActionResult> RemoveFolder([FromBody] FolderListModelView folder)
        {
            var result = await toDoListService.RemoveFolder(folder);

            return Json(new { isOk = result, folder });
        }

        //[HttpPost]
        //public async Task<IActionResult> RemoveItem([FromBody] ToDoListItemModelView item)
        //{
        //    var result = await toDoListService.RemoveItem(item);

        //    return Json(new { isOk = result, item });
        //}

        [HttpGet]
        public async Task<IActionResult> Recovery(int listID)
        {
            var result = await toDoListService.Recovery(listID);

            return Json(new { isOk = result, listID });
        }

        [HttpGet]
        public async Task<IActionResult> ToggleFavorite(int listID, bool isFavorite)
        {
            var result = await toDoListService.ToggleFavorite(listID, isFavorite);

            return Json(new { isOk = result, listID, isFavorite });
        }

        [HttpGet]
        public JsonResult GetListsByPeriodType(PeriodTypesEnum periodType)
        {
            return Json(new { isOk = true, lists = toDoListService.GetListByPeriodTypeID(periodType)});
        }
        
        [HttpGet]
        public JsonResult HideList(int listID , PeriodTypesEnum periodType)
        {
            toDoListService.HideList(listID, periodType);
            return Json(new { isOk = true});
        }

    }
}
