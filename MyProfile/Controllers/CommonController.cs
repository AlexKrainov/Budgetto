using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyProfile.Entity.Repository;
using MyProfile.LittleDictionaries.Service;
using System.Threading.Tasks;

namespace MyProfile.Controllers
{
    [Authorize]
    public class CommonController : Controller
    {
        private IBaseRepository repository;
        private DictionariesService dictionariesService;

        public CommonController(IBaseRepository repository,
            DictionariesService dictionariesService)
        {
            this.repository = repository;
            this.dictionariesService = dictionariesService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetFooterAction()
        {
            var data = dictionariesService.GetTotalActions();
            return Json(new { isOk = true, data = data });
        }

        [HttpGet]
        public async Task<JsonResult> GetCurrenciesInfo()
        {
            var data = await dictionariesService.GetCurrencyInfoForClient();
            return Json(new { isOk = true, data = data });
        }
    }
}