using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyProfile.Entity.ModelView.HelpCenter;
using MyProfile.HelpCenter.Service;
using MyProfile.Identity;
using MyProfile.User.Service;

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
            await userLogService.CreateUserLog(UserInfo.Current.UserSessionID, UserLogActionType.HelpCenter_Page);

            return View();
        }

        public IActionResult Edit()
        {
            return View();
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
