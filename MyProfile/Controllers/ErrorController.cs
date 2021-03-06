using Microsoft.AspNetCore.Mvc;
using MyProfile.Identity;
using MyProfile.UserLog.Service;
using System.Net;
using System.Threading.Tasks;

namespace MyProfile.Controllers
{
    public class ErrorController : Controller
    {
        private UserLogService userLogService;

        public ErrorController(UserLogService userLogService)
        {
            this.userLogService = userLogService;
        }

        public IActionResult StatusCode()
        {
            if (Response.StatusCode == (int)HttpStatusCode.NotFound)
            {
                return RedirectToAction("Error_404");
            }
            else
            {
                return RedirectToAction("Error_500");
            }

            return View();
        }
        public async Task<IActionResult> Error_404()
        {
            if (UserInfo.Current != null)
            {
                await userLogService.CreateUserLogAsync(UserInfo.Current.UserSessionID, UserLogActionType.Error404_Page);
            }
            return View();
        }

        public async Task<IActionResult> Error_500()
        {
            if (UserInfo.Current != null)
            {
                await userLogService.CreateUserLogAsync(UserInfo.Current.UserSessionID, UserLogActionType.Error500_Page);
            }
            return View();
        }
    }
}
