using Microsoft.AspNetCore.Mvc;
using MyProfile.Entity.ModelView.ToDoList;
using MyProfile.ToDoList.Service;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyProfile.Controllers
{
    public class ToDoListController : Controller
    {
        private ToDoListService toDoListService;

        public ToDoListController(ToDoListService toDoListService)
        {
            this.toDoListService = toDoListService;
        }

        public IActionResult List()
        {

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
        public async Task<IActionResult> EditList([FromBody] ToDoFolderList list)
        {
            var result = await toDoListService.CreateOrUpdateList(list);

            return Json(new { isOk = result, list });
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
    }
}
