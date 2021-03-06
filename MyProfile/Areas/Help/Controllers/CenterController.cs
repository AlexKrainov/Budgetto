using Microsoft.AspNetCore.Mvc;
using MyProfile.HelpCenter.Service;
using MyProfile.Identity;
using MyProfile.UserLog.Service;
using System.Threading.Tasks;

namespace MyProfile.Areas.Help.Controllers
{
    //[Authorize]
    [Area("Help")]
    public class CenterController : Controller
    {
        private HelpCenterService helpCenterService;
        private UserLogService userLogService;

        public CenterController(HelpCenterService helpCenterService, UserLogService userLogService)
        {
            this.helpCenterService = helpCenterService;
            this.userLogService = userLogService;
        }
        public async Task<IActionResult> Index()
        {
            await userLogService.CreateUserLogAsync(UserInfo.Current.UserSessionID, UserLogActionType.HelpCenter_Page);

            return View();
        }

        public IActionResult Edit()
        {
            return View();
        }
        public IActionResult LimitAdd()
        {
            return View();
        }

        public async Task<IActionResult> LoadMenu()
        {
            var menus = await helpCenterService.GetMenus();
            return Json(new
            {
                isOk = true,
                menus
            });
        }

        public async Task<IActionResult> Load(int id)
        {
            return Json(new
            {
                isOk = true,
                article = await helpCenterService.GetFullArticleByID(id),
                topArticles = await helpCenterService.GetTopArticles(),
                relatedArticles = await helpCenterService.GetRelatedArticlesByID(id)
            });
        }


        public async Task<IActionResult> ViewArticle(int id)
        {
            var article = await helpCenterService.GetArticleByID(id);

            return View(article);
        }
    }
}
